# Publish self-contained
Write-Host "Publish X64"
dotnet publish --nologo -c Release -r win-x64 /p:Platform=AnyCPU --self-contained true ./src/MoveNativeLibraries.AppWpf/MoveNativeLibraries.AppWpf.csproj
Write-Host "Publish X86"
dotnet publish --nologo -c Release -r win-x86 /p:Platform=AnyCPU --self-contained true ./src/MoveNativeLibraries.AppWpf/MoveNativeLibraries.AppWpf.csproj