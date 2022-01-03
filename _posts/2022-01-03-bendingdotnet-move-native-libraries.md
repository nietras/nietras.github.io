---
layout: post
title: Bending .NET - Move Native Libraries to Sub-directory After Publish
---
or [how to make rebel humans hide due to hunting sentinels and still find them](https://www.youtube.com/watch?v=pQ0db2ERil8).

In this post, part of the [Bending .NET]({{ site.baseurl
}}/2021/11/18/bendingdotnet-series) series, I will cover how to move native
libraries (like WPF dependencies) to a sub-directory on publish and ensure these
are properly loaded during startup using
[`NativeLibrary`](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativelibrary?view=net-6.0).
The goal is to have only the single exe-file in the top-level directory and
sweeping everything else under the rug in a sub-directory.

![sweep under rug]({{ site.baseurl }}/images/2022-01-bendingdotnet-move-native-libraries-to-sub-directory/sweep-under-rug.jpg)
Source: [flickr](https://www.flickr.com/photos/gomattolson/4321594214/)

I have created a WPF application project based on 
[Bending .NET - Common Flat Build Output]({{ site.baseurl
}}/2021/11/19/bendingdotnet-common-flat-build-output)
ending up with the files shown below.

```
.\src\MoveNativeLibraries.AppWpf
.\src\MoveNativeLibraries.AppWpf\App.xaml
.\src\MoveNativeLibraries.AppWpf\App.xaml.cs
.\src\MoveNativeLibraries.AppWpf\AssemblyInfo.cs
.\src\MoveNativeLibraries.AppWpf\MainWindow.xaml
.\src\MoveNativeLibraries.AppWpf\MainWindow.xaml.cs
.\src\MoveNativeLibraries.AppWpf\MoveNativeLibraries.AppWpf.csproj
.\src\MoveNativeLibraries.AppWpf\Program.cs
.\src\Directory.Build.props
.\src\Directory.Build.targets
.\src\OutputBuildProps.props
.\src\OutputBuildTargets.props
.\global.json
.\MoveNativeLibraries.sln
.\publish.ps1
```

With the csproj-file defined initially as:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <DebugType>embedded</DebugType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <UseWinForms>true</UseWinForms>
    <StartupObject>MoveNativeLibaries.AppWpf.Program</StartupObject>
    
    <PublishSingleFile>true</PublishSingleFile>
    <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
  </PropertyGroup>

</Project>
```
this will output:
```
.\MoveNativeLibraries.AppWpf.exe
.\D3DCompiler_47_cor3.dll
.\PenImc_cor3.dll
.\PresentationNative_cor3.dll
.\vcruntime140_cor3.dll
.\wpfgfx_cor3.dll
```
in the flat output at
`build\MoveNativeLibraries.AppWpf_AnyCPU_Release_net6.0-windows_win-x64`
when running:
```powershell
dotnet publish --nologo -c Release -r win-x64 /p:Platform=AnyCPU \
  --self-contained true ./src/MoveNativeLibraries.AppWpf/MoveNativeLibraries.AppWpf.csproj
```

Now above is just an example. For a real application we have a large number of
native libraries totaling a couple of GBs in size and we very much prefer these
to be "hidden away" from the user in a sub-directory instead of next to the
exe-file. While .NET 6 does have an option
`IncludeNativeLibrariesForSelfExtract` as detailed in [Single file deployment
and
executable](https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file),
this means native libraries are extracted to a temporary directory that is hard
to find. With a couple of GBs this can quickly add up. It also makes the
executable itself much larger, which adds friction when doing updates where you
don't need to update native libraries. Note that there is an option to exclude
certain files from being embedded with `ExcludeFromSingleFile`, as discussed in
the link, but this doesn't work for our case either.

What we really want is for the dll-files to be moved to a sub-directory that for
example is named according to the `RuntimeIdentifier` like `win-x64` so we get:
```
.\win-x64\D3DCompiler_47_cor3.dll
.\win-x64\PenImc_cor3.dll
.\win-x64\PresentationNative_cor3.dll
.\win-x64\vcruntime140_cor3.dll
.\win-x64\wpfgfx_cor3.dll
.\MoveNativeLibraries.AppWpf.exe
```
Note that you can name the sub-directory whatever you want; `bin`, `libs` or
just `x64` and `x86` which is actually what we do due to pre-existing native
library conventions. Moving the files is very easy to do with a simple target
that is run after publish.
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <DebugType>embedded</DebugType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <UseWinForms>true</UseWinForms>
    <StartupObject>MoveNativeLibaries.AppWpf.Program</StartupObject>
    
    <PublishSingleFile>true</PublishSingleFile>
    <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
  </PropertyGroup>

  <Target Name="MoveNativeDllsToSubDirectory" AfterTargets="Publish">
    <PropertyGroup>
      <NativeDllSubDir>$(RuntimeIdentifier)</NativeDllSubDir>
    </PropertyGroup>
    <ItemGroup>
      <NativeDllsToMove Include="$(PublishDir)*.dll" />
    </ItemGroup>
    <Move SourceFiles="@(NativeDllsToMove)" 
          DestinationFolder="$(PublishDir)\$(NativeDllSubDir)\" />
    <Message Text="Moved native dlls to sub-directory '$(NativeDllSubDir)'" 
             Importance="high" />
  </Target>

</Project>
```

However, if we then try to run the executable nothing happens 🤦‍ Or to be
precise, the application crashes on startup. The exception that occurs can be
found with `EventViewer` under **Windows logs -> Application** and can look like
(from source **.NET Runtime**):
```
Application: MoveNativeLibraries.AppWpf.exe
CoreCLR Version: 6.0.21.52210
.NET Version: 6.0.0
Description: The process was terminated due to an unhandled exception.
Exception Info: System.DllNotFoundException: Dll was not found.
   at MS.Internal.WindowsBase.NativeMethodsSetLastError.SetWindowLongPtrWndProc(HandleRef hWnd, Int32 nIndex, WndProc dwNewLong)
   at MS.Win32.UnsafeNativeMethods.CriticalSetWindowLong(HandleRef hWnd, Int32 nIndex, WndProc dwNewLong)
   at MS.Win32.HwndSubclass.HookWindowProc(IntPtr hwnd, WndProc newWndProc, IntPtr oldWndProc)
   at MS.Win32.HwndSubclass.SubclassWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam)
```
The exception message `Dll was not found` is not useful at all. It doesn't say
which dll was not found. As far I can tell the `DllNotFoundException` for
P/Invoke methods has been changed in .NET 6 (Core?) to no longer include the dll
file name in question, which is a bit annoying. Fortunately, the stack trace
gives us a clue that this is due to WPF (`WindowsBase`) not being able to find a
native library. It is not a big issue here since we know that we have moved some
dlls and what they are, but in a large application with lots of native
dependencies this can be troublesome.

Normally, I would fix this by simply adding the `win-x64` sub-directory to the
list of directories searched with
[AddDllDirectory](https://docs.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-adddlldirectory)
(or
[SetDllDirectory](https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setdlldirectorya))
as also discussed in [Load native libraries from a dynamic
location](https://www.meziantou.net/load-native-libraries-from-a-dynamic-location.htm).
Another approach is discussed in [Native bindings in
C#](https://www.lostindetails.com/articles/Native-Bindings-in-CSharp). However,
for some reason none of these work without further changes for the WPF native
libraries. Instead, I had to manually pre-load the WPF native libraries using
[NativeLibrary.TryLoad](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativelibrary?view=net-6.0)
meaning the `Program.cs` ends up looking like:
```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MoveNativeLibaries.AppWpf;

class Program
{
    [STAThread]
    static void Main()
    {
        PreloadDotnetDependenciesFromSubdirectoryManually();

        RunApp();
    }

    static void RunApp()
    {
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    static void PreloadDotnetDependenciesFromSubdirectoryManually()
    {
        // https://www.lostindetails.com/articles/Native-Bindings-in-CSharp
        // https://www.meziantou.net/load-native-libraries-from-a-dynamic-location.htm
        // None of the above worked but approach is inspired by it.
        // First, ensure sub-directory with native libraries is 
        // added to dll directories
        var dllDirectory = Path.Combine(AppContext.BaseDirectory,
            Environment.Is64BitProcess ? "win-x64" : "win-x86");
        var r = AddDllDirectory(dllDirectory);
        Trace.WriteLine($"AddDllDirectory {dllDirectory} {r}");

        // Then, try manually loading the .NET 6 WPF 
        // native library dependencies
        TryManuallyLoad("vcruntime140_cor3");
        TryManuallyLoad("wpfgfx_cor3");
        TryManuallyLoad("PresentationNative_cor3");
        TryManuallyLoad("PenImc_cor3.dll");
        TryManuallyLoad("D3DCompiler_47_cor3");
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int AddDllDirectory(string NewDirectory);

    static void TryManuallyLoad(string libraryName)
    {
        // NOTE: For the native libraries we load here, 
        //       we do not care about closing the library 
        //       handle since they live as long as the process.
        var loaded = NativeLibrary.TryLoad(libraryName, 
            Assembly.GetExecutingAssembly(),
            DllImportSearchPath.SafeDirectories | 
            DllImportSearchPath.UserDirectories,
            out var handle);
        if (!loaded)
        {
            Trace.WriteLine($"Failed loading {libraryName}");
        }
        else
        {
            Trace.WriteLine($"Loaded {libraryName}");
        }
    }
}
```

Note that we've only had to manually pre-load the WPF native libraries, all the
other native libraries are loaded fine without this trick. Additionally, one
could forego using `AddDllDirectory` and simply load the dlls directly from
sub-directory e.g. by absolute path, but with the above approach we are leaving
open the option of still finding the dlls in some other path. 

Hence, the application now starts as expected and we can go back to hunting
rebel humans.

![Move Native Libraries Wpf App]({{ site.baseurl }}/images/2022-01-bendingdotnet-move-native-libraries-to-sub-directory/move-native-libraries-wpf-app.png)

PS: Example source code can be found in the GitHub 
repo for this blog [nietras.github.io](https://github.com/nietras/nietras.github.io)
and as a zip-file [MoveNativeLibraries.zip]({{ site.baseurl }}/images/2022-01-bendingdotnet-move-native-libraries-to-sub-directory/MoveNativeLibraries.zip).