---
layout: post
title: Embedded Assembly Loading with support for Symbols and Portable Class Libraries in C#
---
REPOST: Of an old blog post from 2014.

Jeffrey Richter has previously written about how to deploy a single executable file for an application by embedding dependencies as resources in the main application assembly in [Jeffrey Richter: Excerpt #2 from CLR via C#, Third Edition](http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx).  
  
However, this solution has a few issues. It does not handle Portable Class Libraries (PCLs) and does not show how to support loading symbols from embedded pdb-files either. The code presented below handles both.  
  
As with the solution Jeffrey Richter details, one simply adds a handler to the current domains `AssemblyResolve` event, which is called whenever an assembly could not be resolved directly. However, this also occurs when an embedded portable class library (such as `Autofac`) has defined a dependency towards any BCL assembly (e.g. `System.Core 2.0.5.0`), in this case you have to check if the assembly is retargetable and then load it directly via the usual CLR mechanism so the appropriate version is loaded.  
  
For a better debug experience and better exception stack traces it is recommended to include pdb-files as well. pdb-files are handled by simply loading these if they have been embedded, have the same name as the dll-file and then using the `Assembly.Load` overload that also loads raw symbol data from a byte array.  
  
To use the code do the following:  

*   Call `SetupEmbeddedAssemblyResolve` as the first thing in your application
*   Add dependencies incl. pdb-files, if needed, to your project via **Add as Link** and change the build action to **Embedded Resource** and **Copy to Output Directory** to **Do not copy**
*   Change the assembly references properties under `References` and set **Copy Local** to **false**

UPDATE: See [Automatically Embed Copy Local Assemblies with Symbols in MSBuild]({{ site.baseurl }}/2020/10/01/automatically-embed-copy-local-assemblies/) for how to automatically embed assemblies instead of doing this manually.  
```csharp
private static void SetupEmbeddedAssemblyResolve()
{
    AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
    {
        var name = args.Name;
        var asmName = new AssemblyName(name);

        // Any retargetable assembly should be resolved directly using normal load e.g. System.Core issue 
        if (name.EndsWith("Retargetable=Yes"))
        {
            return Assembly.Load(asmName);
        }

        var executingAssembly = Assembly.GetExecutingAssembly();
        var resourceNames = executingAssembly.GetManifestResourceNames();

        var resourceToFind = asmName.Name + ".dll";
        var resourceName = resourceNames.SingleOrDefault(n => n.Contains(resourceToFind));

        if (string.IsNullOrWhiteSpace(resourceName)) { return null; }

        var symbolsToFind = asmName.Name + ".pdb";
        var symbolsName = resourceNames.SingleOrDefault(n => n.Contains(symbolsToFind));

        var assemblyData = LoadResourceBytes(executingAssembly, resourceName);

        if (string.IsNullOrWhiteSpace(symbolsName))
        { 
            Trace.WriteLine(string.Format("Loading '{0}' as embedded resource '{1}'", resourceToFind, resourceName));

            return Assembly.Load(assemblyData);
        }
        else
        {
            var symbolsData = LoadResourceBytes(executingAssembly, symbolsName);

            Trace.WriteLine(string.Format("Loading '{0}' as embedded resource '{1}' with symbols '{2}'", resourceToFind, resourceName, symbolsName));

            return Assembly.Load(assemblyData, symbolsData);
        }
    };
}

private static byte\[\] LoadResourceBytes(Assembly executingAssembly, string resourceName)
{
    using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
    {
        var data = new byte\[stream.Length\];

        stream.Read(data, 0, data.Length);

        return data;
    }
}
```
Based on [Jeffrey Richter: Excerpt #2 from CLR via C#, Third Edition](http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx) and [FileNotFoundException when trying to load Autofac as an embedded assembly](http://stackoverflow.com/questions/18793959/filenotfoundexception-when-trying-to-load-autofac-as-an-embedded-assembly).