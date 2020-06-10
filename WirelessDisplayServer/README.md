# WirelessDisplayServer

This program is intended to be run on the 'projecting-computer'. 
Its purpose is to start the program 'ScriptingRestApiServer' in background.

'ScriptingRestApiServer' is a console-program, while 'WirelessDisplayServer'
is the GUI for interacting with the 'ScriptingRestApiServer'.

The 'presentation-computer' can perform POST-Requests to the 
ScriptingRestApiServer and remote-control the 'projecting-computer' by the 
following means:

- Run a script to change the screen-resolution of the 'projecting-computer'
- Start/stop a script to prevent the screensaver from activating.
- Start/stop a script that runs a streaming-sink.

All the work is done by the scripts and the ScriptingRestApiServer. This
program (WirelessDisplayServer) is only responsible for

- showing a very simple GUI.
- The GUI allows the user to change the Port, that ScriptingRestApiServer
  listens for requests.
- The GUI displays the IP-Address and the hostname of the 
  'projecting-computer' in a big TextBox.
- The GUI shows the log-output of ScriptingRestApiServer in a TextBox with
  a scrollbar.

The only controls provided to the user are a ComboBox for selecting
one of some predefined Port-Numbers and a "Restart-Server"-Button to restart
the ScriptingRestApiServer. By default, port 80 is used, if this port is
in the list of the ports, that can be used by the progran (see Configuration 
below and [config.json]). If this port is available (no other webserver 
running), and if the user has the privilegues to use it, then port 80 is a 
good choice. In this case the user needs not to interact with the GUI, and 
the main-purpose of the GUI is to show the local IP-Address of the server.

On many Linux-systems root-privilegues are necessary for listening on a
port below 1024, so here port 80 is not a good choice.

## Using the program

When the WirelessDisplayServer-program is started, it reads from [config.json]
the port-numbers the user can choose from. Then one of the ports is 
pre-selected: Either port 80, if this port-nubmer is in the list, or
the first port-number in the list in [config.json]. Using this port, the
ScriptingRestApiServer is started immediately (without any necessary 
user-interaction). The Restart-Button is disabled. 

If the user wants the ScriptingRestApiServer to listen on another port,
she first must selct this port with the ComboBox. This change in the
ComboBox enables the Restart-Server-Button. This button must then
be pushed by the user.

So, if there is no need to change pre-selected port-number, the user
only has to start the WirelessDisplayServer-program. No other user-
interaction is necessary on the presentation-computer!

## Configuration

The configuration-parameters for each operating system (of the 
projecting-computer) can be changed in [config.json].

- The (realtive) path to the executable of the ScriptingRestApiServer-program.
  If the directory-structure is not changed, then the predefined path in 
  [config.json] is correct.
- The command-line-argument(s) passed to to the 
  ScriptingRestApiServer-executable. The placeholder {PORT} is replaced by the
  port the user selects in the GUI.
- The list of ports, that are added to the ComboBox. The user can choose one 
  of this ports. Decide, if port 80 should be in this list or not.

## Running the program

Just use `dotnet run`.

## Creating an executable

From within the folder `WirelessDisplayServer` run the following command:

```
dotnet publish -c Release -o ../WirelessDisplayServer_executable/ -r linux-x64 --self-contained true
```

On macOS replace `linux-x64` with `osx-x64` and on Windows with `win-x64`.

The paremter `--self-contained true` creates a 'stand-alone' executable version. 
This  paremeter can be set to `false`, if .NET-Core version 3.1 is installed on 
the target system. All necessary files are put in the directory 
[WirelessDisplayServer_executable]. The executable to start is
[WirelessDisplayServer_executable/WirelessDisplayServer.exe] on Windows or
[WirelessDisplayServer_executable/WirelessDisplayServer] on Linux or macOS. 
The configuration can still be changed, by changing the contents of
[WirelessDisplayServer_executable/config.json].

On Windows, you can create a link for example on the desktop, that links to 
executable [WirelessDisplayServer.exe]. The program can then be run by 
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
- The files [ViewModels/ViewModelBase.cs] and [ViewLocaltor.cs]. These files 
  were not modified. They are used by the Avalonia-framework to realize the
  bindings between views and their view-models.

