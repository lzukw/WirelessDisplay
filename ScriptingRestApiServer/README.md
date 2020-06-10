# ScriptingRestApiServer

## Overview 

This project provides the webserver, that offers a REST-API for the client
('presentation-computer'). It is the heart of the WirelessDisplay-solution. 
Clients can run, start and stop scripts on the server-computer 
('peojecting-computer').

The scripts typically are shell-scripts executed with bash on Linux and 
batch-files executed with cmd.exe on Windows.

Note that the ScriptingRestApiServer-project could be used for every kind of
remote-controlling a Linux-, Windows- or macOS-computer.

## Implementation

ScriptingRestApiServer is an ASP.NET-core webapi-program.

Depending on the operating system, the WirelessDisplayServer is running on, 
the scripts reside in one of the subfolders 

- [../Scripts/Linux]
- [../Scripts/maxOS]
- [../Scripts/Windows]

The script-location, as well as the shell for executing the scripts, for 
example bash, cmd.exe, but also powershell or python, can be configured by 
modifying the file [config.json]. Also the location of the scripts could be 
configured. In most cases there is no need to change the default-configuration 
(A python-program can also be started from within a bash-script or a 
Windows-batch-file).

See the [README.md](../Scripts/README.md) 

This approach is quite generic, so every kind of script could be executed
on the server-computer by the client-computer. For the WirelessDisplay-project
the scripts

- query and modify the screen-resolution of the 'projecting-computer',
- prevent the screensaver from activating, and
- start and stop a streaming-sink on the 'projecting-computer'.

But, as already said, everything that can be done by executing a script, 
can be done. The client-computer uses the REST-API as a sort of remote-
control for the server-computer.

The scripts do not do the work by themselves, but they start external programs.
Some of these external programs have to be made available in the Folder 
[ThirdParty]. See the [README.md](../ThirdParty/README.md) in this folder.

## Allowed REST-API-calls

The allowed API-calls to ScriptingRestApiServer are given below. In the 
examples below, the IP-address and port-number of the 'projecting-computer'
(ScriptingRestApiServer) is 192.168.1.119:6000 . For each API-call a 
curl-command is given as example. If port 80 is used, the port-number can
be omitted. On many systems administrator-privilegues are necessary
for a program to listen on port 80.

All requests are POST-requests, and the posted data is a json-Object. The
reponse of each request is also a json-Object.

Two different type of scripts can be started:

- A script, which returns very fast (for example after changing the
  screen-resolution) is "run". 
- A script, which starts a background-service does not return. It can
  later be stopped by another call to the REST-API. Such a script is
  "started" and "stopped".

### POST: api/ScriptRunner/RunScript

Example:

```
curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ScriptName" : "testscript_for_run", "ScriptArgs" : "arg1 arg2 arg3 arg4", "StdIn" : "\n" }' http://192.168.1.119:6000/api/ScriptRunner/RunScript
```

In the example above a script `testscript_for_run.sh` or 
`testscript_for_run.bat` is run with bash or cmd.exe. The command-line-arguments
agr1 arg2 arg3 and arg4 are provided, when the script is called. The string "\n"
is handed over to the stdin-stream of the shell-script. 

The posted data is

```
{
    "ScriptName" : "<scriptname-without-extension>",
    "ScriptArgs" : "<space-separated list of command-line-arguments>",
    "StdIn"      : "<String passed to standard-input of the shell-script>",
}
```

The value of `"ScriptName"` is the name of the script WITHOUT the file-extension 
(.sh or .bat) of the script. The file-extension depends on the 
operating-system of the server, and will be appended before it is executed.
This file-extension could also be configured in [config.json]. The scripts
are searched in a subdirectory of [WirelssDisplay/Scripts], for example on a 
Linux-machine in [WirelssDisplay/Scripts/Linux]. This directory could
also be configured in [config.json]. It should be possible to provide
a relative path and the script-name without file-extension as value for
`"ScriptName"`, if the script is in another directory, or further subdirectory.

The value of `"ScriptArgs"` is a space- or tab-separated string with
the command-line-arguments of the script (as if they were given on a
command-line when directly executing the script).

The value of `"StdIn"` is a string that is passed to the stdin-stream
of the script. If the script does not read anything from stdin, you can
provide an empty string (`""`). If the script reads more than one line,
you can separate the lines with `'\n'` in the value of `"ScriptArgs"`.

Please note. that the request does not return until the script terminates.
After the script completes, the POST-request returns a Json-object, that looks 
for example like this:

