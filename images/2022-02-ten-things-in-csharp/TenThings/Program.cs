using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Logger;

using var reader = new StringReader("a;b;c;d\ne;f;g");
string line;
while ((line = reader.ReadLine()) != null)
{
    var letters = line.Split(';').Select(n => n[0]).ToArray();
    var letterToIndex = MakeLetterToIndex(letters);
    Log(letterToIndex);

    var upperCaseLetters = UnsafeUpperCase(letters);
    letterToIndex = MakeLetterToIndex(upperCaseLetters);
    Log(letterToIndex);
}

static IReadOnlyDictionary<char, int> MakeLetterToIndex(
    ReadOnlySpan<char> letters)
{
    var nameToIndex = new Dictionary<char, int>(letters.Length);
    for (var i = 0; i < letters.Length; i++)
    {
        nameToIndex.Add(letters[i], i);
    }
    return nameToIndex;
}

unsafe static ReadOnlySpan<char> UnsafeUpperCase(Span<char> letters)
{
    fixed (char* letterPtr = letters)
    {
        for (var i = 0; i < letters.Length; i++)
        {
            letterPtr[i] -= (char)('a' - 'A');
        }
    }
    return letters;
}

static class Logger
{
    static readonly Action<string> _log;
    private static int _logCount = 0;

    static Logger()
    {
        _log = static (string t) => 
        { 
            Console.WriteLine(t);
            Trace.WriteLine(t); 
        };
    }

    public static void Log(string message)
    {
        _log($"{_logCount:D3}: {message}");
        ++_logCount;
    }

    public static void Log(IReadOnlyDictionary<char, int> nameToIndex)
    {
        foreach (var pair in nameToIndex)
        {
            Log(pair);
        }
    }

    private static void Log(KeyValuePair<char, int> pair)
    {
        Log($"{pair.Key} = {pair.Value}");
    }
}
