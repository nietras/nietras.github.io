---
layout: post
title: Setting Up Renovate as Azure DevOps Pipeline for Automatic .NET NuGet Package Updates
---

[Renovate](https://docs.renovatebot.com/) is a tool/bot that can automatically
update dependencies in a repository similar to GitHub
[dependabot](https://github.com/dependabot). Renovate is [open
source](https://github.com/renovatebot/renovate) and written in TypeScript. It
supports a wide range of platforms and package managers. Here I will show how to
set up Renovate as a single central Azure DevOps pipeline to automatically
update NuGet packages (and .NET SDK) in multiple git repositories across
different projects in Azure DevOps.

Renovate has many different settings and deployment options. That is, you can
run Renovate as:

* an [Open Source npm package](https://www.npmjs.com/package/renovate)
* a [pre-built Open Source image on Docker Hub](https://hub.docker.com/r/renovate/renovate)
* [Mend Renovate App](https://github.com/marketplace/renovate) which is hosted
  by [Mend](https://www.mend.io/) (the company behind Renovate).

This makes working with and setting up Renovate a bit confusing, since
documentation will be different for either of these options. Here I will use the
open source npm package since that fits the needs we have where I work.

In principle the guide [Azure DevOps and Azure DevOps
Server](https://docs.renovatebot.com/modules/platform/azure/) should cover this,
but this wasn't the case for our needs and it also appears to include
unnecessary steps like creating a Personal Access Token (PAT). Something we want
to avoid since this would require manual steps to keep that active or updating
it.

## Setting Up Renovate
First, a few definitions. Replace these with your own.

* `<ORG>` - used as a placeholder for your organization in Azure DevOps e.g.
  `https://<ORG>.visualstudio.com`
* `<PRJ>` - used as a placeholder for a project in Azure DevOps e.g.
  `https://<ORG>.visualstudio.com/<PRJ>`
* `<EMAIL>` - used as a placeholder for a build runner users email address for
  your organization e.g. `buildrunner@<ORG>.com`
* `<USERNAME>` - used as a placeholder for a build runner user name for your
  organization e.g. `buildrunner`

In our case, we have the central Renovate pipeline and configuration in a git
repository e.g. `https://<ORG>.visualstudio.com/Development/_git/renovate`. That
is, a git repository called `renovate` in the project called `Development`,
which is a central project for all development related things. This repository
contains the following files:

```
azure-pipelines-renovate.yml
config.js
renovate.json
```

`azure-pipelines-renovate.yml` is shown below. This is the Azure DevOps pipeline
and it is scheduled to run every weekday every 5 minutes between 7-14 UTC time,
which is 9-16 CET, using [cron
syntax](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/scheduled-triggers?view=azure-devops&tabs=yaml#cron-syntax).
Yes we have a lot of package updates since we use them for our machine learning
workflow, but you can use whatever you like here. However, importantly there are
[limits to how often a given
schedule](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/scheduled-triggers?view=azure-devops&tabs=yaml)
can run e.g. per week. While documentation says 1000 runs per pipeline per week
and 10 runs per pipeline per 15 minutes, it appears there also is an
(undocumented) limit to how often a given schedule can run per week (100?). This
is why there is a schedule per week day.

NOTE: Before running this pipeline and onboarding new git repositories you have
to change several [Azure DevOps Settings](#azure-devops-settings), which is
detailed later.

```yml
schedules:
  
  # mm HH DD MM DW
  # Time zone UTC, DW = days of week, 0 = sunday
  - cron: '*/5 7-14 * * 1'
    displayName: 'Monday'
    branches:
      include: [main]
    always: true
  - cron: '*/5 7-14 * * 2'
    displayName: 'Tuesday'
    branches:
      include: [main]
    always: true
  - cron: '*/5 7-14 * * 3'
    displayName: 'Wednesday'
    branches:
      include: [main]
    always: true
  - cron: '*/5 7-14 * * 4'
    displayName: 'Thursday'
    branches:
      include: [main]
    always: true
  - cron: '*/5 7-14 * * 5'
    displayName: 'Friday'
    branches:
      include: [main]
    always: true

trigger: none

pool:
  name: Default

steps:
  - task: NuGetToolInstaller@1
    displayName: 'Use NuGet 6.8'
    inputs:
      versionSpec: 6.8

  - task: NuGetAuthenticate@1
    displayName: 'NuGet Authenticate'
    inputs:
      forceReinstallCredentialProvider: true

  - task: UseNode@1
    inputs:
      version: '20.x'

  - powershell: |
      Write-Host $(System.CollectionUri)
      git config --global user.email '<EMAIL>'
      git config --global user.name '<USERNAME> (Renovate Bot)'
      npx renovate@37.412.2
    env:
      RENOVATE_PLATFORM: azure
      RENOVATE_ENDPOINT: $(System.CollectionUri)
      RENOVATE_TOKEN: $(System.AccessToken)
      RENOVATE_BASE_DIR: $(Agent.TempDirectory)
      VSS_NUGET_ACCESSTOKEN: $(VSS_NUGET_ACCESSTOKEN)
      LOG_LEVEL: debug
```


`config.js` is shown below. This contains our centrally defined configuration
for Renovate, which includes enabling nuget package management and notable also
includes a group for .NET SDK and runtime packages. It also customizes commit
and pull request (PR) details so the PR for an automated package update is as
succinct as possible. That is, no PR description and title similar to `Bump
Autofac to 7.1.0`. It also removes the hourly PR limit, which otherwise is 2 per
hour. See [Configuration
Options](https://docs.renovatebot.com/configuration-options/) for more details.

```js
module.exports = {
    hostRules: [
        {
            hostType: 'nuget',
            matchHost: 'https://<ORG>.pkgs.visualstudio.com/',
            username: '',
            password: process.env.RENOVATE_TOKEN
        }
    ],
    extends: [
        "config:best-practices"
    ],
    repositories: [
        '<PRJA>/<GITREPONAME1>',
        '<PRJB>/<GITREPONAME2>'
    ],
    enabledManagers: [
        'nuget'
    ],
    packageRules: [
        {
            "matchManagers": ["nuget"],
            "commitMessageAction": "Bump",
            "commitMessageTopic": "{{depName}}",
            "prBodyTemplate": "",
        },
        {
            groupName: 'dotnet-sdk',
            description: 'Enable non-major updates for .NET SDK and runtime (global.json)',
            matchPackageNames: [
            'dotnet-sdk',
            'mcr.microsoft.com/dotnet/sdk',
            'mcr.microsoft.com/dotnet/aspnet',
            'mcr.microsoft.com/dotnet/runtime',
            'mcr.microsoft.com/dotnet/runtime-deps'
            ],
            extends: [
            ':disableMajorUpdates',
            ':pinDigestsDisabled'
            ],
        }
    ],
    gitAuthor: "<USERNAME> (Renovate Bot) <<EMAIL>>",
    prHourlyLimit: 0,
};
```

Note that you have to list the repositories you want Renovate to run on in this
file. There are other ways for this, but for our needs this manual listing is
fine.

First time Renovate runs on a new git repository, an onboarding PR called
"Configure Renovate" is created (with a very long description), an example is
given later. Read the description and note that one can commit to the related
branch and customize the `renovate.json` file for the given repository if need
be. The default `renovate.json` file is shown below and is currently empty as
centralized configuration is preferred.

```json
{
  "$schema": "https://docs.renovatebot.com/renovate-schema.json"
}
```

However, for a given git repository you may want to customize Renovate to for
example group certain packages so a single PR is created for updating those
packages or disable automatic updates of certain packages. Or even assign people
to specific updates. For example,
`https://<ORG>.visualstudio.com/<PRJ>/_git/app?path=/renovate.json` could
contain:

```json
{
  "$schema": "https://docs.renovatebot.com/renovate-schema.json",
  "packageRules": [
    {
      "matchPackagePatterns": [
        "<ORG>.<PRJ>.Model.*.onnx"
      ],
      "assignees": [
        "<EMAIL>"
      ]
    },
    {
      "matchPackagePatterns": "OpenCvSharp*",
      "groupName": "OpenCvSharp*"
    },
    {
      "matchPackagePatterns": "Microsoft.Build*",
      "groupName": "Microsoft.Build.*"
    },
    {
      "matchPackagePatterns": "Caliburn.Micro*",
      "groupName": "Caliburn.Micro.*",
      "enabled": false
    }
  ]
}
```

## Azure DevOps Settings

Since Renovate runs as one central pipeline across many projects and repos, one
has to ensure the pipeline has access to all the repos it needs to update.

### Disable "Limit job authorization scope to current project for release/non-release pipelines"

Go to settings for the Azure DevOps project which contains the renovate
repository e.g. `https://<ORG>.visualstudio.com/Development/_settings/settings`.

Turn off "Limit job authorization scope to current project for non-release pipelines"

Turn off "Limit job authorization scope to current project for release pipelines"

It seems necessary to turn off both of the above, however, it may be possible
just the first is necessary, as maybe security changes take some time to
propagate.

![Azdo Settings Disable Limit Job To Current
Project]({{ site.baseurl
}}/images/2024-07-renovate-azure-devops/azdo-settings-disable-limit-job-to-current-project.png)

### Give the "Project Collection Build Service (`<ORG>`)" Access for Repository

For a given repository (e.g. `<PRJ>/app`) the build service user needs to have
accesss to create branches and pull requests for the package updates and
similar.

Go to the project settings for the project containing the repository e.g.
`https://<ORG>.visualstudio.com/<PRJ>/_settings/repositories`

Select the repository e.g. `app`

Select the **Security** tab

Select **`Project Collection Build Service (<ORG>)`** as user (if not listed
search for it).

Set **Contribute to pull requests** to **Allow** if not already inherited.

Set **Create branches** to **Allow** if not already inherited.

NOTE: If the renovate pipeline fails with a lack of permissions error for some
user, but the user id is a GUID then try searching for that to find the specific
user to change permissions for. E.g. `identity
'Build\\xxxxxxxx-yyyy-zzzz-wwww-vvvvvvvvvv'`.

![Azdo Repositories Security Allow Contribute Etc]({{ site.baseurl
}}/images/2024-07-renovate-azure-devops/azdo-repositories-security-allow-contribute-etc.png)

## Onboarding Pull Request
After all this, when the Renovate pipeline first runs on a new git repository an
onboarding pull request similar to below will be created.

![Onboarding Pull Request]({{ site.baseurl
}}/images/2024-07-renovate-azure-devops/onboarding-pr.png)

And if this is merged soon after that pull requests for package updates will
occur. If you get emails about this it can be a bit overwhelming, so consider
setting up an email rule or filter for them. 😅

## Links

[Automate your .NET SDK updates for consistent and reproducible builds
with global.json and
Renovate](https://anthonysimmon.com/automate-dotnet-sdk-updates-global-json-renovate/)

[Locally test and validate your Renovate configuration
files](https://anthonysimmon.com/locally-test-validate-renovate-config-files/)

[Using Renovate Bot in Azure DevOps](https://blog.ostebaronen.dk/2024/01/renovate.html)

[Cool Renovate Bot Features](https://blog.ostebaronen.dk/2024/02/more-renovate.html)

[Rerun Onboarding PR](https://github.com/renovatebot/config-help/issues/54)

[Cannot rename abandoned PR to trigger replacement PR? (AzureDevOps
Server)](https://github.com/renovatebot/renovate/discussions/10456)

That's all!
