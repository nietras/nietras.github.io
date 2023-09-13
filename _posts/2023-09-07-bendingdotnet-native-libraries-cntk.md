---
layout: post
title: Bending .NET - Native Libraries - CNTK ()
---
In this blog post, part of the [Bending .NET - Native Libraries]({{ site.baseurl
}}/2023/09/06/bendingdotnet-native-libraries) series, I take a look at how [CNTK
2.7.0](https://learn.microsoft.com/en-us/cognitive-toolkit/nuget-package) is
packaged for .NET. This is before the CNTK releases I made and discussed in
[Reviving CNTK with CUDA 11.4 for Ampere support]({{ site.baseurl
}}/2021/08/05/reviving-cntk-with-cuda-11-4/) and [Refreshing CNTK with CUDA 11.8
for Ada Lovelace support]({{ site.baseurl
}}/2023/06/15/refreshing-cntk-with-cuda-11-8/). The blog post serves as an
example of how the "legacy" `.targets` based approach to packaging .NET
libraries with native library dependencies is structured and works.

TODO TLDR BULLET POINTS; PROS/CONS

From [Cognitive Toolkit / NuGet Package](https://learn.microsoft.com/en-us/cognitive-toolkit/nuget-package)
the NuGet packages are described as:

---

The CNTK NuGet package is a NuGet package containing the necessary libraries and assemblies to enable .NET and Windows C++ applications to perform CNTK model evaluation. There are 3 NuGet packages:

* **CNTK.CPUOnly**: provides [CNTK C#/.NET Managed Library](./CNTK-Library-Managed-API.md) and [C++ Library](./CNTK-Library-Native-Eval-Interface.md) for CPU only machines.
* **CNTK.GPU**: provides [CNTK C#/.NET Managed Library](./CNTK-Library-Managed-API.md) and [C++ Library](./CNTK-Library-Native-Eval-Interface.md) for GPU enabled machines.
* **CNTK.UWP.CPUOnly**: provides [CNTK C++ UWP Eval Library](./CNTK-Library-Native-Eval-Interface.md) for applications using Universal Windows Platform (UWP) on CPU only machines.

The package may be obtained through the NuGet Package Manager inside Visual Studio by searching for "CNTK", or downloaded directly from nuget.org:

* [https://www.nuget.org/packages/CNTK.CPUOnly](https://www.nuget.org/packages/CNTK.CPUOnly)
* [https://www.nuget.org/packages/CNTK.GPU](https://www.nuget.org/packages/CNTK.GPU)
* [https://www.nuget.org/packages/CNTK.UWP.CPUOnly](https://www.nuget.org/packages/CNTK.UWP.CPUOnly)

The current version is `2.7.0`.

The CNTK NuGet packages may be installed on a Visual C++, .NET(C#, VB.Net, F#, ...), or UWP projects. The NuGet package contains both debug and release versions of C++ library and DLLs, and the release version of C# assembly and its dependent DLLs. Once installed the project will contain a reference to the managed DLL and the required dependent binary libraries will be copied to the output directory after building the project.

For instructions on how to install a NuGet package, please refer to the NuGet documentation at:
[https://docs.nuget.org/consume/installing-nuget](https://docs.nuget.org/consume/installing-nuget)

---

In reality CNTK consists of several packages as listed below. The first three
packages are the ones intended for developer usage, while the `CNTK.Deps.*` are
extra packages for additional native dependencies. Mainly there due to size
limitations on NuGet.

* [CNTK.GPU](https://www.nuget.org/packages/CNTK.GPU/2.7.0)
* [CNTK.CPUOnly](https://www.nuget.org/packages/CNTK.CPUOnly/2.7.0)
* [CNTK.UWP.CPUOnly](https://www.nuget.org/packages/CNTK.UWP.CPUOnly/2.7.0)
* [CNTK.Deps.cuDNN](https://www.nuget.org/packages/CNTK.Deps.cuDNN/2.7.0)
* [CNTK.Deps.MKL](https://www.nuget.org/packages/CNTK.Deps.MKL/2.7.0)
* [CNTK.Deps.OpenCV.Zip](https://www.nuget.org/packages/CNTK.Deps.OpenCV.Zip/2.7.0)
* [CNTK.Deps.Cuda](https://www.nuget.org/packages/CNTK.Deps.Cuda/2.7.0)

An immediate observation one can make then is that this forces a developer to
have to choose between CPU or GPU support or even application type when
referencing a package. Even in a class library. This is bad. For example, in a
class library with extensions to the CNTK .NET API you would be forced to choose
between CPU or GPU support at the level of the class library, which means users
of that extension library would be forced to use the same. Therefore:

> 📜 The first rule of packaging .NET libraries with native library dependencies is
to have a separate and isolated package for the managed library alone, so class
libaries can reference that and only that.

So the first thing I did with my CNTK fork was to fix this by creating a new
[nietras.Cntk.Core.Managed](https://www.nuget.org/packages/nietras.Cntk.Core.Managed/2.7.1)
package that only contains the managed library. But let's continue our look at
the CNTK packages.

Note that you can easily browse the contents of a nuget package by opening the
packages in the NuGet Package Explorer on [nuget.org](https://www.nuget.org/). I
will try to show the most important parts of the packages below.

[]({{ site.baseurl }}/)

## Project
To investigate the packages I have created a simple .NET 7.0 project `CntkConsole.csproj`.
```xml
<Project  Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="CNTK.GPU" Version="2.7.0" />
  </ItemGroup>
</Project>
```
with a simple `Program.cs`.
```csharp
using System;
using CNTK;
var device = DeviceDescriptor.GPUDevice(0);
Console.WriteLine(device.Type);
```
Building with `dotnet build` works, but running with `dotnet run` will fail.
```
Unhandled exception. System.TypeInitializationException: 
 The type initializer for 'CNTK.CNTKLibPINVOKE' threw an exception.
 ---> System.TypeInitializationException: 
      The type initializer for 'SWIGExceptionHelper' threw an exception.
 ---> System.DllNotFoundException: 
      Unable to load DLL 'Cntk.Core.CSBinding-2.7.dll' or one of its dependencies: 
      The specified module could not be found. (0x8007007E)
```
Looking at the build output we can see that the `Cntk.Core.CSBinding-2.7.dll` is
missing. In fact it appears that only the `Cntk.Core.Managed-2.7.dll` is
present.
```
bin
│   └───Debug
│       └───net7.0
│               Cntk.Core.Managed-2.7.dll
│               CntkConsole.deps.json
│               CntkConsole.dll
│               CntkConsole.exe
│               CntkConsole.pdb
│               CntkConsole.runtimeconfig.json
```
This means the CNTK packages do not work out-of-the-box and as far as I can tell
the documentation does not mention this or why. The reason is that the CNTK
packages requires the `Platform` to be `x64`, which we will see in a moment when
looking at the package definitions and their `.targets` files.

For some reason, however, setting `<Platform>x64</Platform>` in the project file
does not work. I don't know why nor have I spent much time investigating it. What
works is settings `<Platforms>x64</Platforms>` (that is plural) like below.
```xml
<Project  Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <Platforms>x64</Platforms>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="CNTK.GPU" Version="2.7.0" />
  </ItemGroup>
</Project>
```
Note that this requires changes to any solution files that reference the
project, and is one of the reasons why requiring projects to specify the
platforms can be a challenge and why I prefer to avoid it.

When building from the command line you then have to specify the platform
explicitly. In Visual Studio if you have updated the solution you have to choose
e.g. `x64` as platform in the tool bar.
```
dotnet build /p:Platform=x64
```
Ones you then build this the output will change to the below. Note the extra
`x64` level.
```
───bin
│   └───x64
│       └───Debug
│           └───net7.0
│                   Cntk.Composite-2.7.dll
│                   Cntk.Core-2.7.dll
│                   Cntk.Core.CSBinding-2.7.dll
│                   Cntk.Core.Managed-2.7.dll
│                   Cntk.Deserializers.Binary-2.7.dll
│                   Cntk.Deserializers.HTK-2.7.dll
│                   Cntk.Deserializers.Image-2.7.dll
│                   Cntk.Deserializers.TextFormat-2.7.dll
│                   Cntk.PerformanceProfiler-2.7.dll
│                   CntkConsole.deps.json
│                   CntkConsole.dll
│                   CntkConsole.exe
│                   CntkConsole.pdb
│                   CntkConsole.runtimeconfig.json
│                   cublas64_100.dll
│                   cudart64_100.dll
│                   cudnn64_7.dll
│                   curand64_100.dll
│                   cusparse64_100.dll
│                   libiomp5md.dll
│                   mkldnn.dll
│                   mklml.dll
│                   nvml.dll
│                   opencv_world310.dll
│                   zip.dll
│                   zlib.dll
```
Running this from Visual Studio succeeds and prints.
```
GPU
```
Running from the command line with `dotnet run /p:Platform=x64` in the directory
of the `csproj`, however, still fails with the same exception as above. Let's
change the `Program.cs` to:
```
using System;
using CNTK;
Console.WriteLine(Environment.CurrentDirectory);
var device = DeviceDescriptor.GPUDevice(0);
Console.WriteLine(device.Type);
```
and try running it again from the command line. This will then print:
```
<LOCALPATH>\CntkConsole
Unhandled exception. System.TypeInitializationException: 
...
```
When running from Visual Studio it will print:
```
<LOCALPATH>\CntkConsole\bin\x64\Debug\net7.0
GPU
```
So the problem is that the native libraries are not found when running from the
command line since current directory is not the same as the output directory.
`dll`s on Windows are found based on a specific
[search-order](https://learn.microsoft.com/en-us/windows/win32/dlls/dynamic-link-library-search-order)
which includes the current directory. I don't know of any way to specify to
`dotnet run` to use a different working directory. However, you can just run the
`CntkConsole.exe` which also works.
```
.\bin\x64\Debug\net7.0\CntkConsole.exe
<LOCALPATH>\CntkConsole
GPU
```

Whether to use `Platforms`, `PlatformTarget` or `RuntimeIdentifier` can be
challenging and is touched upon in the GitHub issue [Design/Doc issue:
PlatformTarget vs Platforms vs RuntimeIdentifier handled
inconsistently](https://github.com/dotnet/sdk/issues/1553), but with no clear
answer.

My take is to avoid having to specify either of these unless publishing (or
running) at which point use the `RuntimeIdentifier`. There are too many issues
with solutions and projects having different platforms. This means packages have
to be compatible with that. Let's dig into why the CNTK packages aren't and how
they are structured.

## Package Definitions
Looking in the [nuget
cache](https://learn.microsoft.com/en-us/nuget/consume-packages/managing-the-global-packages-and-cache-folders)
for the specific CNTK.GPU package version at:
```
C:\Users\<USERNAME>\.nuget\packages\cntk.gpu\2.7.0\
```
we can see the package directory contains the following files (some
non-essential or irrelevant C++ files have been omitted):
```
│   cntk.gpu.2.7.0.nupkg
│   cntk.gpu.nuspec
├───build
│   ├───netstandard2.0
│   │       CNTK.GPU.targets
├───lib
│   └───netstandard2.0
│           Cntk.Core.Managed-2.7.dll
│
└───support
    └───x64
        ├───Debug
        │       Cntk.Composite-2.7d.dll
        │       Cntk.Core-2.7d.dll
        │       Cntk.Core.CSBinding-2.7d.dll
        │       Cntk.Deserializers.Binary-2.7d.dll
        │       Cntk.Deserializers.HTK-2.7d.dll
        │       Cntk.Deserializers.Image-2.7d.dll
        │       Cntk.Deserializers.TextFormat-2.7d.dll
        │       Cntk.Math-2.7d.dll
        │       Cntk.PerformanceProfiler-2.7d.dll
        │
        └───Release
                Cntk.Composite-2.7.dll
                Cntk.Core-2.7.dll
                Cntk.Core.CSBinding-2.7.dll
                Cntk.Deserializers.Binary-2.7.dll
                Cntk.Deserializers.HTK-2.7.dll
                Cntk.Deserializers.Image-2.7.dll
                Cntk.Deserializers.TextFormat-2.7.dll
                Cntk.Math-2.7.dll
                Cntk.PerformanceProfiler-2.7.dll
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