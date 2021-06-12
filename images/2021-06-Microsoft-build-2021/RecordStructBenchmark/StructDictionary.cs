using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;

namespace RecordStructBenchmark
{
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

    public record struct RecordStruct(Type Type, int Value);

    // https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLRIMYHsEBNXIwJwYzIA+AAgEwCMAsAFBMBuUCABAA4cC8HAOwgB3DgAUsASwExqAEQ4BvDgA0+HWgBoOATXXUOAXwDcrdhwCO6nsMkwAFktXqAzEdOMJ02XID8HTn4LADoAUQs4KAAbZAAKLgBKDn8eEEE4KKiPSloATliEBI8mSjdMHFwOQmJScSkZeViBb1VtZpldIqYSsohsPCqiEhgOACU+ioBlIdJYgBUATy4IDkXl7RaANWi4CC7GIA
    //public struct RecordStruct : IEquatable<RecordStruct>
    //{
    //    [CompilerGenerated]
    //    private Type _type_k__BackingField;

    //    [CompilerGenerated]
    //    private int _value_k__BackingField;

    //    public Type Type
    //    {
    //        //[IsReadOnly]
    //        [CompilerGenerated]
    //        get
    //        {
    //            return _type_k__BackingField;
    //        }
    //        [CompilerGenerated]
    //        set
    //        {
    //            _type_k__BackingField = value;
    //        }
    //    }

    //    public int Value
    //    {
    //        //[IsReadOnly]
    //        [CompilerGenerated]
    //        get
    //        {
    //            return _value_k__BackingField;
    //        }
    //        [CompilerGenerated]
    //        set
    //        {
    //            _value_k__BackingField = value;
    //        }
    //    }

    //    public RecordStruct(Type Type, int Value)
    //    {
    //        _type_k__BackingField = Type;
    //        _value_k__BackingField = Value;
    //    }

    //    public override string ToString()
    //    {
    //        StringBuilder stringBuilder = new StringBuilder();
    //        stringBuilder.Append("RecordStruct");
    //        stringBuilder.Append(" { ");
    //        if (PrintMembers(stringBuilder))
    //        {
    //            stringBuilder.Append(" ");
    //        }
    //        stringBuilder.Append("}");
    //        return stringBuilder.ToString();
    //    }

    //    private bool PrintMembers(StringBuilder builder)
    //    {
    //        builder.Append("Type");
    //        builder.Append(" = ");
    //        builder.Append(Type);
    //        builder.Append(", ");
    //        builder.Append("Value");
    //        builder.Append(" = ");
    //        builder.Append(Value.ToString());
    //        return true;
    //    }

    //    public static bool operator !=(RecordStruct left, RecordStruct right)
    //    {
    //        return !(left == right);
    //    }

    //    public static bool operator ==(RecordStruct left, RecordStruct right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return EqualityComparer<Type>.Default.GetHashCode(_type_k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(_value_k__BackingField);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is RecordStruct)
    //        {
    //            return Equals((RecordStruct)obj);
    //        }
    //        return false;
    //    }

    //    public bool Equals(RecordStruct other)
    //    {
    //        if (EqualityComparer<Type>.Default.Equals(_type_k__BackingField, other._type_k__BackingField))
    //        {
    //            return EqualityComparer<int>.Default.Equals(_value_k__BackingField, other._value_k__BackingField);
    //        }
    //        return false;
    //    }

    //    public void Deconstruct(out Type Type, out int Value)
    //    {
    //        Type = this.Type;
    //        Value = this.Value;
    //    }
    //}

    public class StructDictionary
    {
        Dictionary<Struct, long> _structToValue = new ()
        {
            { new Struct(typeof(byte), 101), 0},
            { new Struct(typeof(sbyte), 102), 1},
            { new Struct(typeof(ushort), 201), 2},
            { new Struct(typeof(short), 202), 3},
            { new Struct(typeof(uint), 301), 4},
            { new Struct(typeof(int), 302), 5},
        };

        Dictionary<(Type, int), long> _valueTupleToValue = new ()
        {
            { (typeof(byte), 101), 0},
            { (typeof(sbyte), 102), 1},
            { (typeof(ushort), 201), 2},
            { (typeof(short), 202), 3},
            { (typeof(uint), 301), 4},
            { (typeof(int), 302), 5},
        };

        Dictionary<RecordStruct, long> _recordStructToValue = new()
        {
            { new (typeof(byte), 101), 0 },
            { new (typeof(sbyte), 102), 1 },
            { new (typeof(ushort), 201), 2 },
            { new (typeof(short), 202), 3 },
            { new (typeof(uint), 301), 4 },
            { new (typeof(int), 302), 5 },
        };

        readonly Struct _structKey = new Struct(typeof(ushort), 201);
        readonly RecordStruct _recordStructKey = new RecordStruct(typeof(ushort), 201);
        readonly (Type, int) _valueTupleKey = (typeof(ushort), 201);

        [Benchmark(Baseline = true)]
        public long Struct_() => _structToValue[_structKey];

        [Benchmark()]
        public long RecordStruct_() => _recordStructToValue[_recordStructKey];

        [Benchmark]
        public long ValueTuple_() => _valueTupleToValue[_valueTupleKey];
    }
}
