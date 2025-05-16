---
layout: post
title: How to Get Windows 8.3 Short File Names Using FindFirstFile (UNC) and GetShortPathName (local)
---

Working with files and directories on Windows, especially on network shares,
often leads to issues with long paths. This post explains what Windows 8.3 short
paths are, how they work in NTFS, the difference between local and UNC paths,
why browsers and some tools fail with long network paths, and how to
programmatically obtain short file names for both local and UNC files.

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
- UNC paths are resolved over the network, and some Windows APIs behave differently or have limitations with UNC paths.

**Authoritative source:**  
- [Microsoft Docs: UNC Paths](https://learn.microsoft.com/en-us/dotnet/standard/io/file-path-formats#unc-paths)

## Why Browsers and Some Tools Fail with Long Network Paths

Windows has a traditional `MAX_PATH` limit of 260 characters for file paths.
While modern Windows versions and .NET can support longer paths (with
configuration), many tools—including browsers—do not support long UNC paths due
to:

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

public static class Win32ShortPath
{
    const int MaxPathLength = 260;
    const int UncPrefixPartCount = 2;
    static readonly char DirectorySeparator = Path.DirectorySeparatorChar;
    const string UncPrefix = @"\\";

    /// <summary>
    /// Returns the short (8.3) path for any file or directory if available,
    /// covering both local and UNC paths.
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
        return isUnc ? GetByFindFirstFile(longPath) : GetByGetShortPathName(longPath);
    }

    // Local drive (C:\…), use GetShortPathName directly
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
        // Rebuild segment by segment, use FindFirstFile for the 8.3 name of
        // each.
        var parts = longPath.TrimStart(DirectorySeparator).Split(DirectorySeparator);
        if (parts.Length < UncPrefixPartCount)
        { throw new ArgumentException($"Invalid UNC path '{longPath}'", nameof(longPath)); }

        var sb = new StringBuilder(MaxPathLength);

        // Re-prefix with \\server\share
        sb.Append(UncPrefix)
          .Append(parts[0])
          .Append(DirectorySeparator)
          .Append(parts[1]);

        AppendPartsByFindFirstFile(parts, UncPrefixPartCount, sb);

        return sb.ToString();
    }

    /// <summary>
    /// Shortens the path parts in-place by using the 8.3 name format.
    /// </summary>
    /// <param name="sb"><see cref="StringBuilder"/> for building path, can be
    /// pre-appended with initial base path.</param>
    public static void ShortenPartsByFindFirstFile(Span<string> parts, StringBuilder sb)
    {
        foreach (ref var currentPart in parts)
        {
            sb.Append(Path.DirectorySeparatorChar);
            sb.Append(currentPart);
            var findHandle = FindFirstFileW(sb, out var findData);
            if (findHandle != IntPtr.Zero)
            {
                FindClose(findHandle);
                var alternatePart = findData.cAlternateFileName;
                // If there's an alternate (8.3) name, use it
                if (!string.IsNullOrEmpty(alternatePart))
                {
                    // Replace part with short path
                    sb.Remove(sb.Length - currentPart.Length, currentPart.Length);
                    sb.Append(alternatePart);
                    currentPart = alternatePart;
                }
            }
        }
    }

    public static string GetByFindFirstFileReverse(string longPath)
    {
        if (string.IsNullOrWhiteSpace(longPath))
        { throw new ArgumentNullException(nameof(longPath)); }

        // Normalize
        longPath = Path.GetFullPath(longPath);

        if (!File.Exists(longPath) && !Directory.Exists(longPath))
        { throw new FileNotFoundException("Path not found", longPath); }

        var parts = longPath.TrimStart(DirectorySeparator).Split(DirectorySeparator);
        if (parts.Length < UncPrefixPartCount)
        { throw new ArgumentException($"Invalid UNC path '{longPath}'", nameof(longPath)); }

        var sb = new StringBuilder(MaxPathLength);

        // Start from the end and rebuild the path in reverse
        for (int i = parts.Length - 1; i >= UncPrefixPartCount; i--)
        {
            var currentPart = parts[i];
            var pathToQuery = $"{UncPrefix}{string.Join(DirectorySeparator, parts.Take(i + 1))}";
            var findHandle = FindFirstFileW(pathToQuery, out var findData);
            if (findHandle != IntPtr.Zero)
            {
                FindClose(findHandle);
                // If there's an alternate (8.3) name, use it
                var alternatePart = findData.cAlternateFileName;
                currentPart = string.IsNullOrEmpty(alternatePart)
                    ? currentPart : alternatePart;
            }
            sb.Insert(0, $"{DirectorySeparator}{currentPart}");
        }

        // Re-prefix with \\server\share
        sb.Insert(0, $"{UncPrefix}{parts[0]}{DirectorySeparator}{parts[1]}");

        return sb.ToString();
    }

    static void AppendPartsByFindFirstFile(string[] parts, int partStart, StringBuilder sb)
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

## Further Reading

- [Naming Files, Paths, and Namespaces (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file)
- [How to get the short path for a UNC path (Stack Overflow)](https://stackoverflow.com/questions/214858/getting-short-filename-in-net-for-unc-path)
- [Maximum Path Length Limitation (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation)
- [GetShortPathName function (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getshortpathnamew)
- [FindFirstFileW function (Microsoft Docs)](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-findfirstfilew)

## Summary

Windows 8.3 short paths are a legacy but still useful feature for compatibility
and working around path length limitations, especially on network shares. For
local paths, use `GetShortPathName`. For UNC/network paths, use `FindFirstFileW`
on each segment. This can help applications and scripts work with long paths
that would otherwise be inaccessible, especially in environments where browser
or tool support is limited.
