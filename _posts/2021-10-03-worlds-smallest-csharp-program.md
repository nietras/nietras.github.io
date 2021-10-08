---
layout: post
title: World's Smallest C# Program (featuring `N`)
---
**TLDR:** Tongue in cheek post on how - in .NET 5+ and C# 9+ - the 
smallest possible C# program appears to be `{}` or 2 characters long. 
This doesn't do much, though. 
Using `N` ([github](https://github.com/nietras/N), [nuget](https://www.nuget.org/packages/N/))
you can write a program doing something in 4 characters with e.g. `N();`
in .NET 6 and C# 10. Along the way learn a few C# 10 and nuget packaging tricks incl.
[global usings](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive#global-modifier) 
and [implicit usings](https://docs.microsoft.com/en-us/dotnet/core/tutorials/top-level-templates#implicit-using-directives).

---

Recently, while lying facedown on an exercise mat, I had a fun idea
for how to write the world's smallest C# program in .NET 6/C# 10 by
abusing the [global usings](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#global-using-directives) 
feature (that actually does something).

Writing as small as possible code for achieving something is an
old tradition going back to the [demoscene](https://en.wikipedia.org/wiki/Demoscene)
and is also known as [code golfing](https://en.wikipedia.org/wiki/Code_golf).
Just like golf centers around using the fewest strokes to get the golf ball in the hole,
code golfing centers around using as little code as possible.There is even a
dedicated stackexchange site to it 
[https://codegolf.stackexchange.com](https://codegolf.stackexchange.com/).

So let's start with the challenge:

> What is the world's smallest C# program?

## Questions
If you are anything like me this question only raises more questions:

- Smallest in what way?
  - Bytes? I doubt I can beat [Michal Strehovský](https://twitter.com/mstrehovsky)s 
    small self-contained [SeeSharpSnake](https://github.com/MichalStrehovsky/SeeSharpSnake)
    as detailed in [Building a self-contained game in C# under 8 kilobytes](https://medium.com/@MStrehovsky/building-a-self-contained-game-in-c-under-8-kilobytes-74c3cf60ea04).
  - Lines? Characters?
  - One or more files?
- Which C# and .NET version?

## Challenge: The Devil is in the Details
Hence, let's make the challenge more clear. Given:

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) is installed and:
  ```powershell
  dotnet --version
  6.0.100-rc.2.21471.3 (or later)
  ```
- Create an empty console application using:
  ```powershell
  mkdir SmallestPossibleCSharpProgram
  cd SmallestPossibleCSharpProgram
  dotnet new console
  ```

Then:
 - Write the smallest possible program - fewest characters - in `Program.cs`,
   that compiles and runs without adding any other code files to the project 
   and with the contents of `Main` (or actually `<Main>$`) solely defined 
   by the contents of `Program.cs`. 
 - Any nuget package can be added to the project. No other changes can be made.
 
The project created is very simple, it has two files:
```
Program.cs
SmallestPossibleCSharpProgram.csproj
```
with `SmallestPossibleCSharpProgram.csproj` containing:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```
and `Program.cs`:
```csharp
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
```
welcome to the new world of .NET 6 and C# 10 with 
[implicit `using` directives](https://docs.microsoft.com/en-us/dotnet/core/tutorials/top-level-templates#implicit-using-directives)!

## Trial and Error
Let's just try the absolute smallest possible C# program by deleting everything in
`Program.cs`:
```csharp
```
and compile. This outputs:
```
Error	CS5001	Program does not contain a static 'Main' 
method suitable for an entry point
```
Damn. Well, it wouldn't have been worth a blog post if this had been possible.
Apparently, you have to have at least one 
[top-level statement](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/top-level-statements) 🤔

Let's try the next best case with just 1 character in `Program.cs`:
```csharp
0
```
compile:
```
CS1002	; expected
```
Of course, we love those. Really, I do. Parenthesis and braces too. 
So?
```csharp
;
```
compile:
```
CS8937	At least one top-level statement must be non-empty.
```
"non-empty" eh? So we need 2 characters at least, perhaps:
```csharp
0;
```
and compile:
```
CS0201	Only assignment, call, increment, decrement, await, 
and new object expressions can be used as a statement
```
Ah, a nice error message telling us what to try. So:

 - **Assignment** - this requires something to assign to e.g. a variable or field.
   There are no global scope fields or variables available here without `using static`,
   so best case seems to be creating and assigning a variable:
   ```csharp
   var i=0;
   ```
   this compiles, but with a pesky warning:
   ```
   CS0219	The variable 'i' is assigned 
   but its value is never used
   ```
   I know, I know 😅 This is our current smallest C# program at `8` characters incl. space.
 - **Call** - this requires something to call, which in C# requires an instance to call an
   instance method on, or type to call a static method on, or 
   again `using static` pulling in static methods from a type, which would be too long.
   How about:
   ```csharp
   0.Equals(0);
   ```
   this compiles and no warning, but it's longer at `12` characters.
   Seems like a dead end, let's move on.
 - **Increment** - so?
   ```csharp
   ++0;
   ```
   this fails:
   ```
   CS1059	The operand of an increment or decrement 
   operator must be a variable, property or indexer
   ```
   So we are back to "Assignment" as shown above. Dead end without any available 
   variables, properties or indexers.
 - **Decrement** - same as increment.
 - **`await`** - so?
   ```csharp
   await;
   ```
   this fails:
   ```
   CS1525	Invalid expression term ';'
   ```
   Not the best error message here, but clearly we have to await on something.
   ```csharp
   await 0;
   ```
   this fails:
   ```
   CS1061	'int' does not contain a definition for 'GetAwaiter' and 
   no accessible extension method 'GetAwaiter' accepting a first argument 
   of type 'int' could be found (are you missing a using directive or 
   an assembly reference?)
   ```
   The `no accessible extension method 'GetAwaiter'` part seems like 
   a loop hole here, we could create such an extension method,
   but we are still at `8` characters so no better than assignment. Moving on.
 - **`new object`** - seems a bit misleading here why "object"? 
   Do value types or structs not count? Or are they considered "objects"?
   In any case, so?
   ```csharp
   new int();
   ```
   This compiles, but at `10` characters still too long.
   ```csharp
   new int;
   ```
   this fails:
   ```
   CS1526	A new expression requires an argument list 
   or (), [], or {} after type
   ```
   Hmm, perhaps an anynomous type then:
   ```csharp
   new{i=0};
   ```
   this fails:
   ```
   CS0201	Only assignment, call, increment, decrement, 
   await, and new object expressions can be used as a statement
   ```
   wait.. what?! Isn't that a `new object expression`? 😅
   Still at `9` characters too long. 

Was that it? Is `8` characters the world's smallest C# program? 
Something is missing here isn't it? Aren't top-level statements supposed to support
four different `Main` signatures as defined in 
[Implicit entry point method](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/program-structure/top-level-statements#c-language-specification)?

|Top-level code contains | Implicit `Main` signature |
|-|-|
|`await` and `return`|`static async Task<int> Main(string[] args)`|
|`await`|`static async Task Main(string[] args)`|
|`return`|`static int Main(string[] args)`|
|No `await` or `return`|`static void Main(string[] args)`|

Yes! We are missing `return`. The compiler was trying to fool us, sneeky 😆

```csharp
return;
```
This compiles and runs:
```
SmallestPossibleCSharpProgram.exe 
(process 16400) exited with code 0.
```
Thus, this comes in at `7` characters our current best.

Also looking at the IL and the signature of the compiler generated `<Main>$` 
it appears the above table is a bit misleading or does not take into account 
an empty return.
```
.method private hidebysig static void  '<Main>$'(string[] args) cil managed
```

7 characters still seems long. Didn't we miss something above? 

Yes, lots of [C# keywords](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/)
one could try, but only few make sense given length etc., 
so not going to go through all of them.

However, let's see. We had assignment, but that required a variable. 
But do we need to assign a variable? 
No we can just declare it so...
```csharp
int i;
```
This compiles, but with a warning. 
```
CS0168	The variable 'i' is declared but never used
```
But there was nothing in the challenge about warnings, and it runs:
```
SmallestPossibleCSharpProgram.exe 
(process 16400) exited with code 0.
```
at `6` characters this is our smallest program yet.

But do we actually need to declare anything? Couldn't we just do nothing?
We already tried that with an empty `Program.cs` file, but what is closest
to empty? A comment?
```csharp
//
```
This, of course, fails as it is equivalent to an empty file:
```
CS5001 Program does not contain a static 'Main' method 
       suitable for an entry point
```
We are, however, close.

## World's Smallest C# Program (an empty block `{}`)
What other code would trigger an entry point but do nothing? An empty block:
```csharp
{}
```
Yes, this compiles and appears to be **The World's Smallest C# Program at `2` characters**.

In IL this becomes:
```
.method private hidebysig static void  '<Main>$'(string[] args) cil managed
{
  .entrypoint
  // Code size       3 (0x3)
  .maxstack  8
  IL_0000:  nop
  IL_0001:  nop
  IL_0002:  ret
} // end of method Program::'<Main>$'
```
It runs but of course does nothing:
```
SmallestPossibleCSharpProgram.exe (process 8044) exited with code 0.
```

Yeah so that was a lot of words for nothing 😅 And no need for global 
or implicit usings or anything.
In fact this compiles fine in .NET 5 and C# 9 with the following.

`SmallestPossibleCSharpProgram.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <Nullable>enable</Nullable>
    <LangVersion>9</LangVersion>
  </PropertyGroup>
</Project>
```
`Program.cs`
```csharp
{}
```

I'm sure plenty of people have figured that out before.

## Extended Challenge: Do Something
Let's make the challenge more interesting and also require that `Main`
actually does something i.e. it must have more than `nop` and `ret` 
in the compiled IL. We've seen examples of that above, but we can do 
better.

## World's Smallest C# Programs Doing Something (featuring `N`)
Introducing `N` ([github](https://github.com/nietras/N), [nuget](https://www.nuget.org/packages/N/))! 
*The* library for writing the world's smallest C# programs "that do something"™.
Let's add it to the project with:
```
dotnet add SmallestPossibleCSharpProgram.csproj package N
```
this means `SmallestPossibleCSharpProgram.csproj` becomes:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="N" Version="0.2.0" />
  </ItemGroup>
</Project>
```
We can then change `Program.cs` to:
```csharp
N();
```
Let's compile and run!
```
Hello friend from nietras!

SmallestPossibleCSharpProgram.exe 
(process 24920) exited with code 0.
```
Success!

**The world's smallest C# program that does something in just 4 characters is `N();`***

\* Using `N` 😉

However, this is not the only solution. As one can guess from the
previous trail and error there are lots of ways to write 
4 character programs and `N` let's you write the smallest 
C# program that does something in many ways:

Want to increment?
```csharp
++I;
```
or
```csharp
I++;
```
Decrement?
```csharp
--I;
```
or
```csharp
I--;
```
Absolutely need to assign something?
```csharp
I=0;
```
or
```csharp
I=I;
```
Declare a variable for a reference type (with warning) - this doesn't really do much:
```csharp
O o;
```
or for a value type (with warning):
```csharp
S s;
```

In addition, `N` let's you write the smallest programs featuring
specific constructs:

Want to be asynchronous? **World's Smallest C# async/await Program** (8 characters)
```csharp
await T;
```
Want to return a code?  (9 characters)
```csharp
return I;
```
Need a while loop (9 characters):
```csharp 
while(B);
```
or for loop (9 characters):
```csharp 
for(;B;);
```
or do while loop (13 characters):
```csharp 
do{}while(B);
```
Need to new something up (7 characters).
```csharp
new O();
```
What about `args`? `N`s got you covered (8 characters).
```csharp
N(args);
```
Remember the `GetAwaiter()` loop hole? `N` let's you await an integer (8 characters).
```csharp
await 1;
```
Need to dispose with using:
```csharp
using(D);
```
This compiles with warning `CS0642	Possible mistaken empty statement`, though.

## How
Now where was I? Yes, lying facedown on an exercise mat and the idea involving 
[global usings](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#global-using-directives).
To have something smaller we need to "pull in" variables, properties, methods or similar
to the scope of `Main` i.e. top-level statements in `Program.cs`. There are many ways to do
this but the simple one is to add something like:
```csharp
global using static NAMESPACE.CLASSNAME;
```
to the project and then define types, properties, methods or similar on that class,
that allows us to write a smaller program. However, you can also do this using
the `<Using>` item in MSBuild as discussed in 
[Update implicit global usings feature to address issues](https://github.com/dotnet/sdk/issues/19521).
```xml
  <ItemGroup>
    <Using Include="NAMESPACE.CLASSNAME" Static="true"  />
  </ItemGroup>
```
Basically, cheating. This is what the `N` library and nuget package does 
and in fact it does both ways just to showcase the ways you can abuse this.
The library consists of the following files:

 - `N.csproj` - project file with a few customizations to use the `N.nuspec` file.
 - `N.nuspec` - nuspec file to customize the nuget package and apply the above mentioned global using hacks.
 - `N.props` - properties file included in the nuget package to hack the consuming project.
 - `NGlobalUsings.cs` - the file containing the `global using static` statements for consuming project.
 - `C.cs` - ordinary C# file with the code providing the class `C` with properties, methods etc.
 
 Let's start with the project file `N.csproj`.
 ```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <RootNamespace>nietras</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>10</LangVersion>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>

    <Version>0.2.0</Version>
    <PackageVersion>$(Version)</PackageVersion>

    <NuspecFile>N.nuspec</NuspecFile>
    <NuspecProperties>version=$(Version);configuration=$(Configuration)</NuspecProperties>
  </PropertyGroup>
</Project>
```
Note the following:

 - Line 6: `<ImplicitUsings>enable</ImplicitUsings>` <br/> enable the implicit usings defined for this project type.
 - Line 13: `<NuspecFile>N.nuspec</NuspecFile>` <br/> specifies we use a custom nuspec file for nuget package.
 - Line 14: `<NuspecProperties>version=$(Version);configuration=$(Configuration)</NuspecProperties>` <br/> forwards properties to
   the nuspec file when building, making it easier to test. And  only have version here.

The rest is pretty standard.

The `N.nuspec` file (with some details omitted) is:
```xml
<?xml version="1.0"?>
<package>
  <metadata minClientVersion="4.0">
    <id>N</id>
    <version>$version$</version>
    <description>Tired of boilerplate? Then N is for you! The Library for World's Smallest C# Programs.</description>
    <authors>nietras</authors>
    <!-- details omitted -->
    <contentFiles>
      <files include="contentFiles/any/any/NGlobalUsings.cs" buildAction="Compile" copyToOutput="false" flatten="false" />
    </contentFiles>
    <dependencies>
      <group targetFramework=".NETStandard2.0" />
    </dependencies>
  </metadata>
  <files>
    <file src="bin\$configuration$\netstandard2.0\N.dll" target="lib/netstandard2.0" />
    <file src="NGlobalUsings.cs" target="contentFiles/any/any" />
    <file src="N.props" target="build" />
    <file src="Icon.png" target="" />
  </files>
</package>
```
Alright there is a little to unpack here (just remember a nuget package is a zip-file 
defined by it's contents by conventions for different directories):

 - Line 17: `<file src="bin\$configuration$\netstandard2.0\N.dll" target="lib/netstandard2.0" />` <br/>
   simply defines `N.dll` as a dll to be copied to target `lib/netstandard2.0`,
   which per convention defines the dll for `netstandard2.0`.
 - Line 18: `<file src="NGlobalUsings.cs" target="contentFiles/any/any" />` <br/>
   ensures this source file will be added as a content file for any platform and "framework".
 - Line 10: `<files include="contentFiles/any/any/NGlobalUsings.cs" buildAction="Compile" copyToOutput="false" flatten="false" />`  <br/>
   adds `NGlobalUsings.cs` to compilation and hence this file becomes 
   part of the source files of the project consuming the package.
 - Line 19: `<file src="N.props" target="build" />` <br/>
   add the `N.props` file to `build` directory, which by convention adds 
   it to build of the consuming project.

The `N.props` file is defined with:
```xml
<Project>
  <!-- Wickedly allow unsafe blocks -->
  <PropertyGroup>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>
  
  <!-- Define global usings see https://github.com/dotnet/sdk/issues/19521 -->
  <ItemGroup>
    <Using Include="System.Runtime.CompilerServices" />
    <Using Include="System.Console" Static="true"  />
    <Using Include="System.Int32" Alias="J" />
  </ItemGroup>

  <!--
  Hide files added to compile from being visible in e.g. Visual Studio
  See https://til.cazzulino.com/dotnet/nuget/hide-contentfiles-from-your-nuget-packages
  -->
  <ItemGroup>
    <Compile Update="@(Compile)">
      <Visible Condition="'%(NuGetItemType)' == 'Compile' and '%(NuGetPackageId)' == 'N'">false</Visible>
    </Compile>
  </ItemGroup>
</Project>
```
This does 3 things. First, it exemplifies how a props file can set or 
define any property just like you can normally. Here by setting 
`AllowUnsafeBlocks` to `true`. Allowing the consuming project to use
`unsafe` code.

Second, it defines 3 different types of global or implicit usings. 
Equivalent to:
```csharp
global using System.Runtime.CompilerServices;
global using static System.Console;
global using J = System.Int32;
```

Third, it uses a trick as discussed in 
[Hide contentFiles from your nuget packages](https://til.cazzulino.com/dotnet/nuget/hide-contentfiles-from-your-nuget-packages)
to hide the `NGlobalUsings.cs` file in Visual Studio, as shown below.

![Smallestpossiblecsharpprogram In Vs]({{ site.baseurl }}/images/2021-10-worlds-smallest-csharp-program/smallestpossiblecsharpprogram-in-vs.png)

If you expand the **Dependencies** in Solution Explorer you can see this file
under **Content Files**, though.

![Smallestpossiblecsharpprogram In Vs Expanded]({{ site.baseurl }}/images/2021-10-worlds-smallest-csharp-program/smallestpossiblecsharpprogram-in-vs-expanded.png)

The `NGlobalUsings.cs` file is just a single line:
```csharp
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras;

public static class C
{
    public static void N(string[]? args = default)
    {
        Console.WriteLine($"Hello friend from nietras!\n{string.Join("\n", args ?? Array.Empty<string>())}");
    }

    public static bool B { get; set; } = false;

    public static E D { get; set; } = new ();

    public record E : IDisposable
    {
        public void Dispose() { }
    }

    public static int I { get; set; } = 42;

    public unsafe static byte* P { get; set; } = (byte*)Marshal.AllocHGlobal(1);

    public record O;

    static int _r = 0;
    public static ref int R { get => ref _r; }

    public readonly record struct S;

    public static Task T { get; set; } = Task.CompletedTask;

    public static Int32Awaiter GetAwaiter(this int i)
    {
        return new Int32Awaiter(i);
    }

    public readonly struct Int32Awaiter : INotifyCompletion
    {
        readonly int _i;

        public Int32Awaiter(int i) => _i = i;

        public bool IsCompleted { get; } = true;
        public int GetResult() => _i;
        public void OnCompleted(Action continuation) => continuation();
    }
}
```
This simply defines a lot of properties and types to be able to write the
world's smallest C# programs.

Of note here, are a few C# 10 features:

 - `namespace nietras;` namespace without `{}` so no more namespace indentation. 
  This is called [File-scoped namespace declaration](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#file-scoped-namespace-declaration).
 - `readonly record struct` as covered in [C# 10 - `record struct` Deep Dive & Performance Implications]({{ site.baseurl }}/2021/06/14/csharp-10-record-struct)

For more see [What's new in C# 10.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10).

All in all this allows you to write a `Program.cs` with such code as:
```csharp
{}
N();
++I;
I++;
--I;
I--;
I=0;
I=I;
O o;
S s;
J j=1;
await T;
while(B);
for(;B;);
do{}while(B);
new O();
N(args);
await 1;
using(D);
unsafe{*P=2;}
Unsafe.As<int,byte>(ref R)=3;
WriteLine("wicked implicit global usings");
return I;
```
you don't need `N` for `{}` of course. 😃

Note in above that I didn't have to make an assembly `N.dll` for this.
I could just have added all the code in `NGlobalUsings.cs`, 
but what fun would that have been. And you don't actually need
`NGlobalUsings.cs` since you can just use the `props` trick instead,
which is certainly a better way.

But why do this? And is it a good idea? Definitely don't think
it is a good idea to make a nuget package that does this, but
let's discuss this a bit more.

## Why
Why look at the world's smallest possible program? Why not 🤷‍ 
Mainly I saw this as a fun exercise and hopefully learn something
new along the way. I don't pretend to be the first or only one who 
thought of this and the worlds smallest C# program `{}`. 
I am sure others have. Just haven't found anyone reporting it.

Perhaps the most controversial feature of C# 10 and .NET 6 are
[global usings](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive#global-modifier)
and
[implicit using directives](https://docs.microsoft.com/en-us/dotnet/core/tutorials/top-level-templates#implicit-using-directives).
With any new feature, innovation or development there are questions around 
whether this is "good" or "bad". In my view it depends. Let's look at `Program.cs`:
```csharp
N();
```
Does this convey any context about what this program does or it's intent? 
No, so clearly this is a bad idea. But if you have a project called
`SimpleHelloWorldWebApi` with `Program.cs`:
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
 
if (app.Environment.IsDevelopment()){ app.UseDeveloperExceptionPage(); }
 
app.MapGet("/", () => "Hello World!");
 
app.Run();
```
is the context and intent not clear? I think so. Do we need a long list
of `using`s at the top of the file to help us define context here? No, that's
probably fine. But what if we had a big application with many different concerns
and externals, would it then be a good idea to use global usings for all
possible namespaces? I don't think so. `using`s can help establish context
for a given source file. What things does this code use etc.? E.g. 
a file with:
```csharp
using Contoso.Orders.WebApi;
using Contoso.Fulfilment.WebApi;
```
Establishes context about what this code file is using and hence what it is about.
Code should be written for **reading**. It should [read like a book](https://www.google.com/search?q=clean+code+book). 
While [fitting in your head](https://blog.ploeh.dk/2021/06/14/new-book-code-that-fits-in-your-head/).


In C# `using`s are part of this. But do you need any of:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
```
in every file? Probably not. On the other hand effectively pulling in any of:
```csharp
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
```
in every file, as discussed in 
[Exploring a minimal Web API with ASP.NET Core 6](https://www.hanselman.com/blog/exploring-a-minimal-web-api-with-aspnet-core-6) 
seems excessive to me. At least I like to have using scope clearly defined
for a given file as a way to understand what a given file might be "touching".
Especially, as a code reviewer this can help define much needed context and 
scope of the code in question. Why does this code related to say sorting a
list of values use `System.IO` for example? It shouldn't so let's keep an
eye out for this.

It all depends on what application you are building and the scope of it.
The more different concerns, the larger the context, the more precise I would
want the `using`s to be. The smaller and more focused an application is, like
a small micro-service, the less would I need context from `using`s.

In a long book a table of contents is nice. In a short article, 
there is no need. It depends, as always.

## Conclusion
`{}` or `N();` featuring [`N`](https://github.com/nietras/N) -
 perhaps the shortest named nuget library in the world 😉.