The following files were created:

- [config.json]
- The folder [Services]
- The interface [Services/IServerController.cs] and the class 
  [Services/ServerController.cs] that implements this interface.
- The static class [Services/HostAddressProvider.cs]

IServerController and ServerController provide two methods to start the 
ScriptingRestApiServer in background and to stop it again.

The static class HostAddressProvider contains two simple static readonly 
properties to get the hostname and the IPv4-Address of the local computer.

MainWindow.xaml contains four relevant UI-Elemnts:

- A TextBox to display the hostname of the WirelessDisplayServer
  (projecting computer) to the user.
- A TextBox to display the IPv4-address of the WirelessDisplayServer.
- A ComboBox to select the port, the ScriptingRestApiServer listens on.
- A Restart-Server-Button. After selecting another port, the user must
  push this Button. The ScriptingRestApiServer is stopped, and started
  again using the selected Port-Number.
- A ListBox containing the lines, the ScriptingRestApiServer writes to
  its standard-output and error-output.

The code-behind MainWindow.xaml.cs is nearly empty. Only one line was
added: When the user closed the window with the (X)-Button in the title-bar,
the OnWindowClosed()-Method to the MainWindowViewModel-class is called, which
in turn stops the ScriptingRestApiServer.

## MainWindowViewModel

This class is the viewmodel for the MainWindow-view. It contains properties,
that are directly bound to the MainWindow-view. For example, the property
`MainWindowViewModel.SelectedPortNumberIndex` in the viewmodel is all
the time equal to the index of the selected Port-Number in the ComboBox.
If the user selects another port in the view, this property in the 
viewmodel is updated immediately. If the program changes the value of
this protperty, in the ComboBox another item is selected.

Besides the properties that are bound to the view, there are two 
methods:

- `ButtonRestartServer_Clicked()` is called, when the user clicks the
  Restart-Server-Button (if it is enabled).
- `OnWindowClosed()` is called, when the user closes the window with
  the (X)-Button in the title-bar of the window.

When the MainViewModel-object is constructed, it receives the following
things as constructor-parameters (dependecy-injection):

- A logger-object used for logging.
- An object implementing IServerController. With this object the.
  ScriptingRestApiServer can be started and stopped.
- A string containing the IP-address of the local computer.
- A string containing the hostname of the local computer.
- A list of port-numbers, which are addes as items to the ComboBox.

## Startup-code in App.xaml.cs - depencency-injection

In [App.xaml.cs], all startup-code is placed. The original contents of this
file create a `MainWindowViewModel`-object (viewmode) and a 
`MainWindow`-object (view). Then a reference to the 
`MainWindowViewModel`-object is stored in `DataContext`-property of
the `MainWindow`-view-object. 

Own code to [App.xaml.cs] does the following things

- Define string-constants that are used as keys in [config.json]
- Create the objects and strings, that have to be passed to the constructor
  of the `MainWindowViewModel`. This is done with the method 
  `realizeDependencyInjection()` which is called before construction of the
  `MainWindowViewModel`.

In `realizeDependencyInjection()` the following thigs happen: 

First a `LoggerFactory` is created, and with this `LoggerFactory` loggers for
`MainWindowViewModel`, `CustomConfigProvider` and `ServerController`
are created. These loggers are passed later on to the constructors of
each of the three classes.

A `CustomConfigProvider`-object is created to read [config.json]. This class
is defined in [Common], and used by the other three projects as well. Using
the `CustomConfigProvider`-object the port-numbers, the path the 
ScriptingRestApiServer-executable and the command-line-arguments for it are 
read from [config.json].

The ScriptingRestApiServer-executable and the command-line-arguments for it
are used to create an instance of `ServerController`.

Finally, using the static methods of `HostAddressProvider` the hostname and 
the IP-address of the local computer are read into strings.

The method `realizeDependencyInjection()` returns the following things (as
out-paramters), which then are used to construct an instance of
`MainWindowViewModel`:

- `ILogger<MainWindowViewModel> loggerforMainWindowModel`,
- `IServerController serverController`,
- `string hostName`,
- `string iPAddress`,
- `List<UInt16> portNumbers`.

