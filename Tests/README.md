# Tests

Each subfolder here contains a Test-suite for different parts of the whole
project. Since most development was done on a Linux-machine, most tests
only work on a Linux-computer.

For the following test-projects, just start a terminal in the test-project-
folder, and type `dotnet test`. This searches all tests decribed in the 
project, runs that tests, and reports how many tests have passed. All tests
should pass. For debugging tests in VSCode, support for `dotnet run` was 
also added. The overall launch.json contains launch-configurations for all
Tests. The folder-names describe, what is tested:

- [Common.CustomConfigProvider_Test]
- [Common.LocalScriptRunner_Test]
- [Common.RemoteScriptRunner_Test]
- [WirelessDisplayClient.WDClientServices_Test]

The latter two tests start a server-program and a client-program on the
same Linux-computer. 

The folder [ScriptingRestApiServer_Tests] contains the shell-script
[testScriptingRestApiServer.sh], that can be run from a terminal. Watch the
output of this script, to see, if all tests have passed.

## Technical Details

In the remainder of this document is shown, how to create a unit-test, and run 
it with `dotnet test` as well as with Visual-Studio-Code-Debug.

As example the [Common]-classlib-project and its class `CustomConfigProvider` 
is used.

See [Unit testing C# with NUnit and .NET Core](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit).

# Set-up the test

The test was created with the following steps:

- Create a folder [WirelssDisplay/Tests/], where subfolders for each test-group
  will go.

- Created the subfolder [WirelssDisplay/Tests/Common.CustomConfigProvider_Test].

- Open Terminal in this folder and run `dotnet new nunit`. This
  creates the project-config-file [CustomConfigProvider_Test.csproj] and a
  file [UnitTest1.cs].

- Add a project-reference to the project, where the class-under-test is:

```
dotnet add reference ../../Common/Common.csproj
```

- Rename [UnitTest1.cs] to [Test.cs], and change namespace,
  using directives, and the name of the method `public void Test1()`. Later 
  other Test-methods are created.

The Attribute [TestFixture] tells the Nunit-test-framework, that a class 
contains some tests. The Attribute [Test] for test-methods marks a function
as a one of these tests. The attribute [Setup] marks a function that is called
before each test, and the attribute [TearDown] marks a function that is called
after each test.

A [Test]-method normally makes some assertions (for example by calling
`Assert.True(...)` or `Assert.Throws(...)` ). If an assertion fails, an
exception is thrown, and the Nunit-test-framework considers the test as having
failed.

For naming-conventions of [Test]-methods see:
[7 Popular Unit Test Naming Conventions](https://dzone.com/articles/7-popular-unit-test-naming).

- Add needed package-references for the Test-code:

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

- Added `public static void Main()`-function to [Test.cs]. This main-Method
  also calls all [Setup]-, [TearDown]- and [Test]-functions in the correct
  order (Note, that [Setup]- and [TearDown]-methods are not called before and 
  after each test - this is what `dotnet test` does - but only once for all
  tests).

- In [launch.json] add a new configuration for the project. 
Here append 'bin/Debug/netcoreapp3.1' to the path from "cwd". So the
working directory for debugging is the same as for `dotnet test`.

If the program is no run from the VSCode-debugger, VScode complains about
more than one main-function. Fix this by:

- Change the program entry-point: In the project-file 
  [Common.CustomConfigProvider_Test.csproj] add the following line in the
  element `<PropertyGroup>`:

```
<StartupObject>WirelessDisplay.Tests.Common.CustomConfigProvider_Test.Test</StartupObject>
```

# VScode tasks and launch-configurations.

This section is not only valid for tests, but also describes, how to 
set up a VScode workspace, that contains more than one project. The good
this about VSCode is, that every configuration is visible in a json-file.

In the folder [WirelessDisplay/.vscode] are the files [launch.json] and 
[tasks.json].

## tasks.json

The classic "build"-task is renamed to "build_CustomConfigProvider_Test":
```
"label": "build_CustomConfigProvider_Test",
```

# launch.json

In [launch.json] the name of the debug-configuration is changed from:
`"name": ".NET Core Launch (console)"` to `"name": "CustomConfigProvider_Test"`.

Later other debug-configurations are added to [launch.json] with the 
"Add Configuration"-Button, or just by copy and paste.

Since the task "build" has been renamed (in [tasks.json]), also the 
pre-launch-task in [launch.json] must be renamed:

```
"preLaunchTask": "build_CustomConfigProvider_Test"
```

The "cwd" is also changed  to get the same behaviour as the 
`dotnet test`-command (see above).

