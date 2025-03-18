---
layout: post
title: Retrieving Azure DevOps Pull Requests for Entire Organization with PowerShell
---

In this 99% LLM generated post, I’ll walk you through a PowerShell script that
retrieves pull requests (PRs) descriptions from your Azure DevOps organization.

The script targets scenarios where you want to list all PRs (up to 1000 per
project) and filter them based on criteria such as creation date and the
author’s display name (for example, matching initials or part of a full name).
Although the REST API enforces pagination, this appears not to be working for
querying pull requests, this simplified version gets top 1000 PRs per project,
filters them, and accumulates all to finally display them sorted by creation
date.

## Why Use This Script?

Many teams need a quick way to audit or analyze PRs across multiple projects in
Azure DevOps. The built-in UI doesn’t provide an easy method to view all PRs
authored by a specific user across the entire organization. This script:
 
- Retrieves all projects in your organization.
- Pulls up to 1000 PRs per project using the Azure DevOps REST API.
- Filters PRs created in the last year by checking the `createdBy.displayName`
  field.
- Adds the project name to the PR object for easier reporting.
- Outputs the results in a neatly formatted table.

## Prerequisites

- **Azure DevOps Personal Access Token (PAT):** You need a PAT with at least
  _Code (read)_ permissions. Generate one from your Azure DevOps account
  settings.
- **PowerShell Environment:** This script uses basic PowerShell commands
  available on modern Windows systems.
- **Basic Understanding of REST APIs:** Familiarity with REST API concepts can
  help in customizing the script further.

## The Script

Below is the complete PowerShell script. Update the variable values (e.g.,
organization name, PAT, author initials, full name) to match your environment.

```powershell
# Define variables
$organization = "your-org-name"              # Replace with your Azure DevOps organization name
$personalAccessToken = "your-pat-token"      # Replace with your Azure DevOps PAT
$authorInitials = "your-initials"            # Substring to match in createdBy.displayName
$fullName = "your-name"                      # Full name to match in createdBy.displayName
$lastYear = (Get-Date).AddYears(-1).ToString("yyyy-MM-dd")
$top = 1000                                  # Count to get per project (hard AZDO limit)

# Encode PAT for authentication
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$personalAccessToken"))

# Get all projects in the organization
$projectsUrl = "https://dev.azure.com/$organization/_apis/projects?api-version=7.1-preview.4"
$projects = Invoke-RestMethod -Uri $projectsUrl -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method Get

$allPullRequests = @()

foreach ($project in $projects.value) {
    $projectName = $project.name
    $prUrl = "https://dev.azure.com/$organization/$projectName/_apis/git/pullrequests?searchCriteria.status=all&`$top=$top&api-version=7.1-preview.1"
    $prs = Invoke-RestMethod -Uri $prUrl -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method Get

    $filteredPRs = $prs.value | Where-Object {
        $_.creationDate -ge $lastYear -and ( $_.createdBy.displayName -match $authorInitials -or $_.createdBy.displayName -match $fullName )
    } | ForEach-Object {
        # Manually add the project name (since it's not part of the PR object)
        $_ | Add-Member -NotePropertyName "Project" -NotePropertyValue $projectName -PassThru
    }
    $allPullRequests += $filteredPRs
}

# Sort the results by creationDate and output desired columns
$allPullRequests |
    Sort-Object creationDate |
    Select-Object Project, @{Name="Repository"; Expression={ $_.repository.name }}, pullRequestId, title, creationDate, status |
    Format-Table -AutoSize
```

I have verified this script works for my needs, but use at your own peril.
Output may look something like.

```
Project     Repository pullRequestId title           creationDate    status
-------     ---------- ------------- -----           ------------    ------
ProjectName       repo         10001 PR title 2024-01-01T11:00:00 completed
ProjectName       repo         10002 PR title 2024-01-02T12:00:00 completed
ProjectName       repo         10003 PR title 2024-01-03T13:00:00 completed
ProjectName       repo         10004 PR title 2024-01-04T14:00:00 completed
```

That's all!
