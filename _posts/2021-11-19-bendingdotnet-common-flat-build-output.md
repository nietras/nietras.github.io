---
layout: post
title: Bending .NET - Common Flat Build Output
---
In this post, part of the [Bending .NET]({{ site.baseurl }}/2021/11/18/bendingdotnet-series) 
series, I will cover a quick and simple way to move and flatten all build output from
.NET - so it is not [Mariana Trench](https://en.wikipedia.org/wiki/Mariana_Trench) deep - 
to a common `build` directory .

![mariana trench]({{ site.baseurl }}/images/2021-11-bendingdotnet-common-flat-build-output/mariana-trench.jpg)
Source: [wikimedia](https://commons.wikimedia.org/wiki/File:Mariana-trench.jpg)


I have the recently released awesome .NET 6 installed and copy a `global.json` file
to a directory to ensure I am running fully specified.
```json
{
  "sdk": {
    "version": "6.0.100",
    "rollForward": "latestMajor",
    "allowPrerelease": false
  }
}
```
Which means `dotnet --info` returns something like:
```
.NET SDK (reflecting any global.json):
 Version:   6.0.100
 Commit:    9e8b04bbff

Runtime Environment:
 OS Name:     Windows
 OS Version:  10.0.19043
 OS Platform: Windows
 RID:         win10-x64
 Base Path:   C:\Program Files\dotnet\sdk\6.0.100\

Host (useful for support):
  Version: 6.0.0
  Commit:  4822e3c3aa
...
```
I then create a set of projects and a solution using a 
quick [PowerShell](https://github.com/PowerShell/PowerShell) script 
[`create-new.ps1`]({{ site.baseurl }}/images/2021-11-bendingdotnet-common-flat-build-output/create-new.ps1).
> PowerShell has been cross-platform since January 2018 and works almost anywhere.

```powershell
#!/usr/local/bin/powershell
mkdir src
pushd src
mkdir CommonFlatBuild
pushd CommonFlatBuild
dotnet new classlib
popd
mkdir CommonFlatBuild.AppConsole
pushd CommonFlatBuild.AppConsole
dotnet new console
popd
mkdir CommonFlatBuild.AppWpf
pushd CommonFlatBuild.AppWpf
dotnet new wpf
popd
mkdir CommonFlatBuild.AppWinForms
pushd CommonFlatBuild.AppWinForms
dotnet new winforms
popd
mkdir CommonFlatBuild.Test
pushd CommonFlatBuild.Test
dotnet new mstest
popd
popd
dotnet new sln -n CommonFlatBuild
$projects = gci -Recurse *.csproj
$projects | % { Invoke-Expression -Command "dotnet sln add --in-root ""$_""" }
```
The files created can be seen with:
```powershell
gci -Recurse *.* | Resolve-Path -Relative
```
ooops, I forget I already built the solution and now there are a ton of files
and a deep hierarchy mixed together with the code files.
```
.\src\CommonFlatBuild\bin\Debug\net6.0
.\src\CommonFlatBuild\bin\Debug\net6.0\ref\CommonFlatBuild.dll
.\src\CommonFlatBuild\bin\Debug\net6.0\CommonFlatBuild.deps.json
.\src\CommonFlatBuild\bin\Debug\net6.0\CommonFlatBuild.dll
.\src\CommonFlatBuild\bin\Debug\net6.0\CommonFlatBuild.pdb
.\src\CommonFlatBuild\bin\Release\net6.0
.\src\CommonFlatBuild\obj\Debug\net6.0
.\src\CommonFlatBuild\obj\Debug\net6.0\ref\CommonFlatBuild.dll
.\src\CommonFlatBuild\obj\Debug\net6.0\.NETCoreApp,Version=v6.0.AssemblyAttributes.cs
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.AssemblyInfo.cs
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.AssemblyInfoInputs.cache
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.assets.cache
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.csproj.AssemblyReference.cache
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.csproj.CoreCompileInputs.cache
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.csproj.FileListAbsolute.txt
...
(continues with lots of files)
...
.\CommonFlatBuild.sln
.\create-new.ps1
.\global.json
```
I find this really annoying and don't want to waste my time having to navigate
so deep - 5 directories `\src\CommonFlatBuild\bin\Debug\net6.0\` - either 
in console or file explorer to find the built output. Additionally, even if you run:
```
dotnet clean
```
the output is still full of files from the build.
```
.\src\CommonFlatBuild\bin\Debug\net6.0
.\src\CommonFlatBuild\bin\Release\net6.0
.\src\CommonFlatBuild\obj\Debug\net6.0
.\src\CommonFlatBuild\obj\Debug\net6.0\.NETCoreApp,Version=v6.0.AssemblyAttributes.cs
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.assets.cache
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.csproj.FileListAbsolute.txt
.\src\CommonFlatBuild\obj\Debug\net6.0\CommonFlatBuild.GlobalUsings.g.cs
.\src\CommonFlatBuild\obj\Release\net6.0
.\src\CommonFlatBuild\obj\Release\net6.0\.NETCoreApp,Version=v6.0.AssemblyAttributes.cs
.\src\CommonFlatBuild\obj\Release\net6.0\CommonFlatBuild.AssemblyInfo.cs
.\src\CommonFlatBuild\obj\Release\net6.0\CommonFlatBuild.AssemblyInfoInputs.cache
.\src\CommonFlatBuild\obj\Release\net6.0\CommonFlatBuild.assets.cache
.\src\CommonFlatBuild\obj\Release\net6.0\CommonFlatBuild.csproj.AssemblyReference.cache
.\src\CommonFlatBuild\obj\Release\net6.0\CommonFlatBuild.GeneratedMSBuildEditorConfig.editorconfig
.\src\CommonFlatBuild\obj\Release\net6.0\CommonFlatBuild.GlobalUsings.g.cs
.\src\CommonFlatBuild\obj\CommonFlatBuild.csproj.nuget.dgspec.json
...
(continues with lots of files)
```
Therefore, I also prefer being able to delete all this output by running
a simple `rmdir` command. While we now have [git clean](https://git-scm.com/docs/git-clean)
you still have to remember options 
([git clean -d -X -f](https://stackoverflow.com/questions/673407/how-do-i-clear-my-local-working-directory-in-git)) 
to run this and you risk deleting untracked code files, so I don't like this option either.
I'll use it now to show the files generated with the above script, though.
```
.\src\CommonFlatBuild\Class1.cs
.\src\CommonFlatBuild\CommonFlatBuild.csproj
.\src\CommonFlatBuild.AppConsole
.\src\CommonFlatBuild.AppConsole\CommonFlatBuild.AppConsole.csproj
.\src\CommonFlatBuild.AppConsole\Program.cs
.\src\CommonFlatBuild.AppWinForms
.\src\CommonFlatBuild.AppWinForms\CommonFlatBuild.AppWinForms.csproj
.\src\CommonFlatBuild.AppWinForms\CommonFlatBuild.AppWinForms.csproj.user
.\src\CommonFlatBuild.AppWinForms\Form1.cs
.\src\CommonFlatBuild.AppWinForms\Form1.Designer.cs
.\src\CommonFlatBuild.AppWinForms\Program.cs
.\src\CommonFlatBuild.AppWpf
.\src\CommonFlatBuild.AppWpf\App.xaml
.\src\CommonFlatBuild.AppWpf\App.xaml.cs
.\src\CommonFlatBuild.AppWpf\AssemblyInfo.cs
.\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
.\src\CommonFlatBuild.AppWpf\MainWindow.xaml
.\src\CommonFlatBuild.AppWpf\MainWindow.xaml.cs
.\src\CommonFlatBuild.Test
.\src\CommonFlatBuild.Test\CommonFlatBuild.Test.csproj
.\src\CommonFlatBuild.Test\UnitTest1.cs
.\CommonFlatBuild.sln
```
Luckily, there is a somewhat easy solution to this by using `Directory.Build.props`
and `Directory.Build.targets`, which we will place in the `src` directory,
so all sub-projects will pick these up. How to use these is covered in
[Customize your build](https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2022).
An important take-away from this is:

> Directory.Build.props is imported very early in Microsoft.Common.props, and properties defined later are unavailable to it. So, avoid referring to properties that are not yet defined (and will evaluate to empty).
>
> Properties that are set in Directory.Build.props can be overridden elsewhere in the project file or in imported files, so you should think of the settings in Directory.Build.props as specifying the defaults for your projects.
>
> Directory.Build.targets is imported from Microsoft.Common.targets after importing .targets files from NuGet packages. So, it can override properties and targets defined in most of the build logic, or set properties for all your projects regardless of what the individual projects set.

so:
* `Directory.Build.props` is imported **early**
* `Directory.Build.targets` is imported **later** - after package `.targets` files

Here, I have added two more files to be able to more easily reuse and update the properties 
needed to flatten and build to common directory giving us:
```
.\src\Directory.Build.props
.\src\Directory.Build.targets
.\src\OutputBuildProps.props
.\src\OutputBuildTargets.props
```
That is, `OutputBuildProps.props` is imported by `Directory.Build.props`
and `OutputBuildTargets.props` is imported by `Directory.Build.targets`
as detailed below.

Before diving in, the following resources were very helpful in figuring
out how to get the built output I wanted.

* [Common MSBuild project properties](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2022)
covers the most frequently used properties.
* [MSBuild targets](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-targets?view=vs-2022)
covers default build targets.
* The excellent [MSBuild Binary and Structured Log Viewer](https://msbuildlog.com/) tool 
  was extremely helpful in debugging build errors and finding targets and what properties
  to override. Thanks to [Kirill Osenkov](https://twitter.com/KirillOsenkov) for this and 
  a tip to use it. 👍

`Directory.Build.props` is shown below and is pretty straight-forward.
```xml
<Project>

  <PropertyGroup>
    <!-- Other common properties like -->
    <Deterministic>true</Deterministic>
    <LangVersion>10.0</LangVersion>
  </PropertyGroup>

  <Import Project="$(MSBuildThisFileDirectory)\OutputBuildProps.props" />

</Project>
```

`OutputBuildProps.props` is shown below and both defines custom properties
for easy reuse and overrides the most important top-level properties like
`BaseIntermediateOutputPath`, `IntermediateOutputPath` and
`OutputPath`. Note that the latter forces a trailing slash `\`, which is
why we need the `OutputPathWithoutEndSlash` property.

```xml
<Project>
  <PropertyGroup Label="OutputBuildProps">
    <Configuration Condition="$(Configuration) == ''">Debug</Configuration>
    <BuildDir>$(MSBuildThisFileDirectory)..\build\</BuildDir>
    <BaseIntermediateOutputPath>$(BuildDir)obj\$(MSBuildProjectName)_$(Configuration)\</BaseIntermediateOutputPath>
    <IntermediateOutputPath>$(BaseIntermediateOutputPath)</IntermediateOutputPath>
    <ProjectBuildDirectoryName>$(MSBuildProjectName)_$(Platform)_$(Configuration)</ProjectBuildDirectoryName>
    <OutputPathWithoutEndSlash>$(BuildDir)$(ProjectBuildDirectoryName)</OutputPathWithoutEndSlash>
    <OutputPath>$(OutputPathWithoutEndSlash)</OutputPath>
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

`OutputBuildTargets.props` is shown below, and it took a little while to figure
out that I needed to override the different `Target`* properties to get everything working
incl. packing nuget packages, since the evaluation of properties differs from target to target.
```xml
<Project>
  <PropertyGroup Label="OutputBuildTargets">
    <BaseOutDir>$(OutputPathWithoutEndSlash)</BaseOutDir>
    <OutDir>$(BaseOutDir)_$(TargetFramework)\</OutDir>
    <TargetDir>$(OutDir)</TargetDir>
    <TargetPath>$(TargetDir)$(TargetFileName)</TargetPath>
    <TargetRefPath>$(TargetDir)ref\$(TargetFileName)</TargetRefPath>
    <PublishDir>$(BaseOutDir)_$(TargetFramework)_$(RuntimeIdentifier)</PublishDir>
  </PropertyGroup>
</Project>

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
```
This also includes a "hack" needed to cleanup WPF temporary output, 
as is discussed in the linked issue. This is unfortunate, and if anyone
knows how this could be solved differently please let me know.

Let's build, publish and pack to be sure output is as expected.
```
dotnet build -c Debug
dotnet build -c Release
dotnet publish -c Release -r win-x64
dotnet pack -c Release
```
The end result in tree form (with details omitted) then is:
```
├───build
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Debug_net6.0
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Release
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Release_net6.0
│   ├───CommonFlatBuild.AppConsole_AnyCPU_Release_net6.0_win-x64
│   ├───CommonFlatBuild.AppWinForms_AnyCPU_Debug_net6.0-windows
│   ├───CommonFlatBuild.AppWinForms_AnyCPU_Release
│   ├───CommonFlatBuild.AppWinForms_AnyCPU_Release_net6.0-windows
│   ├───CommonFlatBuild.AppWinForms_AnyCPU_Release_net6.0-windows_win-x64
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Debug_net6.0-windows
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Release
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Release_net6.0-windows
│   ├───CommonFlatBuild.AppWpf_AnyCPU_Release_net6.0-windows_win-x64
│   ├───CommonFlatBuild.Test_AnyCPU_Debug_net6.0
│   ├───CommonFlatBuild.Test_AnyCPU_Release_net6.0
│   ├───CommonFlatBuild.Test_AnyCPU_Release_net6.0_win-x64
│   ├───CommonFlatBuild_AnyCPU_Debug_net6.0
│   ├───CommonFlatBuild_AnyCPU_Release
│   ├───CommonFlatBuild_AnyCPU_Release_net6.0
│   ├───CommonFlatBuild_AnyCPU_Release_net6.0_win-x64
│   └───obj
└───src
    ├───CommonFlatBuild
    ├───CommonFlatBuild.AppConsole
    ├───CommonFlatBuild.AppWinForms
    ├───CommonFlatBuild.AppWpf
    └───CommonFlatBuild.Test
```

Nice and flat. Now granted this can get a bit busy in big solutions with lots of
projects, so you may want to customize for that e.g. separate published output 
(which goes even deeper than I've shown above 😅) from
normal build output, but for small
and focused libraries this is exactly what I want.

Run the below and you can be very certain there is no build output lingering
causing build issues or similar.
```
rmdir build
```