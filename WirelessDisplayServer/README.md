# WirelessDisplayServer

This program is intended to run on the 'projecting-computer'. 
Its purpose is to run the program 'ScriptingRestApiServer' in background. 

The 'presentation-computer' can perform POST-Requests to the 
ScriptingRestApiServer and so remote-control the 'projecting-computer':

- Run a script to change the screen-resolution of the 'projecting-computer'
- Start/stop a script to prevent the screensaver from activating.
- Start/stop a script that runs a streaming-sink.

All the work is done by the scripts and the ScriptingRestApiServer. This
program (WirelessDisplayServer) is only responsible for

- showing a very simple GUI (window).
- The GUI allows the user to change the Port, that ScriptingRestApiServer
  listens for requests.
- The GUI displays the IP-Address of the 'projecting-computer' in a big
  TextBox.
- The GUI shows the log-output of ScriptingRestApiServer in a TextBox with
  a scrollbar.

The only controls provided to the user are a ComboBox for selecting
one of some predefined Port-Numbers and a "Restart-Server"-Button to restart
the ScriptingRestApiServer. By default, port 80 is used. If this port is
available (no other webserver running), and if the user has the privilegues
to use a port lower than 1024, then port 80 is a good choice. In this case
the user needs not to interact with the GUI, and the main-purpose of the
GUI is to show the local IP-Address of the server.


## Configuration

The only configuration needed is the (realtive) path to the executable of the
ScriptingRestApiServer-program. This path can be changed by modifying
[config.json]. If the directory-structure is not changed, then the 
predefined path in [config.json] is correct.

## Running the program

Just use `dotnet run`.

## Creating an executable

From within the folder `WirelessDisplayServer` run the following command:

```
dotnet publish -c Release -o ../WirelessDisplayServer_executable/ -r linux-x64 --self-contained
```

On macOS replace `linux-x64` with `osx-x64` and on Windows with `win-x64`.

The paremter `--self-contained` creates a 'stand-alone' executable version. This 
paremeter can be omitted, if .NET-Core version 3.1 is installed on the target 
system. All necessary files are put in the directory 
[WirelessDisplayServer_executable]. The executable to start is
[WirelessDisplayServer_executable/WirelessDisplayServerGUI.exe] on Windows or
[WirelessDisplayServer_executable/WirelessDisplayServerGUI] on Linux or macOS. 
The configuration can still be changed, by changing the contents of
[WirelessDisplayServer_executable/config.json].

TODO: must config.json be copied manually to WirelessDisplayServer_executable???

On Windows, you can create a link for example on the desktop, that links to 
executable [WirelessDisplayServerGUI.exe]. The program can then be run by 
double-clicking this link. You can also create a 
start-menu-entry, by creating the link in 
[%AppData%\Microsoft\Windows\Start Menu\Programs].


# Technical Details

The platform-independent GUI-Framwork [Avalonia](https://avaloniaui.net/) is 
used. To be able to create a new Avalonia-App, the dotnet-templates must
be installed (in order to just use the code, no installation of templates
should be required). See [here]():

```
cd to-some-folder-where-the-templates-will-reside
git clone https://github.com/AvaloniaUI/avalonia-dotnet-templates.git
dotnet new --install path-to-the-cloned-repository
```

This program was created with:

```
cd WirelessDisplayServer
dotnet new avalonia.mvvm 
dotnet add reference ../Common/Common.csproj
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
```

The avalonia.mvvm template uses dotnet-core 3.0 by default. This was changed by 
modifiying the following line in [WirelessDisplayServer.csproj]. See
[here](https://avaloniaui.net/docs/quickstart/change-target-framework).

```
<TargetFramework>netcoreapp3.1</TargetFramework>
```

The `dotnet new`-command created the following relevant files:

From the created files, the following ones are important:

- [Porgram.cs]: This file was not modified. It starts the Avalonia-engine. This
  engine creates and uses an instance of the `App`-class.
- [App.xaml] and [App.xaml.cs]: Together they define the `App-class`. Here all
  startup-code is placed.
- [Views/MainWindow.xaml] and [Views/MainWindow.xaml.cs]: Toghether they define 
  the `MainWindow`-class, which is the 'view' for the GUI-Main-Window. The file
  [Views/MainWindow.xaml.cs] is called 'code-behind' for the 'view'.
- [ViewModels/MainWindowViewModel.cs]: The so called 'viewmodel' for the view.
  See [Views and ViewModels](http://avaloniaui.net/docs/quickstart/mvvm#views-and-viewmodels)
  for a good explanation of the idea behind 'views' and  their 'viewmodel', 
  and so called 'bindings' between them.
- The [Models]-folder is not used in this project, since there is no data to
  be managed.

The avalonia.mvvm template uses dotnet-core 3.0 by default. This was changed by 
modifiying the following line in [WirelessDisplayServer.csproj]. See
[here](https://avaloniaui.net/docs/quickstart/change-target-framework).

```
<TargetFramework>netcoreapp3.1</TargetFramework>
```

The following files were created:

- [config.json]
  ScriptingRestApiServer-executable.
- The folder [Services] containing the classes ...TODO...

TODO ...more files

MainWindow.xaml contains two relevant UI-Elemnts:

- TODO.

### Configuration

As already said, the path to the ScriptingRestApiServer-executable can be
modified in [config.json]. Also, a list of port-numbers, from which the user
can select, are in this file.

### MainWindow.xaml.cs

This file contains the 'Interaction logic for `MainWindow.xaml`' by means of a
partial class `MainWindow`. (The other parts of this class are created by the
compiler from the file `MainWindow.xaml` and are only present in the 
`obj`-directory).

In `MainWindow.xaml.cs` the whole program is realized. In the Constructor of
the `MainWindow`-class the following things are done:

- The local IPv4-Address is retrieved by using static methods from 
  `System.Net.Dns`. The IPv4-Address then is displayed to the user (inserted
  into labelIp).
- Configuration is read in from `App.config` containing the filepath to the
  WirelessDisplayServer-executable. 
- Eventually still running WirelessDisplayServer-processes are killed.
- A new WirelessDisplayServer-process is created and run in background.
- Two event-handlers are registered: Each time the started 
  WirelessDisplayServer-background-process writes a line to its stdout or stderr,
  the events `OutputDataReceived` or `ErrorDataReceived` occur. Both events call
  the method `void backgroundProcess_DataReceived(object sender, DataReceivedEventArgs e)`,
  where the line written by the process to stdout/stderr is contained in `e.Data`.
  In this method, the line is appended to `textblockLog` and so displayed to
  the user.

When the user closes the MainWindow (and the whole program), the event-handler
`void mainWindow_Closing(object sender, object e)` is called. Here the
WirelessDisplayServer-background-process is killed.
