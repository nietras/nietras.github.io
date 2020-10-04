---
layout: post
title: Automatically Embed Copy Local Assemblies with Symbols in MSBuild
---
REPOST: Of an old blog post from 2014.

Many have written about how to automatically embed assemblies into an executable, such as:  

*   Daniel Chambers in [Combining multiple assemblies into a single EXE for a WPF application](http://www.digitallycreated.net/Blog/61/combining-multiple-assemblies-into-a-single-exe-for-a-wpf-application)
*   Thomas Freudenberg shows some code in [Embed referenced assemblies, because ILMerge won't work with WPF applications](https://gist.github.com/thoemmi/3724333)

Lots of questions about this on stackoverflow as well, such as:

*   [Embedding DLLs in a compiled executable](http://stackoverflow.com/questions/189549/embedding-dlls-in-a-compiled-executable)

However, none of these show how pdb-files can be embedded as well to ensure symbols are also loaded when resolving embedded assemblies as detailed in [Embedded Assembly Loading with support for Symbols and Portable Class Libraries in C#]({{ site.baseurl }}/2020/10/01/embedded-assembly-loading/).  
  

Automatically embed all dll- and pdb-files exclude xml-files
------------------------------------------------------------

The solution, shown below, is a simple extension of what Daniel Chambers has described, but also includes `pdb`\-files and exclude copying of `xml`\-files to the output directory since many libraries often include these documentation files.
```xml
<Target Name="EmbedReferencedAssemblies" AfterTargets="ResolveAssemblyReferences">
  <ItemGroup>
    <!-- get list of assemblies marked as CopyToLocal -->
    <FilesToEmbed Include="@(ReferenceCopyLocalPaths)" 
                  Condition="('%(ReferenceCopyLocalPaths.Extension)' == '.dll' Or '%(ReferenceCopyLocalPaths.Extension)' == '.pdb')" />
    <FilesToExclude Include="@(ReferenceCopyLocalPaths)" 
                  Condition="'%(ReferenceCopyLocalPaths.Extension)' == '.xml'" />

    <!-- add these assemblies to the list of embedded resources -->
    <EmbeddedResource Include="@(FilesToEmbed)">
      <LogicalName>%(FilesToEmbed.DestinationSubDirectory)%(FilesToEmbed.Filename)%(FilesToEmbed.Extension)</LogicalName>
    </EmbeddedResource>

    <!-- no need to copy the assemblies locally anymore -->
    <ReferenceCopyLocalPaths Remove="@(FilesToEmbed)" />
    <ReferenceCopyLocalPaths Remove="@(FilesToExclude)" />
  </ItemGroup>

  <Message Importance="high" Text="Embedding: @(FilesToEmbed->'%(Filename)%(Extension)', ', ')" />
</Target>
```
To use this simply copy and paste this into the executable project file (e.g. `*.csproj`) right after:  

<Import Project="$(MSBuildToolsPath)\\Microsoft.CSharp.targets" />

Automatically embed all dll- and pdb-files exclude xml-files and mixed mode assemblies
--------------------------------------------------------------------------------------

However, unfortunately as far as I know embedding mixed mode assemblies (e.g. with both managed and native code from for example a C++/CLI project) does not work. So these still have to be copied to the build output. At least, if you do not want to extract the embedded file, as detailed in [Single Assembly Deployment of Managed and Unmanaged Code](http://weblogs.asp.net/ralfw/archive/2007/02/04/single-assembly-deployment-of-managed-and-unmanaged-code.aspx).  
  
One solution to this is to simply exclude these files by adding exclude conditions to the above xml. For example:  

```xml
<Target Name="EmbedReferencedAssemblies" AfterTargets="ResolveAssemblyReferences">
  <ItemGroup>
    <!-- get list of assemblies marked as CopyToLocal -->
    <FilesToEmbed Include="@(ReferenceCopyLocalPaths)" 
                  Condition="('%(ReferenceCopyLocalPaths.Extension)' == '.dll' Or '%(ReferenceCopyLocalPaths.Extension)' == '.pdb') And '%(Filename)'!='MixedModeAssemblyA' And '%(Filename)'!='MixedModeAssemblyB'" />
    <FilesToExclude Include="@(ReferenceCopyLocalPaths)" 
                  Condition="'%(ReferenceCopyLocalPaths.Extension)' == '.xml'" />

    <!-- add these assemblies to the list of embedded resources -->
    <EmbeddedResource Include="@(FilesToEmbed)">
      <LogicalName>%(FilesToEmbed.DestinationSubDirectory)%(FilesToEmbed.Filename)%(FilesToEmbed.Extension)</LogicalName>
    </EmbeddedResource>

    <!-- no need to copy the assemblies locally anymore -->
    <ReferenceCopyLocalPaths Remove="@(FilesToEmbed)" />
    <ReferenceCopyLocalPaths Remove="@(FilesToExclude)" />
  </ItemGroup>

  <Message Importance="high" Text="Embedding: @(FilesToEmbed->'%(Filename)%(Extension)', ', ')" />
</Target>
```

I would love to have a solution that actually checks whether an assembly is mixed mode (i.e. not pure) before embedding it. Or at least create a list of assembly names to exclude instead of the crude condition hack above.

  

One could also imagine checking the path of the assembly and whether this has `AnyCPU`, `x86`, `x64` in the path or similarly as a convention for embedding or not embedding the given assembly. Lots of other improvements should be possible...  
  
There is also a complete solution out there in the form of [Costura.Fody](https://github.com/Fody/Costura), which exists as a convenient nuget package as well, see [http://www.nuget.org/packages/Costura.Fody](http://www.nuget.org/packages/Costura.Fody). This does, however, rely on IL rewriting which may be a problem for some. It does look as if it handles all possible issues via configuration, though.