---
layout: post
title: Bending .NET - Compiling 65,536 Programs with Roslyn to Find Valid Identifier Separator char's... then just use `SyntaxFacts.IsValidIdentifier` 🤦‍
---
or [how to find invalid human batteries so they can be flushed](https://www.youtube.com/watch?v=HwhB5uCaj3Y).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I look at compiling a complete list
of valid and invalid C# identifier separators by using Roslyn. That is, by being
lazy and instead of checking unicode specification just try every single
character. 

![sloth]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/Mother_and_baby_sloth_crossing_the_road.jpg)
Source: [wikimedia](https://commons.wikimedia.org/wiki/File:Mother_and_baby_sloth_crossing_the_road.jpg)

Encoding information in file or directory names is a simple way of keeping
details about a given file. For example, an image or an ONNX model and the
conditions or origins of it. That is, for an ONNX model it might be something
simple like the version of the ground truth that it was trained on. Assumming
you, of course, version your ground truth. Defining a simple schema for this
encoding using simple separators e.g. `_`, `=` can make it easy to then parse in
a position independent manner like `M=Surface_V=1.2.1.onnx`. Now these files
might allow embedding this information in them in some way, but sometimes it is
just easier for reference to have it directly in the file name, for example.

Sometimes it is also nice to be able to embed these assets into .NET assemblies
and have them exposed as simple properties. Preferably properties that have the
same name as the file. However, C# naturally has more [strict rules for
identifier
names](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
than file names, which brings up the question what `char`s are valid separators
in a C# identifier name? 

The above link says:

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
🦥. What I wanted was a single table of valid identifier separators. I googled
for answers but came up short within a reasonable time, so instead I came up
with this `Program.cs` (after some iterations as discussed below):

```csharp
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var findByCompile = false;
var encoding = Encoding.UTF8;
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
Write(invalidFileNameChars);

var totalCount = validSeparators.Count + invalidSeparators.Count;
log($"Found {validSeparators.Count} valid identifier separator chars.");
log($"Found {invalidSeparators.Count} invalid identifier separator chars.");
log($"Found {validFileNameChars.Count} valid file name chars.");
log($"Checked {totalCount} chars in {elapsed_ms} ms or " +
    $"{elapsed_ms / (double)totalCount:F3} ms per char.");

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
    File.WriteAllText(baseDir + $"{fileName}.txt", ToTable(chars), encoding);
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

The program is fairly self explanatory. As the astute reader might have
observed, though, and as it happens with any software development you might
start out going down the "wrong" rabbit hole. I certainly did that by first
simply defining a full [top-level
statement](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/top-level-statements)
single line program like `var _{c}_ = 42;` where `c` would be a given character
and then use Roslyn to compile that program and check if this would succeed
using
[`CSharpCompilation.Emit`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpcompilation?view=roslyn-dotnet-4.3.0).
Carefully encapsulating the char in underscores `_` to handle both the fact the
first character is limited and if a char being tested is considered white space,
which the final underscore handles.

Hence, I ended up compiling 65.536 programs to check if a given identifier was
valid or not. This took about 300 s or about 5 minutes. Faster than scouring
through the unicode database I am sure 😅 This seemed a bit slow... a quick
profiling session revealed that:
```csharp
MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
```
took up most of the time. This basically parses and builds a "metadata
reference" to the assembly containing `object`, which is the only assembly
needed for the `var _{c}_ = 42;` program. This was the reason I moved this to
the top of the program so it only happened once. This brought the run time down
to ~22 s or 14-15x faster:
```
Found 50683 valid identifier separator chars.
Found 14853 invalid identifier separator chars.
Found 65495 valid file name chars.
Checked 65536 chars in 22076 ms or 0.337 ms per char.
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
Checked 65536 chars in 5 ms or 0.000 ms per char.
```
I've kept both approaches in the above program and this also includes finding
valid file name `char`'s for completeness. Without all this (and in particular
all the output related code) you could have simply written the quick program
below to trace valid separator `char`s. 🙄
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

Compiled lists of valid and invalid separator chars and file name chars can be
found in the table below. Note [Jekyll](https://jekyllrb.com/) (used for this
blog) can't handle the large markdown files so files have been given the extra
`.txt` extension.

| CSV | Markdown as txt |
|-|-|
| [validSeparators.csv.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validSeparators.csv.txt) | [validSeparators.md.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validSeparators.md.txt) |
| [validFileNameChars.csv.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validFileNameChars.csv.txt) | [validFileNameChars.md.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/validFileNameChars.md.txt) |
| [invalidSeparators.csv.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/invalidSeparators.csv.txt) | [invalidSeparators.md.txt]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/invalidSeparators.md.txt) |

For reference in the next sections are the first valid separators until decimal
511 and first invalid separators. It is noteworthy - and of course natural -
that most "ascii" non-digit/non-alpha characters are invalid as identifier
separators. `_` being the exception. I selected a few that might be used below,
but these also have issues with being easily confused with the "normal" ascii
range versions.  Of course, this assume you use English as a language, so this
is probably abusing other languages letters. They is another issue though...

|Decimal|Hex|Identifier|
|-:|-:|-|
|00095|`0x005F`|`___`|
|00170|`0x00AA`|`_ª_`|
|00448|`0x01C0`|`_ǀ_`|
|00449|`0x01C1`|`_ǁ_`|
|00450|`0x01C2`|`_ǂ_`|
|00451|`0x01C3`|`_ǃ_`|
|00713|`0x02C9`|`_ˉ_`|
|00714|`0x02CA`|`_ˊ_`|
|00715|`0x02CB`|`_ˋ_`|
|00716|`0x02CC`|`_ˌ_`|
|00717|`0x02CD`|`_ˍ_`|
|00718|`0x02CE`|`_ˎ_`|
|00719|`0x02CF`|`_ˏ_`|
|00720|`0x02D0`|`_ː_`|
|00721|`0x02D1`|`_ˑ_`|
|00748|`0x02EC`|`_ˬ_`|
|00926|`0x039E`|`_Ξ_`|
|01994|`0x07CA`|`_ߊ_`|
|01997|`0x07CD`|`_ߍ_`|

... and that is some characters display as a square or rectangular box, or as a box
with a dot, question mark or “x” inside depending on system or browser used. As
can be seen below with screnshots from a desktop browser and on the phone. On
the phone not all characters are displayed. I am no expert on the matter but as
far as I know this is a font issue. So the end result is it is very hard to find
any other good separator char than underscore `_` in C# identifiers. However,
you could then use multiples of this for separation instead e.g.
`M_Surface__V_1.2.1.onnx`.

|Desktop|Phone|
|-|-|
|![desktop]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/separators-desktop.png)|![phone]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars/separators-phone.png)|

### Some Valid Identifier Separators

|Decimal|Hex|Identifier|
|-:|-:|-|
|00048|`0x0030`|`_0_`|
|00049|`0x0031`|`_1_`|
|00050|`0x0032`|`_2_`|
|00051|`0x0033`|`_3_`|
|00052|`0x0034`|`_4_`|
|00053|`0x0035`|`_5_`|
|00054|`0x0036`|`_6_`|
|00055|`0x0037`|`_7_`|
|00056|`0x0038`|`_8_`|
|00057|`0x0039`|`_9_`|
|00065|`0x0041`|`_A_`|
|00066|`0x0042`|`_B_`|
|00067|`0x0043`|`_C_`|
|00068|`0x0044`|`_D_`|
|00069|`0x0045`|`_E_`|
|00070|`0x0046`|`_F_`|
|00071|`0x0047`|`_G_`|
|00072|`0x0048`|`_H_`|
|00073|`0x0049`|`_I_`|
|00074|`0x004A`|`_J_`|
|00075|`0x004B`|`_K_`|
|00076|`0x004C`|`_L_`|
|00077|`0x004D`|`_M_`|
|00078|`0x004E`|`_N_`|
|00079|`0x004F`|`_O_`|
|00080|`0x0050`|`_P_`|
|00081|`0x0051`|`_Q_`|
|00082|`0x0052`|`_R_`|
|00083|`0x0053`|`_S_`|
|00084|`0x0054`|`_T_`|
|00085|`0x0055`|`_U_`|
|00086|`0x0056`|`_V_`|
|00087|`0x0057`|`_W_`|
|00088|`0x0058`|`_X_`|
|00089|`0x0059`|`_Y_`|
|00090|`0x005A`|`_Z_`|
|00095|`0x005F`|`___`|
|00097|`0x0061`|`_a_`|
|00098|`0x0062`|`_b_`|
|00099|`0x0063`|`_c_`|
|00100|`0x0064`|`_d_`|
|00101|`0x0065`|`_e_`|
|00102|`0x0066`|`_f_`|
|00103|`0x0067`|`_g_`|
|00104|`0x0068`|`_h_`|
|00105|`0x0069`|`_i_`|
|00106|`0x006A`|`_j_`|
|00107|`0x006B`|`_k_`|
|00108|`0x006C`|`_l_`|
|00109|`0x006D`|`_m_`|
|00110|`0x006E`|`_n_`|
|00111|`0x006F`|`_o_`|
|00112|`0x0070`|`_p_`|
|00113|`0x0071`|`_q_`|
|00114|`0x0072`|`_r_`|
|00115|`0x0073`|`_s_`|
|00116|`0x0074`|`_t_`|
|00117|`0x0075`|`_u_`|
|00118|`0x0076`|`_v_`|
|00119|`0x0077`|`_w_`|
|00120|`0x0078`|`_x_`|
|00121|`0x0079`|`_y_`|
|00122|`0x007A`|`_z_`|
|00170|`0x00AA`|`_ª_`|
|00173|`0x00AD`|`_­_`|
|00181|`0x00B5`|`_µ_`|
|00186|`0x00BA`|`_º_`|
|00192|`0x00C0`|`_À_`|
|00193|`0x00C1`|`_Á_`|
|00194|`0x00C2`|`_Â_`|
|00195|`0x00C3`|`_Ã_`|
|00196|`0x00C4`|`_Ä_`|
|00197|`0x00C5`|`_Å_`|
|00198|`0x00C6`|`_Æ_`|
|00199|`0x00C7`|`_Ç_`|
|00200|`0x00C8`|`_È_`|
|00201|`0x00C9`|`_É_`|
|00202|`0x00CA`|`_Ê_`|
|00203|`0x00CB`|`_Ë_`|
|00204|`0x00CC`|`_Ì_`|
|00205|`0x00CD`|`_Í_`|
|00206|`0x00CE`|`_Î_`|
|00207|`0x00CF`|`_Ï_`|
|00208|`0x00D0`|`_Ð_`|
|00209|`0x00D1`|`_Ñ_`|
|00210|`0x00D2`|`_Ò_`|
|00211|`0x00D3`|`_Ó_`|
|00212|`0x00D4`|`_Ô_`|
|00213|`0x00D5`|`_Õ_`|
|00214|`0x00D6`|`_Ö_`|
|00216|`0x00D8`|`_Ø_`|
|00217|`0x00D9`|`_Ù_`|
|00218|`0x00DA`|`_Ú_`|
|00219|`0x00DB`|`_Û_`|
|00220|`0x00DC`|`_Ü_`|
|00221|`0x00DD`|`_Ý_`|
|00222|`0x00DE`|`_Þ_`|
|00223|`0x00DF`|`_ß_`|
|00224|`0x00E0`|`_à_`|
|00225|`0x00E1`|`_á_`|
|00226|`0x00E2`|`_â_`|
|00227|`0x00E3`|`_ã_`|
|00228|`0x00E4`|`_ä_`|
|00229|`0x00E5`|`_å_`|
|00230|`0x00E6`|`_æ_`|
|00231|`0x00E7`|`_ç_`|
|00232|`0x00E8`|`_è_`|
|00233|`0x00E9`|`_é_`|
|00234|`0x00EA`|`_ê_`|
|00235|`0x00EB`|`_ë_`|
|00236|`0x00EC`|`_ì_`|
|00237|`0x00ED`|`_í_`|
|00238|`0x00EE`|`_î_`|
|00239|`0x00EF`|`_ï_`|
|00240|`0x00F0`|`_ð_`|
|00241|`0x00F1`|`_ñ_`|
|00242|`0x00F2`|`_ò_`|
|00243|`0x00F3`|`_ó_`|
|00244|`0x00F4`|`_ô_`|
|00245|`0x00F5`|`_õ_`|
|00246|`0x00F6`|`_ö_`|
|00248|`0x00F8`|`_ø_`|
|00249|`0x00F9`|`_ù_`|
|00250|`0x00FA`|`_ú_`|
|00251|`0x00FB`|`_û_`|
|00252|`0x00FC`|`_ü_`|
|00253|`0x00FD`|`_ý_`|
|00254|`0x00FE`|`_þ_`|
|00255|`0x00FF`|`_ÿ_`|
|00256|`0x0100`|`_Ā_`|
|00257|`0x0101`|`_ā_`|
|00258|`0x0102`|`_Ă_`|
|00259|`0x0103`|`_ă_`|
|00260|`0x0104`|`_Ą_`|
|00261|`0x0105`|`_ą_`|
|00262|`0x0106`|`_Ć_`|
|00263|`0x0107`|`_ć_`|
|00264|`0x0108`|`_Ĉ_`|
|00265|`0x0109`|`_ĉ_`|
|00266|`0x010A`|`_Ċ_`|
|00267|`0x010B`|`_ċ_`|
|00268|`0x010C`|`_Č_`|
|00269|`0x010D`|`_č_`|
|00270|`0x010E`|`_Ď_`|
|00271|`0x010F`|`_ď_`|
|00272|`0x0110`|`_Đ_`|
|00273|`0x0111`|`_đ_`|
|00274|`0x0112`|`_Ē_`|
|00275|`0x0113`|`_ē_`|
|00276|`0x0114`|`_Ĕ_`|
|00277|`0x0115`|`_ĕ_`|
|00278|`0x0116`|`_Ė_`|
|00279|`0x0117`|`_ė_`|
|00280|`0x0118`|`_Ę_`|
|00281|`0x0119`|`_ę_`|
|00282|`0x011A`|`_Ě_`|
|00283|`0x011B`|`_ě_`|
|00284|`0x011C`|`_Ĝ_`|
|00285|`0x011D`|`_ĝ_`|
|00286|`0x011E`|`_Ğ_`|
|00287|`0x011F`|`_ğ_`|
|00288|`0x0120`|`_Ġ_`|
|00289|`0x0121`|`_ġ_`|
|00290|`0x0122`|`_Ģ_`|
|00291|`0x0123`|`_ģ_`|
|00292|`0x0124`|`_Ĥ_`|
|00293|`0x0125`|`_ĥ_`|
|00294|`0x0126`|`_Ħ_`|
|00295|`0x0127`|`_ħ_`|
|00296|`0x0128`|`_Ĩ_`|
|00297|`0x0129`|`_ĩ_`|
|00298|`0x012A`|`_Ī_`|
|00299|`0x012B`|`_ī_`|
|00300|`0x012C`|`_Ĭ_`|
|00301|`0x012D`|`_ĭ_`|
|00302|`0x012E`|`_Į_`|
|00303|`0x012F`|`_į_`|
|00304|`0x0130`|`_İ_`|
|00305|`0x0131`|`_ı_`|
|00306|`0x0132`|`_Ĳ_`|
|00307|`0x0133`|`_ĳ_`|
|00308|`0x0134`|`_Ĵ_`|
|00309|`0x0135`|`_ĵ_`|
|00310|`0x0136`|`_Ķ_`|
|00311|`0x0137`|`_ķ_`|
|00312|`0x0138`|`_ĸ_`|
|00313|`0x0139`|`_Ĺ_`|
|00314|`0x013A`|`_ĺ_`|
|00315|`0x013B`|`_Ļ_`|
|00316|`0x013C`|`_ļ_`|
|00317|`0x013D`|`_Ľ_`|
|00318|`0x013E`|`_ľ_`|
|00319|`0x013F`|`_Ŀ_`|
|00320|`0x0140`|`_ŀ_`|
|00321|`0x0141`|`_Ł_`|
|00322|`0x0142`|`_ł_`|
|00323|`0x0143`|`_Ń_`|
|00324|`0x0144`|`_ń_`|
|00325|`0x0145`|`_Ņ_`|
|00326|`0x0146`|`_ņ_`|
|00327|`0x0147`|`_Ň_`|
|00328|`0x0148`|`_ň_`|
|00329|`0x0149`|`_ŉ_`|
|00330|`0x014A`|`_Ŋ_`|
|00331|`0x014B`|`_ŋ_`|
|00332|`0x014C`|`_Ō_`|
|00333|`0x014D`|`_ō_`|
|00334|`0x014E`|`_Ŏ_`|
|00335|`0x014F`|`_ŏ_`|
|00336|`0x0150`|`_Ő_`|
|00337|`0x0151`|`_ő_`|
|00338|`0x0152`|`_Œ_`|
|00339|`0x0153`|`_œ_`|
|00340|`0x0154`|`_Ŕ_`|
|00341|`0x0155`|`_ŕ_`|
|00342|`0x0156`|`_Ŗ_`|
|00343|`0x0157`|`_ŗ_`|
|00344|`0x0158`|`_Ř_`|
|00345|`0x0159`|`_ř_`|
|00346|`0x015A`|`_Ś_`|
|00347|`0x015B`|`_ś_`|
|00348|`0x015C`|`_Ŝ_`|
|00349|`0x015D`|`_ŝ_`|
|00350|`0x015E`|`_Ş_`|
|00351|`0x015F`|`_ş_`|
|00352|`0x0160`|`_Š_`|
|00353|`0x0161`|`_š_`|
|00354|`0x0162`|`_Ţ_`|
|00355|`0x0163`|`_ţ_`|
|00356|`0x0164`|`_Ť_`|
|00357|`0x0165`|`_ť_`|
|00358|`0x0166`|`_Ŧ_`|
|00359|`0x0167`|`_ŧ_`|
|00360|`0x0168`|`_Ũ_`|
|00361|`0x0169`|`_ũ_`|
|00362|`0x016A`|`_Ū_`|
|00363|`0x016B`|`_ū_`|
|00364|`0x016C`|`_Ŭ_`|
|00365|`0x016D`|`_ŭ_`|
|00366|`0x016E`|`_Ů_`|
|00367|`0x016F`|`_ů_`|
|00368|`0x0170`|`_Ű_`|
|00369|`0x0171`|`_ű_`|
|00370|`0x0172`|`_Ų_`|
|00371|`0x0173`|`_ų_`|
|00372|`0x0174`|`_Ŵ_`|
|00373|`0x0175`|`_ŵ_`|
|00374|`0x0176`|`_Ŷ_`|
|00375|`0x0177`|`_ŷ_`|
|00376|`0x0178`|`_Ÿ_`|
|00377|`0x0179`|`_Ź_`|
|00378|`0x017A`|`_ź_`|
|00379|`0x017B`|`_Ż_`|
|00380|`0x017C`|`_ż_`|
|00381|`0x017D`|`_Ž_`|
|00382|`0x017E`|`_ž_`|
|00383|`0x017F`|`_ſ_`|
|00384|`0x0180`|`_ƀ_`|
|00385|`0x0181`|`_Ɓ_`|
|00386|`0x0182`|`_Ƃ_`|
|00387|`0x0183`|`_ƃ_`|
|00388|`0x0184`|`_Ƅ_`|
|00389|`0x0185`|`_ƅ_`|
|00390|`0x0186`|`_Ɔ_`|
|00391|`0x0187`|`_Ƈ_`|
|00392|`0x0188`|`_ƈ_`|
|00393|`0x0189`|`_Ɖ_`|
|00394|`0x018A`|`_Ɗ_`|
|00395|`0x018B`|`_Ƌ_`|
|00396|`0x018C`|`_ƌ_`|
|00397|`0x018D`|`_ƍ_`|
|00398|`0x018E`|`_Ǝ_`|
|00399|`0x018F`|`_Ə_`|
|00400|`0x0190`|`_Ɛ_`|
|00401|`0x0191`|`_Ƒ_`|
|00402|`0x0192`|`_ƒ_`|
|00403|`0x0193`|`_Ɠ_`|
|00404|`0x0194`|`_Ɣ_`|
|00405|`0x0195`|`_ƕ_`|
|00406|`0x0196`|`_Ɩ_`|
|00407|`0x0197`|`_Ɨ_`|
|00408|`0x0198`|`_Ƙ_`|
|00409|`0x0199`|`_ƙ_`|
|00410|`0x019A`|`_ƚ_`|
|00411|`0x019B`|`_ƛ_`|
|00412|`0x019C`|`_Ɯ_`|
|00413|`0x019D`|`_Ɲ_`|
|00414|`0x019E`|`_ƞ_`|
|00415|`0x019F`|`_Ɵ_`|
|00416|`0x01A0`|`_Ơ_`|
|00417|`0x01A1`|`_ơ_`|
|00418|`0x01A2`|`_Ƣ_`|
|00419|`0x01A3`|`_ƣ_`|
|00420|`0x01A4`|`_Ƥ_`|
|00421|`0x01A5`|`_ƥ_`|
|00422|`0x01A6`|`_Ʀ_`|
|00423|`0x01A7`|`_Ƨ_`|
|00424|`0x01A8`|`_ƨ_`|
|00425|`0x01A9`|`_Ʃ_`|
|00426|`0x01AA`|`_ƪ_`|
|00427|`0x01AB`|`_ƫ_`|
|00428|`0x01AC`|`_Ƭ_`|
|00429|`0x01AD`|`_ƭ_`|
|00430|`0x01AE`|`_Ʈ_`|
|00431|`0x01AF`|`_Ư_`|
|00432|`0x01B0`|`_ư_`|
|00433|`0x01B1`|`_Ʊ_`|
|00434|`0x01B2`|`_Ʋ_`|
|00435|`0x01B3`|`_Ƴ_`|
|00436|`0x01B4`|`_ƴ_`|
|00437|`0x01B5`|`_Ƶ_`|
|00438|`0x01B6`|`_ƶ_`|
|00439|`0x01B7`|`_Ʒ_`|
|00440|`0x01B8`|`_Ƹ_`|
|00441|`0x01B9`|`_ƹ_`|
|00442|`0x01BA`|`_ƺ_`|
|00443|`0x01BB`|`_ƻ_`|
|00444|`0x01BC`|`_Ƽ_`|
|00445|`0x01BD`|`_ƽ_`|
|00446|`0x01BE`|`_ƾ_`|
|00447|`0x01BF`|`_ƿ_`|
|00448|`0x01C0`|`_ǀ_`|
|00449|`0x01C1`|`_ǁ_`|
|00450|`0x01C2`|`_ǂ_`|
|00451|`0x01C3`|`_ǃ_`|
|00452|`0x01C4`|`_Ǆ_`|
|00453|`0x01C5`|`_ǅ_`|
|00454|`0x01C6`|`_ǆ_`|
|00455|`0x01C7`|`_Ǉ_`|
|00456|`0x01C8`|`_ǈ_`|
|00457|`0x01C9`|`_ǉ_`|
|00458|`0x01CA`|`_Ǌ_`|
|00459|`0x01CB`|`_ǋ_`|
|00460|`0x01CC`|`_ǌ_`|
|00461|`0x01CD`|`_Ǎ_`|
|00462|`0x01CE`|`_ǎ_`|
|00463|`0x01CF`|`_Ǐ_`|
|00464|`0x01D0`|`_ǐ_`|
|00465|`0x01D1`|`_Ǒ_`|
|00466|`0x01D2`|`_ǒ_`|
|00467|`0x01D3`|`_Ǔ_`|
|00468|`0x01D4`|`_ǔ_`|
|00469|`0x01D5`|`_Ǖ_`|
|00470|`0x01D6`|`_ǖ_`|
|00471|`0x01D7`|`_Ǘ_`|
|00472|`0x01D8`|`_ǘ_`|
|00473|`0x01D9`|`_Ǚ_`|
|00474|`0x01DA`|`_ǚ_`|
|00475|`0x01DB`|`_Ǜ_`|
|00476|`0x01DC`|`_ǜ_`|
|00477|`0x01DD`|`_ǝ_`|
|00478|`0x01DE`|`_Ǟ_`|
|00479|`0x01DF`|`_ǟ_`|
|00480|`0x01E0`|`_Ǡ_`|
|00481|`0x01E1`|`_ǡ_`|
|00482|`0x01E2`|`_Ǣ_`|
|00483|`0x01E3`|`_ǣ_`|
|00484|`0x01E4`|`_Ǥ_`|
|00485|`0x01E5`|`_ǥ_`|
|00486|`0x01E6`|`_Ǧ_`|
|00487|`0x01E7`|`_ǧ_`|
|00488|`0x01E8`|`_Ǩ_`|
|00489|`0x01E9`|`_ǩ_`|
|00490|`0x01EA`|`_Ǫ_`|
|00491|`0x01EB`|`_ǫ_`|
|00492|`0x01EC`|`_Ǭ_`|
|00493|`0x01ED`|`_ǭ_`|
|00494|`0x01EE`|`_Ǯ_`|
|00495|`0x01EF`|`_ǯ_`|
|00496|`0x01F0`|`_ǰ_`|
|00497|`0x01F1`|`_Ǳ_`|
|00498|`0x01F2`|`_ǲ_`|
|00499|`0x01F3`|`_ǳ_`|
|00500|`0x01F4`|`_Ǵ_`|
|00501|`0x01F5`|`_ǵ_`|
|00502|`0x01F6`|`_Ƕ_`|
|00503|`0x01F7`|`_Ƿ_`|
|00504|`0x01F8`|`_Ǹ_`|
|00505|`0x01F9`|`_ǹ_`|
|00506|`0x01FA`|`_Ǻ_`|
|00507|`0x01FB`|`_ǻ_`|
|00508|`0x01FC`|`_Ǽ_`|
|00509|`0x01FD`|`_ǽ_`|
|00510|`0x01FE`|`_Ǿ_`|
|00511|`0x01FF`|`_ǿ_`|

### Some Invalid Identifier Separators

|Decimal|Hex|Identifier|
|-:|-:|-|
|00032|`0x0020`|`_ _`|
|00033|`0x0021`|`_!_`|
|00034|`0x0022`|`_"_`|
|00035|`0x0023`|`_#_`|
|00036|`0x0024`|`_$_`|
|00037|`0x0025`|`_%_`|
|00038|`0x0026`|`_&_`|
|00039|`0x0027`|`_'_`|
|00040|`0x0028`|`_(_`|
|00041|`0x0029`|`_)_`|
|00042|`0x002A`|`_*_`|
|00043|`0x002B`|`_+_`|
|00044|`0x002C`|`_,_`|
|00045|`0x002D`|`_-_`|
|00046|`0x002E`|`_._`|
|00047|`0x002F`|`_/_`|
|00058|`0x003A`|`_:_`|
|00059|`0x003B`|`_;_`|
|00060|`0x003C`|`_<_`|
|00061|`0x003D`|`_=_`|
|00062|`0x003E`|`_>_`|
|00063|`0x003F`|`_?_`|
|00064|`0x0040`|`_@_`|
|00091|`0x005B`|`_[_`|
|00092|`0x005C`|`_\_`|
|00093|`0x005D`|`_]_`|
|00094|`0x005E`|`_^_`|
|00096|`0x0060`|backtick|
|00123|`0x007B`|`_{_`|
|00124|`0x007C`|`_|_`|
|00125|`0x007D`|`_}_`|
|00126|`0x007E`|`_~_`|
|00127|`0x007F`|`__`|

### Related Stack Overflow Questions

* [What characters are allowed in C# class
  name?](https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name)
  this is what initially led me to go down the wrong rabbit hole, since among
  others the top answer is given below and I definitely didn't want to go
  through those. 😅
  > Essentially, any unicode character (including unicode escapes) in the
  > character classes Lu, Ll, Lt, Lm, Lo, Nl, Mn, Mc, Nd, Pc, and Cf. The first
  > character is an exception and it must be a letter (classes Lu, Ll, Lt, Lm,
  > or Lo) or an underscore. Also, if the identifier is a keyword, you must
  > stick an @ in front of it. The @ is optional otherwise.
* [Is there a method in C# to check if a string is a valid identifier
  [duplicate]](https://stackoverflow.com/questions/1904252/is-there-a-method-in-c-sharp-to-check-if-a-string-is-a-valid-identifier)
  didn't find this at first and although marked as duplicate the second answer
  is exactly what I was looking for:
  ```csharp
  var isValid = Microsoft.CodeAnalysis.CSharp.
      SyntaxFacts.IsValidIdentifier("I'mNotValid");
  Console.WriteLine(isValid);     // False
  ```
* [How to determine if a string is a valid variable
  name?](https://stackoverflow.com/questions/1829679/how-to-determine-if-a-string-is-a-valid-variable-name)
  this is the question/answer the above was considered a duplicate for, but it
  does not mention `SyntaxFacts`.

