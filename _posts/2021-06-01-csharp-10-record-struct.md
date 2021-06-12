---
layout: post
title: C# 10 - `record struct` Deep Dive & Performance Implications
---
In this blog post I will do a quick deep dive into the new 
[`record struct`](https://github.com/dotnet/csharplang/issues/4334) 
being introduced in the upcoming C# 10 and look at the performance implications
of this in a specific context (**20x faster and infinitely less GC pressure** than
a plain `struct`). I will cover:

 * Code generated for `record struct`
 * Importance of the generated code
 * Problem with plain `struct`s
 * Example use case
 * Setup project to use preview compiler via `Microsoft.Net.Compilers.Toolset` nuget package
 * Benchmarks covering common pitfalls

## `record struct`
With `record struct` you can take a plain struct like:
```csharp
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
```
and simplify this to just one line:
```csharp
public record struct RecordStruct(Type Type, int Value);
```
but what does this actually do? Let's first look at what the `PlainStruct` 
looks like in raw form:
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
Pretty straightforward. The compiler generates backing fields for the properties 
and that's about it.

However, for the `record struct` the raw form is:
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

wait... this has `set` properties? Aren't records supposed to be immutable? Yes.
However, C# 9.0 introduced [init only setters](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#init-only-setters) 
for properties, which is basically just a
trick around marking the property as `IsReadOnly` and then still having `set`.
The compiler then just enforces that if the property has the `IsReadOnly`
attribute you can only set the property in an initializer like:
```csharp
var rs = new RecordStruct { Type = typeof(short), Value = 42 };
```
However, to sum up the generated code for this `record struct` has:
 * Backing fields for properties
 * `get` and `init` for properties
 * Constructor matching the properties
 * Custom overridden `ToString()` implementation based on `StringBuilder`
 * Implements `IEquality<RecordStruct>` as value based comparison
 * Equality operators `!=` and `==` that forward to `IEquality<RecordStruct>.Equals`
 * Custom overridden `bool Equals(object obj)` that forwards to `IEquality<RecordStruct>.Equals`
 * Custom `GetHashCode()` with default decent hash combination
 * A `Deconstruct` method for easy deconstruction i.e. you can write
   ```csharp
   var (type, value) = rs;
   ```

That is quite a lot. But doesn't a plain struct support a lot of this already?
Yes, you can write similar code with the two but the output of the operations
differ which can be seen in the below table:

INSERT TABLE

|Operation |`PlainStruct` |`RecordStruct`|Notes|
|---------|------|------|------|
|`x.ToString()`|`PlainStruct`|`RecordStruct { Type = System.String, Value = 42 }`| |
|`x == y`|N/A|`false`| |
|`x != y`|N/A|`true`| |
|`x == x`|N/A|`true`| |

where `x` and `y` are defined as, respectively.
```csharp
var x = new PlainStruct(typeof(string), 42);
var y = new PlainStruct(typeof(long), 17);
// OR
var x = new RecordStruct(typeof(string), 42);
var y = new RecordStruct(typeof(long), 17);
```



? How can that be? 


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




You can see a status table of C# language features on GitHub at 
[Language Feature Status](https://github.com/dotnet/roslyn/blob/master/docs/Language%20Feature%20Status.md).

```ini
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.985 (21H1/May2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=5.0.300
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  Job-CTGYNB : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT

Runtime=.NET 5.0  Toolchain=netcoreapp50
```

|               Method |      Mean |    Error |   StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |----------:|---------:|---------:|------:|-------:|------:|------:|----------:|
|         PlainStruct_ | 227.06 ns | 2.671 ns | 2.498 ns |  1.00 | 0.0110 |     - |     - |     184 B |
|     EquatableStruct_ |  43.22 ns | 0.193 ns | 0.171 ns |  0.19 | 0.0019 |     - |     - |      32 B |
|          HashStruct_ | 177.89 ns | 1.119 ns | 0.874 ns |  0.78 | 0.0091 |     - |     - |     152 B |
| HashEquatableStruct_ |  10.65 ns | 0.101 ns | 0.089 ns |  0.05 |      - |     - |     - |         - |
|          ValueTuple_ |  21.24 ns | 0.417 ns | 0.370 ns |  0.09 |      - |     - |     - |         - |
|        RecordStruct_ |  10.91 ns | 0.111 ns | 0.098 ns |  0.05 |      - |     - |     - |         - |