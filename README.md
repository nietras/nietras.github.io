# nietras.github.io
nietras blog

## Local development on Windows

Install Ruby 3.3 with MSYS2/DevKit using WinGet:

```powershell
winget install --exact --id RubyInstallerTeam.RubyWithDevKit.3.3
```

Restart Visual Studio or the terminal so the updated `PATH` is available, then
install the Ruby development toolchain and the blog dependencies:

```powershell
ruby --version
ridk install
gem install bundler
Remove-Item Gemfile.lock -ErrorAction SilentlyContinue
bundle install
```

When prompted by `ridk install`, select the default recommended MSYS2
development-toolchain components.

`Gemfile.lock` is intentionally ignored. Removing a stale local copy prevents
Bundler from selecting the obsolete Bundler 2.1.4, `github-pages` 204, and `wdm`
0.1.1 dependency set.

Run the site locally:

```powershell
bundle exec jekyll serve
```

Alternatively, use the included script to also preview future-dated posts:

```powershell
./serve.ps1
```

Open <http://localhost:4000> in a browser.
