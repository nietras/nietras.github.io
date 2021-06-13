---
layout: post
title: C# 10 - `record struct` Deep Dive & Performance Implications
---
In this blog post I will do a quick deep dive into the new 
[`record struct`](https://github.com/dotnet/csharplang/issues/4334) 
being introduced in the upcoming C# 10 and look at the performance implications
of this in a specific context (**20x faster and 100% less allocations** than
a plain `struct`). I will cover:

 * Code generated for `record struct`
 * Importance of the generated code
 * Performance implications of default struct equality in C#
 * Setup project to use preview compiler via `Microsoft.Net.Compilers.Toolset` nuget package
 * Types and implementations covering different possibilities and common pitfalls
 * Benchmarks

## `record struct`
With `record struct` you can take a plain struct like 
(this is just an example but note that `Type` is a reference type/`class`):
```csharp
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
```
and simplify this to just one line:
```csharp
public readonly record struct RecordStruct(Type Type, int Value);
```
Note how the `record struct` has `readonly` in front. This is because
currently `record struct` unlike `record class` is not immutable by
default. This is probably to conform with the existing convention of
`readonly struct` vs `struct` similarly with `readonly record struct`
and `record struct`, which makes sense but is a bit contradictory to
a normal reference type `record`.

But what do we get with `record struct`? 

Let's first look at what the `PlainStruct` looks like in raw form:
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
        [CompilerGenerated]
        init
        {
            <Type>k__BackingField = value;
        }
    }

    public int Value
    {
        [CompilerGenerated]
        get
        {
            return <Value>k__BackingField;
        }
        [CompilerGenerated]
        init
        {
            <Value>k__BackingField = value;
        }
    }

    public PlainStruct(Type type, int value)
    {
        Type = type;
        Value = value;
    }
}
```
Pretty straightforward. The compiler generates backing fields for the properties 
and in this case both getters and init setters.

However, for the `record struct` the raw form is:
```csharp
[IsReadOnly]
public struct RecordStruct : IEquatable<RecordStruct>
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
        [CompilerGenerated]
        init
        {
            <Type>k__BackingField = value;
        }
    }

    public int Value
    {
        [CompilerGenerated]
        get
        {
            return <Value>k__BackingField;
        }
        [CompilerGenerated]
        init
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
        return EqualityComparer<Type>.Default.GetHashCode(<Type>k__BackingField) * -1521134295 
            + EqualityComparer<int>.Default.GetHashCode(<Value>k__BackingField);
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
All of this can be easily inspected using [sharplab.io](https://sharplab.io/) by following
this [link](https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLRIMYHsEBNXIwJwYzIA+AAgEwCMAsAFBMBuUCABAshwLwcA7CAHcOAJQjY8AZSIkYAChgBPAA4QsYBZVoAGAJQAaDgBZq+gNxMdATgXdLTawGYu0XFgEAbZR0LFSDgAFLygASwFZAJgmAG8mDkSOSlcQ8Mi5UgUAFTUIDhV1YwiYDjYvOAh9BKT4xiSGjlz1PgK8q3rGxIA1KAr8/nLKjoaAXydOxJSmvJmW2I4IsJgLDgBzCBWOccnk1xKOXv6OBaWtja2dnZc3KVw/TNKJO6j5HNnmiGKBUqPKx0YQA),
where I have selected the **C# Next: Record structs (22 Apr 2021)** compiler.

To sum up the generated code for this `record struct` has:
 * Backing fields for properties
 * `get` and `init` for properties
 * Constructor matching the properties
 * Custom overridden `ToString()` implementation based on `StringBuilder`
 * Implements `IEquality<RecordStruct>` as value based comparison 
   based on `EqualityComparer<T>.Default`
 * Equality operators `!=` and `==` that forward to `IEquality<RecordStruct>.Equals`
 * Custom overridden `bool Equals(object obj)` that forwards to `IEquality<RecordStruct>.Equals`
 * Custom `GetHashCode()` with default decent hash combination
   based on `EqualityComparer<T>.Default`
 * A `Deconstruct` method for easy deconstruction i.e. you can write
   ```csharp
   var (type, value) = rs;
   ```

That is quite a lot. But doesn't a plain struct support some of this already?
Yes some and you can write similar code with the two but the output of 
the operations differ which can be seen in the below table,
where `x`, `y` and `z` are defined as:
```csharp
var x = new PlainStruct(typeof(string), 42);
var y = new PlainStruct(typeof(string), 17);
var z = new PlainStruct(typeof(long), 17);
// OR
var x = new RecordStruct(typeof(string), 42);
var y = new RecordStruct(typeof(string), 17);
var z = new RecordStruct(typeof(long), 17);
```

|Operation |PlainStruct |RecordStruct|
|---------|------:|------:|
|`x.ToString()`|`PlainStruct`|`RecordStruct { Type = System.String, Value = 42 }`|
|`x.Equals(y)`|`false`¹|`false`|
|`x == y`|N/A|`false`|
|`x != y`|N/A|`true`|
|`x == x`|N/A|`true`|
|`x.GetHashCode()`|`-1121861486`²|`-2044458748`|
|`y.GetHashCode()`|`-1121861486`²|`-2044458773`|
|`z.GetHashCode()`|`-1117405627`|`725897014`|

¹ `PlainStruct` will box `y` on every call here since the only method available
  is the default `bool Equals(object other)`. This can be surprising to some.

² Note how `PlainStruct` returns the same hash code for `x` and `y`.
  `RecordStruct` on the other hand returns different hash codes.
  We will get back to that in the next section.

Only the `RecordStruct` supports deconstruction out-of-the-box,
but both support initializer and `with` use (if the plain struct has `init`s)
so you can write:
```csharp
var i = new PlainStruct { Type = typeof(byte) };
var j = i with { Value = 3 };
```
or
```csharp
var i = new RecordStruct { Type = typeof(byte) };
var j = i with { Value = 3 };
```

Notice in the above that this allows creating the type and only setting
one of the properties. If both should be set this should in the future 
be able to be enforced with the new C# 10 keyword `required`.
```csharp
public required Type Type { init; get; }
public required int Value { init; get; }
```

As you can probably tell from the above already there are some key differences
between a plain `struct` and `record struct`, but there is more to this
than just functionality. And the key here is that a `record struct`
implements `IEquality<T>` and overrides `int GetHashCode()` with
a good default implementation whereas `struct` does not.

## Performance implications of default struct equality in C#
In [Performance implications of default struct equality in C#](https://devblogs.microsoft.com/premier-developer/performance-implications-of-default-struct-equality-in-c/)
by [Sergey Tepliakov](https://twitter.com/STeplyakov) the issues around
`struct` are covered in detail with regards to both default equality and hash codes. 
The following key points can be summarized from the post:

 * If a struct does not provide `Equals` and `GetHashCode`, 
   then the default versions of these methods from `System.ValueType` are used. 
 * The default `GetHashCode` version just returns a hash code 
   of a first non-null field and “munges” it with a type id
   * If the first field is always the same, the default hash function 
     returns the same value for all the elements. This effectively 
     transforms a hash set into a linked list with O(N) for insertion 
     and lookup operations. And the operation that populates the 
     collection becomes O(N^2) (N insertions with O(N) complexity per insertion).
 * Both `Equals` and `GetHashCode` have reflection-based implementations
   if the optimized default version is not applied. This means they are very slow.
   * The optimized version will only be used, if the value type has no 
     references and is properly packed (no padding between members).
   * The optimized Equals is based on comparing bytes directly,
     but for example `double` -0.0 and +0.0 are equal, 
     yet have different binary representations.
 * The default equality and hash code implementation for structs may easily 
   cause a severe performance impact for your application. 
   The issue is real, not a theoretical one.

This is why it is so important that for `record struct` the 
compiler generates code for these instead, as it is quite common
to have value types with references and few developers ensure there
is no padding in their value types. As I will show next this
has a major impact on performance since  `record struct` avoids 
the "possibly" reflection-based versions and implements the 
`IEquality<T>` interface avoiding the boxings as mentioned above,
but first we need to be able to use `record struct`.

## Setup
Fortunately, it is very easy to use preview versions of 
[Roslyn](https://github.com/dotnet/roslyn) - the .NET compiler - via
the nuget package [`Microsoft.Net.Compilers.Toolset`](https://www.nuget.org/packages/Microsoft.Net.Compilers.Toolset/).
We just need to get the latest preview version of this package from the
nuget feed [dotnet-tools](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json).
This we can either add to nuget feeds in Visual Studio or simply add
a `nuget.config` next to our solution.

So to test `record struct` I created a new C# console project and
added a few files to end up with:
```
nuget.config
Program.cs
RecordStructBenchmark.csproj
RecordStructBenchmark.sln
```
where `nuget.config` is:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="dotnet-tools" value="https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json" />
  </packageSources>
</configuration>
```
which contains the nuget feed and allows us to install the latest compilers toolset
and hence our project file ends up as:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net5.0</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>
  
  <PropertyGroup>
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <DebugSymbols>true</DebugSymbols>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Optimize>true</Optimize>
    <Configuration>Release</Configuration>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" Version="0.13.0" />
    <PackageReference Include="BenchmarkDotNet.Diagnostics.Windows" Version="0.13.0" />
    <PackageReference Include="Microsoft.Net.Compilers.Toolset" Version="4.0.0-2.21310.45">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```
This is based on the recommended definitions for use with 
[`BenchmarkDotNet`](https://github.com/dotnet/BenchmarkDotNet), 
which has also been added as a nuget package to the project.

Note that `<LangVersion>preview</LangVersion>` which means
we get the latest C# preview compiler with whatever features are
available in the compilers toolset package. This property was not
supported in BenchmarkDotnet 0.12.1, but luckily this was fixed in
0.13.0, so be sure to use that or a later version .

## Types
To fully examine the set of possibilities given the default behavior of `struct`s,
we need to cover both whether it is plain `struct` or `record struct` with or
without manual/custom implementations of `IEquality<T>` and/or `GetHashCode`.
To do this I created the following types:

 * `PlainStruct` - plain struct with no custom equality or hash code.
 * `EquatableStruct` - plain struct which implements `IEquatable<EquatableStruct`.
 * `HashStruct` - plain struct which .
 * `HashEquatableStruct` - plain struct which implements both `IEquatable<EquatableStruct` and overrides `GetHashCode`.
 * `RecordStruct` - straightforward `record struct` as discussed above.
 * `HashEquatableRecordStruct` - `record struct` which implements both `IEquatable<EquatableStruct` and overrides `GetHashCode`.

as shown in the code below:
```csharp
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
```
Above I am using the exact same hash method as the `record struct`
to make sure they are comparable. However, if you do your own
`GetHashCode()` prefer using `HashCode.Combine` 
(if you target a platform where this is available):
```csharp
HashCode.Combine(Type.GetHashCode(), Value.GetHashCode());
```


## Benchmarks
To examing the performance implications of the different type implementations 
we will look at three benchmarks:

 * `Equals` - 
 * `GetHashCode` - 
 * `DictionaryGet` - 

```
dotnet run -c Release -f net5.0 -- -m -d --runtimes netcoreapp50 --filter *
```

[sharplab](https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLRIMYHsEBNXIwJwYzIA+AAgEwCMAsAFBMBuUCABAshwLwcA7CAHcOAJQjY8AZSIkYAChgBPAA4QsYBZVoAGAJQAaDgBZq+gNxMdATgXdLTawGYuknLg6Fipce5lypAoAKmoQHKHqxgCWAjAcAGpQADZwEI6MQA===)


```ini
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.985 (21H1/May2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=5.0.300
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  Job-CTGYNB : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT

Runtime=.NET 5.0  Toolchain=netcoreapp50
```

|               Method |      Mean |    Error |   StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------:|----------:|---------:|---------:|------:|-------:|------:|------:|----------:|
|         PlainStruct_ | 227.06 ns | 2.671 ns | 2.498 ns |  1.00 | 0.0110 |     - |     - |     184 B |
|     EquatableStruct_ |  43.22 ns | 0.193 ns | 0.171 ns |  0.19 | 0.0019 |     - |     - |      32 B |
|          HashStruct_ | 177.89 ns | 1.119 ns | 0.874 ns |  0.78 | 0.0091 |     - |     - |     152 B |
| HashEquatableStruct_ |  10.65 ns | 0.101 ns | 0.089 ns |  0.05 |      - |     - |     - |         - |
|          ValueTuple_ |  21.24 ns | 0.417 ns | 0.370 ns |  0.09 |      - |     - |     - |         - |
|        RecordStruct_ |  10.91 ns | 0.111 ns | 0.098 ns |  0.05 |      - |     - |     - |         - |


## Conclusion

You can see a status table of C# language features on GitHub at 
[Language Feature Status](https://github.com/dotnet/roslyn/blob/master/docs/Language%20Feature%20Status.md).


## Appendix: `record struct` default mutable
As an appendix here I show what the default `record struct` looks like
if it is not marked as `readonly` e.g.:
```csharp
public record struct RecordStruct(Type Type, int Value);
```
The raw form can be seen below. As can be seen this gets
setters for both properties.
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


```csharp
[IsReadOnly]
public struct CustomRecordStruct : IEquatable<CustomRecordStruct>
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
        [CompilerGenerated]
        init
        {
            <Type>k__BackingField = value;
        }
    }

    public int Value
    {
        [CompilerGenerated]
        get
        {
            return <Value>k__BackingField;
        }
        [CompilerGenerated]
        init
        {
            <Value>k__BackingField = value;
        }
    }

    public CustomRecordStruct(Type Type, int Value)
    {
        <Type>k__BackingField = Type;
        <Value>k__BackingField = Value;
    }

    public bool Equals(CustomRecordStruct other)
    {
        if (Type.Equals(other.Type))
        {
            return Value.Equals(other.Value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode() * -1521134295 + Value.GetHashCode();
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("CustomRecordStruct");
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

    public static bool operator !=(CustomRecordStruct left, CustomRecordStruct right)
    {
        return !(left == right);
    }

    public static bool operator ==(CustomRecordStruct left, CustomRecordStruct right)
    {
        return left.Equals(right);
    }

    public override bool Equals(object obj)
    {
        if (obj is CustomRecordStruct)
        {
            return Equals((CustomRecordStruct)obj);
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

[//]: # Superscript/subscript unicodes https://gist.github.com/molomby/9bc092e4a125f529ae362de7e46e8176
[//]: # https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLRIMYHsEBNXIwJwYzIA+AAgEwCMAsAFBMBuUCABAshwLwcA7CAHcOAJQjY8AZSIkYAChgBPAA4QsYBZVoAGAJQAaDgBZq+gNxMdATgXdLTawGYu0XFgEAbZR0LFSDgAFLygASwFZAJgmAG8mDkSOSlcQ8Mi5UgUAFTUIDhV1YwiYDjYvOAh9BKT4xiSGjlz1PgK8q3rGxIA1KAr8/nLKjoaAXydOxJSmvJmW2I4IsJgLDgBzCBWOccnk1xKOXv6OBaWtja2dnZc3KVw/TNKJO6j5HNnmiGKBUqPKx2YjGmSCgHm8vkwOHu/nkHAAwnBCFgALbPKGvLKfOZfRY/Q59f4cGqJEAcACSAFEAI5wWBQYBeCAAHgRSNRknRjwAfHFiXsOMAsFgvBxqbSvMgFKyYCi0TJHhwsDAABYQBD6Pg83YNT4AOjFfUlStVCF1nw1ADILfj+vqaYaFMa1bq/lUOnzplgWGqEGFcPkDgBxTYACSgyGVcKw/oUGt4Wq6iT1wZgYYjUZjGoAVBxULQAKx0WjOMw2fMcADUNsqupTacj0YgsY6oyAA