# WirelessDisplayClient

This program is intended to be run on the 'presentation computer'. It provides
a GUI for the user to change the screen-resolutions of 
both computers (local 'presentation-computer' and remote 'projecting computer'),
and start streaming. The method used for streaming (VNC, FFmpeg) can be
chosen with a ComboBox.

The following actions are performed via calls to the REST-API offered by the
ScriptingRestApiServer which is running on the remote 'projecting computer':

- Change the screen-resolution of the remote computer.
- Start a program, that prevents the display of the remote computer from
  blanking.
- Start a streaming-sink on the remote computer.

The ScriptingRestApiServer provides POST-Requests, which run, start and stop 
scripts on the remote computer. The scripts then perform the above actions
by using external programs (either installed programs, or executables
in the [TirdParty]-folder of the remote computer).

On the local computer the following actions are performed

- Change the screen-resolution of the local computer.
- Start a sreaming-source on the local computer, that streams the local 
  desktop to the remote computer.

The WirelessDisplayClient uses a `LocalScriptRunner`-class to run, start
or stop scripts on the local computer. Again the scripts on the local
computer use other programs to do the real work.

## Using this program

This program is a GUI-program with the following relevant elements:

- A connection-Panel with:
  * A TextBox where the user can enter the IP-Address of the remote computer.
  * A NumericUpDown for the port-Number, the remote ScriptingRestApiServer
    listens on.
  * A Connect-Button and Disconnect-Button to start/stop a connection to
    the remote ScriptingApiServer.
- A screen-resolutions-panel:
  * With four TextBoxes to show the initial and the current screen-resolution
    of the local and the remote computer.
  * Two ComboBoxes to select the screen-resolutions of the local and the 
    remote computer when the streaming is started or stopped.
- A streaming-Panel with:
  * A ComboBox to choose the streaming-type (VNC, FFmpeg for now, but 
    others can be added just by modifying the scripts and [config.json]).
  * A NumbericUpDown for choosing the port-number for streaming (the
    streaming-sink on the remote computer will listen on this port)
  * Start- and stop-Buttons to start and stop the streaming.
- A Status-Panel with:
  * A Listbox showing Log-lines

## Configuration

The configuration-parameters for each operating system (of the 
presentation-computer) as well as platform-independent parameters can be changed 
in [config.json]. There is no need to recompile after changing [config.json],
only be sure to edit the correct [config.json]-file. The configurable 
parameters are:

- Names of local and remote scripts (without the platform-dependent 
  file-extension).
- A template with command-line-arguments for each script. These templates
  contain placeholders, that are replaced with actual values when the
  script is executed, or when the remote computer is asked via a POST-request 
  to execute a script.
- The name of the command used to execute a script (bash, cmd.exe).
- The template with command-line arguments passed to this command. These
  command-line-arguments contain placeholders for the path to the script to
  execute and for arguments passed to the script.
- The preferred screen-width for the computers. A screen-resolution, which
  is nearest to this preferred screen-width is pre-selected in the both
  ComboBoxes.

## Running the program

Just use `dotnet run`.

## Creating an executable

From within the folder `WirelessDisplayClient` run the following command:

```
dotnet publish -c Release -o ../WirelessDisplayClient_executable/ -r linux-x64 --self-contained false
cp config.json ../WirelessDisplayClient_executable/
```

