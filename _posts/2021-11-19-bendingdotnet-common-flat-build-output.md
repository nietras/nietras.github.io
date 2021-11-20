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
so all sub-projects will pick these up.

