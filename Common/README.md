# Common

This folder contains a C#-class-library that is used by all projects

The class-library (assembly) provides:

- A class for reading a custom configuration.file ([config.json]) defined in 
  [CustomConfigProvider.cs].
- A class for executing scripts (shell-scripts or batch-scritps) in a 
  platform-independet way on the local computer [LocalScriptRunner.cs].
- A class that asks via POST-requests to the ScriptingRestApiServer running
  on the remote computer to exeute scripts on the remote computer
  [RemoteScriptRunner.cs]. 
- A static class [Services/HostAddressProvider.cs] for getting get the 
  hostname and the IPv4-Address of the local computer
- A static class containing all magic strings for all projects as 
  readonly members [MagicStrings.cs].
- The Exception `WDException` used by all WirelessDisplay-projects. 

The classes `LocalScriptRunner` and `RemoteScriptRunner` expose their
functionalities vie the interfaces `ILocalScriptRunner` and 
`IRemoteScriptRunner`.

The folder [JsonObjects] contain simple classes that can be serialized to
Json-strings. Also Json-strings can be deserialized to these objects. These
Json-objects are used as data and as responses for the POST-requests to the
ScriptingRestApiServer.

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
