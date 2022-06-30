---
layout: post
title: Bending .NET - Determine C# Compiler Version and Language Version with `#error version`
---
or [TODO](https://www.youtube.com/watch?v=pQ0db2ERil8).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I cover a single source line in an
 entire blog post. 🤷‍ A line that can be used for determining the effective C#
compiler version and language version used to compile a given `cs`-file.
Unfortunately though it appears to only be able to give us this information as
an error. 🤦‍

![TODO]({{ site.baseurl }}/images/2022-06-bendingdotnet-compiler-version/TODO.jpg)
Source: [TODO](https://www.flickr.com/photos/gomattolson/4321594214/)

I have created a quick `net6.0` console project with two files.

`CompilerVersionTest.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
</Project>
```
 `Program.cs`
```csharp
#error version
```
Compile this with `dotnet build` and it will output something like:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 10.0.
```
This is the only way I have found to determine the C# compiler version directly
in code. There does not appear to be a symbol defined. You cannot write
`#warning version` to avoid build to fail.

Add `LangVersion` like below:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
  </PropertyGroup>
</Project>
```
and this will change output:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 9.0.
```
Try `preview`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>preview</LangVersion>
  </PropertyGroup>
</Project>
```
And output will be:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: preview.
```
Remove `LangVersion` and we are back to `10.0`:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 10.0.
```

Now if I then place a `global.json` file next to the project file with:
```json
{
  "sdk": {
    "version": "6.0.301",
    "rollForward": "disable",
    "allowPrerelease": false
  }
}
```
Then `dotnet build` will output:
```
Program.cs(1,8): error CS8304: 
  Compiler version: '4.2.0-4.22220.5 (432d17a8)'.
  Language version: 10.0.
```
By default we get the latest version of whatever .NET SDK I have installed which
in my case is (as reported by `dotnet --version`):
```
7.0.100-preview.5.22307.18
```
Changing SDK version again to `6.0.106` and output is:
```
Program.cs(1,8): error CS8304:
  Compiler version: '4.0.1-1.22181.2 (487283bc)'.
  Language version: 10.0.
```

If I then open this project in Visual Studio 2022 (17.3 Preview 2.0) and build
it I will get:
```
Program.cs(1,8,1,15): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 10.0.
```
Visual Studio does it's own thing and does not use `global.json`. This is also
kind of documented in [global.json
overview](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json) that
says:

> The global.json file allows you to define which .NET SDK version is used when
> you run .NET CLI commands. Selecting the .NET SDK version is independent from
> specifying the runtime version a project targets. The .NET SDK version
> indicates which version of the .NET CLI is used.

The key being "**.NET CLI commands**". Hence, Visual Studio appears to use the
some other .NET SDK installed regardless of `global.json`.

Now when building in Visual Studio this will also output:
```
C:\Program Files\dotnet\sdk\7.0.100-preview.5.22307.18\
  Sdks\Microsoft.NET.Sdk\targets\
  Microsoft.NET.RuntimeIdentifierInference.targets(219,5):
  message NETSDK1057: You are using a preview version of .NET.
```
Let's try and uninstall this SDK version to see how this impacts Visual Studio
and hence the compiler version.

First I install the latest [.NET Uninstall
Tool](https://docs.microsoft.com/en-us/dotnet/core/additional-tools/uninstall-tool?tabs=windows)
(1.5.255402). Open a new PowerShell window or similar and run:
```powershell
dotnet-core-uninstall list
```
this will show:
```
This tool cannot uninstall versions of the runtime or SDK that are 
    - SDKs installed using Visual Studio 2019 Update 3 or later.
    - SDKs and runtimes installed via zip/scripts.
    - Runtimes installed with SDKs (these should be removed by removing that SDK).
The versions that can be uninstalled with this tool are:

.NET Core SDKs:
  7.0.100-preview.5.22307.18  x64    [Cannot uninstall version 7.0.0 and above]
  6.0.301                     x64    [Used by Visual Studio. Specify individually or use --force to remove]
  6.0.106                     x64
  5.0.408                     x64
  3.1.420                     x64
  2.1.818                     x86    [Used by Visual Studio 2019. Specify individually or use --force to remove]

.NET Core Runtimes:
  2.1.30  x64

ASP.NET Core Runtimes:
  6.0.6   x86
  5.0.8   x64
  5.0.16  x86
  3.1.26  x86
  2.1.30  x64
```
That seems a bit confusing...
```
The versions that can be uninstalled with this tool are:

.NET Core SDKs:
  7.0.100-preview.5.22307.18  x64    [Cannot uninstall version 7.0.0 and above]
```
so it lists the 7.0.100 version as a version that can be uninstalled but then
that it "Cannot uninstall version 7.0.0 and above"? 😅 Let's try a dry run just
to be sure:
```powershell
dotnet-core-uninstall dry-run --sdk 7.0.100-preview.5.22307.18
```
and:
```
Uninstallation not allowed. This tool cannot uninstall 
.NET Core SDKs with version 7.0.0 or above.
```
Alright, alright 😆

Let's try a downgrade if possible

https://dotnet.microsoft.com/en-us/download/dotnet/7.0

https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-7.0.100-preview.4-windows-x64-installer

![Uninstall .NET SDK using Apps & Features]({{ site.baseurl }}/images/2022-06-bendingdotnet-compiler-version/uninstall-dotnet-sdk-apps-and-features.png)

After this we can run `dotnet --list-sdks` and we no longer have the 7.0.100
preview 5 listed:
```
3.1.420 [C:\Program Files\dotnet\sdk]
5.0.100 [C:\Program Files\dotnet\sdk]
5.0.408 [C:\Program Files\dotnet\sdk]
5.0.409 [C:\Program Files\dotnet\sdk]
6.0.100-rc.2.21505.57 [C:\Program Files\dotnet\sdk]
6.0.106 [C:\Program Files\dotnet\sdk]
6.0.301 [C:\Program Files\dotnet\sdk]
6.0.400-preview.22301.10 [C:\Program Files\dotnet\sdk]
```
Let's rename `global.json` to `__global.json` to render it ineffective and run
`dotnet build` again:
```
Program.cs(4,8): error CS8304: 
  Compiler version: '4.3.0-3.22281.14 (b4fa0937)'. 
  Language version: 10.0.
```
as it now picks the `6.0.400-preview.22301.10` version.

But let's try from Visual Studio again:
```
Program.cs(4,8,4,15): error CS8304:
  Compiler version: '4.3.0-2.22307.7 (069a85a7)'.
  Language version: 10.0.
```
Hmm... 🤔 That's the same version as before when no `global.json` but now 7.0
preview 5 is gone and it's not the same version as 6.0.301 which was
`4.2.0-4.22220.5 (432d17a8)`. It's also missing the `C:\Program
Files\dotnet\sdk\7.0.100-preview.5.22307.18\...` path in the output. Yet, it's
the same version, so .NET SDK 7.0 preview 5 did not have compiler version
4.3.0-2.22307.7 and was only used for tooling/entry? It seems so.

So where does this `6.0.400-preview.22301.10` come from? It is not listed by
`dotnet-core-uninstall list` and since I haven't installed this myself, I assume
this is what is shipped/installed with Visual Studio 2022 17.3 Preview 2.0. In
fact that appears to be case since it matches Visual Studio About dialog:

![Visual Studio About with C# Tools Version 4.3.0-2.22307.7 (069a85a7)]({{ site.baseurl }}/images/2022-06-bendingdotnet-compiler-version/visual-studio-about.png)

Let's hit `CTRL + Q` in VS and search for `verbosity`.

![Feature Search Verbosity]({{ site.baseurl }}/images/2022-06-bendingdotnet-compiler-version/feature-search-verbosity.png)

And the change **MSBuild project build output verbosity** to **Detailed**.

![Verbosity Detailed]({{ site.baseurl }}/images/2022-06-bendingdotnet-compiler-version/msbuild-verbosity-detailed.png)

Then rebuild in Visual Studio and in the detailed output we will find (edited to reduce line lengths):
```
Using "Csc" task from assembly "C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\Roslyn\Microsoft.Build.Tasks.CodeAnalysis.dll".
Task "Csc"
  C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\Roslyn\csc.exe 
    /noconfig /unsafe- /checked- /nowarn:1701,1702,1701,1702,2008 /fullpaths 
    /nostdlib+ /errorreport:prompt /warn:6 
    /define:TRACE;DEBUG;NET;NET6_0;NETCOREAPP;NET5_0_OR_GREATER;NET6_0_OR_GREATER;NETCOREAPP1_0_OR_GREATER;NETCOREAPP1_1_OR_GREATER;NETCOREAPP2_0_OR_GREATER;NETCOREAPP2_1_OR_GREATER;NETCOREAPP2_2_OR_GREATER;NETCOREAPP3_0_OR_GREATER;NETCOREAPP3_1_OR_GREATER 
    /errorendlocation /preferreduilang:en-US /highentropyva+ 
    /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\Microsoft.CSharp.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\Microsoft.VisualBasic.Core.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\Microsoft.VisualBasic.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\Microsoft.Win32.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\Microsoft.Win32.Registry.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\mscorlib.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\netstandard.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.AppContext.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Buffers.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Collections.Concurrent.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Collections.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Collections.Immutable.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Collections.NonGeneric.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Collections.Specialized.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ComponentModel.Annotations.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ComponentModel.DataAnnotations.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ComponentModel.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ComponentModel.EventBasedAsync.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ComponentModel.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ComponentModel.TypeConverter.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Configuration.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Console.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Core.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Data.Common.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Data.DataSetExtensions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Data.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.Contracts.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.Debug.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.DiagnosticSource.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.FileVersionInfo.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.Process.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.StackTrace.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.TextWriterTraceListener.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.Tools.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.TraceSource.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Diagnostics.Tracing.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Drawing.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Drawing.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Dynamic.Runtime.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Formats.Asn1.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Globalization.Calendars.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Globalization.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Globalization.Extensions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.Compression.Brotli.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.Compression.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.Compression.FileSystem.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.Compression.ZipFile.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.FileSystem.AccessControl.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.FileSystem.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.FileSystem.DriveInfo.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.FileSystem.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.FileSystem.Watcher.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.IsolatedStorage.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.MemoryMappedFiles.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.Pipes.AccessControl.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.Pipes.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.IO.UnmanagedMemoryStream.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Linq.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Linq.Expressions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Linq.Parallel.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Linq.Queryable.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Memory.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Http.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Http.Json.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.HttpListener.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Mail.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.NameResolution.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.NetworkInformation.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Ping.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Requests.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Security.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.ServicePoint.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.Sockets.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.WebClient.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.WebHeaderCollection.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.WebProxy.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.WebSockets.Client.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Net.WebSockets.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Numerics.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Numerics.Vectors.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ObjectModel.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.DispatchProxy.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.Emit.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.Emit.ILGeneration.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.Emit.Lightweight.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.Extensions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.Metadata.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Reflection.TypeExtensions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Resources.Reader.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Resources.ResourceManager.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Resources.Writer.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.CompilerServices.Unsafe.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.CompilerServices.VisualC.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Extensions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Handles.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.InteropServices.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.InteropServices.RuntimeInformation.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Intrinsics.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Loader.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Numerics.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Serialization.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Serialization.Formatters.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Serialization.Json.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Serialization.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Runtime.Serialization.Xml.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.AccessControl.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Claims.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Cryptography.Algorithms.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Cryptography.Cng.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Cryptography.Csp.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Cryptography.Encoding.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Cryptography.OpenSsl.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Cryptography.Primitives.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Cryptography.X509Certificates.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Principal.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.Principal.Windows.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Security.SecureString.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ServiceModel.Web.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ServiceProcess.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Text.Encoding.CodePages.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Text.Encoding.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Text.Encoding.Extensions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Text.Encodings.Web.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Text.Json.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Text.RegularExpressions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Channels.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Overlapped.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Tasks.Dataflow.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Tasks.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Tasks.Extensions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Tasks.Parallel.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Thread.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.ThreadPool.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Threading.Timer.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Transactions.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Transactions.Local.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.ValueTuple.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Web.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Web.HttpUtility.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Windows.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.Linq.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.ReaderWriter.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.Serialization.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.XDocument.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.XmlDocument.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.XmlSerializer.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.XPath.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\System.Xml.XPath.XDocument.dll" /reference:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\ref\net6.0\WindowsBase.dll" /debug+ /debug:portable /filealign:512 /optimize- /out:obj\Debug\net6.0\CompilerVersionTest.dll /refout:obj\Debug\net6.0\refint\CompilerVersionTest.dll /target:exe /warnaserror- /utf8output /deterministic+ /langversion:10.0 /analyzerconfig:obj\Debug\net6.0\CompilerVersionTest.GeneratedMSBuildEditorConfig.editorconfig /analyzerconfig:"C:\Program Files\dotnet\sdk\6.0.400-preview.22301.10\Sdks\Microsoft.NET.Sdk\analyzers\build\config\analysislevel_6_default.editorconfig" /analyzer:"C:\Program Files\dotnet\sdk\6.0.400-preview.22301.10\Sdks\Microsoft.NET.Sdk\targets\..\analyzers\Microsoft.CodeAnalysis.CSharp.NetAnalyzers.dll" /analyzer:"C:\Program Files\dotnet\sdk\6.0.400-preview.22301.10\Sdks\Microsoft.NET.Sdk\targets\..\analyzers\Microsoft.CodeAnalysis.NetAnalyzers.dll" /analyzer:"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.5\analyzers/dotnet/cs/System.Text.Json.SourceGeneration.dll" 
    Program.cs "obj\Debug\net6.0\.NETCoreApp,Version=v6.0.AssemblyAttributes.cs" 
    obj\Debug\net6.0\CompilerVersionTest.AssemblyInfo.cs 
    /warnaserror+:NU1605
  Microsoft (R) Visual C# Compiler version 4.3.0-2.22307.7 (069a85a7)
  Copyright (C) Microsoft Corporation. All rights reserved.
  C:\Users\Niels\source\repos\CompilerVersionTest\CompilerVersionTest\
    Program.cs(4,8,4,15): error CS1029: #error: 'version'
  C:\Users\Niels\source\repos\CompilerVersionTest\CompilerVersionTest\
    Program.cs(4,8,4,15): error CS8304: Compiler version: '4.3.0-2.22307.7 (069a85a7)'. Language version: 10.0.
```
That means the C# Compiler executable `csc.exe` used is located at `C:\Program
Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\Roslyn\csc.exe`.
Note also how `/errorendlocation` is what means we get `(4,8,4,15)´ and not just
`(4,8)`.

So the C# compiler version used in Visual Studio is fully determined by Visual
Studio itself, which of course makes sense given the tight coupling between the
two with regards to IntelliSense and so forth. But at the same time Visual
Studio uses files from the latest SDK installed as we could see from the build
message:
```
C:\Program Files\dotnet\sdk\7.0.100-preview.5.22307.18\
  Sdks\Microsoft.NET.Sdk\targets\
  Microsoft.NET.RuntimeIdentifierInference.targets(219,5):
  message NETSDK1057: You are using a preview version of .NET.
```
so it's not clear cut.

Can we then change the compiler version used when building from Visual Studio?
And not the least is there some way to define a minimum compiler version for a
project in source code, so users will be notified that they should update Visual
Studio if the compiler version is too low?





# REVISIT
Why should I care? If you specify `LangVersion` to say `10.0` you get a
predefined C# version right? Yes. And everything works? Yes... but the C#
compiler is constantly changing, which means the generated IL might be changing
too.

Not just IL inside methods, but also compiler generated code for say `record`s
(see [C# 10 - `record struct` Deep Dive & Performance Implications]({{
 site.baseurl }}/2021/06/14/csharp-10-record-struct/) for example). In 99% of
 cases that's perhaps not important, but recently I stumbled upon such a case,
