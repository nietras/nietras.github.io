using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var encoding = Encoding.Unicode;
Console.OutputEncoding = encoding;
Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

var validSeparatorChars = new List<char>();
var invalidSeparatorChars = new List<char>();

var invalidFileNameChars = Path.GetInvalidFileNameChars();
var validFileNameChars = new List<char>();

var stopwatch = Stopwatch.StartNew();
for (int i = char.MinValue; i <= char.MaxValue; ++i)
{
    var c = (char)i;
    var source = Program(c);
    if (Compiles(source))
    {
        validSeparatorChars.Add(c);
        log(CsvLine(c));
    }
    else
    {
        invalidSeparatorChars.Add(c);
    }
    if (!invalidFileNameChars.Contains(c)) { validFileNameChars.Add(c); }
}
var elapsed_s = stopwatch.ElapsedMilliseconds;

var baseDir = "../../../../";
File.WriteAllText(baseDir + "ValidSeparatorChars.csv", ToCsv(validSeparatorChars), encoding);
File.WriteAllText(baseDir + "ValidSeparatorChars.md", ToMarkdownTable(validSeparatorChars), encoding);

File.WriteAllText(baseDir + "InvalidSeparatorChars.csv", ToCsv(invalidSeparatorChars), encoding);
File.WriteAllText(baseDir + "InvalidSeparatorChars.md", ToMarkdownTable(invalidSeparatorChars), encoding);

File.WriteAllText(baseDir + "ValidFileNameChars.csv", ToCsv(validFileNameChars), encoding);
File.WriteAllText(baseDir + "ValidFileNameChars.md", ToMarkdownTable(validFileNameChars), encoding);

log($"Found {validSeparatorChars.Count} valid and {invalidSeparatorChars.Count} invalid " +
    $"separator chars and {validFileNameChars.Count} valid file name chars " +
    $"among {validSeparatorChars.Count + invalidSeparatorChars.Count} " +
    $"in {elapsed_s} ms");

static string Program(char c) => $"var {Identifier(c)} = 42;";
static string Identifier(char c) => $"_{c}_";

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
static string CsvHeader() => "Decimal,Hex,Char,Identifier";
static string CsvLine(char c) => $"{(int)c:D5},0x{(int)c:X4},{c},{Identifier(c)}";

static string ToMarkdownTable(List<char> chars) => string.Join(Environment.NewLine,
    new[] { TableHeader() }.Concat(chars.Select(c => TableLine(c))));
static string TableHeader() => $"|Decimal|Hex|Char|Identifier|{Environment.NewLine}|-:|-:|-:|-|";
static string TableLine(char c) => $"|{(int)c:D5}|`0x{(int)c:X4}`|`{c}`|`{Identifier(c)}`|";
