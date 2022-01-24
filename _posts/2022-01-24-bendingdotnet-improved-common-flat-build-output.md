---
layout: post
title: Bending .NET - Improved Common Flat Build Output
---
or [how a human rebel helped contain renegade program](https://matrix.fandom.com/wiki/Agent_Smith).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I improve upon [Bending .NET -
Corrected Common Flat Build Output]({{ site.baseurl
}}/2022/01/15/bendingdotnet-corrected-common-flat-build-output/), which had the
issue of having to include a common `props`-file at the end of each
`csproj`-file. This is no longer necessary and hence the coast is clear.

![clear coast]({{ site.baseurl }}/images/2022-01-bendingdotnet-improved-common-flat-build-output/clear-coast.jpg)
Source: [pixabay](https://pixabay.com/photos/background-baltic-sea-beach-1702938//)

Third time is the charm 😅. A kind reader of the [previous blog post]({{
site.baseurl }}/2022/01/15/bendingdotnet-corrected-common-flat-build-output/)
reached out to me on
[github](https://github.com/nietras/nietras.github.io/issues/21) with a simple
trick involving `BeforeTargetFrameworkInferenceTargets`, so this is a quick
update for that. Thanks, Stefan!

To recap, we had to add a new `props`-file `OutputBuildProject.props` and
then include this in each `csproj`-file as for example shown below.
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

This was unfortunate since it means we can't just copy/paste the new props into
a parent directory to get common flat build output, but have to "dirty up" the
project files. Fortunately, the SDK MSBuild files have various import points or
hooks that we can leverage. One of them is the
`BeforeTargetFrameworkInferenceTargets` property that let's us define a file to
be imported just before target framework inference occurs in
[Microsoft.NET.TargetFrameworkInference.targets](https://github.com/dotnet/sdk/blob/14b117b7088653b694e16ac2071fcbf634a2a9ab/src/Tasks/Microsoft.NET.Build.Tasks/targets/Microsoft.NET.TargetFrameworkInference.targets#L47).
```xml
<!-- Hook for importing custom target framework parsing -->
<Import Project="$(BeforeTargetFrameworkInferenceTargets)" 
        Condition="$(BeforeTargetFrameworkInferenceTargets) != ''" />
```

The key here is this a normal import and hence we can define properties as we
would normally. It also occurs **after** the `TargetFramework` property is
defined.
```xml
Note that this file is only included when $(TargetFramework) 
is set and so we do not need to check that here.
```
And after the project file is imported. Import order is partially covered in
[Document the import order of the common msbuild extension
points](https://github.com/dotnet/msbuild/issues/2767#issuecomment-514342730).

Therefore, we can simply change `OutputBuildProps.props` to define this
property to point to the `OutputBuildProject.props` as shown below.
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

    <BeforeTargetFrameworkInferenceTargets>$(MSBuildThisFileDirectory)OutputBuildProject.props</BeforeTargetFrameworkInferenceTargets>
  </PropertyGroup>
</Project>
```

With this we can remove the imports in the `csproj`-files so these are nice and
clean again.
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net5.0;net6.0</TargetFrameworks>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

The build output is exactly the same as discussed in the previous blog post and
code navigation still works in Visual Studio. Great.

Back to siphoning energy off rebel humans (thanks for helping out!).

PS: Example source code can be found in the GitHub 
repo for this blog [nietras.github.io](https://github.com/nietras/nietras.github.io)
and as a zip-file [CommonFlatBuild.zip]({{ site.baseurl }}/images/2022-01-bendingdotnet-improved-common-flat-build-output/CommonFlatBuild.zip).