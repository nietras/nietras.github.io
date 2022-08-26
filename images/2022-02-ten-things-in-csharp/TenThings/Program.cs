using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var log = static (string t) =>
    { Console.WriteLine(t); Trace.WriteLine(t); };
using var reader = new StringReader("a;b;c;d");
var line = reader.ReadLine()!;
var split = line.Split(';');
var nameToIndex = Enumerable.Range(0, split.Length)
    .ToDictionary(i => split[i], i => i);

Logger.Log(nameToIndex, log);

static class Logger
{
    public static void Log(
        IReadOnlyDictionary<string, int> nameToIndex, 
        Action<string> log)
    {
        foreach (var pair in nameToIndex)
        {
            Log(pair, log);
        }
    }

    private static void Log(
        KeyValuePair<string, int> pair, 
        Action<string> log)
    {
        log($"'{pair.Key}' = {pair.Value}");
    }
}