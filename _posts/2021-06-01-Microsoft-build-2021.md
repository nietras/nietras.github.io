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
[Rich Lander](https://twitter.com/runfaster2000).

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
### []()
**Summary:** []() 

You can see a status table of C# language features on GitHub at 
[Language Feature Status](https://github.com/dotnet/roslyn/blob/master/docs/Language%20Feature%20Status.md).
Personally I wish for value type records e.g. `record struct`, which is being worked on.


## Videos
I have selected a few videos I found interesting below incl. full
playlist of all videos from the conference. With summaries some of 
which are just copies of the official summary.

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

***
### [Application Development with Scott Hanselman & Friends](https://youtu.be/Idf1iJtYNrE)
[![Application Development with Scott Hanselman & Friends](https://img.youtube.com/vi/Idf1iJtYNrE/0.jpg)](https://youtu.be/Idf1iJtYNrE)

**Summary:** [Scott Hanselman](https://twitter.com/shanselman) and friends, in a
theater together, in a fast paced "imprompto" keynote. Entertaining for sure.
I really wish more focus and time had been spent on the below 😁.

![Application Development With Scott Hanselman]({{ site.baseurl }}/images/2021-06-Microsoft-build-2021/application-development-with-scott-hanselman.png)

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
Most videos can be found below for direct download:

[https://dayngo.com/channel9/events/c8f775c57bc144649a15acc800fba570/Focus-on-Windows](https://dayngo.com/channel9/events/c8f775c57bc144649a15acc800fba570/Focus-on-Windows)

## People on Twitter


[Rich Lander](https://twitter.com/runfaster2000)

[David Ortinau](https://twitter.com/davidortinau)

[Dmitry Lyalin](https://twitter.com/LyalinDotCom)

[Shay Rojansky](https://twitter.com/shayrojansky)

[Justin Johnson](https://twitter.com/profexorgeek)

[Amanda Silver](https://twitter.com/amandaksilver)

[Philip Carter](https://twitter.com/_cartermp)

[Scott Hanselman](https://twitter.com/shanselman)

[Seth Juarez](https://twitter.com/sethjuarez)

[Scott Hunter](https://twitter.com/coolcsh)

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
