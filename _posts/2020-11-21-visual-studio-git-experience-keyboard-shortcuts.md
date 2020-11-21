---
layout: post
title: Visual Studio Git Experience Keyboard Shortcuts
---

One thing I did not cover in [Digest - .NET Conf 2020]({{ site.baseurl }}/2020/11/21/dotnet-conf-2020/)
was the release of the Git Experience, that previously was in preview in Visual Studio, as
discussed in [Announcing the Release of the Git Experience in Visual Studio](https://devblogs.microsoft.com/visualstudio/announcing-the-release-of-the-git-experience-in-visual-studio/).
The reason for this is I wanted to cover how I setup keyboard shortcuts for
this in Visual Studio. 

By default there aren't that many keyboard shortcuts
defined, so in this post I will cover how you can re-purpose
`Ctrl + G` for git commands instead of the rarely used `Edit.GoTo` (that is
goto line) that I almost never use. Don't worry we will still define a shortcut
for it if you like to keep it.

Keyboard shortcuts can be found under 
**TOOLS** -> **Options** -> **Environment** -> **Keyboard** or 
just by going to search with `Ctrl + Q` and searching for `keyboard shortcuts` and
opening this as shown below.

![INSERT Ctrl+Q]()

This will open up the options dialog for the keyboard mapping. Enter `git.` under
**Show commands containing:** and this will show almost all the git commands available.
And there are a lot as partially shown below! Which is great.

![INSERT KEYBOARD OPTIONS]()

As an example of command lets take the `Team.Git.GoToGitChanges` command which brings up or 
takes focus to the **Git Changes** view, which can also be found under **VIEW** in the top menu
as shown below.

![INSERT VIEW GIT CHANGES]()

As shown above and below this is already mapped to `Ctrl + 0, Ctrl + G`, but for fun
and ease of use we are gonna add another shortcut under the `Ctrl + G` prefix. 
