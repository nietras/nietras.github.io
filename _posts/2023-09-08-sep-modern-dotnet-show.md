---
layout: post
title: Sep with me on The Modern .NET Show
---
On July 11th, 2023 I was a guest on [The Modern .NET
Show](https://dotnetcore.show/) and had a great time talking to Jamie Taylor
([twitter](https://twitter.com/podcasterJay),
[fediverse](https://mastodon.social/@gaprogman@hachyderm.io)) about
[Sep](https://github.com/nietras/Sep) (the world's fastest .NET CSV library) and
in particular performance and optimizations. The podcast has just been released
and you can listen to it and read the show notes I made below.

TODO INSERT LINK TO PODCAST

# Show Notes
Below are edited notes I made before the recording of the podcast.

## Questions and Topics
* Could you please introduce yourself to the listeners?
    * **CTO** at a small industrial AI/computer vision company. 
    * Programming professionally for about **20 years. About 15 years .NET/C#**. 
    * Started out as a kid on an **Atari console**, where if you wanted to play
      a game you had to type in a program from an accompanying book. 
    * **Masters degree in computer engineering** with a specialty in computer
      vision and graphics.
    * Very passionate about **performance and mechanical sympathy**.
* So what’s Sep? It’s a CSV parser right?
    * Sep is a **modern but minimal library** for reading and writing
      CSV/separated values.
    * Meant as a **replacement** for an **internal +10 year old library** I’ve
      also written that we use at work for our machine learning pipelines.
    * Based on an idea of using the new [Generic Math/static abstract
      interface](https://devblogs.microsoft.com/dotnet/preview-features-in-net-6-generic-math/)
      features in .NET/C#. In particular `ISpanParsable<T>` and
      `ISpanFormattable` for parsing or formatting numbers or similar to/from a
      span of characters.
    * It’s an **exploration of a different API for working with CSV files** that
      is targeted at the machine learning use cases we have at work. Not meant
      as a general CSV library like CsvHelper, hence it has a different perhaps
      minimal feature set compared to that.
    * Use cases at work are for CSV files we have full control over and are
      well-formed.
    * Since I did this in my spare time I wanted to have fun though and hence
      **got hooked on optimizing Sep to be as fast as possible**. Utilizing the
      latest and greatest features in .NET/C#.
* But not just a CSV parser, but the fastest, right? How did you measure that?
    * Right, so fastest is of course a relative measure. It requires context.
      Comparison.
    * I measured that (in part) using an existing benchmark (**NCsvPerf**) made
      by **Joel Verhagen**. Joel works on NuGet for Microsoft, so the benchmark
      relates to reading information on nuget package assets, their names, ids,
      versions and similar.
    * Great thing about this is it compares about **35 different libraries and
      approaches**.
    * So I basically took that benchmark and replicated it in the Sep git repo,
      but focusing on comparing it to just 3 other approaches. 
        * A simple read line, split approach.
        * [CsvHelper](https://joshclose.github.io/CsvHelper/) - since most
          widely used (158M downloads!) and no doubt the broadest feature set.
          By [Josh Close](https://twitter.com/JoshClose).
        * [Sylvan.Data.Csv](https://github.com/MarkPflug/Sylvan/blob/main/docs/Csv/Sylvan.Data.Csv.md)
          - or Sylvan - since shown to be the fastest in NCsvPerf. By [Mark
          Pflug](https://twitter.com/MarkPflug).
    * Based on that Sep 0.1.0 is (was) about **1.2x faster than Sylvan and 2.8x
      than CsvHelper**. For this specific benchmark. This is at the "top level",
      at the low level of just parsing CSV and enumerating columns, Sep is (was)
      about **16x faster than CsvHelper**.
    * Disclaimer: Every CSV library has its own feature set. Every use case is
      different. For example, Sep supports common line endings supported by
      .NET, that is `\r` (MacOS), `\r\n` (Windows), `\n` (Linux, new MacOS),
      while some only support `\r\n`, `\n`. On the other hand Sep doesn’t
      support automatic escaping/unescaping of quotes or similar since I/we have
      no need for that. Every library is different.
* Did you just async all the things?
    * There is **no `async`/`await`** in Sep currently, that is not currently
      supported. async/await is more about scaling related to IO operations and
      perhaps multi-threading. All the benchmarks and optimizations relate to
      single threaded performance or efficiency.
* `Span<T>`?
    * Span is an **integral** part of both the API and the performance, since
      this means one can **avoid repeated allocations**. The "package assets"
      benchmark contains a lot of repeated strings, so to optimize that **string
      pooling** is applied. I borrowed this approach from Sylvan, but then
      optimized. So in fact for this benchmark a large part of the edge of Sep
      is not related to the actual parsing of the separated values, but how
      those values are then turned into strings. (optimizing, simple
      `GetHashCode`)
* SIMD? Vectorization?
    * This is the fun part! Vectorization is at the very **core** of why Sep is
      blazing fast at the very lowest level of parsing separated values. That
      is, at **finding the special characters** like comma (`,`), carriage
      return (`\r`), line feed (`\n`) and quote (`"`).
    * Instead of finding these 1 character at a time, we can use SIMD (single
      instruction multiple data) to **look at multiple characters at a time**.
      Several existing libraries already do that like Sylvan.
    * **`char` in .NET is 16-bits**. Using a 128-bit register means we can look
      at 8 chars at a time, like with x86 SSE. Using a 256-bit register we can
      look at 16 chars at a time.
    * For Sep I realized that given that all special characters have values less
      than 255 (8-bit), we can pack the chars as bytes using
      `PackUnsignedSaturate` so we can look at twice as many chars at a time. 32
      at a time using AVX, for example.
    * So basically, Sep **loads two vectors of 16 chars**, then **packs them to
      a single register of 32 bytes**, with any char having a **value above 255
      "saturated" to 255**. Then compares this register to the 4 special
      characters also loaded into SIMD registers. This creates a **byte mask**
      of where the special chars are. This can be converted to a **bit mask**
      using a special instruction (e.g. `movemask` on x86), which we can then
      scan using another special instruction `lzcnt` (leading zero count).
      [Alexander Mutel (xoofx)](https://mastodon.social/@xoofx)  just published
      a [blog post discussing code
      similar](https://xoofx.com/blog/2023/07/09/10x-performance-with-simd-in-csharp-dotnet/)
      to this if people want to get a more in depth idea of how this works. 
    * For Sep I then implemented this not only for x86 but using **"generic"
      vectorization**, which is a great feature of .NET, so the vectorization
      also works on ARM for example. This meant coming up with another way than
      using PackUnsignedSaturate as there is not yet a NarrowSaturated.
    * Mark Pflug has now adopted the `PackUnsignedSaturate` and AVX approach for
      Sylvan to get a significant speedup which has meant Sylvan would be
      marginally faster than Sep at the very lowest level of parsing and
      enumerating rows. This, of course, has meant that I had to improve Sep, so
      I’ve rewritten some of the internals of Sep so it is now about **25% than
      before**! I hope to release that soon in a 0.2.0 release. Sep now **hits
      about 10 GB/s** for parsing only on the package assets benchmark. Hence,
      **both Sylvan and Sep are now faster! Win-win!**
* You used the preview bits of .NET 6 initially, right?
    * Yes, I started prototyping the API based on initial preview bits. This was
      before any notion of the library being called Sep or trying to be the
      fastest or anything. I wanted to try out the `ISpanParsable`,
      `ISpanFormattable` interfaces. During that I encountered a couple of bugs,
      so trying out previews can have its challenges.
* Is Sep <em>just</em> fast, or is it fully featured too?
    * Sep has a **<em>different</em> feature set than other CSV libraries** that
      I know of. It definitely lacks some features that some might expect from a
      CSV library, for example escaping/unescaping quotes. But on the other hand
      there are some unique features and optimizations in Sep like being able to
      **easily select and parse multiple columns at a time** without repeated
      allocations. **Caching named column access by order of access for quick
      look ups**. Those are features I try to feature in the "floats"-benchmark
      I designed myself, that better fits the use case Sep was built for.
      "Package assets" really isn’t what Sep was designed for. I just couldn’t
      stop optimizing for it! 😅
* Any recommended tools?
    * [Visual Studio
      Profiler](https://learn.microsoft.com/en-us/visualstudio/profiling/beginners-guide-to-performance-profiling?view=vs-2022)
      is a great starting point. It’s my go to tool for finding *where* time is
      spent (CPU usage). Or how many and where allocations occur (.NET Object
      Allocation Tracking).
    * [BenchmarkDotNet](https://benchmarkdotnet.org/) for benchmarking.
    * [Disasmo](https://github.com/EgorBo/Disasmo) for inspecting assembly to
      look for potential code changes to improve performance at the very lowest
      level. Developed by Egor Bogatov who works on .NET JIT Compiler.
* What would you recommend developers do if they want to make their code faster?
  Measure, measure, measure or just start hacking away? 
    * Measure! Depending on context;
        * Measure
        * Isolate
        * Measure
        * Optimize 
        * 🔁
     * Don't count lines! Priorities are:
       * 📖 Readable
       * ✅ Correct
       * 🔍 Debuggable
       * 🚀 Performant
       * 🤏 As succinct as possible without changing above
     * Low line count is **not** the primary goal (unless you are [code
       golfing](https://en.wikipedia.org/wiki/Code_golf) of course).
     * In many cases writing decently performant code can be done from the start
       with very little extra effort (<5%). Unfortunately, many developers are
       too focused on counting lines, instead of writing "good code". For
       example, instead of writing a for/foreach loop using
       `values.ToList().ForEach(i => Do)` (BAD ❌). Line count is not a primary
       metric and writing for/foreach takes zero extra time.
     * That is probably easy for me to say, given that it’s very easy for me to
       spot these things when looking at code, which can both be a blessing and
       a curse, as I almost always see a whole set of "red" flags when looking
       at code. It’s like an essay you get back from a teacher at school with
       red notes all over 😅

## Socials, Websites and Services
* Sep on [nietras.com](https://nietras.com):
  * [Introducing Sep - Possibly the World's Fastest .NET CSV Parser]({{
    site.baseurl }}/2023/06/05/introducing-sep/)
  * [Sep 0.2.0 - Even Faster Parsing (~10 GB/s on Zen 3) and More]({{
    site.baseurl }}/2023/08/07/sep-0-2-0/)
  * [Sep 0.2.3 - .NET 8 and AVX-512 Preview]({{
    site.baseurl }}/2023/09/05/sep-0-2-3/)
* Other:
  * [Preview Features in .NET 6 – Generic
    Math](https://devblogs.microsoft.com/dotnet/preview-features-in-net-6-generic-math/)
    blog post by Tanner Gooding, August 10th, 2021.
  * [.NET 7 Preview 5 – Generic
    Math](https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/) - second
    blog by Tanner Gooding on the subject.
  * [.NET and C# Versions - 7/11
    Update](https://nietras.com/2022/11/26/dotnet-and-csharp-versions/) - my
    blog post with overview of .NET/C# versions.
  * [The fastest CSV parser in
    .NET](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers) -
    blog post with CSV benchmark by [Joel
    Verhagen](https://twitter.com/joelverhagen)
  * [Introduction to vectorization with Vector128 and
    Vector256](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/vectorization-guidelines.md)
    - guideline in .NET github repo by [Adam
    Sitnik](https://twitter.com/SitnikAdam) 
  * [10x Performance with SIMD Vectorized Code in
    C#/.NET](https://xoofx.com/blog/2023/07/09/10x-performance-with-simd-in-csharp-dotnet/)
    - blog post by [Alexander Mutel (xoofx)](https://mastodon.social/@xoofx)
  * Vectorization/SIMD/optimization
    * Tanner Gooding - Mastodon:
      [https://mastodon.social/@tannergooding@tech.lgbt](https://mastodon.social/@tannergooding@tech.lgbt)
      * Specific blog post: [Hardware Intrinsics in .NET
Core](https://devblogs.microsoft.com/dotnet/hardware-intrinsics-in-net-core/)
    * Wojciech Mula - Twitter:
      [https://twitter.com/pshufb](https://twitter.com/pshufb) Blog:
      [http://0x80.pl/](http://0x80.pl/) 
      * Specific blog post: [SIMDized check which bytes are in a
        set](http://0x80.pl/articles/simd-byte-lookup.html)
    * Daniel Lemire - Twitter:
      [https://twitter.com/lemire](https://twitter.com/lemire) Mastodon:
      [https://mastodon.social/@lemire](https://mastodon.social/@lemire) Blog:
      [https://lemire.me/](https://lemire.me/) 
    * Geoff Langdale - Twitter:
      [https://twitter.com/geofflangdale](https://twitter.com/geofflangdale)
      Blog: [https://branchfree.org/](https://branchfree.org/) 
    * Peter Cordes
      [https://stackoverflow.com/users/224132/peter-cordes](https://stackoverflow.com/users/224132/peter-cordes)
      * It’s nuts how many questions related to x86 SIMD that Peter Cordes has
      answered here. Incredible work.
  * Libraries and Tools
    * [BenchmarkDotNet](https://benchmarkdotnet.org/) - .NET library for
      thorough benchmarking.
    * [Disasmo](https://github.com/EgorBo/Disasmo) - Visual Studio extension for
      quick inspection of assembly code for C# (with .NET 7+ very easy to use no
      need to clone and build .NET from source) - by [Egor
      Bogatov](https://twitter.com/EgorBo) (works on .NET JIT Compiler).
    * Visual Studio Profiler - see [Measure application performance by analyzing
      CPU usage (C#, Visual Basic, C++,
      F#)](https://learn.microsoft.com/en-us/visualstudio/profiling/beginners-guide-to-performance-profiling?view=vs-2022)
      for getting started.