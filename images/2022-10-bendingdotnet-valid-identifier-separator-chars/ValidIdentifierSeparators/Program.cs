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
    File.WriteAllText(baseDir + $"{fileName}.csv.txt", ToCsv(chars), encoding);
    File.WriteAllText(baseDir + $"{fileName}.md.txt", ToTable(chars), encoding);
}

static string ToCsv(IReadOnlyList<char> chars) => string.Join(Environment.NewLine,
    new[] { CsvHeader() }.Concat(chars.Select(c => CsvLine(c))));
static string CsvHeader() => "Decimal,Hex,Identifier";
static string CsvLine(char c) => $"{(int)c:D5},0x{(int)c:X4},{Identifier(c)}";

static string ToTable(IReadOnlyList<char> chars) => string.Join(Environment.NewLine,
    new[] { TableHeader() }.Concat(chars.Select(c => TableLine(c))));
static string TableHeader() => $"|Decimal|Hex|Identifier|{Environment.NewLine}|-:|-:|-|";
static string TableLine(char c) => $"|{(int)c:D5}|`0x{(int)c:X4}`|`{Identifier(c)}`|";