On macOS replace `linux-x64` with `osx-x64` and on Windows with `win-x64` (On 
windows also be sure to use `\` as filepath seperator instead of `/`).

The paremter `--self-contained true` creates a 'stand-alone' executable version. 
This  paremeter can be set to `false`, if .NET-Core version 3.1 is installed on 
the target system. 

All necessary files are put in the directory 
[WirelessDisplayClient_executable]. The executable to start is 
[WirelessDisplayClient_executable/WirelessDisplayClient.exe] on Windows or
[WirelessDisplayClient_executable/WirelessDisplayClient] on Linux or macOS. 

The configuration can still be changed, by changing the contents of
[WirelessDisplayClient_executable/config.json].

On Windows, you can create a link for example on the desktop, that links to 
executable [WirelessDisplayClient.exe]. The program can then be run by 
double-clicking this link. You can also create a 
start-menu-entry, by creating the link in 
[%AppData%\Microsoft\Windows\Start Menu\Programs].

On Linux, when starting the executable, the current-working-directory be the
folder where this executable resides:

[/path/to/WirelessDisplayClient_executable]

# Technical details

Please have a look at [README.md] in the folder [WirelessDisplayServer] first.
Everthing explained at the beginning of the section "Technical details" there
also applies to this project:

- How to create the project and add the references and packages.
- How to change the target-framework from dotnet-core 3.0 to 3.1
- The files created by the `dotnet new`-command.

The following files were created later:

- [config.json]
- The folder [Services] conatining the interface [IWDClientServices.cs] and 
  the class [WDClientServices.cs] implementing this interface.

## Startup-code and dependency-injection

[App.xaml.cs] contains the whole startup-code. With the method
`realizeDependencyInjection()` the follwing things are done:

- A `loggerFactory` is created, which is used to create logger-objects for
  every other object, that needs do some logging. The loggers are injected into
  the contructors ot these objects.
- A `CustomConfigProvider`-instance is created to read the contents of 
  [config-json]. 
- A `LocalScriptRunner`-object is instantiated using some values read from
  [config.json]. This object is used by the `WDClientServices` to execute
  scripts on the local presentation-computer. The `WDClientServices` "sees" 
  the `LocalScriptRunner`-object only through its interface 
  `ILocalScriptRunner`.
- A `RemoteScriptRunner`-object is instantiated, also using some values from 
  [config.json]. This object is used by the `WDClientServices` to execute 
  scripts on the remote projecting-computer. The `WDClientServices` "sees" the 
  `RemoteScriptRunner`-object  only through its interface 
  `IRemoteScriptRunner`.
- Finally, the already mentioned instance of `WDClientServices` is created,
  receiving references to a logger, the `(I)LocalScriptRunner`- and the 
  `(I)RemoteScriptRunner`-objects, and some strings containing information
  about the local and remote scripts to run.

Then a `MainWindowViewModel`-instance is created. It receives a reference to 
the just created `WDClientServices`-instance, and some data read from
[config.json] (for example a List with the used streaming-types). Again, the
`MainWindowViewModel`-instance "sees" the `WDClientServices`-object only 
through its interface `IWDClientServices`. Using this object/interface, the 
`MainWindowViewModel`-instance is able to run scripts on the local and on the
remote computer to change screen-resolutions, prevent the display of the 
projecting-computer from blanking, and to start ans stop streaming.

Finally the code in [App.xaml.cs] creates a `MainWindow`-instance and
sets its `DataContext`-property to the just created 
`MainWindowViewModel`-instance.

## Bindings between the MainWindow-view and the MainWindowViewModel

The `MainWindow`-object (the view) is defined in the two files 
[Views/MainWindow.xaml] and [Views/MainWindow.xaml.cs]. The second file, the
so called 'code-behind' is nearly empty. It only ensures, that the async 
`OnWindowClose()`-method of the `MainWindowModel` is called once, when the
user closes the window using the (X)-Button in the title-bar.

The file [Views/MainWindow.xaml] defines the GUI-Elements of the 
MainWindow-view. Some of the values of xaml-Attributes have the form
`"{Binding ...}"`. These Attributes are bound to the corresponding 
properties of the viewmodel (`MainWindowViewModel`). 

For example: In [MainWindow.xaml] a TextBox is defined, that the user uses to enter the IP-Address of the remote computer:

```
<TextBox Grid.Row="1" Grid.Column="0" Text="{Binding IpAddress}" IsEnabled="{Binding !ConnectionEstablished}" VerticalAlignment="Center" Margin="4" MinWidth="120" Watermark="192.168.x.y" />
```

The `MainWindowViewModel`-class defines two corresponding properties:

```
private string _ipAddress="";
public string IpAddress
{
    get => _ipAddress;
    set => this.RaiseAndSetIfChanged(ref _ipAddress, value);
}
```

and

```
private bool _connectionEstablished = false;
public bool ConnectionEstablished
        {
            get => _connectionEstablished;
            set => this.RaiseAndSetIfChanged(ref _connectionEstablished, value);
        }
```

The code in `MainWindowViewModel` change enable or disable the TextBox by
setting the value of its `ConnectionEstablished`-property. The value, the
user typed into the TextBox can be read by the code in `MainWindowViewModel`
by reading the `IpAddress`-property. (The code could also set this property
to a certain string, and the TextBox then would contain this string. The user
then sees this string in the TextBox.)

## The program-logic defined in the MainWindowViewModel

When the user pushes the "Connect"-Button, the mehtod 
`MainWindowViewModel.ButtonConnect_Click()` is executed. In this method the 
following things happen:

- `WDClientServices.ConnectToServer()` is called, which only performs a 
  POST-request to the remote ScriptingRestApiServer to see, if it can
  communicate to it.
- The current and the initial screen-resolutions of the local and the
  remote computer are fetched and displayed with four TextBlocks to the user.
- The available screen-resolutions on both computers are fetched and put
  as items in the two ComboBoxes. The user can later choose the 
  local and remote screen-resolution with these two ComboBoxes.
- In each ComboBox the screen-resolution that fits best to the preferred
  screen-widths for streaming, given in [config.json] is pre-selected.

When the user then pushes the "Start streaming"-Button, the following things
happen:

- The local and the remote screen-resolution are changed to the selected
  values in the ComboBoxes. 
- The script to prevent the display from blanking is started on the remote
  projecting-computer.
- The streaming-sink is started on the remote projecting-computer
- The streaming-source is started on the local presentation-computer
- In the ComboBoxes for both screen-resolutions, the initial screen-resolutions
  are pre-selected (They take effect, when the user stops the streaming).

When the user pushes the "Stop streaming"-Button (or the "Disconnect"-Button,
or closes the Application), the following things happen:

- The local streaming-source is stopped.
- The remote streaming-sink is stopped.
- The remote script to prevent the display from blanking is stopped.
- The local and the remote screen-resolutions are set to the values selected
  in the ComboBoxes (which are the previously preselected initial 
  screen-resolutions, if the user didn't select another value).
- Again, the screen-resolutions that fit best to the preferred
  screen-widths for streaming, given in [config.json] are pre-selected in the
  ComboBoxes (in case the user wants to start the streaming again).

