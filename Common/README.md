# Common

This folder contains a C#-class-library that is used by all projects

The class-library (assembly) provides:

- A class for reading a custom configuration (json-file) 
  [CustomConfigProvider.cs].
- A class for executing scripts (shell-scripts or batch-scritps) in a 
  platform-independet way on the local computer [LocalScriptRunner.cs].
- A class that asks via POST-requests the remote computer to exeute 
  scripts [RemoteScxriptRunner.cs]. 
- A static class containing all magic strings for all projects as 
  readonly members [MagicStrings.cs].

The classes `LocalScriptRunner` and `RemoteScxriptRunner` expose their
functionalities vie the interfaces `ILocalScriptRunner` and 
`IRemoteScxriptRunner`.

## Project creation

The project was created with

```
mkdir Common
cd Common
dotnet new classlib --framework netcoreapp3.1
dotnet add package Mircosoft.Extensions.Logging
```

The contents of the file [Common/.gitignore] were created using the website
[gitignore.io](https://www.toptal.com/developers/gitignore)
specifying the templates Csharp, VisualStudio and VisualStudioCode

The other projects, that uses the classes defined in [Common] have a reference
added to this assembly in their .csproj-files.
