using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var encoding = Encoding.UTF8;
Console.OutputEncoding = encoding;
Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

// https://www.unicode.org/Public/15.0.0/ucd/UnicodeData.txt
// The set of Unicode character categories containing non-rendering,
// unknown, or incomplete characters.
// !! Unicode.Format and Unicode.PrivateUse can NOT be included in
// !! this set, because they may be (private-use) or do (format)
// !! contain at least *some* rendering characters.
var nonRenderingCategories = new UnicodeCategory[]
{
    UnicodeCategory.Control,
    UnicodeCategory.OtherNotAssigned,
    UnicodeCategory.Surrogate
};

var validChars = new List<char>();
var invalidChars = new List<char>();

using var dllStream = new MemoryStream();
var max = 256; //char.MaxValue;
for (int i = char.MinValue; i <= max; ++i)
{
    var c = (char)i;

    // Char.IsWhiteSpace() includes the ASCII whitespace characters that
    // are categorized as control characters. Any other character is
    // printable, unless it falls into the non-rendering categories.
    var maybeUseable = !char.IsWhiteSpace(c)
        && !nonRenderingCategories.Contains(char.GetUnicodeCategory(c));
    if (maybeUseable)
    {
        var source = GetSourceLine(c);

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
            log(Format(c));
        }
        else
        {
            invalidChars.Add(c);
            //log($"INVALID: {Format(c)}");
        }
    }
}

var validText = string.Join(Environment.NewLine, validChars.Select(c => Format(c)));
var invalidText = string.Join(Environment.NewLine, invalidChars.Select(c => Format(c)));
File.WriteAllText("ValidChars.txt", validText, encoding);
File.WriteAllText("InvalidChars.txt", invalidText, encoding);
log(validText);

static string GetSourceLine(char c) => $"var _{c}_ = 42;";

static string Format(char c) => $"{(int)c:D5},{(int)c:X4},{c},{GetSourceLine(c)}";
