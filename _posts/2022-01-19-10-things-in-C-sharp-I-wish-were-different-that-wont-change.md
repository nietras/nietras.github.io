---
layout: post
title: 10 Things in C# I Wish Were Different, That Won't Change
---
[The first version of .NET Framework was released on 13 February 2002, bringing managed code to Windows NT 4.0, 98, 2000, ME and XP.](https://en.wikipedia.org/wiki/.NET_Framework_version_history#:~:text=The%20first%20version%20of%20.,released%20nine%20more%20upgrades%20for%20.) 
Hence, it is soon the 20th anniversary of .NET. A lot has happens since, notably .NET Framework 2.0
introduced generics and later versions expanded further as shown below.

![.NET Framework Versions](https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/DotNet.svg/300px-DotNet.svg.png)

Source: [wikipedia](https://en.wikipedia.org/wiki/.NET_Framework)

Then .NET got bifurcarted in 2016 with the introduction of the cross-platform .NET Core.

![.NET Core Versions](https://executecommands.com/wp-content/uploads/2020/02/dotnet-core-version-history.jpg)
Source: [Microsoft .NET Core Versions History](https://executecommands.com/microsoft-net-core-versions/)


https://www.tutorialsteacher.com/csharp/csharp-version-history


* `using` should be `use`
* `Dictionary<,>` should be named `Map<,>`
* `KeyValuePair<,>` should be named `KeyValue<,>`
* `private` should be implicit only 
* `readonly` should not exist, instead `mutable`/`mut`, `let`/`var`, 
* `ReadOnly` should not exist in interface names, instead should be `Mut`/`Mutable`, that is default for interfaces should be readonly
members vs locals
* dictionary array initializor is an abomination - 1. harder to read (subjective), harder to refactor/change 3 vs 2 changes, its a lie!! 
* `ctor` instead of type name, annoying to have to constantly change that
* delegates don't implement interface e. g. IFunc
* `Void` as a type so no more Func vs Action. 

Why not just use F#? I like paranthesis and brace and dislike allocations. 🤷‍
