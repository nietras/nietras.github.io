---
layout: post
title: Bending .NET - Corrected Common Flat Build Output
---
or [how gathering and destroying all human rebels in Zion failed due to renegade
program](https://matrix.fandom.com/wiki/Battle_of_Zion).

**UPDATE 2022-01-24: The approach defined here has been improved in [Bending
.NET - Improved Common Flat Build Output]({{ site.baseurl
}}/2022/01/24/bendingdotnet-improved-common-flat-build-output/)**

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I try to correct the ship wreck that
was [Bending .NET - Common Flat Build Output]({{ site.baseurl
}}/2021/11/19/bendingdotnet-common-flat-build-output), which had a serious flaw
causing **Go To Definition (F12)** not to work, as now covered in that blog
post. 

![ship wreck]({{ site.baseurl }}/images/2022-01-bendingdotnet-corrected-common-flat-build-output/ship-wreck.jpg)
Source: [pixabay](https://pixabay.com/photos/ship-wreck-stranded-wreck-shipwreck-1882087/)

In the previous blog post I thought I had finally found a way to define common
flat build output without having to change anything in `csproj`-files.
Unfortunately that had issues and I have found no way to accomplish this without
changing `csproj`-files, so in this blog post I present the for now best
solution I could find; including a common `props`-file at the end of each
`csproj`-file. 

At the same time I try to simplify the properties changed to only focus on
flattening the final build and publish output. Prerequisites are the same as in
the previous blog post, so without further ado I will present the new approach.

A new file `OutputBuildProject.props` is added to the `src` level common files.
```
.\src\Directory.Build.props
.\src\Directory.Build.targets
.\src\OutputBuildProps.props
.\src\OutputBuildProject.props
.\src\OutputBuildTargets.props
```
Again, `OutputBuildProps.props` is imported by `Directory.Build.props`
and `OutputBuildTargets.props` is imported by `Directory.Build.targets`,
but `OutputBuildProject.props` then has to be imported explicitly in
each `csproj`-file. This is very annoying and cumbersome if you have a lot
of projects in a solution, but I've found no other way.

`Directory.Build.props` is shown below and is pretty straight-forward.
```xml
<Project>
  <PropertyGroup>
    <!-- Other common properties omitted for brevity -->
    <Deterministic>true</Deterministic>
    <LangVersion>10.0</LangVersion>
  </PropertyGroup>

  <Import Project="$(MSBuildThisFileDirectory)\OutputBuildProps.props" />
</Project>
```

`OutputBuildProps.props` is shown below and both defines custom properties for
easy reuse and overrides the most important top-level properties like
`BaseIntermediateOutputPath` and `BaseOutputPath`. Note that this includes a new
property `BasePublishDir`, since we now separate the published output from build
output in a new top-level directory `publish`.

```xml
<Project>
  <PropertyGroup Label="OutputBuildProps">
    <Platform Condition="$(Platform) == ''">AnyCPU</Platform>
    <Configuration Condition="$(Configuration) == ''">Debug</Configuration>
    
    <!-- Custom properties -->
    <BuildDir>$(MSBuildThisFileDirectory)..\build\</BuildDir>
    <ProjectBuildDirectoryName>$(MSBuildProjectName)_$(Platform)_$(Configuration)</ProjectBuildDirectoryName>
    <OutputPathWithoutEndSlash>$(BuildDir)$(ProjectBuildDirectoryName)</OutputPathWithoutEndSlash>
    <BaseOutDir>$(OutputPathWithoutEndSlash)</BaseOutDir>
    <BasePublishDir>$(MSBuildThisFileDirectory)..\publish\</BasePublishDir>

    <!-- MSBuild defined properties redefined -->
    <BaseIntermediateOutputPath>$(BuildDir)obj\$(MSBuildProjectName)\</BaseIntermediateOutputPath>
    <BaseOutputPath>$(BuildDir)bin\$(MSBuildProjectName)\</BaseOutputPath>
    <PackageOutputPath>$(BaseOutDir)</PackageOutputPath>
  </PropertyGroup>
</Project>
```

`Directory.Build.targets` basically just forwards to `OutputBuildTargets.props`,
the reason for this is to allow a specific git repository to still override
or define other properties as needed, but still be able to easily update the
common build output properties by pasting a file.
```xml
<Project>
  <Import Project="$(MSBuildThisFileDirectory)\OutputBuildTargets.props" />
</Project>
```

`OutputBuildTargets.props` is shown below. This no longer reassigns/overrides
any properties as that was the issue causing problems in Visual Studio. Again it
still includes a "hack" needed to cleanup WPF temporary output, as is discussed
in the [linked issue](https://github.com/dotnet/wpf/issues/2930). 
```xml
<Project>
  <!--
  WPF projects output temporary assemblies in directories that are not deleted after use.
  See https://github.com/dotnet/wpf/issues/2930
  -->
  <Target Name="RemoveWpfTemp" AfterTargets="Build">
    <ItemGroup>
      <WpfTempDirectories Include="$([System.IO.Directory]::GetDirectories(&quot;$(BuildDir)&quot;,&quot;$(MSBuildProjectName)*_wpftmp_*&quot;))"/>
    </ItemGroup>
    <RemoveDir Directories="@(WpfTempDirectories)" />
  </Target>  
</Project>
```

`OutputBuildProject.props` is the new file and this now sets the properties
defining the final build and publish output locations.
```xml
<Project>
  <PropertyGroup>
    <OutDir>$(BaseOutDir)_$(TargetFramework)\</OutDir>
    <TargetDir>$(OutDir)</TargetDir>
    <PublishDir>$(BasePublishDir)$(ProjectBuildDirectoryName)</PublishDir>
    <PublishDir Condition="$(TargetFramework) != ''">$(PublishDir)_$(TargetFramework)</PublishDir>
    <PublishDir Condition="$(RuntimeIdentifier) != ''">$(PublishDir)_$(RuntimeIdentifier)</PublishDir>
  </PropertyGroup>
</Project>
```

Each project then needs to import this file for example at the end of the file
as shown below.
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net5.0;net6.0</TargetFrameworks>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <Import Project="$(MSBuildThisFileDirectory)..\OutputBuildProject.props" />
</Project>
```

Let's build, publish and pack to be sure output is as expected.
```
dotnet build -c Debug
dotnet build -c Release
dotnet publish -c Debug .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish -c Release .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true --self-contained .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish -c Release -r win-x86 /p:PublishSingleFile=true --no-self-contained .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish -c Release -r linux-x64 /p:PublishSingleFile=true --self-contained .\src\CommonFlatBuild.AppConsole\CommonFlatBuild.AppConsole.csproj
dotnet pack -c Release
```
The end result in tree form (with details omitted) then is:
```
├───build
│   ├───bin
│   │   └───CommonFlatBuild.Test
│   │       ├───Debug
│   │       │   └───net6.0
│   │       └───Release
│   │           └───net6.0
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Debug_net6.0
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Release
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Release_net6.0
│   ├───CommonFlatBuild.AppWinForms_AnyCPU_Debug_net6.0-windows
│   ├───CommonFlatBuild.AppWinForms_AnyCPU_Release
│   ├───CommonFlatBuild.AppWinForms_AnyCPU_Release_net6.0-windows
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Debug_net6.0-windows
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Release
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Release_net6.0-windows
│   ├───CommonFlatBuild.Test_AnyCPU_Debug_net6.0
│   ├───CommonFlatBuild.Test_AnyCPU_Release_net6.0
│   ├───CommonFlatBuild_AnyCPU_Debug_net5.0
│   ├───CommonFlatBuild_AnyCPU_Debug_net6.0
│   ├───CommonFlatBuild_AnyCPU_Release
│   ├───CommonFlatBuild_AnyCPU_Release_net5.0
│   ├───CommonFlatBuild_AnyCPU_Release_net6.0
│   └───obj
│       ├───CommonFlatBuild
│       │   ├───Debug
│       │   │   ├───net5.0
│       │   │   └───net6.0
│       │   └───Release
│       │       ├───net5.0
│       │       └───net6.0
│       ├───CommonFlatBuild.AppConsole
│       │   ├───Debug
│       │   │   └───net6.0
│       │   └───Release
│       │       └───net6.0
│       ├───CommonFlatBuild.AppWinForms
│       │   ├───Debug
│       │   │   └───net6.0-windows
│       │   └───Release
│       │       └───net6.0-windows
│       ├───CommonFlatBuild.AppWpf
│       │   ├───Debug
│       │   │   └───net6.0-windows
│       │   └───Release
│       │       └───net6.0-windows
│       └───CommonFlatBuild.Test
│           ├───Debug
│           │   └───net6.0
│           └───Release
│               └───net6.0
├───publish
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Release_net6.0_linux-x64
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Debug_net6.0-windows
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Release_net6.0-windows
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Release_net6.0-windows_win-x64
│   └───CommonFlatBuild.AppWpf_AnyCPU_Release_net6.0-windows_win-x86
└───src
    ├───CommonFlatBuild
    ├───CommonFlatBuild.AppConsole
    ├───CommonFlatBuild.AppWinForms
    ├───CommonFlatBuild.AppWpf
    └───CommonFlatBuild.Test
```
Note that for intermediate build output the deep hierarchical output is not changed.
The focus with the new approach is primarily on the final output files.

With this build and publish can be easily found and deleted like below:
```
rmdir build;rmdir publish
```
No more rebel humans and no more agent issues... I hope 🤞

In the hope that we might in the future be able to define common flat build
output without changing `csproj`-files I have [filed an issue on Github for
MSBuild](https://github.com/dotnet/msbuild/issues/7300).

PS: Example source code can be found in the GitHub 
repo for this blog [nietras.github.io](https://github.com/nietras/nietras.github.io)
and as a zip-file [CommonFlatBuild.zip]({{ site.baseurl }}/images/2022-01-bendingdotnet-corrected-common-flat-build-output/CommonFlatBuild.zip).