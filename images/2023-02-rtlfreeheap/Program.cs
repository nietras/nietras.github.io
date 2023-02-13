using System.Diagnostics;
using System.Runtime.InteropServices;

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

const int count = 8 * 1024;
const nuint bytes = 19_200;

Test(count, bytes, sort: false, log);

static unsafe void Test(int count, nuint bytes, bool sort, Action<string> log)
{
    var ptrs = new IntPtr[count];
    for (var i = 0; i < count; i++)
    {
        ptrs[i] = new(NativeMemory.Alloc(bytes));
    }
    Shuffle(ptrs);

    if (sort) { ptrs.AsSpan().Sort(); }

    var times_s = new double[count];
    for (var i = 0; i < count; i++)
    {
        var b = Stopwatch.GetTimestamp();
        NativeMemory.Free(ptrs[i].ToPointer());
        var a = Stopwatch.GetTimestamp();
        var s = (a - b) * 1.0 / Stopwatch.Frequency;
        times_s[i] = s;
    }
    Array.Sort(times_s);
    var min = times_s[0];
    var sum = times_s.Sum();
    var mean = times_s.Average();
    var median = times_s[times_s.Length / 2];
    var max = times_s[^1];
    log($"{count} of {bytes} [s]: Total {sum,9:F6} Mean {mean,8:F6} [{min,8:F6}, {median,8:F6}, {max,8:F6}]");
}

static void Shuffle<T>(T[] array, int seed = 72137)
{
    var random = new Random(seed);
    var n = array.Length;
    while (n > 1)
    {
        var k = random.Next(n);
        --n;
        var temp = array[n];
        array[n] = array[k];
        array[k] = temp;
    }
}