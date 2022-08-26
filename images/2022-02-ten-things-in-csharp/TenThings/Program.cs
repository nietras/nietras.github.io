using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Logger;

using var reader = new StringReader("a;b;c;d");
var line = reader.ReadLine()!;

var split = line.Split(';');
var nameToIndex = Enumerable.Range(0, split.Length)
    .ToDictionary(i => split[i], i => i);

Log(nameToIndex);

static class Logger
{
    static readonly Action<string> _log = 
        static (string t) =>
        { Console.WriteLine(t); Trace.WriteLine(t); };
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
