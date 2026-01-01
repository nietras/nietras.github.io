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

    [Benchmark]
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

    public static IEnumerable<ReadOnlyMemory<char>> EnumerateLinesVector512Masked(string text)
    {
        var memory = text.AsMemory();
        var start = 0;
        while (start < memory.Length)
        {
            if (!TryFindLineEnding(text, start, out var newlineIndex))
            {
                yield return memory[start..];
                yield break;
            }

            var nextStart = newlineIndex + 1;
            var newlineChar = text[newlineIndex];
            if (newlineChar == '\r' && nextStart < memory.Length && text[nextStart] == '\n')
            {
                nextStart++;
            }

            yield return memory.Slice(start, newlineIndex - start);
            start = nextStart;
        }
    }

    private static bool TryFindLineEnding(string text, int start, out int lineEnd)
    {
        var span = text.AsSpan();
        var length = span.Length;
        var limit = length - s_vectorCharCount;
        var i = start;
        Span<ushort> laneResults = stackalloc ushort[s_vectorCharCount];
        while (i <= limit)
        {
            var chunk = MemoryMarshal.Cast<char, Vector512<ushort>>(span.Slice(i, s_vectorCharCount))[0];
            var matches = Vector512.BitwiseOr(
                Vector512.Equals(chunk, s_newlineVector),
                Vector512.Equals(chunk, s_carriageVector));
            matches.CopyTo(laneResults);
            long mask = 0;
            for (var lane = 0; lane < s_vectorCharCount; lane++)
            {
                mask |= (long)((laneResults[lane] != 0 ? 1L : 0L) << lane);
            }

            if (mask != 0)
            {
                lineEnd = i + BitOperations.TrailingZeroCount((ulong)mask);
                return true;
            }

            i += s_vectorCharCount;
        }

        while (i < length)
        {
            var ch = span[i];
            if (ch == '\n' || ch == '\r')
            {
                lineEnd = i;
                return true;
            }
            i++;
        }

        lineEnd = -1;
        return false;
    }

    private static readonly Vector512<ushort> s_newlineVector = Vector512.Create((ushort)'\n');
    private static readonly Vector512<ushort> s_carriageVector = Vector512.Create((ushort)'\r');
    private static readonly int s_vectorCharCount = Vector512<ushort>.Count;
}

