using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DemonstrativeLetterSplitter;

Split("a;b;c;d\ne;f;g", char.ToUpper);

static class DemonstrativeLetterSplitter
{
    static readonly Action<string> Log;
    static DemonstrativeLetterSplitter() => Log = Console.WriteLine;

    public static void Split(string text, Func<char, char> change)
    {
        using var reader = new StringReader(text);
        int lineNumber = 1;
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            Log($"Line {lineNumber}:'{line}'");
            var letters = line.Split(';').Select(n => n[0]).ToArray();
            Apply(letters, change);
            var letterToIndex = MakeLetterToIndex(letters);
            foreach (var pair in letterToIndex)
            {
                Log($"{pair.Key} = {pair.Value}");
            }
            ++lineNumber;
        }
    }

    private unsafe static void Apply(Span<char> letters, Func<char, char> change)
    {
        fixed (char* letterPtr = letters)
        {
            for (var i = 0; i < letters.Length; i++)
            {
                ref var letter = ref letterPtr[i];
                letter = change(letter);
            }
        }
    }

    private static IReadOnlyDictionary<char, int> MakeLetterToIndex(
        ReadOnlySpan<char> letters)
    {
        var nameToIndex = new Dictionary<char, int>(letters.Length);
        for (var i = 0; i < letters.Length; i++)
        {
            nameToIndex.Add(letters[i], i);
        }
        return nameToIndex;
    }
}
