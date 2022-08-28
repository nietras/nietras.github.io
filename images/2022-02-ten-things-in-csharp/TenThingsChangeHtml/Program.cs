var text = File.ReadAllText(@"../../../../TenThings/ProgramAfter.html");
var after = text
    .Replace("using", "use")
    .Replace("var", "let")
    .Replace("ReadOnly", "RO")
    .Replace("Dictionary", "Map")
    .Replace("KeyValuePair", "KeyValue")
    .Replace("private", "")
    .Replace("readonly", "")
    .Replace("let i", "var i")
    .Replace("static int", "static mut int");
File.WriteAllText(@"../../../../TenThings/ProgramAfterAuto.html", after);

var letters = new [] { 'a', 'b' };
foreach (var letter in letters)
{
    Console.WriteLine(letter);
}

letters.ToList().ForEach(Console.WriteLine);

Array.ForEach(letters, Console.WriteLine);

unsafe
{
    fixed (char* ptr = letters)
    {
        for (var i = 0; i < letters.Length; ++i)
        {
            ptr[i] += (char)2;
        }
    }
}