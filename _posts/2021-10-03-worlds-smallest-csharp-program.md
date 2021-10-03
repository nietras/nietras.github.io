---
layout: post
title: World's Smallest C# Program (with `N`)
---
Recently, while lying facedown on an exercise mat, I had a fun idea
for how to write the world's smallest C# program in .NET 6/C# 10 by
abusing the [global usings](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#global-using-directives) 
feature. 

The best (and worst) ideas often come when not sitting in front of the 
computer. I'm personally blessed/cursed with storing a mental image of 
code I've seen or worked with and this being processed in the back of my 
mind, often flashing red in parts of the code that 
"[smells](https://en.wikipedia.org/wiki/Code_smell)"
hours or days later. 😅

Writing as small as possible code for achieving something is an
old tradition going back to the [demoscene](https://en.wikipedia.org/wiki/Demoscene)
and is also known as [code golfing](https://en.wikipedia.org/wiki/Code_golf).
Just like golf centers around using the fewest strokes to get the golf ball in the hole,
code golfing centers around using as little code as possible.

So let's start with the challenge:

> What is the world's smallest C# program?

## Questions
If you are anything like me, this question only raises more questions:

- Smallest in what way?
  - Bytes? I doubt I can beat [Michal Strehovský](https://twitter.com/mstrehovsky)s 
    small self-contained [SeeSharpSnake](https://github.com/MichalStrehovsky/SeeSharpSnake).
  - Lines? Characters?
  - One or more files?
- What C# and .NET version?

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
 - Write the smallest possible program in `Program.cs` that compiles and runs 
   without adding any other code files to the project and with the contents of 
   `Main` (or actually `<Main>$`) solely defined by the contents of `Program.cs`. 
   Any nuget package can be added to the project. No other changes can be made.
 
I'm sure readers can guess the add any nuget package part is key 😉 

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
Of course, we love those. So?
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
   Still at `9` characters still too long. 

Was that it? Is `8` characters the world's smallest C# program? 
Something is missing here isn't it? Are top-level statements supposed to support
four different `Main` signatures as defined in 
[Implicit entry point method](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/program-structure/top-level-statements#c-language-specification)?

|Top-level code contains | Implicit `Main` signature |
|-|-|
|`await` and `return`|`static async Task<int> Main(string[] args)`|
|`await`|`static async Task Main(string[] args)`|
|`return`|`static int Main(string[] args)`|
|No `await` or `return`|`static void Main(string[] args)`|

Yes! We are missing `return`. The compiler was trying to fool us, sneeky 😆

## World's Smallest C# Program (out-of-the-box)
```csharp
return;
```
This compiles! And runs:
```
SmallestPossibleCSharpProgram\bin\Debug\net6.0\SmallestPossibleCSharpProgram.exe 
(process 16400) exited with code 0.
Press any key to close this window . . .
```
Thus, this appears to be **The World's Smallest C# Program at `7` characters** ... out-of-the-box.

Also looking at the IL and the signature of the compiler generated `<Main>$` 
it appears the above table is a bit misleading or does not take into account 
an empty return.
```
.method private hidebysig static void  '<Main>$'(string[] args) cil managed
```

