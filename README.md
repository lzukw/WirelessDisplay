# WirelessDisplay

## Overview

With the projects in this repository the desktop of a 'presentation-computer'
can be streamed via a WLAN to a 'projecting-computer'. The projecting-computer
is connected to a projector and displays the streamed content.

![Overview over computers and projector](doc_images/overview.svg)

On the presentation-computer the GUI-program 'WirelessDisplayServer' is 
started. Its main-purpose is to show the IP-address of the presentation-
computer, and to start a webservice in backgorund.

![GUI of WirelessDisplayServer](doc_images/WirelessDisplayServerGUI.png)

On the projecting-computer another GUI-program, called 'WirelessDisplayClient'
is started. This GUI-program is used to connect to the presentation-computer,
and start/stop the stream. The GUI also allows to control the 
screen-resoltutions of both computers.

![GUI of WirelessDisplayClient](doc_images/WirelessDisplayClientGUI.png)

Linux, macOS and Windows are the supported operating-systems for both 
computers. As GUI-framework the platform-independent 
[Avalonia-Framework](http://avaloniaui.net) is used.

## The projects in this repository

This repository contains projects for three programs, written in C# using 
dotnet core 3.1:

- ScriptingRestApiServer
- WirelessDisplayServer
- WirelessDisplayClient

The programs 'ScriptingRestApiServer' and 'WirelessDisplayServer' run on the
projecting-computer. The project 'WirelessDisplayClient' runs on the 
presentation-computer.

The heart of the project is the 'ScriptingRestApiServer'-project, running
on the server. This project is a webservice that provides a REST-API. With
this REST-API the client (presentation-computer) is able to start and stop
the execution of scripts on the server (projecting-computer) via POST-Requests
to the webservice. There are scripts for 

- managing the screen-resolution, 
- preventing the screensaver from activating, and
- start the streaming sink on the projecting-computer.

The program 'WirelessDisplayClient' runs on the presentation-computer. The user
can "connect" to the webservice on the projecting-computer, an start the
streaming. When the streaming is started by the user, th e following things
happen:

- The screen-resolution of the local computer (presentation-computer) is
  set to the desired value, running the script for managing the 
  screen-resolution on the local computer.
- The screen-resolution of the remote computer (projecting computer) is
  set to the desired value, by asking the remote-computer with a POST-request
  to run the script for managing the screen-resolution.
- With another POST-request toe remote-computer is asked to execute a script
  that runs a streaming-sink.
- With another POST-request the remote-computer is instructed to run a script
  that prevents the screensaver from activating.
- On the local computer a script is started, that runs a streaming-source.

## Scripts

To ensure platform-independency, the "real work" is done by scripts. On a 
Linux-Computer the scripts in the folder [Scripts/Linux] are executed with
`bash`, on Windows the batch-files in [Scritps/Windows] are executed with
`cmd.exe`. The file-name of the scripts, and the command-line arguments they 
receive, are the same for each operating-system. Only the file-extension and
the used shell different for each operating-system.

Feel free to modify the scripts to your needs.

See the [REAMDE.md](Scripts/README.d) in the [Scripts]-Folder for more
information.

## Folder-structure

The folder [ScriptingRestApiServer] contains the source-code of the 
ScriptingRestApiServer-project. The folders [WirelessDisplayServer] and
[WirelessDisplayClient] contian the source-code of the other two projects.

The folder [Common] contains a class-library, that provides classe, that are
used by all three projects.

The folders [Scripts/Linux], [Scripts/maxOS] and [Scripts/Windows] contain
the scripts that do all the relevant work. Some scripts rely on 
third-party-programs, which have to be present in [ThirdParty/macOS] and
[ThirdParty/Windows]. (On Linux only common programs are called by the
scripts). 

See the [README.md] in the folder [ThirdParty] and its subfolders for details.

The folders [ScriptingRestApiServer_executable], 
[WirelessDisplayServer_executable] and [WirelessDisplayClient_executable] are
empty and will contain the executable programs after building them.

## Installation instructions for the projecting-computer (server)

On the server-computer the projects 'ScriptingRestApiServer' and 
'WirelessDisplayServer' have to be built. The scripts in the [Scripts]-folder 
and eventually necessary third-party-programs must be present.

### Build-instructions for the projecting-computer using Windows

Assuming, that the dotnet-core 3.1-runtime is installed on the target-
Windows-computer used as projecting-computer, you can use the following
build-instructions:

- Download and extract, or clone the WirelessDisplay-repository. Then `cd` into
  the [WirelessDisplay]-folder

- Build the ScriptingRestApiServer-project:
```
cd ScriptingRestApiServer
dotnet publich -c Release -r win-x64 --self-contained false -o ../ScriptingRestApiServer_executable
cd ..
```

- Build the 

