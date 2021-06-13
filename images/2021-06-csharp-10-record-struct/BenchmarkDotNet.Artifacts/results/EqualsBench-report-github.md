``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.985 (21H1/May2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=5.0.300
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  Job-GIPAIU : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT

Runtime=.NET 5.0  Toolchain=netcoreapp50  

```
|                           Method |      Mean |     Error |    StdDev | Ratio | Code Size |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------------------- |----------:|----------:|----------:|------:|----------:|-------:|------:|------:|----------:|
|               PlainStruct_Equals | 94.744 ns | 0.7827 ns | 0.6938 ns |  1.00 |     403 B | 0.0062 |     - |     - |     104 B |
|           EquatableStruct_Equals |  1.119 ns | 0.0263 ns | 0.0246 ns |  0.01 |      59 B |      - |     - |     - |         - |
|                HashStruct_Equals | 94.763 ns | 0.6107 ns | 0.5414 ns |  1.00 |     406 B | 0.0062 |     - |     - |     104 B |
|       HashEquatableStruct_Equals |  1.291 ns | 0.0138 ns | 0.0129 ns |  0.01 |      59 B |      - |     - |     - |         - |
|                ValueTuple_Equals |  2.385 ns | 0.0166 ns | 0.0155 ns |  0.03 |     152 B |      - |     - |     - |         - |
|              RecordStruct_Equals |  2.380 ns | 0.0138 ns | 0.0129 ns |  0.03 |     148 B |      - |     - |     - |         - |
| HashEquatableRecordStruct_Equals |  1.089 ns | 0.0150 ns | 0.0140 ns |  0.01 |      59 B |      - |     - |     - |         - |
