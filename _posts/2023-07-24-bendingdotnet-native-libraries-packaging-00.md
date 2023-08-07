---
layout: post
title: Bending .NET - Native Libraries Packaging
---
or [how to find invalid human batteries so they can be flushed](https://www.youtube.com/watch?v=HwhB5uCaj3Y).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I look at . 

![sloth]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/Mother_and_baby_sloth_crossing_the_road.jpg)
Source: [wikimedia](https://commons.wikimedia.org/wiki/File:Mother_and_baby_sloth_crossing_the_road.jpg)

https://commons.wikimedia.org/wiki/File:Sorting_packages_at_Terminal_Annex_Post_Office,_Los_Angeles.jpg

https://commons.wikimedia.org/w/index.php?search=library+packages&title=Special:MediaSearch&go=Go&type=image


## Debugging

### Debugging Native Library Loading
https://ten0s.github.io/blog/2022/07/01/debugging-dll-loading-errors

Use Command Prompt
```
set PATH=C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\;%PATH%
gflags /i APP.exe +sls
cdb -c "g;q" APP.exe
```

![Gflags Show Loader Snaps]({{ site.baseurl }}/images/2023-07-native-libraries-packaging/gflags-show-loader-snaps.png)

Debugging DLL Loading Issues in Windows
https://www.theresearchkitchen.com/archives/359

### Debugging Managed Assembly/Library Loading
Assembly Binding Log Viewer (Fuslogvw.exe)
https://learn.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/e74a18c4(v=vs.71)?redirectedfrom=MSDN

What is Fuslogvw.exe and How to Check Logs?
https://www.winosbite.com/what-is-fuslogvw-exe-and-how-to-check-logs/

https://stackoverflow.com/questions/1674279/how-to-locate-fuslogvw-exe-on-my-machine

https://github.com/dotnet/runtime/issues/12506#issuecomment-895192112.

https://github.com/dotnet/diagnostics/blob/main/documentation/dotnet-trace-instructions.md

COREHOST_TRACE should log AssemblyLoadContext
https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/collect-details
```
dotnet tool install --global dotnet-trace
dotnet-trace collect --providers Microsoft-Windows-DotNETRuntime:4 -- myapp.exe argument1
```
https://github.com/dotnet/diagnostics/blob/main/documentation/dotnet-trace-instructions.md#using-dotnet-trace-to-launch-a-child-process-and-trace-it-from-startup

Then open `.nettrace` file with `PerfView`.

Go to **Events**.

Select all event types.

Use **Text filter** to search for events related to assembly/dll in question

If does not load use e.g.
```
perfview /continueonError APP.exe_20230724_115403.nettrace
```

https://learn.microsoft.com/en-us/windows/win32/dlls/dynamic-link-library-search-order


https://github.com/dotnet/sdk/issues/26324