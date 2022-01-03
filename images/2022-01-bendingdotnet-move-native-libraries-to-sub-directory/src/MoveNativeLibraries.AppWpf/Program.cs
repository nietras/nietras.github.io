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