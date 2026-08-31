#!/usr/bin/env pwsh
[CmdletBinding()]
param(
    [ValidateSet("Check", "Set", "Restore")]
    [string] $Action = "Check",

    [ValidateSet("http", "https")]
    [string[]] $Protocol = @("http", "https"),

    [string] $ProgId = "ChromeHTML",

    [string] $BackupPath,

    [string] $SftaPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$associationsPath = "HKCU:\Software\Microsoft\Windows\Shell\Associations\UrlAssociations"
$associationsRegPath = "HKCU\Software\Microsoft\Windows\Shell\Associations\UrlAssociations"
$sftaCommit = "22a32292e576afc976a1167d92b50741ef523066"
$sftaSha256 = "3EB6F6DEE3FD8C91604042060B9D658F08EC85D3FD0A14769119DFD78BC30851"
$sftaUrl = "https://raw.githubusercontent.com/DanysysTeam/PS-SFTA/$sftaCommit/SFTA.ps1"

function Get-RegistryValue {
    param(
        [Parameter(Mandatory)]
        [string] $Path,

        [Parameter(Mandatory)]
        [string] $Name
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return $null
    }

    $item = Get-ItemProperty -LiteralPath $Path -Name $Name -ErrorAction SilentlyContinue
    $property = $item.PSObject.Properties[$Name]
    if (-not $property) {
        return $null
    }

    return $property.Value
}

function Get-ProgIdCommand {
    param(
        [Parameter(Mandatory)]
        [string] $Id
    )

    $path = "Registry::HKEY_CLASSES_ROOT\$Id\shell\open\command"
    if (-not (Test-Path -LiteralPath $path)) {
        return $null
    }

    return (Get-Item -LiteralPath $path).GetValue("")
}

function Get-UrlAssociationState {
    param(
        [Parameter(Mandatory)]
        [string[]] $Protocols
    )

    foreach ($item in $Protocols) {
        $currentPath = "$associationsPath\$item\UserChoice"
        $latestPath = "$associationsPath\$item\UserChoiceLatest"
        $latestProgIdPath = "$latestPath\ProgId"
        $currentProgId = Get-RegistryValue -Path $currentPath -Name "ProgId"
        $latestProgId = Get-RegistryValue -Path $latestProgIdPath -Name "ProgId"

        [pscustomobject]@{
            Protocol = $item
            CurrentProgId = $currentProgId
            CurrentHash = Get-RegistryValue -Path $currentPath -Name "Hash"
            LatestProgId = $latestProgId
            LatestHash = Get-RegistryValue -Path $latestPath -Name "Hash"
            CurrentCommand = if ($currentProgId) { Get-ProgIdCommand -Id $currentProgId } else { $null }
            LatestCommand = if ($latestProgId) { Get-ProgIdCommand -Id $latestProgId } else { $null }
        }
    }
}

function Get-UcpdState {
    $driver = Get-CimInstance Win32_SystemDriver -Filter "Name='UCPD'" -ErrorAction SilentlyContinue
    $configuration = Get-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Services\UCPD" -ErrorAction SilentlyContinue

    [pscustomobject]@{
        State = if ($driver) { $driver.State } else { "Not installed" }
        Start = if ($configuration) { $configuration.Start } else { $null }
        StartDescription = switch ($configuration.Start) {
            0 { "Boot" }
            1 { "System" }
            2 { "Automatic" }
            3 { "Manual" }
            4 { "Disabled" }
            default { "Unknown" }
        }
    }
}

function Show-State {
    $state = @(Get-UrlAssociationState -Protocols $Protocol)
    $state | Format-Table Protocol, CurrentProgId, CurrentHash, LatestProgId, LatestHash -AutoSize

    foreach ($item in $state) {
        Write-Host "[$($item.Protocol)] current command: $($item.CurrentCommand)"
        Write-Host "[$($item.Protocol)] latest command:  $($item.LatestCommand)"
    }

    Write-Host ""
    Get-UcpdState | Format-List
}

if ($Action -eq "Check") {
    Show-State
    return
}

if ($Action -eq "Restore") {
    if (-not $BackupPath) {
        throw "-BackupPath is required for Restore."
    }

    if (-not (Test-Path -LiteralPath $BackupPath)) {
        throw "Backup file not found: $BackupPath"
    }

    & reg.exe import $BackupPath
    if ($LASTEXITCODE -ne 0) {
        throw "Registry import failed with exit code $LASTEXITCODE."
    }

    Show-State
    return
}

$ucpd = Get-UcpdState
if ($ucpd.State -eq "Running") {
    throw @"
UCPD is running and blocks creation of legacy HTTP/HTTPS UserChoice records.
No association was changed.

This script deliberately does not disable UCPD automatically. To prepare the machine,
run the following in an elevated PowerShell window, reboot, and then rerun this script:

  Set-ItemProperty 'HKLM:\SYSTEM\CurrentControlSet\Services\UCPD' -Name Start -Value 4
  Get-ScheduledTask -TaskName 'UCPD velocity' -ErrorAction SilentlyContinue | Disable-ScheduledTask
  Restart-Computer

After setting the choices, restore UCPD with Start=1 and reboot:

  Set-ItemProperty 'HKLM:\SYSTEM\CurrentControlSet\Services\UCPD' -Name Start -Value 1
  Restart-Computer
"@
}

$command = Get-ProgIdCommand -Id $ProgId
if (-not $command) {
    throw "ProgID '$ProgId' has no registered shell open command."
}

if (-not $BackupPath) {
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $backupDirectory = Join-Path $env:LOCALAPPDATA "LegacyUrlChoice\Backups"
    New-Item -ItemType Directory -Path $backupDirectory -Force | Out-Null
    $BackupPath = Join-Path $backupDirectory "url-associations-$timestamp.reg"
}

& reg.exe export $associationsRegPath $BackupPath /y
if ($LASTEXITCODE -ne 0) {
    throw "Registry export failed with exit code $LASTEXITCODE."
}

if (-not $SftaPath) {
    $toolDirectory = Join-Path $env:LOCALAPPDATA "LegacyUrlChoice"
    $SftaPath = Join-Path $toolDirectory "SFTA-$sftaCommit.ps1"
    New-Item -ItemType Directory -Path $toolDirectory -Force | Out-Null

    if (-not (Test-Path -LiteralPath $SftaPath)) {
        Invoke-WebRequest -Uri $sftaUrl -OutFile $SftaPath -UseBasicParsing
    }
}

if (-not (Test-Path -LiteralPath $SftaPath)) {
    throw "PS-SFTA source not found: $SftaPath"
}

$actualHash = (Get-FileHash -LiteralPath $SftaPath -Algorithm SHA256).Hash
if ($actualHash -ne $sftaSha256) {
    throw "PS-SFTA source hash mismatch. Expected $sftaSha256 but found $actualHash."
}

Set-StrictMode -Off
try {
    . $SftaPath
    foreach ($item in $Protocol) {
        Set-PTA -ProgId $ProgId -Protocol $item -Verbose
    }
}
finally {
    Set-StrictMode -Version Latest
}

$result = @(Get-UrlAssociationState -Protocols $Protocol)
$invalid = @($result | Where-Object { $_.CurrentProgId -ne $ProgId -or -not $_.CurrentHash })
if ($invalid.Count -gt 0) {
    throw "One or more legacy associations were not written successfully. Backup: $BackupPath"
}

Write-Host "Legacy associations set to $ProgId."
Write-Host "Backup: $BackupPath"
Show-State
