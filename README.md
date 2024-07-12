# CheckedRepo

## Install

First add the package source
```pwsh
dotnet nuget add source "https://nuget.pkg.github.com/StarlyCode/index.json" --name Starly --username "%STARLYCODE_GITHUB_PACKAGES_USER%" --password "%STARLYCODE_GITHUB_PACKAGES_PASSWORD%" --store-password-in-clear-text
```

Then install as a dotnet tool

```pwsh
dotnet tool install -g StarlyCode.CheckedRepo
```
