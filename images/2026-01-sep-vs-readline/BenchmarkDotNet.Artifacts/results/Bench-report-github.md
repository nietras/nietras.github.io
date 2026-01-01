```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6691/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
  [Host]     : .NET 10.0.1, X64 NativeAOT x86-64-v4
  DefaultJob : .NET 10.0.1, X64 NativeAOT x86-64-v4


```
| Method    | Mean      | Error    | StdDev   | Gen0   | Allocated |
|---------- |----------:|---------:|---------:|-------:|----------:|
| Sep______ | 221.32 ns | 0.721 ns | 0.639 ns | 0.0434 |     728 B |
| ReadLine_ |  24.91 ns | 0.061 ns | 0.057 ns | 0.0076 |     128 B |
