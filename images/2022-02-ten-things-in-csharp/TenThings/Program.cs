using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DemonstrativeLetterSplitter;

using var reader = new StringReader("a;b;c;d");
Split(reader, char.ToUpper);

static class DemonstrativeLetterSplitter
{
    static readonly Action<string> Log;
    static DemonstrativeLetterSplitter() => Log = Console.WriteLine;
    static int _count = 0;

    public static void Split(TextReader reader, Func<char, char> change)
    {
        var text = reader.ReadToEnd();
        var letters = text.Split(';').Select(n => n[0]).ToArray();
        Do(letters, change);
        var letterToIndex = MakeLetterToIndex(letters);
        foreach (var pair in letterToIndex)
        {
            Log($"{_count++:D3}: {pair.Key} = {pair.Value}");
        }
    }

    private unsafe static void Do(Span<char> letters, Func<char, char> change)
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
