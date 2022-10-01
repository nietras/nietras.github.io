---
layout: post
title: Bending .NET - Compiling 65,536 Programs with Roslyn to Find Valid Identifier Separator char's... then just use `SyntaxFacts.IsValidIdentifier` 🤦‍
---
or [how to find invalid human batteries so they can be flushed](https://www.youtube.com/watch?v=HwhB5uCaj3Y).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I look at compiling a complete list
of valid and invalid C# identifier separators by using Roslyn and hence by being
lazy instead of checking unicode specification just try every single character. 

![???]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/???.jpg)
Source: [pixabay](https://pixabay.com/photos/???)

Encoding information in file or directory names is a simple way of keeping
details about a given file. For example, an image or an ONNX model and the
conditions or origins of it. That is, for an ONNX model it might be something
simple like the version of the ground truth that it was trained on. Assumming
you of course version your ground truth. Defining a simple schema for this
encoding using simple separators e.g. `_`, `=` can make it easy to then parse in
a position independent manner like `M=Surface_V=1.2.1.onnx`. Now these files
might allow embedding this information in them in some way, but sometimes it
just easier for reference to have it directly in the file name, for example.

Sometimes it is also nice to be able to embed these assets into .NET assemblies
and have them exposed as simple properties. Preferably properties that have the
same name as the file. However, C# naturally has more [strict rules for
identifier
names](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
than file names, which brings up the question what `char`s are valid separators
in a C# identifier name? The above link says:

> - Identifiers must start with a letter or underscore (`_`).
> - Identifiers may contain Unicode letter characters, decimal digit characters,
Unicode connecting characters, Unicode combining characters, or Unicode
formatting characters. For more information on Unicode categories, see the
[Unicode Category Database](https://www.unicode.org/reports/tr44/). You can
declare identifiers that match C# keywords by using the `@` prefix on the
identifier. The `@` is not part of the identifier name. For example, `@if`
declares an identifier named `if`. These [verbatim
identifiers](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/verbatim) are primarily for
interoperability with identifiers declared in other languages.

Now this is a perfectly fine answer, but I was just too lazy to go scouring
through the [Unicode Category Database](https://www.unicode.org/reports/tr44/)
🦥. What I wanted was a single table of potential valid identifier separators. I
googled for answers but came up short within a reasonable time, so instead I
came up with this quick `Program.cs`:

```csharp
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var findByCompile = true;
var encoding = Encoding.Unicode;
Console.OutputEncoding = encoding;
Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };
// Cache metadata reference since this reduces time significantly
var metadataReferences = new[] { MetadataReference.CreateFromFile(
    typeof(object).Assembly.Location) };

var validSeparators = new List<char>();
var invalidSeparators = new List<char>();

var validFileNameChars = new List<char>();
var invalidFileNameChars = Path.GetInvalidFileNameChars();
Array.Sort(invalidFileNameChars);

var stopwatch = Stopwatch.StartNew();
for (int i = char.MinValue; i <= char.MaxValue;)
{
    var c = (char)i;

    (IsValidSeparator(c) ? validSeparators : invalidSeparators).Add(c);

    if (Array.BinarySearch(invalidFileNameChars, c) < 0)
    { validFileNameChars.Add(c); }

    if (++i % 4096 == 0 && findByCompile) { log($"Compiled {i:D5} programs"); }
}
var elapsed_ms = stopwatch.ElapsedMilliseconds;

Write(validSeparators);
Write(invalidSeparators);
Write(validFileNameChars);

var totalCount = validSeparators.Count + invalidSeparators.Count;
log($"Found {validSeparators.Count} valid identifier separator chars.");
log($"Found {invalidSeparators.Count} invalid identifier separator chars.");
log($"Found {validFileNameChars.Count} valid file name chars.");
log($"Checked {totalCount} chars in {elapsed_ms} ms or " +
    $"{elapsed_ms / (double)totalCount:F3} ms per program.");

bool IsValidSeparator(char c) => findByCompile ? DoesCompile(c)
    : SyntaxFacts.IsValidIdentifier(Identifier(c));

bool DoesCompile(char c)
{
    var program = $"var {Identifier(c)} = 42;";
    var syntaxTree = CSharpSyntaxTree.ParseText(program);
    var compilation = CSharpCompilation.Create("assemblyName",
        new[] { syntaxTree }, metadataReferences,
        new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    using var dllStream = new MemoryStream();
    var emitResult = compilation.Emit(dllStream);
    return emitResult.Success;
}

static string Identifier(char c) => $"_{c}_";

void Write(IReadOnlyList<char> chars,
    [CallerArgumentExpression("chars")] string fileName = "")
{
    const string baseDir = "../../../../";
    File.WriteAllText(baseDir + $"{fileName}.csv", ToCsv(chars), encoding);
    File.WriteAllText(baseDir + $"{fileName}.md", ToTable(chars), encoding);
}

static string ToCsv(IReadOnlyList<char> chars) => string.Join(Environment.NewLine,
    new[] { CsvHeader() }.Concat(chars.Select(c => CsvLine(c))));
static string CsvHeader() => "Decimal,Hex,Identifier";
static string CsvLine(char c) => $"{(int)c:D5},0x{(int)c:X4},{Identifier(c)}";

static string ToTable(IReadOnlyList<char> chars) => string.Join(Environment.NewLine,
    new[] { TableHeader() }.Concat(chars.Select(c => TableLine(c))));
static string TableHeader() => $"|Decimal|Hex|Identifier|{Environment.NewLine}|-:|-:|-|";
static string TableLine(char c) => $"|{(int)c:D5}|`0x{(int)c:X4}`|`{Identifier(c)}`|";
```
For completeness here also the accompanying `ValidIdentifierSeparators.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.3.0" />
  </ItemGroup>
</Project>
```
As can be seen this references `Microsoft.CodeAnalysis.CSharp` or
[Roslyn](https://github.com/dotnet/roslyn) as a nuget package. 

The program is fairly self explanatory... as the astute reader might have
observed, though, and as it happens with any software development you might
start out going down the "wrong" rabbit hole. I certainly did that by first
simply defining a full [top-level
statement](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/top-level-statements)
single line program like `var _{c}_ = 42;` where `c` would be a given character
and then use Roslyn to compile that program and check if this would succeed
using
[`CSharpCompilation.Emit`}(https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpcompilation?view=roslyn-dotnet-4.3.0).
Hence, I ended up compiling 65.536 programs to check if a given identifier was
valid or not. This took about 300 s or about 5 minutes. Faster than scouring
through the unicode database I am sure 😅 This seemed a bit slow though. A quick
profiling session revealed that:
```csharp
MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
```
took up most of the time. This basically parses and builds a "metadata
reference" to the assembly containing `object`, which is the only assembly
needed for the `var _{c}_ = 42;` program. This was the reason I moved this to
the top of the program so it only happened once. This brough the run time down
to ~22 s or 14-15x faster:
```
Found 50683 valid identifier separator chars.
Found 14853 invalid identifier separator chars.
Found 65495 valid file name chars.
Checked 65536 chars in 22076 ms or 0.337 ms per program.
```
Later, I finally found out Roslyn has the very nice
[`SyntaxFacts`](https://learn.microsoft.com/ja-jp/dotnet/api/microsoft.codeanalysis.csharp.syntaxfacts?view=roslyn-dotnet)
and the obvious method to use namely
[`IsValidIdentifer`](https://learn.microsoft.com/ja-jp/dotnet/api/microsoft.codeanalysis.csharp.syntaxfacts.isvalididentifier?view=roslyn-dotnet#microsoft-codeanalysis-csharp-syntaxfacts-isvalididentifier(system-string)).
Using this cut down run time to 5 ms or 60,000x faster than the initial program.
😆
```
Found 50683 valid identifier separator chars.
Found 14853 invalid identifier separator chars.
Found 65495 valid file name chars.
Checked 65536 chars in 5 ms or 0.000 ms per program.
```
I've kept both approaches in the above program and this also includes finding
valid file name `char`'s for completeness. Without all this you could have
simply written the quick program below to trace valid separator `char`s.
```csharp
for (int i = char.MinValue; i <= char.MaxValue; i++)
{
    var c = (char)i;
    var identifier = $"_{c}_";
    if (SyntaxFacts.IsValidIdentifier(identifier))
    {
        Trace.WriteLine(c);
    }
}
```

Note Jekyll can't hand the large markdown files so those have given the `txt`
extension instead.
* [validSeparators.csv]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validSeparators.csv)
* [validFileNameChars.csv]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validFileNameChars.csv)
* [invalidSeparators.csv]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/invalidSeparators.csv)
* [validFileNameChars.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validFileNameChars.md)
* [validSeparators.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validSeparators.md)
* [invalidSeparators.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/invalidSeparators.md)

https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name

https://stackoverflow.com/questions/1904252/is-there-a-method-in-c-sharp-to-check-if-a-string-is-a-valid-identifier

https://stackoverflow.com/questions/1829679/how-to-determine-if-a-string-is-a-valid-variable-name

