#nullable enable
using System;
using System.IO;

Action<string?> log = Console.WriteLine;
var text = "1\n2\n3\n";

log("SYNC");

log("classic"); // one ReadLine call, out of loop variable, inside loop assignment
{
    using var reader = new StringReader(text);
    string? line;
    while ((line = reader.ReadLine()) != null)
    { log(line); }
}
log("is string"); // no out of loop variable, implicit null check, explicit type
{
    using var reader = new StringReader(text);
    while (reader.ReadLine() is string line)
    { log(line); }
}
log("is {}"); // no out of loop variable, implicit null check, implicit type
{
    using var reader = new StringReader(text);
    while (reader.ReadLine() is { } line)
    { log(line); }
}
log("");

log("ASYNC");

log("classic"); // one ReadLine call, out of loop variable, inside loop assignment
{
    using var reader = new StringReader(text);
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    { log(line); }
}
log("is string"); // no out of loop variable, implicit null check, explicit type
{
    using var reader = new StringReader(text);
    while (await reader.ReadLineAsync() is string line)
    { log(line); }
}
log("is {}"); // no out of loop variable, implicit null check, implicit type
{
    using var reader = new StringReader(text);
    while (await reader.ReadLineAsync() is { } line)
    { log(line); }
}
log("DONE");

// Infinite loop version since `is var` does not perform implicit null check
var infinite = false;
if (infinite)
{
    using var reader = new StringReader(text);
    while (reader.ReadLine() is var line)
    { log(line); }
}
