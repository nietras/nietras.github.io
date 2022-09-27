using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var encoding = Encoding.Unicode;
Console.OutputEncoding = encoding;
Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };
// Cache metadata reference since this reduces time from 300 s to 40 s
var metadataReferences = new[] { MetadataReference.CreateFromFile(
    typeof(object).Assembly.Location) };

var validSeparatorChars = new List<char>();
var invalidSeparatorChars = new List<char>();

var invalidFileNameChars = Path.GetInvalidFileNameChars();
Array.Sort(invalidFileNameChars);
var validFileNameChars = new List<char>();

var stopwatch = Stopwatch.StartNew();
for (int i = char.MinValue; i <= char.MaxValue; ++i)
{
    var c = (char)i;
    var program = $"var {Identifier(c)} = 42;";

    (Compiles(program) ? validSeparatorChars : invalidSeparatorChars).Add(c);

    if (Array.BinarySearch(invalidFileNameChars, c) < 0)
    { validFileNameChars.Add(c); }

    log(CsvLine(c));
}
var elapsed_ms = stopwatch.ElapsedMilliseconds;

Write(validSeparatorChars);
Write(invalidSeparatorChars);
Write(validFileNameChars);

var totalCount = validSeparatorChars.Count + invalidSeparatorChars.Count;
log($"Found {validSeparatorChars.Count} valid and {invalidSeparatorChars.Count} invalid " +
    $"identifier separator chars and {validFileNameChars.Count} valid file name chars " +
    $"among {totalCount} in {elapsed_ms} ms or " +
    $"{elapsed_ms / (double)totalCount:F1} ms per program");

static string Identifier(char c) => $"_{c}_";

bool Compiles(string source)
{
    var syntaxTree = CSharpSyntaxTree.ParseText(source);
    var compilation = CSharpCompilation.Create("assemblyName",
        new[] { syntaxTree }, metadataReferences,
        new CSharpCompilationOptions(OutputKind.ConsoleApplication));

    using var dllStream = new MemoryStream();
    var emitResult = compilation.Emit(dllStream);
    var compiles = emitResult.Success;
    return compiles;
}

void Write(IReadOnlyList<char> chars, [CallerArgumentExpression("chars")] string fileName = "")
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
