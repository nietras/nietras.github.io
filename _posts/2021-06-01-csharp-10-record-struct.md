---
layout: post
title: C# 10 - `record struct` Deep Dive & Performance Implications
---


https://github.com/dotnet/roslyn
nuget source:
https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json

revisiting example from Build 2020 and C# 9 we can write:
```csharp
global using System;

var p = new Point2D { X = 1, Y = 2 };
var q = p with { X = 3 };
Point2D? r = q.Equals(p) ? p : null;
Console.WriteLine(r);

public record struct Point2D(nint X, nint Y);
```
[sharplab](https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLRIMYHsEBNXIwJwYzIA+AAgEwCMAsAFBMBuUCABAA4cC8HAOwgB3DgAUsASwExqAEQ4BvDgA0+HWgBoOATXXUOAXwDcrdhwCO6nsMkwAFktXqAzEdOMJ02XID8HTn4LADoAUQs4KAAbZAAKLgBKDn8eEEE4KKiPSloATliEBI8mSjdMHFwOQmJScSkZeViBb1VtZpldIqA===)
```csharp
public struct Point2D : IEquatable<Point2D>
{
    [CompilerGenerated]
    private nint <X>k__BackingField;

    [CompilerGenerated]
    private nint <Y>k__BackingField;

    public nint X
    {
        [IsReadOnly]
        [CompilerGenerated]
        get
        {
            return <X>k__BackingField;
        }
        [CompilerGenerated]
        set
        {
            <X>k__BackingField = value;
        }
    }

    public nint Y
    {
        [IsReadOnly]
        [CompilerGenerated]
        get
        {
            return <Y>k__BackingField;
        }
        [CompilerGenerated]
        set
        {
            <Y>k__BackingField = value;
        }
    }

    public Point2D(nint X, nint Y)
    {
        <X>k__BackingField = X;
        <Y>k__BackingField = Y;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Point2D");
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
        builder.Append("X");
        builder.Append(" = ");
        builder.Append(((IntPtr)X).ToString());
        builder.Append(", ");
        builder.Append("Y");
        builder.Append(" = ");
        builder.Append(((IntPtr)Y).ToString());
        return true;
    }

    public static bool operator !=(Point2D left, Point2D right)
    {
        return !(left == right);
    }

    public static bool operator ==(Point2D left, Point2D right)
    {
        return left.Equals(right);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<IntPtr>.Default.GetHashCode(<X>k__BackingField) * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(<Y>k__BackingField);
    }

    public override bool Equals(object obj)
    {
        if (obj is Point2D)
        {
            return Equals((Point2D)obj);
        }
        return false;
    }

    public bool Equals(Point2D other)
    {
        if (EqualityComparer<IntPtr>.Default.Equals(<X>k__BackingField, other.<X>k__BackingField))
        {
            return EqualityComparer<IntPtr>.Default.Equals(<Y>k__BackingField, other.<Y>k__BackingField);
        }
        return false;
    }

    public void Deconstruct(out nint X, out nint Y)
    {
        X = this.X;
        Y = this.Y;
    }
}
```


https://devblogs.microsoft.com/premier-developer/performance-implications-of-default-struct-equality-in-c/
* The default equality implementation for structs may easily cause a severe performance impact for your application. The issue is real, not a theoretical one.
* The default equliaty members for value types are reflection-based.
* The default `GetHashCode` implementation may provide a very poor distribution if a first field of many instances is the same.
* There is an optimized default version for `Equals` and `GetHashCode` but you should never rely on it because you may stop hitting it with an innocent code change.
* You may rely on FxCop rule to make sure that every struct overrides equality members, but a better approach is to catch the issue when the “wrong” struct is stored in a hash set or in a hash table using an analyzer.

You can see a status table of C# language features on GitHub at 
[Language Feature Status](https://github.com/dotnet/roslyn/blob/master/docs/Language%20Feature%20Status.md).

