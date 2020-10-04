---
layout: post
title: GZip EmbeddedResource in MSBuild with RoslynCodeTaskFactory
---
**TLDR**: See title and [github](https://github.com/nietras/GZipEmbeddedResource) / [git](https://github.com/nietras/GZipEmbeddedResource.git)

Versioned package and dependency management is key in reproducible software and
machine learning development. There are many ways to handle digital assets for 
this including very big and complicated cloud infrastructure. I prefer to keep 
things simple and as much as possible keep these assets as simple text files in
a git repository and distribute them via package management. For example for
machine learning keep annotation data in simple `csv`-files.

For .NET a simple way to do this is to simply embed these assets into an assembly,
and distribute it via [NuGet](https://www.nuget.org) or similar package feeds like
[Azure Artefacts](https://azure.microsoft.com/en-us/services/devops/artifacts/).

## Approach
The basic approach is to have a single git repository for one or more
digital assets (e.g. text files with an image file name and a corresponding set
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
   Facilitating simple `git clone` and `dotnet run` usage. For example for
   reproducibly building software and running machine learning training.

## Problem
There is, however, one problem with this. If you like many machine learning 
practitioners are relying on "manually"-annotated data where data can be updated
many times (hundreds or thousands of times). 
Constantly expanding ground truth data or re-annotating to make
the data as clean and as full as possible. Since, the data stored in text
format can get quite large (e.g. 1 GB) and you have a lots of versions the
local nuget cache, e.g. `C:\Users\<USERNAME>\.nuget\packages`, can quickly 
consume 10s or 100s of GB. Not great if you are on a space limited laptop 
or similar. Saving disk space would be nice.
The nuget package itself is [just a zip-file](https://docs.microsoft.com/en-us/nuget/what-is-nuget) 
so the size of this is not the problem as such. 
The cached unpacked package is. An example from
a "model"/build server is shown below, where the cache is about ~75 GB.

![nuget cache size example]({{ site.baseurl }}/images/2020-10-GZipEmbeddedResource/gzip-embeddedresource-example-nuget-cache-size.png)


## Solution
Fortunately, this can be done in a simple way by using MSBuild to
[GZip](https://en.wikipedia.org/wiki/Gzip) each individual 
[`EmbeddedResource`](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items?view=vs-2019#embeddedresource) file.
Now MSBuild already has a [`ZipDirectory`](https://docs.microsoft.com/en-us/visualstudio/msbuild/zipdirectory-task?view=vs-2019) 
task, but as the apt name implies this will zip an entire directory 
into a single file. This is less than ideal since you won't be able to easily
inspect an assembly with [`ildasm`](https://docs.microsoft.com/en-us/dotnet/framework/tools/ildasm-exe-il-disassembler) 
for example to see which assets are  embedded. 
It also makes it a bit harder to get the individual files.

GZip'ing `EmbeddedResource` can be done quite easily as shown in the below
`csproj` file `GZipEmbeddedResource.csproj` from the github example repository 
[GZipEmbeddedResource](https://github.com/nietras/GZipEmbeddedResource).
This example is using the new condensed `csproj` format, but works in old style
`csproj` format too. The approach works in both .NET Framework and .NET Core.

The example uses a rather large example text file `enwik8` from 
[http://mattmahoney.net/dc/textdata.html](http://mattmahoney.net/dc/textdata.html)
consisting of ~100 MB XML text.

As can be seen, the files that you want to embed are defined as you would normally
via `EmbeddedResource`. Following that a new `GZip` inline task is defined using
the [`RoslynCodeTaskFactory`](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-roslyncodetaskfactory?view=vs-2019).
This defines two parameters `Files` and `Result`, 
which are both arrays of `ITaskItem`, which are in this context are simply 
files or rather file paths. `Files` is the set of files we want to zip,
and `Result` will contain the paths of these, 
where `.gz` has been suffixed to the path. Zipped files are written to disk,
but ignored by the `.gitignore` file, this allows inspecting the zipped files 
if needed.

This task is then used `BeforeBuild` to zip all `EmbeddedResource` files, and
replace this set of file paths with the zipped file paths, 
so only the zipped files are embedded. Every time you build the project 
and something has changed the files will be zipped. 
This can take a while for large files, but since this is a
project only for these assets it is rarely a problem.

```xml
<Project>
  <Import Project="Sdk.props" Sdk="Microsoft.NET.Sdk" />

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- Test file from: http://mattmahoney.net/dc/textdata.html -->
    <EmbeddedResource Include="enwik8" />
  </ItemGroup>

  <Import Project="Sdk.targets" Sdk="Microsoft.NET.Sdk" />

  <UsingTask TaskName="GZip" 
             TaskFactory="RoslynCodeTaskFactory" 
             AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll">
    <ParameterGroup>
      <Files ParameterType="Microsoft.Build.Framework.ITaskItem[]" 
             Required="true" />
      <Result ParameterType="Microsoft.Build.Framework.ITaskItem[]" 
              Output="true" />
    </ParameterGroup>
    <Task>
      <Using Namespace="System.IO" />
      <Using Namespace="System.IO.Compression" />
      <Code Type="Fragment" Language="cs">
  <![CDATA[
    if (Files.Length > 0)
    {
        Result = new TaskItem[Files.Length];
        for (int i = 0; i < Files.Length; i++)
        {
            ITaskItem item = Files[i];
            string sourcePath = item.GetMetadata("FullPath");
            string sourceItemSpec = item.ItemSpec;
                  
            string destinationSuffix = ".gz";
            string destinationPath = sourcePath + destinationSuffix;
            string destinationItemSpec = sourceItemSpec + destinationSuffix;

            Log.LogMessage(MessageImportance.Normal, 
                "EmbeddedResource Src : " + sourceItemSpec);
                  
            using (var sourceStream = File.OpenRead(sourcePath))
            using (var destinationStream = File.OpenWrite(destinationPath))
            using (var destinationGZip = new GZipStream(destinationStream, 
              CompressionLevel.Optimal))
            {
                sourceStream.CopyTo(destinationGZip);
            }
                  
            var destinationItem = new TaskItem(destinationItemSpec);
            
            Log.LogMessage(MessageImportance.Normal, 
                "EmbeddedResource GZip: " + destinationItem.ItemSpec);
                  
            Result[i] = destinationItem;
        }
    }
  ]]>
      </Code>
    </Task>
  </UsingTask>
  
  <Target Name="BeforeBuild">
    <GZip Files="@(EmbeddedResource)">
      <Output ItemName="GZipEmbeddedResource" TaskParameter="Result" />
    </GZip>
    <Message Text="Source EmbeddedResources: @(EmbeddedResource)" Importance="High" />
    <Message Text="GZipped EmbeddedResources: @(GZipEmbeddedResource)" Importance="High" />
    <ItemGroup>
      <EmbeddedResource Remove="@(EmbeddedResource)" />
      <EmbeddedResource Include="@(GZipEmbeddedResource)" />
    </ItemGroup>
  </Target>

</Project>
```

## Example Build
When building with e.g. `dotnet build` this will output the following, 
where the list of files that have been GZipped is logged.

```
Microsoft (R) Build Engine version 16.7.0+7fb82e5b2 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored C:\git\oss\GZipEmbeddedResource\src\GZipEmbeddedResource\GZipEmbeddedResource.csproj (in 199 ms).
  Restored C:\git\oss\GZipEmbeddedResource\src\GZipEmbeddedResource.Test\GZipEmbeddedResource.Test.csproj (in 237 ms).
  Source EmbeddedResources: enwik8
  GZipped EmbeddedResources: enwik8.gz
  GZipEmbeddedResource -> C:\git\oss\GZipEmbeddedResource\build\Libs_AnyCPU_Debug\GZipEmbeddedResource.dll
  GZipEmbeddedResource.Test -> C:\git\oss\GZipEmbeddedResource\build\Tests\GZipEmbeddedResource.Test_AnyCPU_Debug\GZipEmbeddedResource.Test.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Doing this for the `enwik8` cuts the file size from ~100 MB to ~37 MB and hence 
saves about 63%. However, for machine learning `csv`-files which contain mainly
ASCII text consisting of alpha-numeric characters it is very common to see 90-97%
savings.

```
   Length Name
   ------ ----
101128023 enwik8
 37040314 enwik8.gz
```

The `dll` in question will contain this file and hence have about the same size.
```
  Length Name
  ------ ----
37046272 GZipEmbeddedResource.dll
```

Viewing this in `ildasm` you can see easily see the embedded file and it's size.

![EmbeddedResource file in ildasm]({{ site.baseurl }}/images/2020-10-GZipEmbeddedResource/gzip-embeddedresource-ildasm.png)

One thing to keep in mind here is that the text files being embedded will 
have the line endings that existed at the time of embedding so this might
change based on git checkout policy and OS this is being build on.

That's it. While you could define this task as a nuget package, keeping it as simple
text in the project file allows easy customization like filtering or similar.
