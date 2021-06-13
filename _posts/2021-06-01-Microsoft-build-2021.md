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

***
### [Announcing .NET 6 Preview 4](https://devblogs.microsoft.com/dotnet/announcing-net-6-preview-4/)
**Summary:**  Great, long and detailed post about everything .NET 6 Preview 4 by 
[Rich Lander](https://twitter.com/runfaster2000). To pick one thing to highlight
the `FileStream` performance on Windows has been significantly improved based on
work by [Ben \{chmark\} Adams](https://twitter.com/ben_a_adams), [Adam Sitnik](https://twitter.com/SitnikAdam) 
and more. Results below speak for themselves, see the blog post for details.

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

![.NET MAUI Preview 4](https://devblogs.microsoft.com/dotnet/wp-content/uploads/sites/10/2021/05/maui-weather-hero-sm-768x536.png)

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
### [ASP.NET Core updates in .NET 6 Preview 4](https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-6-preview-4/)
**Summary:** [Daniel Roth](https://twitter.com/danroth27) shares 
what’s new in this preview release:

* Introducing minimal APIs
* Async streaming
* HTTP logging middleware
* Use Kestrel for the default launch profile in new projects
* `IConnectionSocketFeature`
* Improved single-page app (SPA) templates
* .NET Hot Reload updates
* Generic type constraints in Razor components
* Blazor error boundaries
* Blazor WebAssembly ahead-of-time (AOT) compilation
* .NET MAUI Blazor apps
* Other performance improvements

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
the first public preview of Visual Studio 2022 being released this summer. 64-bit!

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
**Tools > Options > Text Editor > C# > IntelliSense** in 
Visual Studio.



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
Red Shirt Web Architecture, though 😁.

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
  * Many syntactic simplifications, less boilerplate
  * `record struct` 👍 (and by extension also `record class` which is the same as just writing `record`)
    * NOTE!: I cover the performance implications of `record struct` in a
      separate blog post [TODO!!!]().
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

  * [Mads Tørgersen](https://twitter.com/MadsTorgersen) & 
    [Dustin Campbell](https://twitter.com/dcampbell) cover this and more 
    in a small segment in the video.
* Minimal web APIs for cloud native apps
  * Lightweight, single-file, cloud native APIs
  * Low ceremony, top-level C# programs 
  * Path to MVC
  
  ```csharp
  var app = WebApplication.Create(args);

  app.MapGet("/", (Func<string>)(() => "Hello World!"));

  app.Run();
  ```

  * Demo by [Maria Naggaga](https://twitter.com/LadyNaggaga) and 
    [Stephen Halter](https://twitter.com/halter73).
* .NET Multi-platform App UI (MAUI)
  * Cross-platform, native UI
  * Single project system, single codebase
  * Deploy to multiple devices, mobile & desktop
  * Demo by [David Ortinau](https://twitter.com/davidortinau)
  * During this David demos a cool trick to set a random background color
    on all view elements, which can be used to quickly debug bounding boxes
    or similar.
    ![Dotnet Maui Random Color Trick]({{ site.baseurl }}/images/2021-06-Microsoft-build-2021/dotnet-maui-random-color-trick.png)
* Blazor desktop apps
  * Reuse UI components across native and web
  * Built on top of .NET Multi-platform App UI
  * Native app container & embedded controls
  * For .NET 6 primary focus is rich desktop apps for Windows and Max
* ASP .NET Core new features (see coverage of blog post above)
* Demo of ASP .NET Core and Blazor for web development 
  by [Daniel Roth](https://twitter.com/danroth27) incl.
  demo of .NET Hot Reload. Boom! 🔥 Also .NET AOT for WASM.
* Developer productivity improvements 
  * Build time perf improvements in CLI & Visual Studio
  * Hot reload everywhere, all project types
    > No more death by 1000 F5s! -Scott Hunter
    * Which should be the official .NET Hot Reload slogan 🔥
    * No process restart, maintains state
    * Optionally attach the debugger
    * Evolution of "Edit & Continue"
    * Demo by [Dmitry Lyalin](https://twitter.com/LyalinDotCom)
* ML.NET
  * Now supports ARM64 and Apple M1
  * Config-based training enables saved training state
  * Model builder perf improvements
  * Improved AutoML
* Announcing .NET Conf 2021 Novemeber 9-11, 2021!

Must watch!

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
 * Play a sound when tests finish  
   (guessing this will be quite annoying in an open office 😱)

And more, also see the above coverage of 
[Learn What’s New in .NET Productivity](https://devblogs.microsoft.com/visualstudio/learn-whats-new-in-net-productivity/).

***
### [Increase Developer Velocity with Microsoft’s end-to-end developer platform](https://youtu.be/jeCmyZuG8Ig)
[![Increase Developer Velocity with Microsoft’s end-to-end developer platform](https://img.youtube.com/vi/jeCmyZuG8Ig/0.jpg)](https://youtu.be/jeCmyZuG8Ig)

**Summary:** [Amanda Silver](https://twitter.com/amandaksilver) (with
[Donovan Brown](https://twitter.com/donovanbrown), Julie Strauss) talks about
developers and [Developer Velocity Index](https://www.mckinsey.com/industries/technology-media-and-telecommunications/our-insights/developer-velocity-how-software-excellence-fuels-business-performance#)
(a McKinsey concept) and how Microsoft Cloud increases it. This includes:
 * Announcement of Visual Studio 2022 going 64-bit
 * GitHub codespaces
 * DevSecOps
 * Power Platform
 * Customer stories

***
### [What's new in Windows 10 for ALL developers](https://youtu.be/hzmtaS5I29Q)
[![What's new in Windows 10 for ALL developers](https://img.youtube.com/vi/hzmtaS5I29Q/0.jpg)](https://youtu.be/hzmtaS5I29Q)

**Summary:** [Kayla Cinnamon](https://twitter.com/cinnamon_msft), 
Deondre Davis, and [Craig Loewen](https://twitter.com/craigaloewen)
cover innovations with Terminal and WSL2, 
performance improvements and delighters like PowerToys and 
the Windows Package Manager. If you develop for web, cloud, or 
other platforms including Windows, this is for you.

 * WinGet (Windows Package Manager) 1.0 also shipping by default Q3
 * Windows Terminal 
   * Default included in new Windows version coming
   * New settings UI
   * Fragments
 * Windows Shell
   * Virtual desktops (`CTRL + WIN + LEFT/RIGHT`)
     * Custom backgrounds
     * Custom title
     * Reposition
   * Performance improvements in Windows to improve Android tools
 * [Power Automate Desktop](https://flow.microsoft.com/en-us/desktop/) - looks very cool!
 * WSL2 with GPU support
 * Blizzard Entertainment segment on their Linux servers and 
   how there devs can use Visual Studio on Windows to debug 
   applications on Linux via remote debug. 
 * WSLg announcement and demo

***

That's all! However, there is a ton more videos so please be sure to check that out.

## Download Videos for Offline Viewing
[dayngo.com](dayngo.com) doesn't appear to have the event listed yet but you can try checking at:

[https://dayngo.com/channel9/](https://dayngo.com/channel9/)

## People on Twitter

[Rich Lander](https://twitter.com/runfaster2000)

[Satya Nadella](https://twitter.com/satyanadella)

[Scott Guthrie](https://twitter.com/scottgu)

[Scott Hanselman](https://twitter.com/shanselman)

[Scott Hunter](https://twitter.com/coolcsh)

[David Ortinau](https://twitter.com/davidortinau)

[Dmitry Lyalin](https://twitter.com/LyalinDotCom)

[Daniel Roth](https://twitter.com/danroth27)

[Shay Rojansky](https://twitter.com/shayrojansky)

[Justin Johnson](https://twitter.com/profexorgeek)

[Amanda Silver](https://twitter.com/amandaksilver)

[Philip Carter](https://twitter.com/_cartermp)

[Seth Juarez](https://twitter.com/sethjuarez)

[Ben \{chmark\} Adams](https://twitter.com/ben_a_adams)

[Adam Sitnik](https://twitter.com/SitnikAdam)

[Mads Tørgersen](https://twitter.com/MadsTorgersen)

[Dustin Campbell](https://twitter.com/dcampbell)

[Maria Naggaga](https://twitter.com/LadyNaggaga)

[Stephen Halter](https://twitter.com/halter73)

[Kayla Cinnamon](https://twitter.com/cinnamon_msft), 

[Craig Loewen](https://twitter.com/craigaloewen)

[//]: # @runfaster2000 @davidortinau @shanselman @satyanadella @scottgu @coolcsh @LyalinDotCom @danroth27 @shayrojansky @profexorgeek @amandaksilver @_cartermp @sethjuarez @ben_a_adams @SitnikAdam @MadsTorgersen @dcampbell @LadyNaggaga @halter73 @cinnamon_msft @craigaloewen
