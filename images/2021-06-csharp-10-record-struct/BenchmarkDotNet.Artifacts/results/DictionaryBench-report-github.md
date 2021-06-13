``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.985 (21H1/May2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=5.0.300
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  Job-GIPAIU : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT

Runtime=.NET 5.0  Toolchain=netcoreapp50  

```
|                                  Method |       Mean |     Error |    StdDev | Ratio | Code Size |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------------------- |-----------:|----------:|----------:|------:|----------:|-------:|------:|------:|----------:|
|               PlainStruct_DictionaryGet | 213.739 ns | 0.9996 ns | 0.9351 ns |  1.00 |     110 B | 0.0110 |     - |     - |     184 B |
|           EquatableStruct_DictionaryGet |  39.478 ns | 0.1974 ns | 0.1847 ns |  0.18 |     113 B | 0.0019 |     - |     - |      32 B |
|                HashStruct_DictionaryGet | 167.100 ns | 1.1699 ns | 1.0371 ns |  0.78 |     113 B | 0.0091 |     - |     - |     152 B |
|       HashEquatableStruct_DictionaryGet |   7.555 ns | 0.0429 ns | 0.0401 ns |  0.04 |     113 B |      - |     - |     - |         - |
|                ValueTuple_DictionaryGet |  20.471 ns | 0.0664 ns | 0.0519 ns |  0.10 |     174 B |      - |     - |     - |         - |
|              RecordStruct_DictionaryGet |  10.562 ns | 0.0381 ns | 0.0356 ns |  0.05 |     113 B |      - |     - |     - |         - |
| HashEquatableRecordStruct_DictionaryGet |   8.707 ns | 0.0935 ns | 0.0780 ns |  0.04 |     113 B |      - |     - |     - |         - |
