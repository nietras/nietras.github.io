---
layout: post
title: Bending .NET - Native Libraries - CNTK (.targets)
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

## .targets Approach TLDR
 * The `.targets` approach is the "legacy" approach to packaging .NET libraries
   with native library dependencies. It predates .NET Core and any of the
   options available today.
 * Native libraries are packaged in a sub-directory not recognized by [NuGet
   Folder Conventions](#nuget-folder-conventions) e.g. `support/x64/Dependency`.
 * Native libraries are linked and referenced in a project via a `.target` file
   that gets inserted into the consuming project.
 * `.target` file only links native libraries if `Platform` is `x64`.
 * `dll`s are copied to build output as defined by the `.target` file.


TODO TLDR BULLET POINTS; PROS/CONS

## Packages
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
of that extension library would be "forced" to use the same. Therefore:

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

## Example Project
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
Once you then build this the output will change to the below. Note the extra
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
This contains the `nupkg` file itself from which the rest has been extracted,
the `nuspec` file, the `build` directory with `.targets` file, the `lib`
directory with the managed library and the `support` directory with both Debug
and Release versions of the native libraries. For a .NET library you would
rarely include the Debug versions, these are here mainly for C++, since the
package can be used for both.

The `cntk.gpu.nuspec` file is pretty straightforward (again omitting some
details for brevity) and references four other packages. The reason probably
being the 250MB package size limit on nuget.org. The main package the includes
the `Cntk.*.dll` only while the rest `dll`s as seen in the build output above. 
```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">
  <metadata>
    <id>CNTK.GPU</id>
    <version>2.7.0</version>
    <!--omitted-->
    <dependencies>
      <dependency id="CNTK.Deps.Cuda" version="[2.7.0]" />
      <dependency id="CNTK.Deps.cuDNN" version="[2.7.0]" />
      <dependency id="CNTK.Deps.MKL" version="[2.7.0]" />
      <dependency id="CNTK.Deps.OpenCV.Zip" version="[2.7.0]" />
    </dependencies>
  </metadata>
</package>
```

The `dll`s are packages inside the `support` directory which means these are not
automatically picked up by `dotnet`/MSBuild as part of the build process. ,
[NuGet Folder Conventions](#nuget-folder-conventions) table (at end of blog
post) does not specify `support` as a known directory, so basically these files
are just extra payload. The conventions do, however, state that `build` is a
known and that `.target` and `.props` files will be automatically inserted into
the project (that references the package directly). 

Note that the `build` directory contains a `netstandard2.0` directory, so there
is a convention of using Target Framework Monikers (TFM) for the `build`
directory too.

So let's look at the `CNTK.GPU.targets` file. Note that the targets file should
be named the same as the package.
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <CNTKDllDir>$(MSBuildThisFileDirectory)\..\..\support\x64\</CNTKDllDir>
    <CNTKComponentVersion>2.7</CNTKComponentVersion>
  </PropertyGroup>
  <ItemGroup Condition="'$(Platform.ToLower())' == 'x64'">
    <None Include="$(CNTKDllDir)Release\Cntk.Core.CSBinding-$(CNTKComponentVersion).dll">
      <Link>Cntk.Core.CSBinding-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.Core-$(CNTKComponentVersion).dll">
      <Link>Cntk.Core-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.Math-$(CNTKComponentVersion).dll">
      <Link>Cntk.Math-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.PerformanceProfiler-$(CNTKComponentVersion).dll">
      <Link>Cntk.PerformanceProfiler-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.Deserializers.Binary-$(CNTKComponentVersion).dll">
      <Link>Cntk.Deserializers.Binary-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.Deserializers.TextFormat-$(CNTKComponentVersion).dll">
      <Link>Cntk.Deserializers.TextFormat-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.Composite-$(CNTKComponentVersion).dll">
      <Link>Cntk.Composite-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.Deserializers.HTK-$(CNTKComponentVersion).dll">
      <Link>Cntk.Deserializers.HTK-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$(CNTKDllDir)Release\Cntk.Deserializers.Image-$(CNTKComponentVersion).dll">
      <Link>Cntk.Deserializers.Image-$(CNTKComponentVersion).dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
</Project>
```
This links each native `dll` and sets them to be copied to output directory´,
but only on the condition that platform is `x64`, which explains why the `dll`s
only got copied after this was defined. I don't know why the dlls are listed
individually or why version is explicitly defined, as I would think a simple
`$(CNTKDllDir)Release\*.dll` include would suffice. 

The condition for the platform being `x64` I am not sure why was thought
necessary given there are no `x86` packages or similar, but could perhaps be to
avoid those `dll`s being copied if the project is a class library with platform
`AnyCPU`. However, if you look at the `Cntk.Core.Managed-2.7.dll` (which
together with `Cntk.Core.CSBinding-2.7.dll` is generated via
[SWIG](https://www.swig.org/)) with [corflags](https://learn.microsoft.com/en-us/dotnet/framework/tools/corflags-exe-corflags-conversion-tool)
it will show:
```
corflags .\lib\netstandard2.0\Cntk.Core.Managed-2.7.dll

Microsoft (R) .NET Framework CorFlags Conversion Tool.  Version  4.8.3928.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Version   : v4.0.30319
CLR Header: 2.5
PE        : PE32+
CorFlags  : 0x9
ILONLY    : 1
32BITREQ  : 0
32BITPREF : 0
Signed    : 1
```

Which based on the stackoverflow answer [Interpreting the CorFlags
flags](https://stackoverflow.com/questions/18608785/interpreting-the-corflags-flags/23614024#23614024)
as shown below, means the managed is `x64 (64-bit)` and ILONLY so not
mixed-mode.
```
CPU Architecture           PE      32BITREQ   32BITPREF
------------------------   -----   --------   ---------
x86 (32-bit)               PE32           1           0
x64 (64-bit)               PE32+          0           0
Any CPU                    PE32           0           0
Any CPU 32-Bit Preferred   PE32           0           1
```

This means you get a warning when referencing the `Cntk.Core.Managed-2.7.dll` in
a class library with platform `AnyCPU`.
```
Warning	MSB3270	There was a mismatch between the processor architecture of the
project being built "MSIL" and the processor architecture of the reference
".\cntk.gpu\2.7.0\lib\netstandard2.0\Cntk.Core.Managed-2.7.dll",
"AMD64". This mismatch may cause runtime failures. Please consider changing the
targeted processor architecture of your project through the Configuration
Manager so as to align the processor architectures between your project and
references, or take a dependency on references with a processor architecture
that matches the targeted processor architecture of your project.
```
Which is a challenge and there is as far as I can tell no reason
`Cntk.Core.Managed-2.7.dll` could not just have been `Any CPU` since it only
contains managed code and `DllImport` P/Invoke definitions. Just because the
native libraries are `x64` does not mean the managed library necessarily should
be too, as this in general could cause issues if you wanted to use and reference
multiple different libraries with different platforms and targets in a central
class library that could be used in different contexts.

Keeping a solution build configuration with just `Any CPU` platform and not a
mix or similar is just a lot simpler.

Another issue with the way the package is defined is that the `dll`s, due to how
they are linked, will be shown in the Visual Studio Solution Explorer, which is
visual eye sore, although it does also make things explicit. 😅

![Cntk Dlls In Solution Explorer]({{ site.baseurl
}}/images/2023-09-bendingdotnet-native-libraries/cntk-dlls-in-solution-explorer.png)

The other packages are defined similarly, but then do not have other
dependencies. For example `CNTK.Deps.Cuda` has the following files (via `tree /F`):
```
│   cntk.deps.cuda.2.7.0.nupkg
│   cntk.deps.cuda.nuspec
│
├───build
│   └───netstandard2.0
│           CNTK.Deps.Cuda.targets
│
└───support
    └───x64
        └───Dependency
                cublas64_100.dll
                cudart64_100.dll
                curand64_100.dll
                cusparse64_100.dll
                nvml.dll
```

## Modified .target Approach
At work we have used a similar `.targets` based approach as the above for years
(remember this is before .NET Core, before any of the possible approaches you
can use today). However, we have modified it to our needs by avoiding having the
`Platform` must be `x64` condition and are instead copying `dll`s to `x64` or
`x86` sub-directories. Additionally, when possible and the assembly is ILONLY we
have packaged that separately. Since, we are mostly on Windows we have then used
[SetDllDirectory](https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setdlldirectoryw)
to specify the sub-directory at runtime (either `x64` or `x86`) so the `dll`s
are found by Windows via that, as this modifies the search order as discussed in
the link. The issue with that is that `SetDllDirectory` is global and only
allows one directory to be defined. It's basically a single global variable. So
one has to be very careful with this approach.

## NuGet Folder Conventions
Copied from [NuGet folder
conventions](https://learn.microsoft.com/en-us/nuget/create-packages/creating-a-package#from-a-convention-based-working-directory).

| Folder | Description | Action upon package install |
| --- | --- | --- |
| (root) | Location for readme.txt | Visual Studio displays a readme.txt file in the package root when the package is installed. |
| lib/{tfm} | Assembly (`.dll`), documentation (`.xml`), and symbol (`.pdb`) files for the given Target Framework Moniker (TFM) | Assemblies are added as references for compile as well as runtime; `.xml` and `.pdb` copied into project folders. See [Supporting multiple target frameworks](supporting-multiple-target-frameworks.md) for creating framework target-specific sub-folders. |
| ref/{tfm} | Assembly (`.dll`), and symbol (`.pdb`) files for the given Target Framework Moniker (TFM) | Assemblies are added as references only for compile time; So nothing will be copied into project bin folder. |
| runtimes | Architecture-specific assembly (`.dll`), symbol (`.pdb`), and native resource (`.pri`) files | Assemblies are added as references only for runtime; other files are copied into project folders. There should always be a corresponding (TFM) `AnyCPU` specific assembly under `/ref/{tfm}` folder to provide corresponding compile time assembly. See [Supporting multiple target frameworks](supporting-multiple-target-frameworks.md). |
| content | Arbitrary files | Contents are copied to the project root. Think of the **content** folder as the root of the target application that ultimately consumes the package. To have the package add an image in the application's */images* folder, place it in the package's *content/images* folder. |
| build | *(3.x+)* MSBuild `.targets` and `.props` files | Automatically inserted into the project. |
| buildMultiTargeting | *(4.0+)* MSBuild `.targets` and `.props` files for cross-framework targeting | Automatically inserted into the project. |
| buildTransitive | *(5.0+)* MSBuild `.targets` and `.props` files that flow transitively to any consuming project. See the [feature](https://github.com/NuGet/Home/wiki/Allow-package--authors-to-define-build-assets-transitive-behavior) page. | Automatically inserted into the project. |
| tools | Powershell scripts and programs accessible from the Package Manager Console | The `tools` folder is added to the `PATH` environment variable for the Package Manager Console only (Specifically, *not* to the `PATH` as set for MSBuild when building the project). |