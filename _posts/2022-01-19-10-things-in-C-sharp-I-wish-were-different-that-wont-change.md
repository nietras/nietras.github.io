---
layout: post
title: 10 Things in C# I Wish Were Different, That Won't Change
---

both C# and .NET naming

1. `using` should be `use`. Just like git commit messages should be in the [imperative present tense](https://git.kernel.org/pub/scm/git/git.git/tree/Documentation/SubmittingPatches?h=v2.36.1#n181)
  like `Add X`, `Remove Y`, `Fix Z` C# should be the same. I don't know 
  the reason for why C# selected `using` vs `use` but both read fine when read out.
  * "Using namespace `System` in this file"
  * "Use namespace `System` in this file"
  but arguably `use` is more succinct:
  ```csharp
  use System;
  use System.IO;
  use System.Text;
  use static Console;
  use var reader = new StringReader("a;b;c;d"); 
  ```
2. `Dictionary<,>` should be named `Map<,>`. I don't think there have 
  been many days where I have been programming in C# where I didn't use `Dictionary` so if `Map`
  is good enough a term for C++ I'd prefer this more succinct term:
  ```csharp
  var nameToIndex = new Map<string, int>();
  ```
3. `KeyValuePair<,>` should be named `KeyValue<,>`.
4. Add `let` as a compliment to `var` but where the declared variable cannot be mutated/re-assigned. This
   is not the same as `const`.
5. `use` without `var` or `let` should be short-hand for `use let` 
6. `private` should be implicit only 
7. `readonly` should not exist, instead `mutable`/`mut`, `let`/`var`, 
8. `ReadOnly` should not exist in interface names, instead should be `Mut`/`Mutable`, that is default for interfaces should be readonly
members vs locals
9. dictionary array initializor is an abomination - 1. harder to read (subjective), harder to refactor/change 3 vs 2 changes, its a lie!! 
10. `ctor` instead of type name, annoying to have to constantly change that
11. delegates don't implement interface e. g. IFunc
12. `fixed` should be `fix`
13. `Void` as a type so no more Func vs Action. 
14. Allow alias for open generic types
15. Some form of automatic RAII

Why not just use F#? I like paranthesis and brace and dislike allocations. 🤷‍
