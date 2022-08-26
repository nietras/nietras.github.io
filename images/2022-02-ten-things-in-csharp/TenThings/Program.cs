using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static Logger;

using var reader = new StringReader("a;b;c;d");
var line = reader.ReadLine()!;
var names = line.Split(';');
var nameToIndex = MakeNameToIndex(names);

Log(nameToIndex);

static IReadOnlyDictionary<string, int> MakeNameToIndex(ReadOnlySpan<string> names)
{
    var nameToIndex = new Dictionary<string, int>(names.Length);
    for (var i = 0; i < names.Length; i++)
    {
        nameToIndex.Add(names[i], i);
    }
    return nameToIndex;
}

static class Logger
{
    static readonly Action<string> _log = 
        static (string t) => { Console.WriteLine(t); Trace.WriteLine(t); };
    static int _logCount = 0;

    public static void Log(IReadOnlyDictionary<string, int> nameToIndex)
    {
        foreach (var pair in nameToIndex)
        {
            Log(pair);
        }
    }

    private static void Log(KeyValuePair<string, int> pair)
    {
        _log($"{_logCount:D3}: '{pair.Key}' = {pair.Value}");
        ++_logCount;
    }
}
