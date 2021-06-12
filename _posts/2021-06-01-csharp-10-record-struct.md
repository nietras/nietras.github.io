---
layout: post
title: C# 10 - `record struct` Deep Dive & Performance Implications
---

what is record struct? show case what it does? what code is generated?

some may argue that record struct doesn't provide much value? There is not much new
functionality as such

why is it important? Performance, IEquality and GetHashCode!


## Problem
[Performance implications of default struct equality in C#](https://devblogs.microsoft.com/premier-developer/performance-implications-of-default-struct-equality-in-c/)
* The default equality implementation for structs may easily cause a severe performance impact for your application. The issue is real, not a theoretical one.
* The default equality members for value types are reflection-based.
* The default `GetHashCode` implementation may provide a very poor distribution if a first field of many instances is the same.
* There is an optimized default version for `Equals` and `GetHashCode` but you should never rely on it because you may stop hitting it with an innocent code change.
* You may rely on FxCop rule to make sure that every struct overrides equality members, but a better approach is to catch the issue when the “wrong” struct is stored in a hash set or in a hash table using an analyzer.

## Example - struct as key in dictionary

### Setup
Compilers.Toolset

BenchmarkDotNet

https://github.com/dotnet/roslyn
nuget source:
https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json


```csharp
using System;

var rs = new RecordStruct(typeof(string), 42);
Console.WriteLine(rs);

public record struct RecordStruct(Type Type, int Value);
```


[sharplab](https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLRIMYHsEBNXIwJwYzIA+AAgEwCMAsAFBMBuUCABAshwLwcA7CAHcOAJQjY8AZSIkYAChgBPAA4QsYBZVoAGAJQAaDgBZq+gNxMdATgXdLTawGYuknLg6Fipce5lypAoAKmoQHKHqxgCWAjAcAGpQADZwEI6MQA===)

```csharp
[IsReadOnly]
public struct PlainStruct
{
    [CompilerGenerated]
    private readonly Type <Type>k__BackingField;

    [CompilerGenerated]
    private readonly int <Value>k__BackingField;

    public Type Type
    {
        [CompilerGenerated]
        get
        {
            return <Type>k__BackingField;
        }
    }

    public int Value
    {
        [CompilerGenerated]
        get
        {
            return <Value>k__BackingField;
        }
    }

    public PlainStruct(Type type, int value)
    {
        <Type>k__BackingField = type;
        <Value>k__BackingField = value;
    }
}
```

```csharp
public struct RecordStruct : IEquatable<RecordStruct>
{
    [CompilerGenerated]
    private Type <Type>k__BackingField;

    [CompilerGenerated]
    private int <Value>k__BackingField;

    public Type Type
    {
        [IsReadOnly]
        [CompilerGenerated]
        get
        {
            return <Type>k__BackingField;
        }
        [CompilerGenerated]
        set
        {
            <Type>k__BackingField = value;
        }
    }

    public int Value
    {
        [IsReadOnly]
        [CompilerGenerated]
        get
        {
            return <Value>k__BackingField;
        }
        [CompilerGenerated]
        set
        {
            <Value>k__BackingField = value;
        }
    }

    public RecordStruct(Type Type, int Value)
    {
        <Type>k__BackingField = Type;
        <Value>k__BackingField = Value;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("RecordStruct");
        stringBuilder.Append(" { ");
        if (PrintMembers(stringBuilder))
        {
            stringBuilder.Append(" ");
        }
        stringBuilder.Append("}");
        return stringBuilder.ToString();
    }

    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append("Type");
        builder.Append(" = ");
        builder.Append(Type);
        builder.Append(", ");
        builder.Append("Value");
        builder.Append(" = ");
        builder.Append(Value.ToString());
        return true;
    }

    public static bool operator !=(RecordStruct left, RecordStruct right)
    {
        return !(left == right);
    }

    public static bool operator ==(RecordStruct left, RecordStruct right)
    {
        return left.Equals(right);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<Type>.Default.GetHashCode(<Type>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(<Value>k__BackingField);
    }

    public override bool Equals(object obj)
    {
        if (obj is RecordStruct)
        {
            return Equals((RecordStruct)obj);
        }
        return false;
    }

    public bool Equals(RecordStruct other)
    {
        if (EqualityComparer<Type>.Default.Equals(<Type>k__BackingField, other.<Type>k__BackingField))
        {
            return EqualityComparer<int>.Default.Equals(<Value>k__BackingField, other.<Value>k__BackingField);
        }
        return false;
    }

    public void Deconstruct(out Type Type, out int Value)
    {
        Type = this.Type;
        Value = this.Value;
    }
}
```


You can see a status table of C# language features on GitHub at 
[Language Feature Status](https://github.com/dotnet/roslyn/blob/master/docs/Language%20Feature%20Status.md).

