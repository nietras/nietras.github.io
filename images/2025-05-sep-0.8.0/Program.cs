using nietras.SeparatedValues;

const string ColNameA = "A";
const string ColNameB = "B";

ReadOnlySpan<int> values = [1, 2, 3];

// Sep
using var sepWriter = Sep.Default.Writer().ToText();
foreach (var v in values)
{
    using var row = sepWriter.NewRow();
    row[ColNameA].Format(v * 10);
    row[ColNameB].Format(v * 100);
}
Console.WriteLine(sepWriter.ToString());

// TextWriter directly
const char Separator = ';';
using var textWriter = new StringWriter();
// Header
textWriter.Write(ColNameA);
textWriter.Write(Separator);
textWriter.Write(ColNameB);
textWriter.WriteLine();
// Rows
foreach (var v in values)
{
    textWriter.Write(v * 10);
    textWriter.Write(Separator);
    textWriter.Write(v * 100);
    textWriter.WriteLine();
}
Console.WriteLine(textWriter.ToString());
