using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using static System.Console;

TestPlain();
TestRecord();

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);

void TestPlain()
{
    WriteLine(nameof(TestPlain));
    var x = new PlainStruct(typeof(string), 42);
    var y = new PlainStruct(typeof(string), 17);
    var z = new PlainStruct(typeof(long), 17);
    WriteLine(x.ToString());
    WriteLine(x.Equals(y));
    //WriteLine(x == y); N/A
    //WriteLine(x != y); N/A
    //WriteLine(x == x); N/A
    WriteLine(x.GetHashCode());
    WriteLine(y.GetHashCode());
    WriteLine(z.GetHashCode());
    //var (k, v) = x; // N/A
    var i = new PlainStruct { Type = typeof(byte), Value = 17 };
    var j = i with { Value = 3 };
    WriteLine();
}

void TestRecord()
{
    WriteLine(nameof(TestRecord));
    var x = new RecordStruct(typeof(string), 42);
    var y = new RecordStruct(typeof(string), 17);
    var z = new RecordStruct(typeof(long), 17);
    WriteLine(x.ToString());
    WriteLine(x.Equals(y));
    WriteLine(x == y);
    WriteLine(x != y);
    var c = x;
    WriteLine(x == c);
    WriteLine(x.GetHashCode());
    WriteLine(y.GetHashCode());
    WriteLine(z.GetHashCode());
    var (k, v) = x;
    var i = new RecordStruct { Type = typeof(byte) };
    var j = i with { Value = 3 };
    WriteLine();
}

public readonly struct PlainStruct
{
    public PlainStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; init; }
    public int Value { get; init; }
}

public readonly struct EquatableStruct 
    : IEquatable<EquatableStruct>
{
    public EquatableStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; init; }
    public int Value { get; init; }

    public bool Equals(EquatableStruct other) =>
        Type == other.Type && Value == other.Value;
}

public readonly struct HashStruct
{
    public HashStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; init; }
    public int Value { get; init; }

    public override int GetHashCode() =>
        Type.GetHashCode() * -1521134295 + Value.GetHashCode();
}

public readonly struct HashEquatableStruct 
    : IEquatable<HashEquatableStruct>
{
    public HashEquatableStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; init; }
    public int Value { get; init; }

    public bool Equals(HashEquatableStruct other) =>
        Type == other.Type && Value == other.Value;

    public override int GetHashCode() =>
        Type.GetHashCode() * -1521134295 + Value.GetHashCode();
}

public readonly record struct RecordStruct(Type Type, int Value);

public readonly record struct HashEquatableRecordStruct(Type Type, int Value) 
    : IEquatable<HashEquatableRecordStruct>
{
    public bool Equals(HashEquatableRecordStruct other) =>
        Type == other.Type && Value == other.Value;

    public override int GetHashCode() =>
        Type.GetHashCode() * -1521134295 + Value.GetHashCode();
}

public abstract class BaseBench
{
    static readonly Type Type = typeof(ushort);
    const int Value = 201;

    protected readonly PlainStruct _plainStructKey = new(Type, Value);
    protected readonly EquatableStruct _equatableStructKey = new(Type, Value);
    protected readonly HashStruct _hashStructKey = new(Type, Value);
    protected readonly HashEquatableStruct _hashEquatableStructKey = new(Type, Value);
    protected readonly (Type, int) _valueTupleKey = (Type, Value);
    protected readonly RecordStruct _recordStructKey = new(Type, Value);
    protected readonly HashEquatableRecordStruct _hashEquatableRecordStructKey = new(Type, Value);
}

public class GetHashCodeBench : BaseBench
{
    [Benchmark(Baseline = true)]
    public int PlainStruct_GetHashCode() => _plainStructKey.GetHashCode();

    [Benchmark]
    public int EquatableStruct_GetHashCode() => _equatableStructKey.GetHashCode();

    [Benchmark]
    public int HashStruct_GetHashCode() => _hashStructKey.GetHashCode();

    [Benchmark]
    public int HashEquatableStruct_GetHashCode() => _hashEquatableStructKey.GetHashCode();

    [Benchmark]
    public int ValueTuple_GetHashCode() => _valueTupleKey.GetHashCode();

    [Benchmark]
    public int RecordStruct_GetHashCode() => _recordStructKey.GetHashCode();

    [Benchmark]
    public int HashEquatableRecordStruct_GetHashCode() => _hashEquatableRecordStructKey.GetHashCode();
}

