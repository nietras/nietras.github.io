using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var encoding = Encoding.Unicode;
Console.OutputEncoding = encoding;
Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

var validSeparatorChars = new List<char>();
var validDirectoryChars = new List<char>();
var invalidSeparatorChars = new List<char>();

var stopwatch = Stopwatch.StartNew();
for (int i = char.MinValue; i <= char.MaxValue; ++i)
{
    var c = (char)i;
    var source = GetSource(c);
    if (Compiles(source))
    {
        validSeparatorChars.Add(c);
        log(CsvLine(c));
    }
    else
    {
        invalidSeparatorChars.Add(c);
    }
    if (IsValidInPath(c)) { validDirectoryChars.Add(c); }
}
var elapsed_s = stopwatch.ElapsedMilliseconds;

var baseDir = "../../../../";
File.WriteAllText(baseDir + "ValidSeparatorChars.csv", ToCsv(validSeparatorChars), encoding);
File.WriteAllText(baseDir + "ValidSeparatorChars.md", ToMarkdownTable(validSeparatorChars), encoding);

File.WriteAllText(baseDir + "ValidDirectoryChars.csv", ToCsv(validDirectoryChars), encoding);
File.WriteAllText(baseDir + "ValidDirectoryChars.md", ToMarkdownTable(validDirectoryChars), encoding);

File.WriteAllText(baseDir + "InvalidSeparatorChars.csv", ToCsv(invalidSeparatorChars), encoding);
File.WriteAllText(baseDir + "InvalidSeparatorChars.md", ToMarkdownTable(invalidSeparatorChars), encoding);

log($"Found {validSeparatorChars.Count} valid and {invalidSeparatorChars.Count} invalid " +
    $"separator chars and {validDirectoryChars.Count} valid directory chars " +
    $"among {validSeparatorChars.Count + invalidSeparatorChars.Count} " +
    $"in {elapsed_s} ms");

static string GetSource(char c) => $"var _{c}_ = 42;";

bool Compiles(string source)
{
    var syntaxTree = CSharpSyntaxTree.ParseText(source, encoding: encoding);
    var compilation = CSharpCompilation.Create("assemblyName",
        new[] { syntaxTree },
        new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
        new CSharpCompilationOptions(OutputKind.ConsoleApplication));

    using var dllStream = new MemoryStream();
    var emitResult = compilation.Emit(dllStream);
    var compiles = emitResult.Success;
    return compiles;
}

static string ToCsv(List<char> chars) => string.Join(Environment.NewLine,
    new[] { CsvHeader() }.Concat(chars.Select(c => CsvLine(c))));

static string CsvHeader() => "Decimal,Hex,Char,Source";
static string CsvLine(char c) => $"{(int)c:D5},0x{(int)c:X4},{c},{GetSource(c)}";

static string ToMarkdownTable(List<char> chars) => string.Join(Environment.NewLine,
    new[] { TableHeader() }.Concat(chars.Select(c => TableLine(c))));

static string TableHeader() => $"|Decimal|Hex|Char|Source|{Environment.NewLine}|-:|-:|-:|-|";
static string TableLine(char c) => $"|{(int)c:D5}|`0x{(int)c:X4}`|`{c}`|`{GetSource(c)}`|";

static bool IsValidInPath(char c)
{
    // Could also use Path.InvalidPathChars
    try { Path.GetFullPath($"_{c}_"); return true; } catch { return false; }
}
