---
layout: post
title: 10 Tiny Things in C# I Wish Were Different
---
As an exercise in futility in this blog post I look at a few things I wish were
different in C# (and .NET as I am including BCL type names or similar too). I
consider this an anti-post in the sense that I actually believe there is an
"unsound" obsession in programming circles with counting lines or characters as
exemplified in my own blog post [World's Smallest C# Program (featuring `N`)]({{
site.baseurl }}/2021/10/09/worlds-smallest-csharp-program). 

It's fun but has less relevance to writing good code than correctness,
readability, debuggability, observability and performance. However, when
possible one should choose the most succinct way to express code as long as the
code is equivalent with regards to the mentioned points of merit. To be
concrete this:
```csharp
var letters = new [] { 'a', 'b' };
foreach (var letter in letters)
{
    Console.WriteLine(letter);
}
```
is better than:
```csharp
var letters = new [] { 'a', 'b' };
letters.ToList().ForEach(Console.WriteLine);
```
It makes me sad when I see the above `ToList()` given the allocations involved.
There is a [reason LINQ doesn't include a `ForEach` extension
method](https://ericlippert.com/2009/05/18/foreach-vs-foreach/). And both
snippets of code can be written in pretty much the same time in Visual Studio.
The above can be expressed using `Array.ForEach` now of course, and I'm fine
with that, but what happens if the type of `letters` changes to `List<char>`?  
```csharp
Array.ForEach(letters, Console.WriteLine);
```
In any case for any developers out there please:
 * Stop counting lines 🤞
 * Stop counting characters 🤞

Yet here I am nagging about minor issues in C# and .NET regarding things that
could be more succinct. The difference is these are things at the foundation of
the developer platform. Things we use every single day, and where I think there
could have been better defaults that would not impact readability in a negative
way. It is, however, pretty futile giving these are also things that probably
won't be changed or implemented. So please indulge me.

Below I show a before and after example demonstrating the 10 things I wish were
different (in some way not necessarily all uses). Just after I go through each
of them one by one. There could be more or less and they are not particularly
well prioritized. Just off the top of my mind.

BEFORE

AFTER


 1. `using` should be `use`. Just like git commit messages should be in the
    [imperative present
    tense](https://git.kernel.org/pub/scm/git/git.git/tree/Documentation/SubmittingPatches?h=v2.36.1#n181)
    like `"Add X"`, `"Remove Y"`, `"Fix Z"` C# should be the same. I don't know
    the reason for why C# selected `using` vs `use` but both read fine when read
    out. **"Using namespace System in this file"** vs **"Use namespace System in
    this file"** but arguably `use` is more succinct:
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
   var nameToIndex = new Map<string, int>();
   ```
4. `KeyValuePair<,>` should be `KeyValue<,>`. `Pair` is simply redundant. It's a
   key and value.
   ```csharp
   KeyValue<string, int> nameIndex = new("E", 4);
   ``` 
5. Add `let` as a compliment to `var` but where the declared variable cannot be
   mutated/re-assigned. This is not the same as `const` as the declared variable
   doesn't have to be a constant.
   ```csharp
   let text = "abc";
   text = "def"; // ERROR: Cannot re-assign 'text'
   use let reader = new StringReader("a;b;c;d");
   ``` 
6. `use` without `var` or `let` should be short-hand for `use let`:
   ```csharp
   use reader = new StringReader("a;b;c;d");
   ``` 
7. `readonly` should not exist instead by default all declarations are by
   default readonly and mutable ones should be defined with `mut`.
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
   quite un-C#'esque, but there are plenty of abbreviations already like `Tcp`
   and `Http`, however, `ReadOnly` is quite a bit more pervasive in modern C#.
   The naming standard says 2 letter abbreviations should be all caps while 3 or
   more only have the first letter capitalized. I'd be fine with relaxing this
   to `Ro` in this case though. 😎
   ```csharp
   IROList<char> letters = new char[] { 'a', 'b' };
   ```
8. `private` should be implicit only (it's almost always redundant and while you
   can remove it with `dotnet format` why not just say it simply can't be used):
   ```csharp
   class C
   {
       int _member = 42;
       private int _nope = 17; // ERROR: 'private' is not valid
       int Double(int i) => i * 2;
       private int Triple(int i) => i * 3; // ERROR: 'private' is not valid
   }
   ``` 
9. dictionary array initializor is an abomination - 1. harder to read (subjective), harder to refactor/change 3 vs 2 changes, its a lie!! 
11. delegates don't implement interface e. g. IFunc
12. `fixed` should be `fix`
13. `Void` as a type so no more Func vs Action. 
14. Allow alias for open generic types
15. Some form of automatic RAII

Why not just use F#? I like paranthesis and brace and dislike allocations. 🤷‍
