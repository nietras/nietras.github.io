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
read. Let's make it rain 🌧

TLDR: `is` pattern matching syntax can be used to simplify `ReadLine` code via
implicit null check to:

```csharp
using var reader = new StringReader(text);
while (reader.ReadLine() is string line)
{
    Console.WriteLine(line);
}
```

![Digital rain animation ala The Matrix]({{ site.baseurl }}/images/2024-11-bendingdotnet-readline/Digital_rain_animation_small_letters_clear.gif)
Source: [wikimedia](https://upload.wikimedia.org/wikipedia/commons/1/17/Digital_rain_animation_small_letters_clear.gif)

`ReadLine` code examples shown below with both synchronous and asynchronous
versions and a version using `var` that results in infinite loop. Be careful of
that 😅 See [code in
sharplab](https://sharplab.io/#v2:D4YgdgrgNlCGBGUCmACJYHILACgACATAIy54AMKeRAdAJIDyA3LqUQQDxVkD8AfClAD2AcxQBeSkQCczHADdYAJxQAXJAA8V4lACIiAHTAFDAZkM7ZuIcIAUOgMoBNAHIBhFAFpKBAJYBjHQBKWQBvXBQIygoFZUUkWAATJGUJMCQAdxR7FUUfMGEAJXikxRs1TWDwyK5uATykWUiUdIALH2QUGxsoeu04xOTqIsSAGXqbQMCUAEJU6CgpkIERbvrglABfXC2cazsnN08UHwBnSTIg0KqI8hQYlH6S7TTM7Nz84ZKyjRVKnCbWu1UDZHoNPmM0hNjmcuHU0otlrYevDGJttlYVg4XO4vKcUCENpdcGF/tVokoHsVks8MlkcnlClTSuVfo1IoCOiCmUNihCkFC8UsNnCkAi9sjRaidjsMbYdABBA447z+Ik4ElNW73UEpFAvOnvRkDZk/P6aog8EVsiIc4GrNLaPBSSnGnmjeryk4ATzAfgmU1mevmYpWEvW0tldkV2KOeK4ao1ZLuFJ1NNe9I+TO+FWtzTanKdLpKboSfM9Pr9UzjFpFIaRayl6N2mOjh1xZwJCeuUWTsSZaYNDM+yWzrO7ts6hZ1JbL3t9Ao7m1r+MR9slaJwOz2OgAIvRnABRNW4AD0J5QtDAADM8j41MtBAAHO7JE4+QRgFBv32oAAGeJiX9T3PBJBCQM4wEELRH2SK9BEUABbY4EMfHo/DvIMYBQPwWiQPwAGtcHuPIbzAO9UAkK9YCgE4GlwHwr06EjbzUQJiW7LUU37VJaTeIcsxZM12XzYFp3BcYqzOe4wxXcUGw3HYgA).

```csharp
#nullable enable
using System;
using System.IO;

Action<string?> log = Console.WriteLine;
var text = "1\n2\n3\n";

log("SYNC - classic");
{
  using var reader = new StringReader(text);
  string? line;
  while ((line = reader.ReadLine()) != null) { log(line); }
}
log("SYNC - is string");
{
  using var reader = new StringReader(text);
  while (reader.ReadLine() is string line) { log(line); }
}
log("SYNC - is {}");
{
  using var reader = new StringReader(text);
  while (reader.ReadLine() is { } line) { log(line); }
}

log("ASYNC - classic");
{
  using var reader = new StringReader(text);
  string? line;
  while ((line = await reader.ReadLineAsync()) != null) { log(line); }
}
log("ASYNC - is string");
{
  using var reader = new StringReader(text);
  while (await reader.ReadLineAsync() is string line) { log(line); }
}
log("ASYNC - is {}");
{
  using var reader = new StringReader(text);
  while (await reader.ReadLineAsync() is { } line) { log(line); }
}
log("DONE");

// Infinite loop version since `is var`
// does not perform implicit null check
var infinite = false;
if (infinite)
{
  using var reader = new StringReader(text);
  while (reader.ReadLine() is var line) { log(line); }
}
```

Output:

```
SYNC - classic
1
2
3
SYNC - is string
1
2
3
SYNC - is {}
1
2
3
ASYNC - classic
1
2
3
ASYNC - is string
1
2
3
ASYNC - is {}
1
2
3
DONE
```

If you remove the `async/await` code and look at the lowered C# code e.g. [via
sharplab.io](https://sharplab.io/#v2:D4YgdgrgNlCGBGUCmACJYHILACgACATAIy54AMKeRAdAJIDyA3LqUQQDxVkD8AfClAD2AcxQBeSkQCczHADdYAJxQAXJAA8V4lACIiAHTAFDAZkM7ZuIcIAUOgMoBNAHIBhFAFpKBAJYBjHQBKWQBvXBQIygoFZUUkWAATJGUJMCQAdxR7FUUfMGEAJXikxRs1TWDwyK5uATykWUiUdIALH2QUGxsoeu04xOTqIsSAGXqbQMCUAEJU6CgpkIERbvrglABfXC2cazsnN08UHwBnSTIg0KqI8hQYlH6S7TTM7Nz84ZKyjRVKnCbWu1UDZHoNPmM0hNjmcuHU0otlrYevDGJttlYVg4XO4vKcUCENpdcGF/tVokoHsVks8MlkcnlClTSuVfo1IoCOiCmUNihCkFC8UsNnCkAi9sjRaidjs9joACL0ZwAUSJOFwAHp1ShaGAAGZ5HxqZaCAAOd2SJx8gjAKEtYD8qAABniYo6NVqEoIkGcwIItCbkrrBIoALbHEMmnp+Q0oSAwFB+FpIPwAa1w9zy+rAhtQEl1sCgJwauB8us6mYNakCxOuUTuFNBKVjtLeDM+yW+FTZEQ5wMbPNG4ymLopErFKzHUvROCAA)
then the patterns result in similar code with slight and probably neglible differences.

```csharp
Action<string> action = <>O.<0>__WriteLine ?? 
  (<>O.<0>__WriteLine = new Action<string>(Console.WriteLine));
string s = "1\n2\n3\n";
action("SYNC - classic");
StringReader stringReader = new StringReader(s);
try
{
    string obj;
    while ((obj = stringReader.ReadLine()) != null)
    {
        action(obj);
    }
}
finally
{
    if (stringReader != null)
    {
        ((IDisposable)stringReader).Dispose();
    }
}
action("SYNC - is string");
StringReader stringReader2 = new StringReader(s);
try
{
    while (true)
    {
        string text = stringReader2.ReadLine();
        if (text != null)
        {
            action(text);
            continue;
        }
        break;
    }
}
finally
{
    if (stringReader2 != null)
    {
        ((IDisposable)stringReader2).Dispose();
    }
}
action("SYNC - is {}");
StringReader stringReader3 = new StringReader(s);
try
{
    while (true)
    {
        string text2 = stringReader3.ReadLine();
        if (text2 != null)
        {
            action(text2);
            continue;
        }
        break;
    }
}
finally
{
    if (stringReader3 != null)
    {
        ((IDisposable)stringReader3).Dispose();
    }
}
action("DONE");
if (0 == 0)
{
    return;
}
StringReader stringReader4 = new StringReader(s);
try
{
    while (true)
    {
        string obj2 = stringReader4.ReadLine();
        action(obj2);
    }
}
finally
{
    if (stringReader4 != null)
    {
        ((IDisposable)stringReader4).Dispose();
    }
}
```

That's all!