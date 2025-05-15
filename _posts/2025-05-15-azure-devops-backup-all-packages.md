---
layout: post
title: Backup (NuGet) Packages for All Azure DevOps Organization Feeds with PowerShell
---

This quick blog post demonstrates how to automate the backup of all packages
from all organization feeds on Azure DevOps using a scheduled pipeline and a
PowerShell script. The script leverages the [Azure DevOps REST
API](https://learn.microsoft.com/en-us/rest/api/azure/devops/artifacts/artifact-details?view=azure-devops-rest-7.1)
to enumerate feeds and download every package version to a network share. 

It handles skipping already downloaded packages and for our case can handle
checking several feeds and ~10,000 packages and versions in less than one minute
when no new packages found. No further details are provided here, but the script
is available below, I thought perhaps someone might find it useful as a
reference and the code is pretty straighforward.

Note that this only works for *organization* feeds not project feeds.

```yml
trigger: none

schedules:
  # 01:00 UTC every Sunday
  - cron: "0 1 * * 0"
    displayName: Weekly NuGet Backup
    branches:
      include:
        - main
    always: true

pool:
  name: 'Default'

variables:
  ORGANIZATION_NAME: 'YOURORGNAME'
  NETWORK_PATH: '\\YOURLOCALPATH' # UNC path to your network share

steps:
  - task: PowerShell@2
    displayName: Download NuGet packages from all feeds
    env:
      SYSTEM_ACCESSTOKEN: $(System.AccessToken)
    inputs:
      targetType: inline
      script: |
        $ErrorActionPreference = 'Stop'

        $org = "$(ORGANIZATION_NAME)"
        $networkBase = "$(NETWORK_PATH)"
        $token = $env:SYSTEM_ACCESSTOKEN
        $headers = @{ Authorization = "Bearer $token" }

        # Get all feeds
        $feedsUrl = "https://feeds.dev.azure.com/$org/_apis/packaging/feeds?api-version=7.1-preview.1"
        Write-Host "Fetching all feeds from $feedsUrl"
        $feedsResp = Invoke-RestMethod -Uri $feedsUrl -Headers $headers
        $feeds = $feedsResp.value

        Write-Host "Feeds count $($feeds.Count)"
        if ($feeds.Count -eq 0) {
          Write-Host "No feeds found."
          exit 0
        }

        foreach ($feed in $feeds) {
          $feedName = $feed.name
          $feedId = $feed.id
          $feedPath = Join-Path $networkBase $feedName

          Write-Host "=================================================================="
          Write-Host "====== '$($feed.name)'"
          Write-Host "=================================================================="

          $fetchBaseUrl = "https://feeds.dev.azure.com/$org/_apis/packaging/feeds/$feedId/packages?protocolType=NuGet&api-version=7.1"
          $downloadBaseUrl = "https://pkgs.dev.azure.com/$org/_apis/packaging/feeds/$feedId/nuget/packages/"
          $pageSize = 50000
          $skip = 0
          $allPkgs = @()
          $page = 1

          # Fetch package names and versions from the feed
          Write-Host "Fetching packages from feed '$feedName'..."

          while ($true) {
            $url = "$fetchBaseUrl&includeAllVersions=true&`$top=$pageSize&`$skip=$skip"
            Write-Host "Requesting page $($page) from $($url)"
            $resp = Invoke-RestMethod -Uri $url -Headers $headers

            if ($resp.value.Count -eq 0) {
              break
            }

            $allPkgs += $resp.value
            Write-Host "Fetched $($resp.value.Count) packages in page $($page)."
            $skip += $resp.value.Count
            $page++
          }

          Write-Host "Package names found $($allPkgs.Count) in feed '$feedName'"

          if ($allPkgs.Count -eq 0) {
            Write-Host "WARNING No packages found in feed '$feedName'."
            continue
          }

          # Ensure the target directory exists
          if (-not (Test-Path $feedPath)) {
            Write-Host "Creating directory '$feedPath'..."
            New-Item -ItemType Directory -Path $feedPath -Force | Out-Null
          } else {
            Write-Host "Directory '$feedPath' already exists."
          }

          # Download packages (skip already exists)
          $totalPackagesCount = 0
          foreach ($pkg in $allPkgs) {
            Write-Host "====== '$($pkg.name)' versions found $($pkg.versions.Count) ======"
            foreach ($ver in $pkg.versions) {
              $nupkgFileName = "$($pkg.name).$($ver.version).nupkg"
              $nupkgPath = Join-Path $feedPath $nupkgFileName

              if (Test-Path $nupkgPath) {
                Write-Host "Skipping '$($nupkgPath)' (already exists)"
                continue
              }

              $downloadUrl = "$($downloadBaseUrl)$($pkg.name)/versions/$($ver.version)/content?api-version=7.1-preview"
              Write-Host "Downloading '$($nupkgFileName)' to '$feedPath' from '$($downloadUrl)'"
              Invoke-RestMethod -Uri $downloadUrl -Headers $headers -OutFile $nupkgPath
            }
            $totalPackagesCount += $pkg.versions.Count
          }
          Write-Host "Total packages found (already downloaded or downloaded) $($totalPackagesCount) in feed '$feedName'"
        }
```

That's all!