```
{
    "success" : true,
    "errorMessage" : "", 
    "scriptExitCode" : 5,
    "stdoutLines" : ["arg1","arg2",""],
    "stderrLines": ["arg3","arg4"]
}
```

The value of `"success"` is `true`, if the script could be run. In this case
the value of `"errorMessage"` is an empty string. If the script could not be 
run, `"success"` is `false` and `"errorMessage"` conains a description of
the occurred error.

The value of `"scriptExitCode"` is the exit-code of the script (In most cases
0 indicates success, and another value an error-condition while executing
the script). Note that a non-zero exit-code does not mean, that the value of 
`"success"` is `false`, because the script could be successfully started.

The values of `"stdoutLines"` and `"stderrLines"` are a list of strings. They
contain the lines, the script wrote to its standard-output- and 
standard-error-stream.

### POST: api/ScriptRunner/StartScript

Example:

```
curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ScriptName" : "testscript_for_start_stop", "ScriptArgs" : "arg1 arg2 arg3 arg4", "StdIn" : "\n" }' http://192.168.1.119:6000/api/ScriptRunner/StartScript
```

This Request starts a script, but the request returns a response, while the
script continues to run in background.

The Json-object that has to be posted, is the same as in the former
POST to api/ScriptRunner/RunScript, but it returns a different respons.
The response looks for example like:

```
{
    "success" : true,
    "errorMessage" : "",
    "processId" : 4281
}
```

The values of `"success"` and `"errorMessage`" have the same meaning as before,
and the value `"processId"` is the process-ID of the shell that is executing
the script. This number has to be used as part of the posted data in the
following two requests.

### POST: api/ScriptRunner/IsScriptRunning

Example:

```
url -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ProcessId" : 4281 }' http://192.168.1.119:6000/api/ScriptRunner/IsScriptRunning
```

This request returns true or false, if the started script with the given 
process-ID is still running. The data is a Json-object, containing just
one integer with the process-ID returned by the POST to 
api/ScriptRunner/StartScript:

```
{
    "ProcessId" : 4281
}
```

The response could look for example like:

```
{
    "success" : true,
    "errorMessage" : "",
    "isRunning" : true
}
```

The values of `"success"` and "`errorMessage`" are as usual, the value of
"`isRunning`" contains the desired information.

### POST: api/ScriptRunner/StopScript

Example:

```
curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ProcessId" : 4281 }' http://192.168.1.119:6000/api/ScriptRunner/StopScript
```

This request stops the script with the given process-ID. If the script is
still running, the shell-process executing the script and its child-processes
(the programs started from the script) are terminated. The exit-code and
the lines written to stdout and stderr by the script are returned.

A response could look like:

```
{
    "success" : true,
    "errorMessage" : "",
    "scriptExitCode" : 137,
    "stdoutLines" : ["arg1","arg2",""],
    "stderrLines" : ["arg3","arg4"]
}
```

The data to POST is the same as for the POST-Request to 
api/ScriptRunner/IsScriptRunning. The returned json-object is the same as for
the response to a POST-request to api/ScriptRunner/RunScript.

Note that, if the process is terminated, the exit-code could be the exit-code
of one of the "killed" child-processes, and not the exit-code of the script.

## Running and Configuration

The source-code of this program (ScriptingRestApiServer) is in the folder 
[WirelessDisplay/ScriptingRestApiServer]. From within this directory the 
program can be started using dotnet run. 

Some scripts started via the REST-API look for further executable programs
in the folder [WirelessDisplay/ThirdParty]. 

On Windows the following third-party programs are used:

- tightvnc-1.3.10_x86.zip for a VNC-viewer,
- ffmpeg-4.2.2-win64-static.zip as an alternative streaming-sink.
- screenres.exe from the folder ScreenRes for managing screen-resolution.
- TODO Program for preventing the screen-saver from activating.

The scripts in [Scripts/Windows] look for the executables in

- [..\..\Third_Party\tightvnc-1.3.10_x86\vncviewer.exe]
- [..\..\Third_Party\ffmpeg-4.2.2-win64-static\bin\ffplay.exe]
- [..\..\Third_Party\ScreenRes\screenres.exe]

