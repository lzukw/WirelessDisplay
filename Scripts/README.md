# Scripts

The programs WirelessDisplayClient and ScriptingRestApiServer don't start 
third-party-executables directly, but uses starting-scripts, which are in the 
folder [Scripts/<operating-system>].

This mechanism enures, that the user can modify the scripts and change the
way, the third-party-programs are executed, without changing or 
recompiling the C#-GUI-programs.

On Linux `bash` is used to execute the scripts, on macOS TODO, and on 
Windows `cmd.exe`. The scripts themselves are bash-scripts with the 
file-extension ".sh" on Linux and on macOS, and batch-Files with
the file-extension ".bat" on Windows.

In [config.json] the paths to the executed scripts are predefined.
Also templates for command-line-arguments passed to these scripts are defined.
It should never be necessary to change
the settings in [config.json], these settings are just used by 
by the programs to start the scripts as a child-process. 

The following scripts are necessary for each operating-system:

- A script to manage the screen-resolution of the local computer.
- A script to prevent the screen-saver from activating.
- A script to start the streaming-sink (VNC-viewer in reverse connection
  or FFplay).
- A script to start the streaming-source (VNC-Server in reverse-connection
  or FFmpeg).

The script to manage the screen-resolutions is expected to execute and 
finish in short time, since the programs wait for its execution. The
scripts for starting streaming-sink or -source don't not return until 
streaming is stopped again by the user.

## Command-line-arguments passed to the scripts and exit-code

Each script must be passed a predefined set of command-line-arguments. After
completion a script has to return an exit-code. An exit-code of 0 means
success, every other code normally indicates a failure or termination by
killing the process.

### Command-line-arguments passed to the manageScreenResolutions-script

The first command-line-argument is either 

- the string "GET", if the script should write the current screen-resolution
  to its standard-output,
- the string "SET", if the script should change the local screen-resolution, or
- the string "ALL", if the script should write all available local
  screen-resolutions to its standard-output (one line for each
  screen-resolution).

The second command-line-argument is a screen-resolution, and is only used,
it the first argument was "SET". This string must have the same form as the
strings returned by "GET" and "ALL". Normally they have a form like
"1024x768".

### Command-line-arguments passed to the preventScreensaver-script

TODO

### Command-line-arguments passed to the start-streaming-source-script

TODO

### Command-line-arguments passed to the startStreamingSink-script

The first command-line-argument is either the string "VNC" or the string
"FFmpeg", depending on which streaming-sink shall be started by the script.
Note all strings denoted here are passed to the script without the 
double-quotes here do denote strings.

The second argument is the port-number used for streaming. The streaming-sink
will listen on this port. So the only requirement is, that this port is not 
used already by another program. Also port-numbers for custom services must 
be in the range 1024...65535.


