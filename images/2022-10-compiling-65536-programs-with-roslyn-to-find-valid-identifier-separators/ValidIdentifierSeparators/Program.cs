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
    var program = Program(c);

    var compiles = Compiles(program, encoding);
    (compiles ? validSeparatorChars : invalidSeparatorChars).Add(c);

    if (!invalidFileNameChars.Contains(c)) { validFileNameChars.Add(c); }

    if (compiles) { log(CsvLine(c)); }
}
var elapsed_s = stopwatch.ElapsedMilliseconds;

WriteFiles(validSeparatorChars, encoding, "ValidSeparatorChars");
WriteFiles(invalidSeparatorChars, encoding, "InvalidSeparatorChars");
WriteFiles(validFileNameChars, encoding, "ValidFileNameChars");

log($"Found {validSeparatorChars.Count} valid and {invalidSeparatorChars.Count} invalid " +
    $"separator chars and {validFileNameChars.Count} valid file name chars " +
    $"among {validSeparatorChars.Count + invalidSeparatorChars.Count} " +
    $"in {elapsed_s} ms");


static string Program(char c) => $"var {Identifier(c)} = 42;";
static string Identifier(char c) => $"_{c}_";

static bool Compiles(string source, Encoding encoding)
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

static void WriteFiles(IReadOnlyList<char> chars, Encoding encoding, string fileName)
{
    const string baseDir = "../../../../";
    File.WriteAllText(baseDir + $"{fileName}.csv", ToCsv(chars), encoding);
    File.WriteAllText(baseDir + $"{fileName}.md", ToTable(chars), encoding);
}

static string ToCsv(IReadOnlyList<char> chars) => string.Join(Environment.NewLine,
    new[] { CsvHeader() }.Concat(chars.Select(c => CsvLine(c))));
static string CsvHeader() => "Decimal,Hex,Char,Identifier";
static string CsvLine(char c) => $"{(int)c:D5},0x{(int)c:X4},{c},{Identifier(c)}";

static string ToTable(IReadOnlyList<char> chars) => string.Join(Environment.NewLine,
    new[] { TableHeader() }.Concat(chars.Select(c => TableLine(c))));
static string TableHeader() => $"|Decimal|Hex|Char|Identifier|{Environment.NewLine}|-:|-:|-:|-|";
static string TableLine(char c) => $"|{(int)c:D5}|`0x{(int)c:X4}`|`{c}`|`{Identifier(c)}`|";
