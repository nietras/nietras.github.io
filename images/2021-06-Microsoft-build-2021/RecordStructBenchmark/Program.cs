using System;
using System.Collections.Generic;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var rs = new RecordStruct { Type = typeof(short), Value = 42 };
var (type, value) = rs;

var x = new PlainStruct(typeof(string), 42);
var y = new PlainStruct(typeof(long), 17);
// OR
//var x = new RecordStruct(typeof(string), 42);
//var y = new RecordStruct(typeof(long), 17);
Console.WriteLine(x.ToString());
//Console.WriteLine(x == y);
//Console.WriteLine(x != y);
//Console.WriteLine(x == x);

//BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);

public readonly struct PlainStruct
{
    public PlainStruct(Type type, int value)
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

public readonly struct HashStruct
{
    public HashStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; }
    public int Value { get; }

    public override int GetHashCode() =>
        Type.GetHashCode() * -1521134295 + Value.GetHashCode();
        // Above I am using the exact same hash method as the record struct
        // to make sure they are comparable. However, if you do your own
        // `GetHashCode()` prefer using `HashCode.Combine`
        //HashCode.Combine(Type.GetHashCode(), Value.GetHashCode());
}
public readonly struct HashEquatableStruct : IEquatable<HashEquatableStruct>
{
    public HashEquatableStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; }
    public int Value { get; }

    public bool Equals(HashEquatableStruct other) =>
        Type.Equals(other.Type) && Value.Equals(other.Value);

    public override int GetHashCode() =>
        Type.GetHashCode() * -1521134295 + Value.GetHashCode();
}

public record struct RecordStruct(Type Type, int Value);

public class StructDictionaryBenchmark
{
    readonly Dictionary<PlainStruct, long> _plainStructToValue = new()
    {
        { new PlainStruct(typeof(byte), 101), 0 },
        { new PlainStruct(typeof(sbyte), 102), 1 },
        { new PlainStruct(typeof(ushort), 201), 2 },
        { new PlainStruct(typeof(short), 202), 3 },
        { new PlainStruct(typeof(uint), 301), 4 },
        { new PlainStruct(typeof(int), 302), 5 },
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

    readonly Dictionary<HashStruct, long> _hashStructToValue = new()
    {
        { new HashStruct(typeof(byte), 101), 0 },
        { new HashStruct(typeof(sbyte), 102), 1 },
        { new HashStruct(typeof(ushort), 201), 2 },
        { new HashStruct(typeof(short), 202), 3 },
        { new HashStruct(typeof(uint), 301), 4 },
        { new HashStruct(typeof(int), 302), 5 },
    };

    readonly Dictionary<HashEquatableStruct, long> _hashEquatableStructToValue = new()
    {
        { new HashEquatableStruct(typeof(byte), 101), 0 },
        { new HashEquatableStruct(typeof(sbyte), 102), 1 },
        { new HashEquatableStruct(typeof(ushort), 201), 2 },
        { new HashEquatableStruct(typeof(short), 202), 3 },
        { new HashEquatableStruct(typeof(uint), 301), 4 },
        { new HashEquatableStruct(typeof(int), 302), 5 },
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

    readonly PlainStruct _plainStructKey = new (typeof(ushort), 201);
    readonly EquatableStruct _equatableStructKey = new(typeof(ushort), 201);
    readonly HashStruct _hashStructKey = new(typeof(ushort), 201);
    readonly HashEquatableStruct _hashEquatableStructKey = new(typeof(ushort), 201);
    readonly (Type, int) _valueTupleKey = (typeof(ushort), 201);
    readonly RecordStruct _recordStructKey = new(typeof(ushort), 201);

    [Benchmark(Baseline = true)]
    public long PlainStruct_() => _plainStructToValue[_plainStructKey];

    [Benchmark()]
    public long EquatableStruct_() => _equatableStructToValue[_equatableStructKey];

    [Benchmark()]
    public long HashStruct_() => _hashStructToValue[_hashStructKey];

    [Benchmark()]
    public long HashEquatableStruct_() => _hashEquatableStructToValue[_hashEquatableStructKey];

    [Benchmark]
    public long ValueTuple_() => _valueTupleToValue[_valueTupleKey];

    [Benchmark()]
    public long RecordStruct_() => _recordStructToValue[_recordStructKey];
}