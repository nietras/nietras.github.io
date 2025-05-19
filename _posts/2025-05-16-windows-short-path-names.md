---
layout: post
title: How to Get Windows 8.3 Short File Names Using FindFirstFileW (UNC) and GetShortPathName (local)
---

Working with files and directories on Windows, especially on network shares,
often leads to issues with long paths. This post explains what Windows 8.3 short
paths are, how they work in NTFS, the difference between local and UNC paths,
why browsers and some tools fail with long network paths, and how to
programmatically obtain short file names for both local and UNC files.

First, a bit of color on why we need this for our machine learning workflow.

## Use Case and Problem

At work we train all our machine learning image models in house on custom build
servers featuring GPUs like NVIDIA RTX 4090. We have a lot of data - collected
from production sites all over the world - and this data is usually split in
two. 

1. **Ground truth annotations** are typically defined in `csv`-files that are stored
   in git repositories and published as versioned NuGet packages. These packages
   are then consumed by different pipelines that are run as Azure Pipelines on
   the servers. We then use [Renovate to automatically bump versions](LINK TO
   BLOG POST) of these.
1. **Images** are stored on a file server on a network share, and typically also
   cached at specific downsampled resolutions locally on the servers to speed up
   training.

At the same time the path to and file name of the images usually follow a schema
so the path alone contains information relevant to a given image. This means
paths can get quite long. Certainly above 260 characters on the file server.

Example path schema for directory:
```
\\fileserver\Pipelines\<PROJECTNAME>\<SETDATETIME>_<SETNAME>_<SITE>\<STATION>\
```
Example file name schema:
```
<DATETIME>_Camera=<CAMERANAME>_Id=<ID>_<DETAILS>.png
```
For example:
```
20250102.123456.789_Camera=Primary_Id=9999999_x=0123_y=0234_w=1234_h=2345.png
```


This is a simple pragmatic setup that works very well and a setup we have
iterated on over many years and runs very smoothly. Each pipeline is a separate
git repo that is easy to get started with by simply cloning and hitting F5 in
Visual Studio or similar for local running.

As part of the output of the pipelines we generate a lot of data either as
simple `csv`-files, plots `png`-files or interactive reports in the form of
`html`-files with inline JavaScript and data.

Below is an example showing box plots including all samples as dots for a
regression model with different "levels". The box plots are interactive and when
pointing on a singular dot the image for that sample will be shown on the right,
usually, but not here since if the path is too long (> 260 chars) the browser
won't show it. 

![]({{ site.baseurl }}/images/2025-05-windows-short-path-names/example-boxplot-outlier-image.png)

