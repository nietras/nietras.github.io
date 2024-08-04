---
layout: post
title: Sep 0.5.3 vs ML.NET 3.0.1 - Up to 2.7x faster & 8.9x less bytes allocated
---

Since my last post on Sep ([Sep 0.4.0-0.5.2 - Insanely Fast Single- &
Multi-threaded .NET CSV Parsing (up to 35x faster than CsvHelper)]({{
site.baseurl }}/2024/05/05/sep-0-4-0-0-5-2/)) not much has happened with Sep,
besides a minor [0.5.3](https://github.com/nietras/Sep/releases/tag/v0.5.3)
release adding a simple ` SepReaderHeader.TryIndexOf`.

In this post I wanted to compare Sep to
[ML.NET](https://github.com/dotnet/machinelearning) an open source and
cross-platform machine learning framework for .NET. Please keep in mind that
ML.NET has a completely different context and set of features witha regards to
data loading, so this is not an apples to apples comparison. The comparison is
motivated by ML.NET by default being multi-threaded so it is a good candidate to
compare the `ParallelEnumerate` feature of Sep to, and since Sep and ML.NET have
the same target usage scenario namely machine learning.

## CSV Data
The benchmark case is pretty simple. It compares parsing a CSV file with 1000,
10000 and 100000 rows and 1000 columns of random floating point values from a
file as detailed in the table below.

| Rows    | Cols | Total Floats | File Size [MB] |
|--------:|----- |-------------:|------:|
|   1,000 | 1000 |    1,000,000 |    10 |
|  10,000 | 1000 |   1,000,0000 |   101 |
| 100,000 | 1000 |  10,000,0000 | 1,013 |

Below exemplifies the file contents. 

```
C0;C1;C2;C3;C4;C5;C6...
0.66810644;0.1409073;0.12551829;0.52276427;0.16843422;0.26259267...
0.98956823;0.21919754;0.31849968;0.010851925;0.3098326;0.6515554...
0.28303733;0.9632272;0.044600043;0.9494399;0.8236798;0.06896791...
0.3721751;0.87286466;0.15162648;0.15530175;0.6253938;0.5741966...
```

Note that this contains a header but this is not actually used in the benchmark.
This is because ML.NET requires a class with a `LoadColumnName` attribute to map
the columns to a class property, and Sep does not require this. Using attributes
for this kind of mapping is very constrained as attributes have to be statically
defined (`const`). This is problematic for many reasons e.g. it couples code to
dynamically changing data. Maybe there are ways around this in ML.NET but I have
not found them.

## Code
This will be more concrete when looking at the code needed to parse the file.
Full source code is given at the end of the post. For Sep all that is needed is
the below:

```csharp
public List<float[]> Sep__()
{
    using var reader = Sep.Reader().FromFile(_filePath);
    var featuresList = reader.ParallelEnumerate(
        row => row[..].ParseToArray<float>()).ToList();
    return featuresList;
}
```

The premise here is we load all columns as a float array into a list containing
all features for some problem. ML.NET requires quite a bit more code as shown
below (e.g. it requires a secondary type `Features` with attributes to define
the parsing):

```csharp
public List<float[]> MLNET()
{
    var context = new MLContext();
    var dataView = context.Data.LoadFromTextFile<Features>(
        _filePath, separatorChar: ';', hasHeader: true);
    var featuresList = context.Data
        .CreateEnumerable<Features>(dataView, reuseRowObject: false)
        .Select(f => f.FeaturesVector)
        .ToList();
    return featuresList;
}

public sealed class Features
{
    public const int Count = 1000;

    [LoadColumn(0, Count - 1)]
    [VectorType(Count)]
    public float[] FeaturesVector { get; init; } = [];
}
```

Again keep in mind ML.NET has a lot of features related to data loading and
management in the context of the ML pipeline it is used.

Often, we would want to dynamically load two float arrays e.g. ground truth and
actual results or similar and we would want that to be done dynamically based on
header column names. I am not sure how one would do that with ML.NET. With Sep
this is easy as one can simply access multiple columns with a string array
defining column names or similar.

## Benchmarks
Results using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) are
shown below. Sep is about 2.7x faster and allocates 8.9x less bytes than ML.NET
for 1000 rows and 1000 columns. As there are more rows the difference becomes
less, but Sep is still significantly faster and allocates a lot less memory for
100 thousand rows and a 1GB file.

```text
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.400-preview.0.24324.5
  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  Job-DEFUWP : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2

| Method | Rows   | Cols | Mean [ms] | Ratio | Alloc [MB]| Alloc Ratio |
|------- |------- |----- |----------:|------:|----------:|------------:|
| Sep__  |   1000 | 1000 |     9.393 |  1.00 |      4.19 |        1.00 |
| MLNET  |   1000 | 1000 |    25.543 |  2.73 |     37.12 |        8.85 |
|        |        |      |           |       |           |             |
| Sep__  |  10000 | 1000 |   103.041 |  1.00 |     39.35 |        1.00 |
| MLNET  |  10000 | 1000 |   200.062 |  1.97 |    269.51 |        6.85 |
|        |        |      |           |       |           |             |
| Sep__  | 100000 | 1000 |   950.624 |  1.00 |     389.9 |        1.00 |
| MLNET  | 100000 | 1000 | 1,715.758 |  1.80 |   2446.85 |        6.28 |
```

## Full Source Code

`SepVsMLNET.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" Version="0.13.12" />
    <PackageReference Include="Microsoft.ML" Version="3.0.1" />
    <PackageReference Include="Sep" Version="0.5.3" />
  </ItemGroup>
</Project>
```

`Program.cs`
```csharp
using System.Data;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.ML;
using Microsoft.ML.Data;
using nietras.SeparatedValues;

BenchmarkRunner.Run(typeof(Bench).Assembly, args: args);

[MemoryDiagnoser]
[MinIterationCount(3)]
[MaxIterationCount(9)]
[HideColumns("Error", "StdDev", "Gen0", "Gen1", "Gen2")]
public class Bench
{
    string _filePath;

    public Bench() => GlobalSetup();

    [MemberNotNull(nameof(_filePath))]
    [GlobalSetup]
    public void GlobalSetup()
    {
        var colNames = Enumerable.Range(0, Cols)
            .Select(i => $"C{i}").ToArray();
        _filePath = @$"B:/Features_{Rows}_{Cols}.csv";
        if (!File.Exists(_filePath))
        {
            Console.WriteLine($"Write test file '{_filePath}'");
            var random = new Random(42);
            using var writer = Sep.Writer().ToFile(_filePath);
            Span<float> colValues = stackalloc float[Cols];
            for (var r = 0; r < Rows; r++)
            {
                using var row = writer.NewRow();
                foreach (ref var v in colValues)
                { v = random.NextSingle(); }
                row[colNames].Format(colValues);
            }
        }
    }

    [Params(1000, 10_000, 100_000)]
    public int Rows { get; set; } = 1000;

    [Params(Features.Count)]
    public int Cols { get; set; } = Features.Count;

    [Benchmark(Baseline = true)]
    public List<float[]> Sep__()
    {
        using var reader = Sep.Reader().FromFile(_filePath);
        var featuresList = reader.ParallelEnumerate(
            r => r[..].ParseToArray<float>()).ToList();
        return featuresList;
    }

    [Benchmark]
    public List<float[]> MLNET()
    {
        var context = new MLContext();
        var dataView = context.Data.LoadFromTextFile<Features>(
            _filePath, separatorChar: ';', hasHeader: true);
        var featuresList = context.Data
            .CreateEnumerable<Features>(dataView, reuseRowObject: false)
            .Select(f => f.FeaturesVector)
            .ToList();
        return featuresList;
    }

    public sealed class Features
    {
        public const int Count = 1000;

        [LoadColumn(0, Count - 1)]
        [VectorType(Count)]
        public float[] FeaturesVector { get; init; } = [];
    }
}
```

That's all!