# Overwiew

In this document is shown, how to create a unit-test, and run it
with `dotnet test` as well as with Visual-Studio-Code-Debug.

As example the [Common]-classlib-project and its class `CustomConfigProvider` 
is used.

See [Unit testing C# with NUnit and .NET Core](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit).

# Set-up the test


Create a folder [WirelssDisplay/Tests/], where subfolders for each test-group.

Created the subfolder [WirelssDisplay/Tests/Common.CustomConfigProvider_Test].

Open Terminal in this folder and run `dotnet new nunit`. This
creates the project-config-file [CustomConfigProvider_Test.csproj] and a
file [UnitTest1.cs].

Add a project-reference to the project, where the class-under-test is.

```
dotnet add reference ../../Common/Common.csproj
```

Rename [UnitTest1.cs] to [CustomConfigProvider_Test.cs], and change namespace,
using directives and the name of the method `public void Test1()`. Later other
Test-methods are created.

The Attibutes [TestFixture], [Test] for the class and each test-method
is necessary for the NUnit-framework and for dotnet run.

For naming-conventions of these methods see:
[7 Popular Unit Test Naming Conventions](https://dzone.com/articles/7-popular-unit-test-naming).

Add needed package-references for the Test-code:

```
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
```

Now `dotnet test` already works. 

## Realtive file-paths.

If the tests are run with `dotnet test`, then all the current working directory
is [Common.CustomConfigProvider_Test/bin/Debug/netcoreapp3.1]. So a '../../../'
has to be appended to all relative file-paths, that otherwise would be realtive 
to [Common.CustomConfigProvider_Test].

# Add support for debugging

Added `public static void Main()`-function to [Test.cs].

In [launch.json] add a new configuration for the project. 
Here append 'bin/Debug/netcoreapp3.1' to tghe path from "cwd". So the
working directory for debugging is the same as for `dotnet test`.

Change the program entry-point: In the project-file 
[Common.CustomConfigProvider_Test.csproj] add the following line in the
element `<PropertyGroup>`:

```
<StartupObject>WirelessDisplay.Tests.Common.CustomConfigProvider_Test.Test</StartupObject>
```

# VScode tasks and launch-configurations.

In the folder [WirelessDisplay/.vscode] are the files [launch.json] and [tasks.json].

## tasks.json

The classic "build"-taks is renamed to "build_CustomConfigProvider_Test":
```
"label": "build_CustomConfigProvider_Test",
```

# launch.json

In [launch.json] the name of the debug-configuration is changed from:
`"name": ".NET Core Launch (console)"` to `"name": "CustomConfigProvider_Test"`.

Later other debug-configurations are added to [launch.json] with the 
"Add Configuraiton"-Button, or just by copy and paste.

Since the task "build" has been renamed (in [tasks.json]), also the 
pre-launch-task in [launch.json] must be renamed:

```
"preLaunchTask": "build_CustomConfigProvider_Test"
```

The "cwd" is also changed  to get the same behaviour as the 
`dotnet test`-command (see above).