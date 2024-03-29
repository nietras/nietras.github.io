﻿---
layout: post
title: 10 Tiny Things in C#/.NET I Wish Were Different
---
In this blog post I look at a few things I wish were different in C# and .NET. I
consider this an anti-post in the sense that I actually believe there is an
unsound obsession in programming circles with counting code lines or characters
as exemplified in my own blog post [World's Smallest C# Program (featuring
`N`)]({{ site.baseurl }}/2021/10/09/worlds-smallest-csharp-program).

It's fun but has less relevance to writing good code than *correctness,
readability, debuggability, observability and performance*. However, when
possible one should choose the most succinct way to express code as long as the
code is equivalent with regards to the mentioned points of merit. To be concrete
this:
```csharp
var letters = new [] { 'a', 'b' };
foreach (var letter in letters)
{
    Console.WriteLine(letter);
}
```
is in my book better than:
```csharp
var letters = new [] { 'a', 'b' };
letters.ToList().ForEach(Console.WriteLine);
```
It makes me sad when I see the above `ToList()` given the allocations involved.
There is a [reason LINQ doesn't include a `ForEach` extension
method](https://ericlippert.com/2009/05/18/foreach-vs-foreach/). And both
snippets of code can be written in pretty much the same time in Visual Studio.

In any case for any developers out there please (unless [code
golfing](https://en.wikipedia.org/wiki/Code_golf) 😉):
 * Stop counting lines *only* 🤞
 * Stop counting characters *only* 🤞

Yet here I am nagging about minor issues in C# and .NET (a developer platform I
♥) regarding things that could be more succinct. The difference is these are
things at the foundation of the developer platform. Things we use every single
day, and where I think there could have been better defaults that would not
impact readability. It is, however, pretty futile giving these are also things
that most likely won't be changed or implemented. So please indulge me.

Below I show a before and after example as a gif demonstrating the things I wish
were different (sorry for the lack of syntax highlighting in the after code).
Just after I go through each of the 10 things one by one. At the end the example
code is also listed as text both before and after.

![]({{ site.baseurl }}/images/2022-09-ten-tiny-things-in-csharp-dotnet/example-before-after.gif)

UPDATE: To clarify based on [responses to the post on
twitter](https://twitter.com/nietras1/status/1566762707281481730) this post is a
**thought experiment**. Not a list of proposals for changing C#/.NET. It's a
"what if?" these things were different. From the inception of the platform for
example. *Some* of the mentioned things could potentially be implemented but
others should clearly not since they would break backwards compatibility or
similar like removing `readonly` and making it default. It's a tremendous value
add of the .NET/C# developer platform that you can take [20 year
old](https://nietras.com/2022/02/13/dotnet-and-csharp-versions/) code and in
most cases it still compiles today on the most recent version of the platform
and compiler. Hence, it is wishful thinking and "pretty futile" 😅

 1. `using` should be `use`. Just like git commit messages should be in the
    [imperative present
    tense](https://git.kernel.org/pub/scm/git/git.git/tree/Documentation/SubmittingPatches?h=v2.36.1#n181)
    like `"Add X"`, `"Remove Y"`, `"Fix Z"` C# should be the same. I don't know
    the reason for why C# selected `using` vs `use` (besides [C++
    heritage](https://en.cppreference.com/w/cpp/language/namespace#Using-directives))
    but both read fine when read out. **"Using namespace System in this file"**
    vs **"Use namespace System in this file"** but arguably `use` is more
    succinct:
    ```csharp
    use System;
    use System.IO;
    use System.Text;
    use static Console;
    use var reader = new StringReader("a;b;c;d"); 
    ```
2. Use `this` instead of repeating type name for constructor/destructor etc. No
   need to constantly change these when copy pasting types or similar.
   ```csharp
   class VeryLongTypeNameThatsAnnoyingToRepeat
   {
       public this() : this(42) { }
       public this(int value) { }
       public ~this() {}
   }
   ``` 
3. `Dictionary<,>` should be `Map<,>`. I don't think there have been many days
   where I have been programming in C# where I didn't use `Dictionary` so if
   `Map` is good enough a term for C++ I'd prefer this more succinct term:
   ```csharp
   var letterToIndex = new Map<char, int>();
   ```
4. `KeyValuePair<,>` should be `KeyValue<,>`. `Pair` is simply redundant. It's a
   key and value.
   ```csharp
   KeyValue<char, int> letterIndex = new('E', 4);
   ``` 
5. Add `let` as a compliment to `var` but where the declared variable cannot be
   mutated/reassigned. This is not the same as `const` as the declared variable
   doesn't have to be a constant.
   ```csharp
   let text = "abc";
   text = "def"; // ERROR: Cannot re-assign 'text'
   use let reader = new StringReader("a;b;c;d");
   ``` 
6. `readonly` should not exist instead by default all declarations by default
   are readonly and mutable ones should be defined with `mut` (or `mutable`).
   Note that `var` variables by default are mutable.
   ```csharp
   struct RO            // readonly struct by default
   {
       int _count = 42; // readonly by default
   }
   mut struct MU
   {
       mut int _index = 0;
   }
   ```
7. `ReadOnly` should be abbreviated `RO` in type names e.g. `IReadOnlyList<>`,
   `ReadOnlySpan<>` should be `IROList<>`, `ROSpan<>`. At first this may feel
   quite un-C#-esque 😨, but there are plenty of abbreviations already like `Tcp`
   and `Http` in .NET. However, `ReadOnly` is quite a bit more pervasive in
   modern C#. The naming standard says 2 letter abbreviations should be all caps
   while 3 or more only have the first letter capitalized.
   ```csharp
   IROList<char> letters = new char[] { 'a', 'b' };
   ```
8. Recognize any `Invoke` method as "invokeable" similar to delegate `Invoke`
   methods can be called by simply writing `delegate(...)`. For example:
   ```csharp
   using System;
   Func<int, int> abs = Math.Abs;
   var u = abs(-42);
   ```
   can be seen in [sharplab.io](https://sharplab.io/#v2:EYLgtghglgdgNAFxFANgHwAICYCMBYAKAwFYAeWBOAAgoD4qJgBnKgXioFkIEALAOgCCzANyEAbhABOVAK5sGzABQBaACxYAlKIJA===)
   to generate the following IL:
   ```
   IL_0000: ldsfld class [System.Runtime]System.Func`2<int32, int32> Program/'<>O'::'<0>__Abs'
   IL_0005: dup
   IL_0006: brtrue.s IL_001b

   IL_0008: pop
   IL_0009: ldnull
   IL_000a: ldftn int32 [System.Runtime]System.Math::Abs(int32)
   IL_0010: newobj instance void class [System.Runtime]System.Func`2<int32, int32>::.ctor(object, native int)
   IL_0015: dup
   IL_0016: stsfld class [System.Runtime]System.Func`2<int32, int32> Program/'<>O'::'<0>__Abs'

   IL_001b: ldc.i4.s -42
   IL_001d: callvirt instance !1 class [System.Runtime]System.Func`2<int32, int32>::Invoke(!0)
   IL_0022: pop
   IL_0023: ret
   ```
   the `abs(-42)` is lowered to calling `Invoke(-42)` on the delegate.
   Instead, C# should recognize **any** method called `Invoke` as invokeable, so
   you can write for example:
   ```csharp
   var abs = new Abs();
   var value = abs(-42); // Calls Invoke
   struct Abs
   {
       int Invoke(int value) => Math.Abs(value);
   }
   ```
   I even proposed this as a new feature for C# more than 2 years ago in
   [Proposal: Add invokeable?(...) as short hand for invokeable?.Invoke(...) and
   add support for any "invoke-able"
   type](https://github.com/dotnet/csharplang/issues/3257) but it was closed as
   duplicate of [Proposal:
   Functors](https://github.com/dotnet/csharplang/discussions/95) and since
   `?()` as short-hand for null-coalescing operator being problematic for the C#
   parser. This feature seems feasible still 🤞 and it is a pattern we use a lot
   for value type functor based algoritms. An old example can be seen in
   [RyuJIT: Poor code quality for tight generic loop with many inlineable calls
   (factor x8 slower than non-generic few calls
   loop)](https://github.com/dotnet/runtime/issues/5252). This is simular to how
   C# recognizes types with a `GetEnumerator()` method without implementing
   `IEnumerable<>` as in
   [System.Private.CoreLib/src/System/Span.cs](https://github.com/dotnet/runtime/blob/2d1e29bdd3183b2e33026ad0606dded83583846e/src/libraries/System.Private.CoreLib/src/System/Span.cs#L221-L261).
9. `private` should be implicit only (it's almost always redundant and while you
   can remove it with `dotnet format` why not just say it simply can't be used -
   I'm disregarding `private protected` or similar here):
   ```csharp
   class C
   {
       int _member = 42;
       private int _nope = 17; // ERROR: 'private' is not valid
       int Double(int i) => i * 2;
       private int Triple(int i) => i * 3; // ERROR: 'private' is not valid
   }
   ``` 
10. `fixed` should be `fix`. Same as 1. Use imperative present tense.
    ```csharp
    var letters = new [] { 'a', 'b' };
    fix (char* ptr = letters)
    {
        for (var i = 0; i < letters.Length; ++i)
        {
            ptr[i] += (char)2;
        }
    }
    ```

### C# (original)
```csharp
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
```

### C# (nietras)
```csharp
use System;
use System.Collections.Generic;
use System.IO;
use System.Linq;
use static DemonstrativeLetterSplitter;

use let reader = new StringReader("a;b;c;d");
Split(reader, new ToUpper());

interface IFunc<T, TResult> { TResult Invoke(T arg1); }
record struct ToUpper : IFunc<char, char>
{
    public char Invoke(char c) => char.ToUpper(c);
}

static class DemonstrativeLetterSplitter
{
    static Action<string> Log;
    static this() => Log = Console.WriteLine;
    static mut int _count = 0;

    public static void Split<TFunc>(TextReader reader, TFunc change)
        where TFunc : IFunc<char, char>
    {
        let text = reader.ReadToEnd();
        let letters = text.Split(';').Select(n => n[0]).ToArray();
        Do(letters, change);
        let letterToIndex = MakeLetterToIndex(letters);
        foreach (let pair in letterToIndex)
        {
            Log($"{_count++:D3}: {pair.Key} = {pair.Value}");
        }
    }

    static unsafe void Do<TFunc>(Span<char> letters, TFunc change)
        where TFunc : IFunc<char, char>
    {
        fix (char* letterPtr = letters)
        {
            for (var i = 0; i < letters.Length; i++)
            {
                ref var letter = ref letterPtr[i];
                letter = change(letter);
            }
        }
    }

    static IROMap<char, int> MakeLetterToIndex(
        ROSpan<char> letters)
    {
        let letterToIndex = new Map<char, int>(letters.Length);
        for (var i = 0; i < letters.Length; i++)
        {
            letterToIndex.Add(letters[i], i);
        }
        return letterToIndex;
    }
}
```
### Output
```
000: A = 0
001: B = 1
002: C = 2
003: D = 3
```

PS: The example program is solely intended to exemplify the C#/.NET changes as
suggested here not good code.