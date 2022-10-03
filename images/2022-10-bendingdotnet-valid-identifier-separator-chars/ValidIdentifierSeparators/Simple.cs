using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace ValidIdentifierSeparators;

class Simple
{
    public void AsPossible()
    {
        for (int i = char.MinValue; i <= char.MaxValue; i++)
        {
            var c = (char)i;
            var identifier = $"_{c}_";
            if (SyntaxFacts.IsValidIdentifier(identifier))
            {
                Trace.WriteLine(c);
            }
        }
    }
}
