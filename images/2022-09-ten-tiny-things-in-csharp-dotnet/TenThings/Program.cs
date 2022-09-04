using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DemonstrativeLetterSplitter;

using var reader = new StringReader("a;b;c;d");
Split(reader, new ToUpper());

interface IFunc<T, TResult> { TResult Invoke(T arg1); }
readonly record struct ToUpper : IFunc<char, char>
{
    public char Invoke(char c) => char.ToUpper(c);
}

static class DemonstrativeLetterSplitter
{
    static readonly Action<string> Log;
    static DemonstrativeLetterSplitter() => Log = Console.WriteLine;
    static int _count = 0;

    public static void Split<TFunc>(TextReader reader, TFunc change)
        where TFunc : IFunc<char, char>
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

    private static unsafe void Do<TFunc>(Span<char> letters, TFunc change)
        where TFunc : IFunc<char, char>
    {
        fixed (char* letterPtr = letters)
        {
            for (var i = 0; i < letters.Length; i++)
            {
                ref var letter = ref letterPtr[i];
                letter = change.Invoke(letter);
            }
        }
    }

    private static IReadOnlyDictionary<char, int> MakeLetterToIndex(
        ReadOnlySpan<char> letters)
    {
        var letterToIndex = new Dictionary<char, int>(letters.Length);
        for (var i = 0; i < letters.Length; i++)
        {
            letterToIndex.Add(letters[i], i);
        }
        return letterToIndex;
    }
}
