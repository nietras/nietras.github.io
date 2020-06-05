---
layout: post
title: Digest - Microsoft Build 2020
---

Digest of the free virtual event at [https://mybuild.microsoft.com/home](https://mybuild.microsoft.com/home)


## Announcements and Blog Posts
A few great announcements and blog posts can be found below.
Looking forward to .NET 5 (`net5.0`) and C# 9 due November 2020. 👍

***
### [Welcome to C# 9.0](https://devblogs.microsoft.com/dotnet/welcome-to-c-9-0/)
**Summary:** Must read! A preview of C# 9.0 features e.g. a full 
working C# program could look like below:
```csharp
var p = new Point2D { X = 1, Y = 2 };
var q = p with { X = 3 };
Point2D? r = q.Equals(p) ? p : null;
System.Console.WriteLine(r);

public data struct Point2D(nint X, nint Y);
```
Maybe! This doesn't compile using the build 2020 demo compiler on 
the awesome [sharplab.io](sharplab.io) see [example](https://sharplab.io/#v2:EYLgJgpgtg9gzgWmAVwJYBswIEwAY8A0YIA1AD4BuAhgE4AEADnQLx0B2EA7nQAoypsALtgAidAN50AGizoBGAnQCas7HQC+AbgCwAKGr0AjrKadUggBYTpsgMwaduvgOEiA/HXqtDAOgCihshU6HAAFAwAlHQeTCDsyOjojgACcj6pAJyhNBGOesn2YFSCVHRwgjTIAMaCvPxCoqFsLtKKzULKuUA==). 
`Main` code comes between any 
`using`s and type/method declarations. 
Everything is early and syntax and naming can still change.
I do hope value type records will be supported. Although,
value tuples - e.g. `(int X, int Y)` - are great for local scope 
things, they aren't great across code/boundaries. Here a proper 
named type is better. Value type records would
be great for that. Below a list of potential C# 9 features:

* `nint/nuint` the final nail in the coffin for [DotNetCross.NativeInts](https://github.com/DotNetCross/NativeInts)
 * Init-only properties
 * Init accessors and readonly fields
 * Records
 * With-expressions
 * Value-based equality
 * Data members
 * Positional records
 * Top-level programs
 * Improved pattern matching

Current proposals considered for C# 9 can be found at [csharplang/milestone/15](https://github.com/dotnet/csharplang/milestone/15).

***
### [Announcing .NET 5 Preview 4 and our journey to one .NET](https://devblogs.microsoft.com/dotnet/announcing-net-5-preview-4-and-our-journey-to-one-net/)

**Summary:** Great (long) post with lots of info including details of roadmap for .NET 5 and beyond. Below the roadmap from the old post [Introducing .NET 5](https://devblogs.microsoft.com/dotnet/introducing-net-5/). Schedule is pretty much the same, but .NET 6 will probably be the first "unified" version of .NET.  

![Dotnet Roadmap](https://devblogs.microsoft.com/dotnet/wp-content/uploads/sites/10/2019/05/dotnet_schedule.png)

***
### [Introducing .NET Multi-platform App UI](https://devblogs.microsoft.com/dotnet/introducing-net-multi-platform-app-ui/)  
**Summary:** Short post on Xamarin.Forms evolved. MVVM or MVU. Go to [https://github.com/dotnet/maui](https://github.com/dotnet/maui)  

![MAUI](https://devblogs.microsoft.com/dotnet/wp-content/uploads/sites/10/2020/05/maui-01-overview-1536x864.png)   

![One project](https://devblogs.microsoft.com/dotnet/wp-content/uploads/sites/10/2020/05/maui-02-1536x864.png)

***
### [Visual Studio 2019 v16.6 & v16.7 Preview 1](https://devblogs.microsoft.com/visualstudio/visual-studio-2019-v16-6-and-v16-7-preview-1-ship-today/)
**Summary:** Finally the new revamped git tooling! This alone is worth
the update. This can be enabled in Visual Studio by:
 * Press `CTRL + Q`
 * Search for `preview features` and open **Environment -> Preview features**
 * Then check the **New Git user experience** as shown below.

![New Git user experience]({{ site.baseurl }}/images/2020-05-Microsoft-build-2020/vs2019-new-git-user-experience.png)

Now this is a big improvement from the old Team Explorer with less navigation.
Unfortunately, keyboard navigation support is very poor.
Below you can see how I am desperately trying to tab my way to the commit message
field... an exercise in futility. 🤦‍

![New Git user experience keyboard navigation 🤦‍]({{ site.baseurl }}/images/2020-05-Microsoft-build-2020/vs2019-new-git-user-experience-keyboard-navigation.gif)

***
### [Windows Forms Designer for .NET Core Released](https://devblogs.microsoft.com/dotnet/windows-forms-designer-for-net-core-released/)
**Summary:** Looks familiar 😅. This also has to be enabled in preview features.

![WinForms designer](https://devblogs.microsoft.com/dotnet/wp-content/uploads/sites/10/2020/05/designer-768x519.png)

***

### [Blazor WebAssembly 3.2.0 now available](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-now-available/)
**Summary:** Cross-platform silverlight... ah no moonlight 😉 MSIL via WASM!  

![Blazor WebAssembly Hello World](https://devblogs.microsoft.com/aspnet/wp-content/uploads/sites/16/2020/05/BlazorApp1-1.png)

***
### [Introducing YARP Preview 1](https://devblogs.microsoft.com/dotnet/introducing-yarp-preview-1/)
**Summary:** Yarp, Another Reverse Proxy. Kestrel-based so should be fast.

***
### [Using Visual Studio Codespaces with .NET Core](https://devblogs.microsoft.com/dotnet/using-visual-studio-codespaces-with-net-core/)
**Summary:** Nothing new to most developers in the trend of "mainframification"
of developer tools and IDEs. VS Code is basically "just" a browser with local
machine access and a gazillion amazing extensions. Codespaces is roughly speaking 
developer tools and code running remotely. 
This post shares how this works with .NET Core and current limitations e.g. 
no apps with UI support.

The ability to quickly get an amazing developer experience inside a browser
for any project anywhere (e.g. GitHub or Azure DevOps) is pretty great.
Especially, for large open source projects that might require an intricate
and time-consuming setup of dependencies (looking at you CNTK 😉). We will
have to see if that promise will be fulfilled. It is also, of course, another 
potential recurring revenue stream for big tech companies.

***
### [F# 5 and F# tools update](https://devblogs.microsoft.com/dotnet/f-5-update-for-net-5-preview-4/)
**Summary:** A quick update on F# 5 news incl. compiler performance improvements.
Always great to see. 👍

***
### [Improvements to XAML tooling in Visual Studio 2019 version 16.7 Preview 1](https://devblogs.microsoft.com/visualstudio/improvements-to-xaml-tooling-in-visual-studio-2019-version-16-7-preview-1/)
**Summary:** New features including:
> * A new XAML data binding failure troubleshooting experience – now you can see when bindings have failed and review the details in a new dedicated tool window
> * A built-in inline color visualizer in the XAML Code editor
> * Two preview features that are still in early development including a new XAML Designer feature called Suggested Actions and our new and improved designer preview for WPF .NET Framework developers

The binding failure troubleshooting looks like a great addition for debugging brittle
binding issues.

![XAML binding failure troubleshooting](https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2020/05/XAML-Binding-Failures-VS-2019-Update-7-Preview-1-1024x836.png)

***
### [Run C# notebooks with Azure Cosmos DB](https://devblogs.microsoft.com/cosmosdb/csharp-notebooks/)
**Summary:** What I found interesting here were the data visualization examples.
Looks neat.


## Videos
I have selected a few videos I found interesting below. I would 
definitely recommend **The Journey to One .NET** and 
**C# Today & Tomorrow** for all things .NET/C# ❤.

***
### [Microsoft Build 2020: CEO Satya Nadella's opening remarks](https://youtu.be/S_wNRx7f7rU)
[![Microsoft Build 2020: CEO Satya Nadella's opening remarks](https://img.youtube.com/vi/S_wNRx7f7rU/0.jpg)](https://youtu.be/S_wNRx7f7rU)  

**Summary:** Always interesting and provides a general overview. There
is also a [youtube playlist](https://www.youtube.com/playlist?list=PLFPUGjQjckXEiPiW868RGBYYHXhBCGLng) 
with the "key segments" from Build 2020. The 48 hour 
live stream seems to have been removed, though. One interesting
segment is the **The Computing Revolution**.

***
### [The Journey to One .NET](https://channel9.msdn.com/Events/Build/2020/BOD106)
[![The Journey to One .NET](https://mediusprodstatic.studios.ms/video-28844/thumbnail.jpg)](https://channel9.msdn.com/Events/Build/2020/BOD106)  

**Summary:** The "Lesser" Scotts talk about many .NET related 
things. Blazor, ML.NET, .NET 5 and 6, tye. **In place** single file 
publish looks to solve the main issues around this right now, 
I hope 😀  

***
### [C# Today & Tomorrow](https://channel9.msdn.com/Events/Build/2020/BOD108)
[![C# Today & Tomorrow](https://mediusprodstatic.studios.ms/video-28908/thumbnail.jpg?sv=2018-03-28&sr=c&sig=BO%2FwlUmCp8H%2BPnbsVOr8Ae5d4fraQ21G%2FBiMG5NRU5w%3D&se=2025-05-18T13%3A58%3A53Z&sp=r)](https://channel9.msdn.com/Events/Build/2020/BOD108)  

**Summary:** C# 8 summarized and a preview of C# 9. See blog post comments above.

***
### [Visual Studio .NET Productivity on PC and Mac](https://channel9.msdn.com/Events/Build/2020/BOD112)
[![Visual Studio .NET Productivity on PC and Mac](https://mediusprodstatic.studios.ms/asset-8b75d11a-5e03-4d8c-98b6-b276a454a562/Thumbnail000001.jpg)](https://channel9.msdn.com/Events/Build/2020/BOD112)  

**Summary:** As always productivity tips almost always have the
biggest impact on your everyday work, if you like me spent most of
your time in Visual Studio 😁, so definitely recommended.
Often there is something you did not know before and sometimes
not even related to VS... e.g. `Windows key + .` brings up
the Windows emoji keyboard. Did not know that.

![Windows Emoji Keyboard]({{ site.baseurl }}/images/2020-05-Microsoft-build-2020/windows-emoji-keyboard.png)

Lots of good tips, although I often feel that some of the 
refactorings result in worse code than before or at 
the very least need to be cleaned up after. 
So as long as you treat these as a help *towards* the end goal of
clean and readable code and not *the end* itself that's good.

**Visual Studio Code** has a nice `pdf` file with short cuts at
[https://code.visualstudio.com/shortcuts/keyboard-shortcuts-windows.pdf](https://code.visualstudio.com/shortcuts/keyboard-shortcuts-windows.pdf)

**Visual Studio** needs a similar one page up-to-date one. 😉 
Note there is a list for VS but it's not as "nice" at
[https://docs.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-for-frequently-used-commands-in-visual-studio?view=vs-2019](https://docs.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-for-frequently-used-commands-in-visual-studio?view=vs-2019)


***
### [The modern Visual Studio experience](https://channel9.msdn.com/Events/Build/2020/BOD111)
[![The modern Visual Studio experience](https://mediusprodstatic.studios.ms/asset-70f27855-2eff-4d97-b19e-2310b90bd3d5/Thumbnail000001.jpg)](https://channel9.msdn.com/Events/Build/2020/BOD111)  

**Summary:** Mads Kristensen, VS extensions author #1, 
from his garage 😎 (and colleagues). 
Shows off the new top level `Git` menu
and the new git tooling, which is covered a bit above too.

PS: If you haven't installed his Markdown extension for
Visual Studio you should. It has easy paste of images,
for example. Quick to do screen shots while documenting
something in markdown.

***
### [Cloud Native Apps with .NET and AKS](https://channel9.msdn.com/Events/Build/2020/BOD105)
[![Cloud Native Apps with .NET and AKS](https://mediusprodstatic.studios.ms/asset-9058b387-5d37-4e1d-b030-2def3b2776cb/Thumbnail000001.jpg)](https://channel9.msdn.com/Events/Build/2020/BOD105)  

**Summary:** `tye` demo of orchestrating a Blazing Pizza web site with
lots of docker containers. Looks interesting. Still in an early 
experimental stage, though.


## Download Videos for Offline Viewing
A great site in general for downloading and getting an overview of 
channel9 videos is 
[https://dayngo.com/channel9](https://dayngo.com/channel9) 
Not all videos  from build 2020 are there, but most can be found at:
[https://dayngo.com/channel9/events/4c9262b67d02462caa94abad0181140c/2020](https://dayngo.com/channel9/events/4c9262b67d02462caa94abad0181140c/2020)

![Download Videos Dayngo]({{ site.baseurl }}/images/2020-05-Microsoft-build-2020/download-videos-dayngo.png)