which I will not go into detail with here. Perhaps I'll get back to that in a
later blog post.

It then surprised me that I could not find a way to express or constrain this
"dependency" to be sure a certain specific (perhaps minimum) version of the C#
compiler was present/used to ensure code generated would contain what was
needed. And not just that if you want fully reproducible builds/runs (including
when building/debugging from Visual Studio) you very much want the output to be
100% identical which requires every tool and dependency to be "fixed".

Where I currently work we do machine learning with 100% reproducible training
(on same GPU) down to the bit of each floating point in the model and results.
Basically, you can `git clone` and `dotnet run` and training will be 100%
reproducible. This works since we version everything and have ground truth in
versioned nuget packages. And use deterministic randomization (even in the face
of multi-threading). With `global.json` you can then also be sure the .NET SDK
is as expected.

Shouldn't this be possible inside Visual Studio too? Decoupling the compiler
version from the Visual Studio version. Of course this is large order to ask for
since Visual Studio features like IntelliSense have a tight relationship to
Roslyn, so at the very least I would think it possible to express in some file
(`csproj`/`Directory.Build.props`) or whatever that you must have Visual Studio
2022 17.2 or later installed or you cannot build the project. Maybe there is a
way?


#### Links
* [How to use Microsoft.CodeAnalysis.PublicApiAnalyzers](https://github.com/dotnet/roslyn-analyzers/blob/main/src/PublicApiAnalyzers/PublicApiAnalyzers.Help.md)
  in which I first stumbled on this to me unknown feature:
  > One way of checking the version of your compiler is to add `#error version` in a
  > source file, then looking at the error message output in your build logs.

* [Display effective language version for #error version](https://github.com/dotnet/roslyn/pull/51880)

* [Overview of .NET, MSBuild, and Visual Studio versioning](https://docs.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs)