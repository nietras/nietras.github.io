---
layout: post
title: GZip EmbeddedResource in MSBuild with RoslynCodeTaskFactory
---
**TLDR**: See title and [github](https://github.com/nietras/GZipEmbeddedResource) / [git](https://github.com/nietras/GZipEmbeddedResource.git)

Versioned package and dependency management is key in reproducible software and
machine learning development. There are many ways to handle digital assets for 
this including very big and complicated cloud infrastructure. I prefer to keep 
things simple and as much as possible keep these assets as simple text files in
a git repository and distribute them via package management. That is, for
machine learning keep annotation data in simple `csv`-files.

For .NET a simple way to do this is to simply embed these assets into an assembly,
and distribute it via [NuGet](https://www.nuget.org) or similar package feeds like
[Azure Artefacts](https://azure.microsoft.com/en-us/services/devops/artifacts/).

The basic approach then is to have a single git repository for one or more
digital assets (e.g. text files with a image file name and a corresponding set
of labels or similar for machine learning), and then define a simple .NET class
library which embeds these assets into it and perhaps adds some extra available
properties on a type in the assembly for easy access to the assets and information
regarding these.

This approach has the following benefits:
 * Assets are kept safe and versioned in git.
 * Normal pull request procedures can be used to review and detail changes
   to the assets. E.g. ground truth data has been re-annotated to remove incorrectly
   annotated data.
 * Assets are readily available in other projects as versioned nuget packages.
   Facilitating, simple `git clone` and `dotnet run` for reproducibly
   building software and running machine learning software.

There is, however, one problem with this. If you like many machine learning 
practitioners are relying on hand-annotated data where data can be updated
many times. Constantly expanding ground truth data or re-annotated to make
the data as clean and as full as possible. Since, the data stored in text
format can get quite big (e.g. 1 GB) and you have a lots of versions the
nuget cache e.g. `C:\Users\<USERNAME>\.nuget\packages` can quickly 
consume 10s or 100s of GB. Not great if you are on a space limited laptop 
or similar. Saving disk space would be nice.

Fortunately, this can be done in a simple way by using MSBuild to
[GZip](https://en.wikipedia.org/wiki/Gzip) each individual 
[`EmbeddedResource`](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items?view=vs-2019#embeddedresource) file.


