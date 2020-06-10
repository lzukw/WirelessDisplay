# WirelessDisplayClient

This program is intended to be run on the 'presentation computer'. It provides
a GUI with that the user can change the screen-resolutions of 
both computers (local 'presentation-computer' and remote 'projecting computer'),
and start streaming. The method used for streaming (VNC, FFmpeg) can be
chosen with a ComboBox.

The following actions are performed via calls to the REST-API offered by the
ScriptingRestApiServer which is running on the remote 'projecting computer':

- Change the screen-resolution of the remote computer.
- Start a program, that prevents the screensaver of the remote computer from
  activating
- Start a streaming-sink on the remote computer.

The ScriptingRestApiServer provides POST-Requests, which run, start and stop 
scripts on the remote computer. The scripts then perform the above actions
by using external programs (either installed programs, or executables
in the [TirdParty]-folder).

On the local computer the following actions are performed

- Change the screen-resolution of the local computer.
- Start a sreaming-source on the local computer, that streams the local 
  desktop to the remote computer.

The WirelessDisplayClient uses a local `ScriptRunner`-class to run, start
or stop scripts on the local computer. Again the scripts on the local
computer use other programs to do the reals work.

## Using this program

This program is a GUI-program with the following relevant elements:

- A connection-Panel with:
  * A TextBox where the user can enter the IP-Address of the remote computer
  * A NumericUpDown for the port-Number, the remote ScriptingRestApiServer
    listens on
  * A Connect-Button and Disconnect-Button to start/stop a connection to
    the remote ScriptingApiServer.
- A screen-resolutions-panel:
  * With four TextBoxes to show the initial and the current screen-resolution
    of the local and the remote computer
  * Two ComboBoxes to select the screen-resolutions of the local and the 
    remote computer when the streaming is started.
- A streaming-Panel with:
  * A ComboBox to choose the streaming-method (VNC, FFmpeg for now, but 
    others can be added just by modifying the scripts and [config.json]).
  * A NumbericUpDown for choosing the port-number for the streaming (the
    streaming-sink on the remote computer will listen on this port)
  * Start- and stop-Buttons to start and stop the streaming.
- A Status-Panel with
  * A Listbox showing status-logs 

## Configuration

The configuration-parameters for each operating system (of the 
projecting-computer) as well as platform-independent parameters can be changed 
in [config.json]. There is no need to recompile after changing [config.json].
The configurable parameters are:

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
```

On macOS replace `linux-x64` with `osx-x64` and on Windows with `win-x64`. 

The paremter `--self-contained true` creates a 'stand-alone' executable version. 
This  paremeter can be set to `false`, if .NET-Core version 3.1 is installed on 
the target system. All necessary files are put in the directory 
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


# Technical details

Please have a look at [README.md] in the folder [WirelessDisplayServer] first.
Everthing explained at the beginning of the section "Technical details" there
also apply for this project:

- How to create the project and add the references and packages.
- How to change the target-framework from dotnet-core 3.0 to 3.1
- The files created by the `dotnet new`-command.

The following files were created later:

- [config.json]
- The folder [Services]
- TODO

## TODO explanation of each file

## TODO startup-code and dependenca-injection (how is everything glued 
   together










