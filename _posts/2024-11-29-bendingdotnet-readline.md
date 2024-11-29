---
layout: post
title: Bending .NET - ReadLine Patterns
---
or [how to make it rain so puny humans get soaked](https://www.youtube.com/watch?v=YBQLEhzlYX8).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I take a quick look at code patterns
for reading lines from
[`TextReader`](https://learn.microsoft.com/en-us/dotnet/api/system.io.textreader?view=net-9.0)
using
[`ReadLine`](https://learn.microsoft.com/en-us/dotnet/api/system.io.textreader.readline?view=net-9.0),
which has existed from the early beginning of .NET (Framework 1.1 as far as I
can tell) based on a pattern of returning `null` when there are no more lines to
read. 

![Digital rain animation ala The Matrix]({{ site.baseurl }}/images/2024-11-bendingdotnet-readline/Digital_rain_animation_small_letters_clear.gif)
Source: [wikimedia](https://upload.wikimedia.org/wikipedia/commons/1/17/Digital_rain_animation_small_letters_clear.gif)

```csharp
#nullable enable
using System;
using System.IO;

Action<string?> log = Console.WriteLine;
var text = "1\n2\n3\n";

log("SYNC");

log("classic"); // one ReadLine call, out of loop variable, inside loop assignment
{
    using var reader = new StringReader(text);
    string? line;
    while ((line = reader.ReadLine()) != null)
    { log(line); }
}
log("is string"); // no out of loop variable, implicit null check, explicit type
{
    using var reader = new StringReader(text);
    while (reader.ReadLine() is string line)
    { log(line); }
}
log("is {}"); // no out of loop variable, implicit null check, implicit type
{
    using var reader = new StringReader(text);
    while (reader.ReadLine() is { } line)
    { log(line); }
}
log("");

log("ASYNC");

log("classic"); // one ReadLine call, out of loop variable, inside loop assignment
{
    using var reader = new StringReader(text);
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    { log(line); }
}
log("is string"); // no out of loop variable, implicit null check, explicit type
{
    using var reader = new StringReader(text);
    while (await reader.ReadLineAsync() is string line)
    { log(line); }
}
log("is {}"); // no out of loop variable, implicit null check, implicit type
{
    using var reader = new StringReader(text);
    while (await reader.ReadLineAsync() is { } line)
    { log(line); }
}
log("DONE");

// Infinite loop version since `is var` does not perform implicit null check
var infinite = false;
if (infinite)
{
    using var reader = new StringReader(text);
    while (reader.ReadLine() is var line)
    { log(line); }
}
```