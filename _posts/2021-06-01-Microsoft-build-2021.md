---
layout: post
title: Digest - Microsoft Build 2021
---

Digest of the free virtual event at 
[https://mybuild.microsoft.com/home](https://mybuild.microsoft.com/home)
held May 25-27, 2021.

Previous year digest can be found at 
[Digest - Microsoft Build 2020]({{ site.baseurl }}/2020/06/07/Microsoft-build-2021/)
for more content 😁.

## Announcements and Blog Posts
A few links can be found below.

***
### [Announcing .NET 6 Preview 4](https://devblogs.microsoft.com/dotnet/announcing-net-6-preview-4/)
**Summary:**  Great, long and detailed post about everything .NET 6 Preview 4 by 
[Rich Lander](https://twitter.com/runfaster2000). To pick one thing to highlight
the `FileStream` performance on Windows has been significantly improved based on
work by [Ben \{chmark\} Adams](https://twitter.com/ben_a_adams), [Adam Sitnik](https://twitter.com/SitnikAdam) and more. Results speak for themselves,
see the blog post for details.

|Method     | Runtime  |      Mean | Ratio | Allocated
|---------- | -------- | ---------:| -----:| ---------:
|ReadAsync  | .NET 5.0 |  3.785 ms |  1.00 |     39 KB
|ReadAsync  | .NET 6.0 |  1.762 ms |  0.47 |      1 KB
|           |          |           |       |          
|WriteAsync | .NET 5.0 | 12.573 ms |  1.00 |     39 KB
|WriteAsync | .NET 6.0 |  3.200 ms |  0.25 |      1 KB

Oh and this release also introduces the `DateOnly` and `TimeOnly` structs. 👍

***
### [Announcing .NET MAUI Preview 4](https://devblogs.microsoft.com/dotnet/announcing-net-maui-preview-4/)
**Summary:** [David Ortinau](https://twitter.com/davidortinau) announces 
the availability of .NET Multi-platform App UI (.NET MAUI) Preview 4, 
which now has enough building blocks to build functional applications 
for all supported platforms (incl. Windows), new capabilities to support 
running Blazor on the desktop, and exciting progress in Visual Studio 
to support .NET MAUI (e.g. .NET Hot Reload 🔥) see next link.

***
### [Introducing the .NET Hot Reload experience for editing code at runtime](https://devblogs.microsoft.com/dotnet/introducing-net-hot-reload/)
**Summary:** [Dmitry Lyalin](https://twitter.com/LyalinDotCom) introduces you 
to the availability of the .NET Hot Reload experience in Visual Studio 2019 
version 16.11 (Preview 1) and through the dotnet watch command-line tooling 
in .NET 6 (Preview 4).

![.NET Hot Reload](https://devblogs.microsoft.com/dotnet/wp-content/uploads/sites/10/2021/05/dotnet-Hot-Reload-VS-2019-version-16_11.gif)

For the full power of this feature .NET 6 (and future releases of .NET) 
and Visual Studio 2022 are targeted.

***
### [Announcing Entity Framework Core 6.0 Preview 4: Performance Edition](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-6-0-preview-4-performance-edition/)
**Summary:** [Shay Rojansky](https://twitter.com/shayrojansky) covers:

* EF Core 6.0 performance is now **70% faster** on the industry-standard TechEmpower Fortunes benchmark, compared to 5.0.
* This is the full-stack perf improvement, including improvements in the benchmark code, the .NET runtime, etc. EF Core 6.0 itself is **31% faster** executing queries.
* Heap allocations have been reduced by **43%**.

Loving the across the board perf push ♥ It is, though, driven primarily by the
TechEmpower web-focused benchmarks. We need industry-standard desktop benchmarks. 🤞

***
### [Visual Studio 2019 v16.10 and v16.11 Preview 1 are Available Today!](https://devblogs.microsoft.com/visualstudio/visual-studio-2019-v16-10-and-v16-11-preview-1-are-available-today/)
**Summary:** [Justin Johnson](https://twitter.com/profexorgeek) announces 
the release of Visual Studio 2019 v16.10 GA (and v16.11 preview 1) with:

 * C++20 features.
 * Improved Git integration.
 * Improved profiling tools.
   * The .NET Object Allocation tool in the Performance Profiler is 
    first tool transitioned to our new analysis engine which is 
    significantly faster and provides more features. 
    After collection you can get to your results and 
    build call-trees faster (**~40% increase**). Nice!
 * A host of features that accelerate productivity.

***
### [Visual Studio 2022](https://devblogs.microsoft.com/visualstudio/visual-studio-2022/)
**Summary:** [Amanda Silver](https://twitter.com/amandaksilver) shares exciting news with
the first public preview of Visual Studio 2022 being released this summer.
Not a Build announcement but worth mentioning. 64-bit!

***
### [F# and F# tools update for Visual Studio 16.10](https://devblogs.microsoft.com/dotnet/f-and-f-tools-update-for-visual-studio-16-10/)
**Summary:** [Philip Carter](https://twitter.com/_cartermp) announces 
updates to the F# tools for Visual Studio 16.10 with:

* Support for Go to Definition on external symbols
* Better support for mixing C# and F# projects in a solution
* More quick fixes and refactorings
* More tooling performance and responsiveness improvements
* More core compiler improvements

***
### [Learn What’s New in .NET Productivity](https://devblogs.microsoft.com/visualstudio/learn-whats-new-in-net-productivity/)
**Summary:** [Mika Dumont](@mika_dumont) shares the latest tooling updates in 
Visual Studio 2019. Jam packed with small clips and screen shots. Covers:

 * [Inheritance margin](https://docs.microsoft.com/visualstudio/ide/reference/options-text-editor-csharp-advanced?view=vs-2019#inheritance-margin)
 * [Remove Unused References](https://docs.microsoft.com/visualstudio/ide/reference/remove-unused-references?view=vs-2019)
 * Add missing using directives on paste
 * Tab twice to insert arguments (experimental)
 * [Simplify LINQ expressions](https://docs.microsoft.com/en-us/visualstudio/ide/reference/simplify-linq-expression?view=vs-2019)
 * Remove unnecessary discard

To enable many of these you need to go to 
**Tools > Options > Text Editor > C# or Basic > Advanced** or 
in **Tools > Options > Text Editor > C# > IntelliSense**
Visual Studio. Or quick search. 😉



You can see a status table of C# language features on GitHub at 
[Language Feature Status](https://github.com/dotnet/roslyn/blob/master/docs/Language%20Feature%20Status.md).
Personally I wish for value type records e.g. `record struct`, which is being worked on.


## Videos
I have selected a few videos I found interesting below incl. full
playlist of all videos from the conference. With summaries some of 
which are just copies of the official summary.

### Playlist and Live Streams

***
### [Microsoft Build 2021 All Sessions Playlist](https://www.youtube.com/playlist?list=PLlrxD0HtieHgMGEnTzEEfkADbaG8aAWRp)
[![Microsoft Build 2021 All Sessions Playlist](https://i.ytimg.com/vi/KQt0v950h6k/hqdefault.jpg)](https://www.youtube.com/playlist?list=PLlrxD0HtieHgMGEnTzEEfkADbaG8aAWRp)

**Summary:** 251 videos excl. the full day streams.

***
### [Microsoft Build 2021 LIVE Day 1](https://youtu.be/xB7vqWEgifc)
[![](https://img.youtube.com/vi/xB7vqWEgifc/0.jpg)](https://youtu.be/xB7vqWEgifc)

**Summary:** Full day 1 live stream.

***
### [Microsoft Build 2021 LIVE Day 2](https://youtu.be/b4ryyEm5rSc)
[![](https://img.youtube.com/vi/b4ryyEm5rSc/0.jpg)](https://youtu.be/b4ryyEm5rSc)

**Summary:** Full day 2 live stream.

### Keynotes

***
### [Build Opening](https://youtu.be/KQt0v950h6k)
[![Build Opening](https://img.youtube.com/vi/KQt0v950h6k/0.jpg)](https://youtu.be/KQt0v950h6k)

**Summary:** [Satya Nadella](https://twitter.com/satyanadella) opens Build with 
an inspiring talk about how... well...
[software is eating the world](https://www.wsj.com/articles/SB10001424053111903480904576512250915629460).


***
### [Scott Guthrie ‘Unplugged’ – Home Edition](https://youtu.be/vqOgvUzxAyM)
[![Scott Guthrie ‘Unplugged’ – Home Edition](https://img.youtube.com/vi/vqOgvUzxAyM/0.jpg)](https://youtu.be/vqOgvUzxAyM)

**Summary:** [Scott Guthrie](https://twitter.com/scottgu)/Microsoft ♥ Developers and covers
what Microsoft has to offer developers from cloud to tools incl. customer stories.


***
### [Future of Technology with Kevin Scott](https://youtu.be/IrbG41L605s)
[![Future of Technology with Kevin Scott](https://img.youtube.com/vi/IrbG41L605s/0.jpg)](https://youtu.be/IrbG41L605s)

**Summary:** [Kevin Scott](https://twitter.com/kevin_scott), from his home workshop,
gives a tour of creative labs and spaces where people are using cutting-edge technology 
to create Grammy award-winning music, explore the possibilities of programming for ML-powered 
Edge and IoT devices, inspire startups and build new applications with supercomputing-scale AI, 
and solve some of the biggest health-care challenges on the planet with biotechnology.

***
### [Application Development with Scott Hanselman & Friends](https://youtu.be/Idf1iJtYNrE)
[![Application Development with Scott Hanselman & Friends](https://img.youtube.com/vi/Idf1iJtYNrE/0.jpg)](https://youtu.be/Idf1iJtYNrE)

**Summary:** [Scott Hanselman](https://twitter.com/shanselman) and friends in a 
fast paced "impromptu" keynote. Entertaining for sure.
I really wish more focus and time had been spent on the below [Scott Guthrie](https://twitter.com/scottgu)
Red Shirt Web Architecture 😁.

![Application Development With Scott Hanselman]({{ site.baseurl }}/images/2021-06-Microsoft-build-2021/application-development-with-scott-hanselman.png)

### Sessions

***
### [.NET 6 deep dive; what's new and what's coming](https://youtu.be/GJ_PaRNDe9E)
[![.NET 6 deep dive; what's new and what's coming](https://img.youtube.com/vi/GJ_PaRNDe9E/0.jpg)](https://youtu.be/GJ_PaRNDe9E)

**Summary:** [Scott Hunter](https://twitter.com/coolcsh) and friends cover all things .NET 6
in a great talk with:

* Introduction covering .NET scope, ecosystem, performance and adoption.
* One .NET - .NET 6: Single SDK, one BCL, unified tool chain
* Entity Framework Core performance, as also covered above
* C# 10
  * Many syntactiv simplifications, less boilerplat
  * `record struct` 👍 (Oh and then also `record class` which is same as just `record`)
    * NOTE!: I cover the performance implications of this in a
      blog post [TODO!!!]() vis a vis the auto-equality implementation.
  * Improvements to lambdas and auto-properties
  ```csharp
  global using System;
  
  namespace Model;
  
  public record struct Person
  {
      public required string Name
      {
          get;
          init => field = value.Trim();
      }
  }
  ```
  [Mads Tørgersen](https://twitter.com/MadsTorgersen) & 
  [Dustin Campbell](https://twitter.com/dcampbell) cover this and more 
  in a small segment in the video. Awesome!
* 

MUST WATCH!

TODO MOVE THIS TO SEPARATE BLOG POST

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

***
### [Increase your .NET Productivity with Visual Studio](https://youtu.be/Ok-csh6FLL0)
[![Increase your .NET Productivity with Visual Studio](https://img.youtube.com/vi/Ok-csh6FLL0/0.jpg)](https://youtu.be/Ok-csh6FLL0)

**Summary:** [Kendra Havens](@gotheap) and [Mika Dumont](@mika_dumont)
give a quick talk about some great additions to Visual Studio 
to boost your .NET productivity. Covering such things as:

 * Extract base class refactoring
 * Source generators
 * Improvements to edit and continue
 * Standard output directly in test explorer! And monospace font 👍 Finally! 
 * Code coverage for VSTest on Linux
 * Play a aound when tests finish 
   (guessing this will be quite annoying in an open office space post-COVID19 😅)

And more, also see the above coverage of 
[Learn What’s New in .NET Productivity](https://devblogs.microsoft.com/visualstudio/learn-whats-new-in-net-productivity/).

***
### [](https://youtu.be/HASH)
[![](https://img.youtube.com/vi/HASH/0.jpg)](https://youtu.be/HASH)

**Summary:** 


***
### [](https://youtu.be/HASH)
[![](https://img.youtube.com/vi/HASH/0.jpg)](https://youtu.be/HASH)

**Summary:** 


## Links



## Download Videos for Offline Viewing
[dayngo.com](dayngo.com) doesn't appear to have the event listed yet but you can try checking at:

[https://dayngo.com/channel9/events/](https://dayngo.com/channel9/events/)

## People on Twitter


[Rich Lander](https://twitter.com/runfaster2000)

[David Ortinau](https://twitter.com/davidortinau)

[Dmitry Lyalin](https://twitter.com/LyalinDotCom)

[Shay Rojansky](https://twitter.com/shayrojansky)

[Justin Johnson](https://twitter.com/profexorgeek)

[Amanda Silver](https://twitter.com/amandaksilver)

[Philip Carter](https://twitter.com/_cartermp)

[Satya Nadella](https://twitter.com/satyanadella)

[Scott Guthrie](https://twitter.com/scottgu)

[Scott Hanselman](https://twitter.com/shanselman)

[Seth Juarez](https://twitter.com/sethjuarez)

[Scott Hunter](https://twitter.com/coolcsh)

[Ben \{chmark\} Adams](https://twitter.com/ben_a_adams)

[Adam Sitnik](https://twitter.com/SitnikAdam)

[Olia Gavrysh](https://twitter.com/oliagavrysh)

[Cathy Sullivan](https://twitter.com/cathysull) 

[Olia Gavrysh](https://twitter.com/oliagavrysh)

[Jasmine Greenaway](https://twitter.com/paladique)

[Isaac Levin](https://twitter.com/isaacrlevin)

[Daniel Roth](https://twitter.com/danroth27)

[Migual Ramos](https://twitter.com/marbtweeting)

[Maddy Leger](https://twitter.com/maddyleger1) 

[David Ortinau](https://twitter.com/davidortinau) 

[Leslie Richardson](https://twitter.com/lyrichardson01)
