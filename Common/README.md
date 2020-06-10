# Common

This folder contains a C#-class-library that is used by all projects

The class-library (assembly) provides:

- A class for reading a custom configuration (json-file).
- A class for executing scripts (shell-scripts or batch-scritps) in a 
  platform-independet way on the local computer.
- A class that asks via POST-requests the remote computer to exeute 
  scripts. 
- A static class containing all magic strings for every project.

## Project creation

The project was created with

```
mkdir Common
cd Common
dotnet new classlib --framework netcoreapp3.1

```

The contents of the file [Common/.gitignore] were created using the website
[gitignore.io](https://www.toptal.com/developers/gitignore)
specifying the templates Csharp, VisualStudio and VisualStudioCode

The other projects, that uses the classes defined in [Common] have a reference
added to this assembly in their .csproj-files.
