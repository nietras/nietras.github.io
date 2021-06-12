using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;

namespace RecordStructBenchmark
{

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
}
