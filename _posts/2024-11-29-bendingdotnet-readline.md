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
// From textReaderText, create a continuous paragraph
// with two spaces between each sentence.
string aLine, aParagraph = null;
StringReader strReader = new StringReader(textReaderText);
while(true)
{
    aLine = strReader.ReadLine();
    if(aLine != null)
    {
        aParagraph = aParagraph + aLine + " ";
    }
    else
    {
        aParagraph = aParagraph + "\n";
        break;
    }
}
Console.WriteLine("Modified text:\n\n{0}", aParagraph);
```