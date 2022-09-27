---
layout: post
title: Bending .NET - Compiling 65,536 Programs with Roslyn to Find Valid Identifier Separators (or just use `SyntaxFacts.IsValidIdentifier` 🤦‍)
---

https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names

https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name

https://stackoverflow.com/questions/1904252/is-there-a-method-in-c-sharp-to-check-if-a-string-is-a-valid-identifier

https://stackoverflow.com/questions/1829679/how-to-determine-if-a-string-is-a-valid-variable-name

```
var isValid = Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsValidIdentifier("I'mNotValid");
Console.WriteLine(isValid);     // False
```

![]({{ site.baseurl }}/images/2022-10-bendingdotnet-valid-identifier-separator-chars)
