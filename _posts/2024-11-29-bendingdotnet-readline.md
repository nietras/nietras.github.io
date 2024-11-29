---
layout: post
title: Bending .NET - ReadLine Patterns
---
or [how to ](https://www.youtube.com/watch?v=HwhB5uCaj3Y).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I take a quick look at code patterns
for reading lines from `TextReader` using
[`ReadLine`](https://learn.microsoft.com/en-us/dotnet/api/system.io.stringreader.readline?view=net-9.0),
which has existed from the very beginning of .NET (.NET Framework 1.1 as far as
I can tell) based on a pattern of returning `null" when there are no more lines
to read. 

![Old wood horizontal lines]({{ site.baseurl }}/images/2024-11-bendingdotnet-readline/Old_wood_horizontal_lines_-_Public_Domain.jpg)
Source: [wikimedia](https://commons.wikimedia.org/wiki/File:Old_wood_horizontal_lines_-_Public_Domain.jpg)



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