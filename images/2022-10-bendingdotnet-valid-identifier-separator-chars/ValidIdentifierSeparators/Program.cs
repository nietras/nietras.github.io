using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var findByCompile = false;
var encoding = Encoding.Unicode;
Console.OutputEncoding = encoding;
Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };
// Cache metadata reference since this reduces time significantly
var metadataReferences = new[] { MetadataReference.CreateFromFile(
    typeof(object).Assembly.Location) };

var validSeparatorChars = new List<char>();
var invalidSeparatorChars = new List<char>();

var invalidFileNameChars = Path.GetInvalidFileNameChars();
Array.Sort(invalidFileNameChars);
var validFileNameChars = new List<char>();

var stopwatch = Stopwatch.StartNew();
for (int i = char.MinValue; i <= char.MaxValue;)
{
    var c = (char)i;

    (IsValidSeparator(c) ? validSeparatorChars : invalidSeparatorChars).Add(c);

    if (Array.BinarySearch(invalidFileNameChars, c) < 0)
    { validFileNameChars.Add(c); }

    if (++i % 4096 == 0 && findByCompile) { log($"Compiled {i:D5} programs"); }
}
var elapsed_ms = stopwatch.ElapsedMilliseconds;

Write(validSeparatorChars);
Write(invalidSeparatorChars);
Write(validFileNameChars);

var totalCount = validSeparatorChars.Count + invalidSeparatorChars.Count;
log($"Found {validSeparatorChars.Count}/{totalCount} valid identifier separator chars.");
log($"Found {invalidSeparatorChars.Count}/{totalCount} invalid identifier separator chars.");
log($"Found {validFileNameChars.Count}/{totalCount} valid file name chars.");
log($"In {elapsed_ms} ms or {elapsed_ms / (double)totalCount:F3} ms per program.");

bool IsValidSeparator(char c) => findByCompile ? Compiles(c)
    : SyntaxFacts.IsValidIdentifier(Identifier(c));

bool Compiles(char c)
{
    var program = $"var {Identifier(c)} = 42;";
    var syntaxTree = CSharpSyntaxTree.ParseText(program);
    var compilation = CSharpCompilation.Create("assemblyName",
        new[] { syntaxTree }, metadataReferences,
        new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    using var dllStream = new MemoryStream();
    var emitResult = compilation.Emit(dllStream);
    var compiles = emitResult.Success;
    return compiles;
}

static string Identifier(char c) => $"_{c}_";

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