Downlaod the zip-Files 
[tightvnc-1.3.10_x86_viewer.zip](https://www.tightvnc.com/download/1.3.10/tightvnc-1.3.10_x86.zip) 
and 
[ffmpeg-4.2.2-win64-static.zip](https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-4.2.2-win64-static.zip) 
and extract them into the [ThirdParty]-folder. Clone the 
[ScreenRes-Repository](https://github.com/lzukw/ScreenRes) 
or download it as zip and also put it into the [ThirdParty]-folder.

Double-check the correct paths. If this program can't find the executables in 
the specified paths, the scripts starting these executable fail, and so 
will the client trying to remote-control the server.

## Installing

An executable version can be created with the following command (from within 
the folder, where Program.cs and Startup.cs are):

```
dotnet publish -c Release -o ..\ScriptingRestApiServer_executable -r win-x64 --self-contained true
```

On Linux, replace `win-x64` with `linux-x64` and on macOS with `osx-x64`.

If dotnet core 3.1 is installed on the target-computer, set the 
parameter `--self-contained` to `false`, which saves a lot of disk-space.
See [here](https://docs.microsoft.com/en-us/dotnet/core/deploying/) for 
further details about publishing, "self-contained" vs. "runtime-dependet" 
deployment and platform-dependent/independent deployment.

# Technical details - the ASP.NET-core-application

This folder contains the C# ASP.NET application providing the webserver serving
responses to REST-API-requests.

# How does this all work?

For people who do not know ASP.NET this document describes how this program 
(WirelessDisplayServer) works.

Personally, I found it difficult to learn ASP.NET using 
[Microsoft's documentation](https://docs.microsoft.com/en-us/aspnet/core).
In my opinion, a very good place to learn ASP.NET is
[Learn razor pages](https://www.learnrazorpages.com/).


## Project Setup

This project was created by the command `dotnet new webapi --no-https` (run in 
the folder [ScriptingRestApiServer], and then edited with Visual Studio Code. 
This command creates a program, that can be run and provides a web-api 
(REST-api) for weather-forecast-Items. All things, that have to do with 
these weather-forcasts are deleted from the project. The remaining relevant 
files are:

- [Program.cs]
- [Startup.cs]
- [appsettings.json]

The followin project-reference has been added, to be able to use interfaces
and classes defined in the folder [Common]:

```
dotnet add reference ../Common/Common.csproj
```

In [appsettings.json] the following line is added (The webserver listens
on all interfaces on HTTP-port 80). In [appsettings.development.json] a 
similar line was added, but the port there is 6000.

```
"urls" : "http://*:80"
```

New files are:

- [config.json]
- [Controllers/ScriptRunnerController.cs]

### The webserver run by ASP.NET

All the magic is done whithin "code behind the scenes" by ASP.NET. All this 
code is configured in the two files [Program.cs] and [Startup.cs]. The File 
[Program.cs] contains the entry-Point for the Program 
`public static void Main(string[] args)`. Here an Object, that implements the 
`IHost`-interface is created and started. This host is an object, which 
encapsulates the app's resources ( see 
[Generic host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1) 
 ). One of the services, that are started by this `IHost` is a web-service, 
that listens for HTTP-Requests on the network.

The file [Program.cs] has not been modified.

When The `IHost` is created, it uses the class `Startup` defined in 
[Startup.cs]. Here all configuration is done. The following features, which are
offered by the ASP.NET framework, are used, and some of these features are 
configured in [Startup.cs]:

- Configuration with appsettings.json and command-line-arguments
- Logging
- Dependency-Injection
- (Serving static files ...EDIT, this feature is not used anymore.)

Own configuration is done with the file [config.json], whose contents are 
read in [Startup.cs].

In the referenced project [../Common/Common.csproj] the most important things
used in this project are the interface `IScriptRunner` and the class 
implementing it, `ScriptRunner`. While the methods defined in 
[Controllers/ScriptRunnerController.cs] react to POST-requests, all the
work for running, starting, querying and stopping scripts is provided
by the `ScriptRunner`-class.

### Configuration with appsettings.json

Configuration-parameters that can be used in an ASP.NET-application can be provided
by the user in several ways. The most important way to provide configration-parameters
are:

- Command-line-arguments (which become the `string[] args` paremter of `Main()`)
- The file [appsettings.json]

The only configured things here are 

- logging, and
- the Interfaces and port-number the webserver listens on.

Normally all interfaces are used to listen, but the port-number in some
cases should be configurable. Either change the value of `"urls"` in
appsettings.json, or provide the commandline-argument

```
--urls:"http://*/5050"
```

when starting the ScriptingRestApiServer. The last number is the port-nubmer,
the asterisk stands for all interfaces to listen on.

The `Startup`-object defined in `Startup.cs` receives all the 
configuration-parameters provided by the different sources 
(command-line-arguments, appsettings.json) when it is instantiated. The 
ASP.NET-framework passes the configuration-parameters in form of an 
object implementing the `IConfiguration`-interface to the constructor of the
`Startup`-class. The constructor stores this opject in a proprty named 
`Configuration`.

Within the methods of the `Startup`-class the configuration-parameters could be 
read in a simple way. For example, to get the value of 'urls' (provided in 
appsettings.json or by a command-line-parameter --urls) the code in `Startup.cs` 
just reads the value of `Configuration["urls"]`.

This used methodology is a form of "dependecy injection", which is called
"constructor-injection". This design-pattern seems to be widely used in the 
ASP.NET-framework. It consists of the following steps:

- A class needs some data: For example the `Startup`-class needs 
  configuration-data.
- Configuration-data can be provided by different classes, but all these
  classes must implement the same interface. In our example the class of the 
  object providing the configuration-data is not known, but it implements the 
  `IConfiguration`-interface. This interface provides properties or methods to 
  read the configuration-data.
- The `Startup`-class has a parameterized constructor. This constructor receives
  a parameter of type `Iconfiguration`. When a `Startup`-object is instatiated
  it stores the received object implementing the `Iconfiguration`-interface in
  a local property. This property can later be used by methods inside the 
  `Startup`-class.
- The "code behind the scenes" in the ASP.NET-framework is responsible for
  instiating an object of the `Startup`-class. At this point, this "code behind
  the scenes" must also pass an object, which is storing the configuration-data
  and implements the `Iconfiguration`-Interface, to the constructor of the 
  `Startup`-class.

### Configuration with config.json

Here the following parameters can be changed:

- The command-name for the shell (for example bash or cmd.exe) for each of the
  three supported operating systems
- The template with the command-line-arguments passed to the shell. This 
  template must contain the placeholders `{SCRIPT}` and `{SCRIPT_ARGS}` which
  are replaced by the correct values when running/starting a script.
- The directory, where the scripts are
- The filename-extension of the scripts.

The contents of config.json are simply read as a 
`Dictionary<string,Dictionary<string,string>>` by own code in [Startup.cs].


### Logging

Loggers are dependency-injected into the constructor of each used class.

TODO: Explanation of injected Ilogger<T>. 

See [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1)

### Dependency-Injection, Controllers and HTTP-Requests 

All objects are very loosly coupled, which results in a very good 
code-structure. The method for the loose coupling is provided by a 
mechanism called "Dependecy-Injection (DI)". 

All references and all data an object needs to work are given as parameters 
to the constructor for this object. Here, the type of a reference-parameter for
the constructor is not the class of a needed object, but an interface that
is implemented by this class. The interface just exposes the needed 
features of the class, and not all the implementation details. The class could
implementing the interface and providing services could change, only 
the interface has to remain stable.

For example: The `ScriptRunner`-object needs an ILogger<ScriptRunner>-object
for logging, it needs a string containing the name of the shell-command, 
the filepath, where it can find the scripts to execute, and so on.
All this information and a reference to an object implementing
ILogger<ScriptRunner> are gathered in the Startup-class. Here an instance
(a singleton) of a `ScriptRunner` is instantiated and its constructor
receives all the necessary data/references.

The services, that the `ScriptRunner`-singleton provides are needed by
the `ScriptRunnerController`-class. Each time a POST-Request occurs, an
instance of this class is instantiated (by the "code behind the scenes" in the
ASP.NET-framework). The method corresponding to the POST is then
called by the "code behind the scenes". If for example a POST to
api/ScriptRunner/StartScript is performed, the method 
`ScriptRunnerController.Post_StartScript()` is called. This method has to use 
the `ScriptRunner`-singleton to perform the starting of the script.

How does the instantiated `ScriptRunnerController`-object receive a 
reference to the `ScriptRunner`-singleton? Correct, by Dependency-injection.
The constructor of `ScriptRunnerController` receives a reference to an
object implementing the `IScriptRunner`-interface, and 
`ScriptRunnerController.Post_StartScript()` just calls 
`IScriptRunner.StartScript()` to start the script. 

The "code behind the scenes" and the code provided in `Startup-cs` glues 
everything togheter, because it is responsible for instantiating objects and 
is able to provide them with the necessary data and references.

### Serving static files

EDIT: Serving static files is not used anymore, but the key how to use this 
feature, is to add the lines `app.UseDefaultFiles();` and 
`app.UseStaticFiles();` to [Startup.cs], and to create a folder [wwwroot] for 
the static files, like html-, css- and javascript-files.