This is where there can be issues since browsers do not support showing long
file paths, e.g. see [chromium: Long file path handling not working on
Windows](https://issues.chromium.org/issues/40134281). And browsers do not
support extended length prefix `\\?\` that one can otherwise use to escape the
`MAX_PATH`/260 char limit in Win32 APIs.

Now if you SHIFT right click on a file in Windows Explorer and click **Copy as
path** you will get the 8.3 short file name path if the file path is longer than
`MAX_PATH`, as shown below both for a UNC file path and a local path name.

Copy as path for a local file resulting in
`C:\Temp\LONGFI~1\VERYLO~1\202501~1.PNG`: 

![]({{ site.baseurl
}}/images/2025-05-windows-short-path-names/verylongpath-local-copy-as-path.png)

Copy as path for a network share file resulting in
`\\files\Pipelines\LXF59T~G\V1WO8N~7\290O13~C.PNG`: 
![]({{ site.baseurl }}/images/2025-05-windows-short-path-names/verylongpath-unc-copy-as-path.png)

Similarly, if you right click and select **Open with...** and select a browser
like Chrome then it will use the short path name for long file paths.

Hence, to fix the above interactive box plots we need to get the short path name
in C#, as this is what we use to generate the `html`-files. This has the added
benefit of reducing the amount of data we have to store in the `html`-file since
the path name is shorter (we already do some custom path compression on this
using a simple common prefix algorithm). Our data sets can have hundreds of
thousands of images so every byte counts.

Incidentally, it would have been nice if the browsers would support loading
gzip'ed html e.g. files directly, but as far as I know they do not, and spinning
up a web server just for this just seemed overkill.

So let's take a quick look at Windows 8.3 short paths and some example code on
how get them in C# for both local and UNC paths.

## What Are Windows 8.3 Short Paths?

Windows 8.3 short paths (also called "short file names" or SFN) are a legacy
feature from MS-DOS and early Windows versions. They provide a way to represent
long file and directory names (introduced with Windows 95 and NTFS) in a format
compatible with older software that only supports 8-character filenames and
3-character extensions (e.g., `MYDOCU~1.TXT`). See [Naming Files, Paths, and
Namespaces - Short vs. Long
Names](https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file#short-vs-long-names)
or [wikipedia 8.3 filename](https://en.wikipedia.org/wiki/8.3_filename) for
more. **Example:**  

- Long name: `C:\Program Files\My Application\readme.txt`  
- Short name: `C:\PROGRA~1\MYAPPL~1\README.TXT`

## How Are 8.3 Short Paths Stored in NTFS?

[NTFS](https://en.wikipedia.org/wiki/NTFS#Metadata) stores both the long and
short (8.3) names for each file and directory, if 8.3 name generation is
enabled. The short name is generated when a file is created, following specific
rules to ensure uniqueness.

- **Short names are stored as metadata** in the NTFS file system. 
- You can disable 8.3 name creation for performance reasons, but this may break
  compatibility with legacy applications.

## Local Paths vs. UNC Paths

- **Local paths** refer to files on a local drive, e.g., `C:\folder\file.txt`.
- **UNC (Universal Naming Convention) paths** refer to files on a network share, e.g., `\\server\share\folder\file.txt`.

**Key differences:**
- Local paths are handled directly by the local file system.
- [UNC Paths](https://learn.microsoft.com/en-us/dotnet/standard/io/file-path-formats#unc-paths) 
  are resolved over the network, and some Windows APIs behave differently or 
  have limitations with UNC paths.

## Why Browsers and Some Tools Fail with Long Network Paths

Windows has a traditional `MAX_PATH` limit of 260 characters for file paths.
While modern Windows versions and .NET can support longer paths (with
configuration), many tools—including browsers — do not support long UNC paths
due to:

- Lack of support for the `\\?\` extended-length path prefix.
- Network shares (UNC) are not always handled with the same APIs as local paths.
- Browsers use standard Windows APIs that may not support long paths or UNC paths.

**Authoritative source:**  
- [Microsoft Docs: Maximum Path Length Limitation](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation)
- [Why browsers can't open long UNC paths (Stack Overflow)](https://stackoverflow.com/questions/1880321/why-does-windows-explorer-fail-to-open-a-path-that-is-longer-than-260-characters)

## Getting the 8.3 Short Path Programmatically

### For Local Paths

Use the Windows API function

```csharp
[`GetShortPathName`](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getshortpathnamew):
[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);
```

- Works for local file system paths (e.g., `C:\...`).
- Does **not** work for UNC paths.

### For UNC Paths (Network Shares)

Use
[`FindFirstFileW`](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-findfirstfilew)
to retrieve the 8.3 name for each segment of the path:

- Iterate over each directory/file segment in the UNC path.
- For each, call `FindFirstFileW` and use the `cAlternateFileName` field from the returned `WIN32_FIND_DATA` structure.
- Rebuild the path using the 8.3 names.

**Example code:**  
See the `Win32ShortPath` class in the code below for a full example implementation.

## Summary Table

| Path Type   | API to Use           | Notes                                 |
|-------------|----------------------|---------------------------------------|
| Local       | GetShortPathName     | Direct, simple                        |
| UNC/Network | FindFirstFileW       | Iterate segments, use cAlternateFileName |

## Win32ShortPath Class

```csharp
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

public static partial class Win32ShortPath
{
    const int MaxPathLength = 260;
    const int UncPrefixPartCount = 2;
    static readonly char DirectorySeparator = Path.DirectorySeparatorChar;
    const string UncPrefix = @"\\";
    const string ExtendedLengthPrefix = @"\\?\";

    /// <summary>
    /// Returns the short (8.3) path for any file or directory if 
    /// available, covering both local and UNC paths.
    /// </summary>
    public static string GetShortPath(string longPath)
    {
        if (string.IsNullOrWhiteSpace(longPath))
        { throw new ArgumentNullException(nameof(longPath)); }

        // Normalize
        longPath = Path.GetFullPath(longPath);

        if (!File.Exists(longPath) && !Directory.Exists(longPath))
        { throw new FileNotFoundException("Path not found", longPath); }

        var isUnc = longPath.StartsWith(UncPrefix, StringComparison.Ordinal);
        if (isUnc) { return GetByFindFirstFile(longPath); }

        var isLong = longPath.Length > MaxPathLength;
        if (isLong)
        {
            // Ensure starts with extended length prefix
            var extendedPrefix = longPath.StartsWith(ExtendedLengthPrefix,
                StringComparison.Ordinal);
            if (!extendedPrefix)
            {
                longPath = ExtendedLengthPrefix + longPath;
            }
            var shortPathName = GetByGetShortPathName(longPath);
            // Remove the extended length prefix if added
            if (!extendedPrefix)
            {
                shortPathName = shortPathName[ExtendedLengthPrefix.Length..];
            }
            return shortPathName;
        }
        else
        {
            return GetByGetShortPathName(longPath);
        }
    }

    // Local drive (C:\…), use GetShortPathName directly, for long prefix with \\?\
    static string GetByGetShortPathName(string longPath)
    {
        var sb = new StringBuilder(MaxPathLength);
        uint size = GetShortPathName(longPath, sb, (uint)sb.Capacity);
        if (size == 0) { throw new Win32Exception(Marshal.GetLastWin32Error()); }
        return sb.ToString();
    }

    // UNC path: \\server\share\…\file.ext, use FindFirstFile iteratively
    static string GetByFindFirstFile(string longPath)
    {
        // Rebuild segment by segment, use FindFirstFile for the 
        // 8.3 name of each.
        var parts = longPath.TrimStart(DirectorySeparator)
                            .Split(DirectorySeparator);
        if (parts.Length < UncPrefixPartCount)
        { throw new ArgumentException($"Invalid UNC path '{longPath}'", nameof(longPath)); }

        var sb = new StringBuilder(MaxPathLength);

        // Re‑prefix with \\server\share
        sb.Append(UncPrefix)
          .Append(parts[0])
          .Append(DirectorySeparator)
          .Append(parts[1]);

        AppendPartsByFindFirstFile(parts, UncPrefixPartCount, sb);

        return sb.ToString();
    }

    static void AppendPartsByFindFirstFile(string[] parts, int partStart,
                                           StringBuilder sb)
    {
        for (int i = partStart; i < parts.Length; i++)
        {
            var currentPart = parts[i];
            var pathToQuery = $"{sb}{DirectorySeparator}{currentPart}";
            var findHandle = FindFirstFileW(pathToQuery, out var findData);
            sb.Append(DirectorySeparator);
            if (findHandle != IntPtr.Zero)
            {
                FindClose(findHandle);
                // If there's an alternate (8.3) name, use it
                var alternatePart = findData.cAlternateFileName;
                currentPart = string.IsNullOrEmpty(alternatePart)
                    ? currentPart : alternatePart;
            }
            sb.Append(currentPart);
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern uint GetShortPathName(string lpszLongPath,
        StringBuilder lpszShortPath, uint cchBuffer);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    struct WIN32_FIND_DATA
    {
        const int AlternateLength = 14;

        public FileAttributes dwFileAttributes;
        public long ftCreationTime;
        public long ftLastAccessTime;
        public long ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPathLength)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AlternateLength)]
        public string cAlternateFileName; // The 8.3 name
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern IntPtr FindFirstFileW(string lpFileName,
        out WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern IntPtr FindFirstFileW(StringBuilder lpFileName,
        out WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool FindClose(IntPtr hFindFile);
}
```

## Links and Further Reading

- [Naming Files, Paths, and Namespaces (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file)
- [How to get the short path for a UNC path (Stack Overflow)](https://stackoverflow.com/questions/214858/getting-short-filename-in-net-for-unc-path)
- [Maximum Path Length Limitation (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation)
- [GetShortPathName function (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getshortpathnamew)
- [FindFirstFileW function (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-findfirstfilew)
- [Long Filename Specification](https://web.archive.org/web/20181025124257/home.teleport.com/~brainy/lfn.htm)
- [The Definitive Guide on Win32 to NT Path Conversion](https://googleprojectzero.blogspot.com/2016/02/the-definitive-guide-on-win32-to-nt.html)

## Summary

Windows 8.3 short paths are a legacy but still useful feature for compatibility
and working around path length limitations, especially on network shares. For
local paths, use `GetShortPathName`. For UNC/network paths, use `FindFirstFileW`
on each segment. This can help applications and scripts work with long paths
that would otherwise be inaccessible, especially in environments where browser
or tool support is limited.



## How to Get the Windows Short Path Name for Very Long File Paths (Local and UNC)

When working with very long file or directory paths in Windows-especially those
exceeding the traditional `MAX_PATH` (260 characters)-obtaining the short (8.3)
path name can be tricky. This guide explains how to reliably get the short path
name for both local and UNC paths, and how to handle common pitfalls.

---

### Why Use the Short Path Name?

The Windows short path (8.3) format is sometimes required for legacy
applications, scripting, or compatibility reasons. However, not all files or
directories have short names, and support depends on the file system and volume
settings.

---

## Step-by-Step Solution

### 1. Use the Correct API

The recommended method is the
[`GetShortPathNameW`](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getshortpathnamew)
function, which retrieves the short path form of a given file or directory.

### 2. Handle Long Paths with Prefixes

By default, Windows APIs limit paths to `MAX_PATH`. To support longer paths (up
to 32,767 characters):

- **Local paths:** Prepend with `\\?\`
- **UNC (network) paths:** Prepend with `\\?\UNC\`

**Examples:**
- Local: `\\?\C:\very\long\path\to\file.txt`
- UNC: `\\?\UNC\server\share\very\long\path\file.txt`

> ?? **Tip:** The
[`\\?\`](https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file#maximum-path-length-limitation)
prefix enables extended-length path support in Windows.

### 3. Check Short Name Availability

- Not all files/folders have short names. If the volume disables short name generation or the file system doesn’t support it (e.g., ReFS, some SMB shares), the function may return the original long path.
- [NTFS and FAT32](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation) generally support short names, but this can be disabled per-volume.

### 4. Avoid Common Pitfalls

- [`FindFirstFileW`](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-findfirstfilew) does **not** reliably return the short name for long paths or for components without short names.
- If you omit the `\\?\` or `\\?\UNC\` prefix, `GetShortPathNameW` will fail for paths longer than `MAX_PATH`.
