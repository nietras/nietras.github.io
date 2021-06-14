``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.985 (21H1/May2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=5.0.300
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  Job-GIPAIU : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT

Runtime=.NET 5.0  Toolchain=netcoreapp50  

```
|                                Method |      Mean |     Error |    StdDev | Ratio | Code Size |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------------------- |----------:|----------:|----------:|------:|----------:|-------:|------:|------:|----------:|
|               PlainStruct_GetHashCode | 34.241 ns | 0.1571 ns | 0.1311 ns |  1.00 |      58 B | 0.0019 |     - |     - |      32 B |
|           EquatableStruct_GetHashCode | 32.694 ns | 0.2103 ns | 0.1967 ns |  0.95 |      58 B | 0.0019 |     - |     - |      32 B |
|                HashStruct_GetHashCode |  2.004 ns | 0.0307 ns | 0.0257 ns |  0.06 |      49 B |      - |     - |     - |         - |
|       HashEquatableStruct_GetHashCode |  1.980 ns | 0.0135 ns | 0.0126 ns |  0.06 |      49 B |      - |     - |     - |         - |
|                ValueTuple_GetHashCode |  4.230 ns | 0.0704 ns | 0.0588 ns |  0.12 |     145 B |      - |     - |     - |         - |
|              RecordStruct_GetHashCode |  2.835 ns | 0.0128 ns | 0.0120 ns |  0.08 |      58 B |      - |     - |     - |         - |
| HashEquatableRecordStruct_GetHashCode |  1.992 ns | 0.0042 ns | 0.0035 ns |  0.06 |      49 B |      - |     - |     - |         - |
