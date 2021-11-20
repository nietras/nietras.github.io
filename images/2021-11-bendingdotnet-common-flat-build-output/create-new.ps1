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
$projects | % { Invoke-Expression -Command "dotnet sln add ""$_""" }