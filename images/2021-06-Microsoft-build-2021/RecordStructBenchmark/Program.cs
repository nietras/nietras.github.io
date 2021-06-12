using System;
using System.Collections.Generic;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);

public readonly struct Struct
{
    public Struct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; }
    public int Value { get; }
}

public readonly struct EquatableStruct : IEquatable<EquatableStruct>
{
    public EquatableStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; }
    public int Value { get; }

    public bool Equals(EquatableStruct other) =>
        Type.Equals(other.Type) && Value.Equals(other.Value);
}

public record struct RecordStruct(Type Type, int Value);

public class StructDictionary
{
    readonly Dictionary<Struct, long> _structToValue = new()
    {
        { new Struct(typeof(byte), 101), 0 },
        { new Struct(typeof(sbyte), 102), 1 },
        { new Struct(typeof(ushort), 201), 2 },
        { new Struct(typeof(short), 202), 3 },
        { new Struct(typeof(uint), 301), 4 },
        { new Struct(typeof(int), 302), 5 },
    };

    readonly Dictionary<EquatableStruct, long> _equatableStructToValue = new()
    {
        { new EquatableStruct(typeof(byte), 101), 0 },
        { new EquatableStruct(typeof(sbyte), 102), 1 },
        { new EquatableStruct(typeof(ushort), 201), 2 },
        { new EquatableStruct(typeof(short), 202), 3 },
        { new EquatableStruct(typeof(uint), 301), 4 },
        { new EquatableStruct(typeof(int), 302), 5 },
    };

    readonly Dictionary<(Type, int), long> _valueTupleToValue = new()
    {
        { (typeof(byte), 101), 0 },
        { (typeof(sbyte), 102), 1 },
        { (typeof(ushort), 201), 2 },
        { (typeof(short), 202), 3 },
        { (typeof(uint), 301), 4 },
        { (typeof(int), 302), 5 },
    };

    readonly Dictionary<RecordStruct, long> _recordStructToValue = new()
    {
        { new(typeof(byte), 101), 0 },
        { new(typeof(sbyte), 102), 1 },
        { new(typeof(ushort), 201), 2 },
        { new(typeof(short), 202), 3 },
        { new(typeof(uint), 301), 4 },
        { new(typeof(int), 302), 5 },
    };

    readonly Struct _structKey = new (typeof(ushort), 201);
    readonly EquatableStruct _equatableStructKey = new(typeof(ushort), 201);
    readonly RecordStruct _recordStructKey = new (typeof(ushort), 201);
    readonly (Type, int) _valueTupleKey = (typeof(ushort), 201);

    [Benchmark(Baseline = true)]
    public long Struct_() => _structToValue[_structKey];

    [Benchmark()]
    public long EquatableStruct_() => _equatableStructToValue[_equatableStructKey];

    [Benchmark]
    public long ValueTuple_() => _valueTupleToValue[_valueTupleKey];

    [Benchmark()]
    public long RecordStruct_() => _recordStructToValue[_recordStructKey];
}