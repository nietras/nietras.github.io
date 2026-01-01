using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using nietras.SeparatedValues;

BenchmarkRunner.Run(typeof(Bench).Assembly, args: args);

[MemoryDiagnoser]
[EvaluateOverhead(false)]
[WarmupCount(3)]
[MinIterationCount(3)]
[MaxIterationCount(7)]
public class Bench
{
    string m_text = "";

    [GlobalSetup]
    public void GlobalSetup()
    {
        m_text = GenerateLargeText();
    }

    //[Benchmark]
    public void Sep______()
    {
        using var reader = Sep
            .Reader(o => o with { HasHeader = false })
            .FromText(m_text);
        var sum = 0;
        foreach (var row in reader)
        {
            sum += row.Span.Length;
        }
    }

    //[Benchmark]
    public void ReadLine_()
    {
        using var reader = new StringReader(m_text);
        var sum = 0;
        while (reader.ReadLine() is { } line)
        {
            sum += line.Length;
        }
    }

    [Benchmark]
    public void EnumerateLines_()
    {
        var lines = MemoryExtensions.EnumerateLines(m_text);
        var sum = 0;
        foreach (var line in lines)
        {
            sum += line.Length;
        }
    }

    [Benchmark]
    public void Vector512Masked_()
    {
        var sum = 0;
        foreach (var line in EnumerateLinesVector512Masked(m_text))
        {
            sum += line.Length;
        }
    }



    public static Vector512LineEnumerable EnumerateLinesVector512Masked(string text)
        => new(text);

    public readonly struct Vector512LineEnumerable
    {
        private readonly string _text;

        public Vector512LineEnumerable(string text) => _text = text;

        public Vector512LineEnumerator GetEnumerator() => new(_text);
    }

    public ref struct Vector512LineEnumerator
    {
        private ReadOnlySpan<char> _span;
        private int _lineStart;
        private int _position;
        private long _mask;
        private int _currentStart;
        private int _currentLength;

        internal Vector512LineEnumerator(string text)
        {
            _span = text.AsSpan();
            _lineStart = 0;
            _position = 0;
            _mask = 0;
            _currentStart = 0;
            _currentLength = 0;
        }

        public ReadOnlySpan<char> Current => _span.Slice(_currentStart, _currentLength);

        public bool MoveNext()
        {
            var span = _span;
            if (_lineStart >= span.Length)
            {
                _currentLength = 0;
                return false;
            }

            var start = _lineStart;
            var newlineIndex = -1;

            while (true)
            {
                if (_mask != 0)
                {
                    var bit = BitOperations.TrailingZeroCount((ulong)_mask);
                    newlineIndex = (_position - s_vectorCharCount) + bit;
                    _mask &= ~(1L << bit);
                    break;
                }

                if (_position <= span.Length - s_vectorCharCount)
                {
                    var chunk = MemoryMarshal.Cast<char, Vector512<ushort>>(span.Slice(_position, s_vectorCharCount))[0];
                    var matches = Vector512.BitwiseOr(
                        Vector512.Equals(chunk, s_newlineVector),
                        Vector512.Equals(chunk, s_carriageVector));
                    _mask = (long)Vector512.ExtractMostSignificantBits(matches);
                    _position += s_vectorCharCount;

                    if (_mask != 0)
                    {
                        continue;
                    }

                    continue;
                }

                break;
            }

            if (newlineIndex == -1)
            {
                for (; _position < span.Length; _position++)
                {
                    var ch = span[_position];
                    if (ch == '\n' || ch == '\r')
                    {
                        newlineIndex = _position;
                        _position++;
                        break;
                    }
                }
            }

            if (newlineIndex == -1)
            {
                if (start >= span.Length)
                {
                    _lineStart = span.Length + 1;
                    _currentLength = 0;
                    return false;
                }

                _currentStart = start;
                _currentLength = span.Length - start;
                _lineStart = span.Length + 1;
                _mask = 0;
                _position = span.Length;
                return true;
            }

            var stride = 1;
            var newlineChar = span[newlineIndex];
            if (newlineChar == '\r' && newlineIndex + 1 < span.Length && span[newlineIndex + 1] == '\n')
            {
                stride = 2;
            }

            _currentStart = start;
            _currentLength = newlineIndex - start;
            _lineStart = newlineIndex + stride;

            if (_lineStart > _position)
            {
                _position = _lineStart;
            }

            return true;
        }
    }

    public static string GenerateLargeText(int totalLength = 1024 * 1024, int maxLineLength = 100)
    {
        var rnd = new Random(42);
        var sb = new StringBuilder(capacity: totalLength);
        int count = 0;
        while (count < totalLength)
        {
            int lineLength = rnd.Next(1, maxLineLength + 1);
            if (count + lineLength + 1 > totalLength)
            {
                lineLength = totalLength - count - 1;
                if (lineLength <= 0) break;
            }
            sb.Append((char)rnd.Next('a', 'z' + 1), lineLength);
            sb.Append('\n');
            count += lineLength + 1;
        }
        return sb.ToString();
    }

    private static readonly Vector512<ushort> s_newlineVector = Vector512.Create((ushort)'\n');
    private static readonly Vector512<ushort> s_carriageVector = Vector512.Create((ushort)'\r');
    private static readonly int s_vectorCharCount = Vector512<ushort>.Count;
}