public class EqualsBench : BaseBench
{
    static readonly Type TypeOther = typeof(long);
    const int ValueOther = 401;

    protected readonly PlainStruct _plainStructKeyOther = new(TypeOther, ValueOther);
    protected readonly EquatableStruct _equatableStructKeyOther = new(TypeOther, ValueOther);
    protected readonly HashStruct _hashStructKeyOther = new(TypeOther, ValueOther);
    protected readonly HashEquatableStruct _hashEquatableStructKeyOther = new(TypeOther, ValueOther);
    protected readonly (Type, int) _valueTupleKeyOther = (TypeOther, ValueOther);
    protected readonly RecordStruct _recordStructKeyOther = new(TypeOther, ValueOther);
    protected readonly HashEquatableRecordStruct _hashEquatableRecordStructKeyOther = new(TypeOther, ValueOther);

    [Benchmark(Baseline = true)]
    public bool PlainStruct_Equals() => _plainStructKey.Equals(_plainStructKeyOther);

    [Benchmark]
    public bool EquatableStruct_Equals() => _equatableStructKey.Equals(_equatableStructKeyOther);

    [Benchmark]
    public bool HashStruct_Equals() => _hashStructKey.Equals(_hashStructKeyOther);

    [Benchmark]
    public bool HashEquatableStruct_Equals() => _hashEquatableStructKey.Equals(_hashEquatableStructKeyOther);

    [Benchmark]
    public bool ValueTuple_Equals() => _valueTupleKey.Equals(_valueTupleKeyOther);

    [Benchmark]
    public bool RecordStruct_Equals() => _recordStructKey.Equals(_recordStructKeyOther);

    [Benchmark]
    public bool HashEquatableRecordStruct_Equals() => _hashEquatableRecordStructKey.Equals(_hashEquatableRecordStructKeyOther);
}

public class DictionaryBench : BaseBench
{
    static readonly (Type Type, int Value)[] Keys = new (Type Type, int Value)[]
    {
        (typeof(byte), 101),
        (typeof(sbyte), 102),
        (typeof(ushort), 201),
        (typeof(short), 202),
        (typeof(uint), 301),
        (typeof(int), 302),
        (typeof(ulong), 401),
        (typeof(long), 402),
    };

    readonly Dictionary<PlainStruct, long> _plainKeyDictionary =
        Keys.ToDictionary(k => new PlainStruct(k.Type, k.Value), k => k.Value * 1L);

    readonly Dictionary<EquatableStruct, long> _equatableKeyDictionary =
        Keys.ToDictionary(k => new EquatableStruct(k.Type, k.Value), k => k.Value * 1L);

    readonly Dictionary<HashStruct, long> _hashKeyDictionary =
        Keys.ToDictionary(k => new HashStruct(k.Type, k.Value), k => k.Value * 1L);

    readonly Dictionary<HashEquatableStruct, long> _hashEquatableKeyDictionary =
        Keys.ToDictionary(k => new HashEquatableStruct(k.Type, k.Value), k => k.Value* 1L);

    readonly Dictionary<(Type, int), long> _valueTupleToValue =
        Keys.ToDictionary(k => k, k => k.Value* 1L);

    readonly Dictionary<RecordStruct, long> _recordKeyDictionary =
        Keys.ToDictionary(k => new RecordStruct(k.Type, k.Value), k => k.Value* 1L);

    readonly Dictionary<HashEquatableRecordStruct, long> _customRecordKeyDictionary =
        Keys.ToDictionary(k => new HashEquatableRecordStruct(k.Type, k.Value), k => k.Value * 1L);

    [Benchmark(Baseline = true)]
    public long PlainStruct_DictionaryGet() => _plainKeyDictionary[_plainStructKey];

    [Benchmark]
    public long EquatableStruct_DictionaryGet() => _equatableKeyDictionary[_equatableStructKey];

    [Benchmark]
    public long HashStruct_DictionaryGet() => _hashKeyDictionary[_hashStructKey];

    [Benchmark]
    public long HashEquatableStruct_DictionaryGet() => _hashEquatableKeyDictionary[_hashEquatableStructKey];

    [Benchmark]
    public long ValueTuple_DictionaryGet() => _valueTupleToValue[_valueTupleKey];

    [Benchmark]
    public long RecordStruct_DictionaryGet() => _recordKeyDictionary[_recordStructKey];

    [Benchmark]
    public long HashEquatableRecordStruct_DictionaryGet() => _customRecordKeyDictionary[_hashEquatableRecordStructKey];
}