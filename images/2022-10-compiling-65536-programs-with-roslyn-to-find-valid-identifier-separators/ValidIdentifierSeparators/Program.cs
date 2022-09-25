using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var encoding = Encoding.UTF8;
Console.OutputEncoding = encoding;
Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

var validChars = new List<char>();
var invalidChars = new List<char>();

using var dllStream = new MemoryStream();
var stopwatch = Stopwatch.StartNew();
for (int i = char.MinValue; i <= char.MaxValue; ++i)
{
    var c = (char)i;

    var source = GetSource(c);
    var syntaxTree = CSharpSyntaxTree.ParseText(source, encoding: encoding);
    var compilation = CSharpCompilation.Create("assemblyName",
        new[] { syntaxTree },
        new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
        new CSharpCompilationOptions(OutputKind.ConsoleApplication));

    dllStream.Position = 0;
    var emitResult = compilation.Emit(dllStream);
    if (emitResult.Success)
    {
        validChars.Add(c);
        log(CsvLine(c));
    }
    else
    {
        invalidChars.Add(c);
    }
}
var elapsed_s = stopwatch.ElapsedMilliseconds;

File.WriteAllText("ValidChars.csv", ToCsv(validChars), encoding);
File.WriteAllText("InvalidChars.csv", ToCsv(invalidChars), encoding);

File.WriteAllText("ValidChars.md", ToMarkdownTable(validChars), encoding);
File.WriteAllText("InvalidChars.md", ToMarkdownTable(invalidChars), encoding);

log($"Found {validChars.Count} valid and {invalidChars.Count} invalid " +
    $"separator chars among {validChars.Count + invalidChars.Count} " +
    $"in {elapsed_s} ms");

static string GetSource(char c) => $"var _{c}_ = 42;";

static string ToCsv(List<char> chars) => string.Join(Environment.NewLine,
    new[] { CsvHeader() }.Concat(chars.Select(c => CsvLine(c))));

static string CsvHeader() => "Decimal,Hex,Char,Source";
static string CsvLine(char c) => $"{(int)c:D5},0x{(int)c:X4},{c},{GetSource(c)}";

static string ToMarkdownTable(List<char> chars) => string.Join(Environment.NewLine,
    new[] { TableHeader() }.Concat(chars.Select(c => TableLine(c))));

static string TableHeader() => $"|Decimal|Hex|Char|Source|{Environment.NewLine}|-:|-:|-:|-|";
static string TableLine(char c) => $"|{(int)c:D5}|`0x{(int)c:X4}`|`{c}`|`{GetSource(c)}`|";
