#!/usr/local/bin/powershell
dotnet publish --nologo -c Debug /p:Platform=AnyCPU .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish --nologo -c Release /p:Platform=AnyCPU .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish --nologo -c Release -r win-x64 /p:Platform=AnyCPU /p:PublishSingleFile=true --self-contained .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish --nologo -c Release -r win-x86 /p:Platform=AnyCPU /p:PublishSingleFile=true --no-self-contained .\src\CommonFlatBuild.AppWpf\CommonFlatBuild.AppWpf.csproj
dotnet publish --nologo -c Release -r linux-x64 /p:Platform=AnyCPU /p:PublishSingleFile=true --self-contained .\src\CommonFlatBuild.AppConsole\CommonFlatBuild.AppConsole.csproj