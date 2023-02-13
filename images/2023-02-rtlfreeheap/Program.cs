using System.Diagnostics;
using System.Runtime.InteropServices;
using nietras.SeparatedValues;

// "Advanced Heap Manipulation in Windows 8"
// https://paper.bobylive.com/Meeting_Papers/BlackHat/Europe-2013/bh-eu-13-liu-advanced-heap-WP.pdf
// Segment heap is a modern heap implementation that will generally reduce your overall memory usage.
// https://learn.microsoft.com/en-us/windows/win32/sbscs/application-manifests#heaptype
// "Notes on Heap Implementation" - https://twitter.com/lectem/status/1151194432991051776
// https://learn.microsoft.com/en-us/previous-versions/ms810466(v=msdn.10)#notes-on-heap-implementation
// "Windows 8 Heap Internals"
// https://illmatics.com/Windows%208%20Heap%20Internals.pdf
// "WINDOWS 10 SEGMENT HEAP INTERNALS"
// https://www.blackhat.com/docs/us-16/materials/us-16-Yason-Windows-10-Segment-Heap-Internals-wp.pdf

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

const nuint bytes = 19_200;
int count = 4096;
const int times = 6;

var process = Process.GetCurrentProcess();
using var writer = Sep.Default.Writer().ToFile("../../../NativeHeapStress.csv");
for (int i = 0; i < times; i++)
{
    var sort = false;

    GC.Collect();

    var m = Test(count, bytes, sort, process);

    var t = SortThenComputeStats(m.Times_us, ts => ts.Sum(), ts => ts.Average());
    var p = SortThenComputeStats(m.PrivateMemorySizes, sz => sz.Sum(), sz => sz.Average());

    log($"{count} of {bytes} [s]: Total {t.Sum,9:F6} Mean {t.Mean,8:F6} [{t.Min,8:F6}, {t.Median,8:F6}, {t.Max,8:F6}]");

    using var row = writer.NewRow();
    row["Bytes"].Format(bytes);
    row["Count"].Format(count);
    row["HeapFree Sum [us]"].Format(t.Sum);
    row["HeapFree Mean [us]"].Format(t.Mean);
    row["HeapFree Min [us]"].Format(t.Min);
    row["HeapFree Median [us]"].Format(t.Median);
    row["HeapFree Max [us]"].Format(t.Max);
    row["Sort [us]"].Set(sort ? $"{m.Sort_us}" : "");
    row["PrivateMemorySize Min [bytes]"].Format(p.Min);
    row["PrivateMemorySize Max [bytes]"].Format(p.Max);

    count *= 2;
}

static unsafe Measurements Test(int count, nuint bytes, bool sort, Process process)
{
    var pointers = new IntPtr[count];
    for (var i = 0; i < count; i++)
    {
        pointers[i] = new(NativeMemory.Alloc(bytes));
    }

    Shuffle(pointers);

    var sortBefore = Stopwatch.GetTimestamp();
    if (sort) { pointers.AsSpan().Sort(); }
    var sortAfter = Stopwatch.GetTimestamp();
    var sort_us = (sortAfter - sortBefore) * 1000_000.0 / Stopwatch.Frequency;

    var times_us = new double[count];
    var privateMemorySizes = new long[count];
    for (var i = 0; i < count; i++)
    {
        var b = Stopwatch.GetTimestamp();
        NativeMemory.Free(pointers[i].ToPointer());
        var a = Stopwatch.GetTimestamp();
        var us = (a - b) * 1000_000.0 / Stopwatch.Frequency;
        times_us[i] = us;
        privateMemorySizes[i] = process.PrivateMemorySize64;
    }
    return new(sort_us, times_us, privateMemorySizes);
}

static Stats<T> SortThenComputeStats<T>(T[] values, Func<T[], double> sumFunc, Func<T[], double> averageFunc)
{
    Array.Sort(values);
    var sum = sumFunc(values);
    var mean = averageFunc(values);
    var min = values[0];
    var median = values[values.Length / 2];
    var max = values[^1];
    return new(sum, mean, min, median, max);
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

record Measurements(double Sort_us, double[] Times_us, long[] PrivateMemorySizes);

record Stats<T>(double Sum, double Mean, T Min, T Median, T Max);

