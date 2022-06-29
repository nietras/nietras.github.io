---
layout: post
title: Bending .NET - Determine C# Compiler Version and Language Version with `#error version`
---
or [TODO](https://www.youtube.com/watch?v=pQ0db2ERil8).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I cover a single source line in an
 entire blog post. 🤷‍ A line that can be used for determining the effective C#
compiler version and language version used to compile a given `cs`-file.
Unfortunately though it appears to only be able to give us this information as
an error. 🤦‍

![TODO]({{ site.baseurl }}/images/2022-06-bendingdotnet-compiler-version/TODO.jpg)
Source: [TODO](https://www.flickr.com/photos/gomattolson/4321594214/)

I have created a quick `net6.0` console project with two files.

`CompilerVersionTest.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
</Project>
```
 `Program.cs`
```csharp
#error version
```
Compile this with `dotnet build` and it will output something like:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 10.0.
```
Add `LangVersion` like below:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
  </PropertyGroup>
</Project>
```
and this will change output:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 9.0.
```
Try `preview`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>
</Project>
```
And output will be:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: preview.
```
Remove `LangVersion` and we are back to `10.0`:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 10.0.
```

Now if I then place a `global.json` file next to the project file with:
```
{
  "sdk": {
    "version": "6.0.301",
    "rollForward": "disable",
    "allowPrerelease": false
  }
}
```
Then `dotnet build` will output:
```
Program.cs(1,8): error CS8304: 
  Compiler version: '4.2.0-4.22220.5 (432d17a8)'.
  Language version: 10.0.
```
By default we get the latest version of whatever .NET SDK I have installed which
in my case is (as reported by `dotnet --version`):
```
7.0.100-preview.5.22307.18
```
Changing SDK version again to `6.0.106` and output is:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.0.1-1.22181.2 (487283bc)'.
  Language version: 10.0.
```

If I then open this project in Visual Studio 2022 and build it I will get:
```
Program.cs(1,8,1,15): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 10.0.
```
... yes VS doesn't give a crap about that `global.json` file. This is also kind
of documented in [global.json
overview](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json) that
says:

> The global.json file allows you to define which .NET SDK version is used when
> you run .NET CLI commands. Selecting the .NET SDK version is independent from
> specifying the runtime version a project targets. The .NET SDK version
> indicates which version of the .NET CLI is used.

The key being "**.NET CLI commands**".

Links

> One way of checking the version of your compiler is to add #error version in a
> source file, then looking at the error message output in your build logs.

[How to use Microsoft.CodeAnalysis.PublicApiAnalyzers](https://github.com/dotnet/roslyn-analyzers/blob/main/src/PublicApiAnalyzers/PublicApiAnalyzers.Help.md)
[Display effective language version for #error version](https://github.com/dotnet/roslyn/pull/51880)