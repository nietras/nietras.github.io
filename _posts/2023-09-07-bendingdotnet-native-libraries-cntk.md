---
layout: post
title: Bending .NET - Native Libraries - CNTK ()
---
In this blog post, part of the [Bending .NET - Native Libraries]({{ site.baseurl
}}/2023/09/06/bendingdotnet-native-libraries) series, I take a look at how CNTK
2.7.0 is packaged for .NET. This is before the CNTK releases I made and
discussed in [Reviving CNTK with CUDA 11.4 for Ampere support]({{ site.baseurl
}}/2021/08/05/reviving-cntk-with-cuda-11-4/) and [Refreshing CNTK with CUDA 11.8
for Ada Lovelace support]({{ site.baseurl
}}/2023/06/15/refreshing-cntk-with-cuda-11-8/). The blog post serves as an
example of how the "legacy" `.targets` based approach to packaging .NET
libraries with native library dependencies is structured and works.

TODO TLDR BULLET POINTS; PROS/CONS

CNTK consists of several packages as listed below. The first three packages are
the ones intended for developer usage, while the `CNTK.Deps.*` packages are for
extra native dependencies. 

* [CNTK.GPU](https://www.nuget.org/packages/CNTK.GPU/2.7.0)
* [CNTK.CPUOnly](https://www.nuget.org/packages/CNTK.CPUOnly/2.7.0)
* [CNTK.UWP.CPUOnly](https://www.nuget.org/packages/CNTK.UWP.CPUOnly/2.7.0)
* [CNTK.Deps.cuDNN](https://www.nuget.org/packages/CNTK.Deps.cuDNN/2.7.0)
* [CNTK.Deps.MKL](https://www.nuget.org/packages/CNTK.Deps.MKL/2.7.0)
* [CNTK.Deps.OpenCV.Zip](https://www.nuget.org/packages/CNTK.Deps.OpenCV.Zip/2.7.0)
* [CNTK.Deps.Cuda](https://www.nuget.org/packages/CNTK.Deps.Cuda/2.7.0)

An immediate observation one can make then is that this forces a developer to
have to choose between CPU or GPU support or even application type when
referencing a package. Even in a class library. This is bad, real bad. For
example, in a class library with extensions to the CNTK .NET API you would be
forced to choose between CPU or GPU support at the level of the class library,
which means users of that extension library would be forced to use the same.
Therefore:

> ⚠ The first rule of packaging .NET libraries with native library dependencies
is to have a separate and isolated package for the managed library alone, so
class libaries can reference that and only that.

So the first thing I did with my CNTK fork was to fix this by creating a new
[nietras.Cntk.Core.Managed](https://www.nuget.org/packages/nietras.Cntk.Core.Managed/2.7.1)
package, that only contains the managed library. But let's continue our look at
the CNTK packages.

Note that you can easily browse the contents of a nuget package by opening the
packages in the NuGet Package Explorer on [nuget.org](https://www.nuget.org/). I
will try to show the most important parts of the packages below.

[]({{ site.baseurl }}/)

## Project
To investigate the packages I have created a couple of simple .NET 7.0 projects.

`CntkConsole.csproj`
```xml
```
`CntkTests.csproj`
```xml
```





```
C:\Users\<USERNAME>\.nuget\packages\cntk.gpu\2.7.0\
```

`cntk.gpu.nuspec`
```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">
  <metadata>
    <id>CNTK.GPU</id>
    <version>2.7.0</version>
    <title>Microsoft CNTK, GPU Build</title>
    <authors>Microsoft</authors>
    <owners>Microsoft, CNTKTeam</owners>
    <requireLicenseAcceptance>true</requireLicenseAcceptance>
    <licenseUrl>https://www.cntk.ai/cntknugetmkllicense.html</licenseUrl>
    <projectUrl>http://www.cntk.ai/</projectUrl>
    <iconUrl>http://go.microsoft.com/fwlink/?LinkID=288890</iconUrl>
    <description>Microsoft Cognitive Toolkit Libraries, GPU Build</description>
    <summary>This NuGet package provides CNTK native and managed library running on CPU-Only devices. It requires Visual Studio 2017. The API documentation is available at https://docs.microsoft.com/cognitive-toolkit/CNTK-Library-Evaluation-on-Windows</summary>
    <releaseNotes>git commit:ff77549b186d717b53c63d3dc5cceb8cc47a6d32</releaseNotes>
    <copyright>© Microsoft Corporation. All rights reserved.</copyright>
    <tags>Microsoft Cognitive Toolkit CNTK deep learning CLR managed C# native C++</tags>
    <dependencies>
      <dependency id="CNTK.Deps.Cuda" version="[2.7.0]" />
      <dependency id="CNTK.Deps.cuDNN" version="[2.7.0]" />
      <dependency id="CNTK.Deps.MKL" version="[2.7.0]" />
      <dependency id="CNTK.Deps.OpenCV.Zip" version="[2.7.0]" />
    </dependencies>
  </metadata>
</package>
```