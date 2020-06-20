# Scripts

The programs WirelessDisplayClient and ScriptingRestApiServer don't start 
third-party-executables directly, but use starting-scripts, which are in the 
folder [Scripts/<operating-system>].

This mechanism enures, that the user can modify the scripts and change the
way, the third-party-programs are executed, without changing or 
recompiling the C#-programs.

On Linux `bash` is used to execute the scripts, on macOS `bash`, and on 
Windows `cmd.exe`. The scripts themselves are bash-scripts with the 
file-extension ".sh" on Linux and on macOS, and batch-Files with
the file-extension ".bat" on Windows.

In the different [config.json]-files the paths to the executed scripts are 
predefined. Also templates for command-line-arguments passed to these scripts 
are defined. It should never be necessary to change
these settings in [config.json], they are just used by 
by the C#-programs to start the scripts as a child-process. 

The following scripts are necessary for each operating-system:

- A script to manage the screen-resolution.
- A script to prevent the display from blanking.
- A script to start the streaming-sink (VNC-viewer in reverse connection
  or FFplay, but more streaming-methods could easily be added.).
- A script to start the streaming-source.

The script to manage the screen-resolutions is expected to execute and 
finish in short time, since the C#-programs wait for its successfull execution. 
The other scripts don't not return until they are stopped again by a 
user-action.

## Scripts for the different operatig systems

The scripts for all supported platforms are in the corresponding subfolder
[Scripts/Linux], [Scripts/Windows], and so on. All scripts share the same 
file-base-name, only the file-extensions differ (.sh on Linux, .bat on Windows).

The scripts for all different operating-systems also receive the same set of 
command-line-arguments. The C#-programs know the shell-command to start a 
script, they know the file-basename, they know the file-extension of the script
and they know the set of command-line-arguments for each script (all these 
things are defined in the different [config.json]-files).

For example: To set the screen-resolution to a value of 1024x768, the 
C#-programs executes the following process:

- On a Linux-computer: `bash manageScreenResolutions.sh SET 1024x768`
- On a Windows-computer: `cmd.exe /c "manageScreenResolutions.bat SET 1024x768"`

## Command-line-arguments passed to the scripts and exit-code

Each script must be passed a predefined set of command-line-arguments. After
completion a script has to return an exit-code. An exit-code of 0 means
success, every other code normally indicates a failure or termination by
killing the process.

### Command-line-arguments passed to the manageScreenResolutions-script

The first command-line-argument ("ACTION") is either 

- the string "GET", if the script should write the current screen-resolution
  to its standard-output,
- the string "SET", if the script should change the local screen-resolution, or
- the string "ALL", if the script should write all available local
  screen-resolutions to its standard-output (one line for each
  screen-resolution).

The second command-line-argument ("SCREEN_RESOLUTION") is a screen-resolution, 
and is only used, if the first argument was "SET". This string must have the 
same form as the strings returned by "GET" and "ALL". Normally they have a form 
like "1024x768".

### Command-line-arguments passed to the preventDisplayBlanking-script

This script receives one command-line-argument ("SECONDS_TO_PREVENT"). It is
the number of seconds the display shall not blank.

Note that this script is stopped, if the user shuts down the streaming, even
if the given number of seconds has not yet ellapsed. This parameter is 
only necessary to allow the computer to blank the display and enter sleep mode,
if one of the WirelessDisplay-programs crash.

### Command-line-arguments passed to the start-streaming-source-script

This script receives four command-line-arguments.

The first argument ("STREAMING_TYPE") is a string indicating the method,
that is used for streaming (for now: "VNC" or "FFMpeg", but new streaming-types
could be added by modifying only the scripts and the [config.json]-files).

The second argument ("IP") is the IP-Address of remote computer receiving the 
stream.

The third argument ("PORT") is the port-number, that the streaming-sink on
the remote computer listens on.

The fourth argument ("SCREEN_RESOLUTION") is a string like "1024x768". ffmpeg
and x11vnc first grab the desktop-content, and then scale it to this 
screen-resolution before streaming. On Windows, if VNC is used, this argument
is ignored, because winvnc4.exe does not support scaling of the sent sream
(as far as I know).

### Command-line-arguments passed to the startStreamingSink-script

The first command-line-argument ("STREAMING_TYPE") is the same as for the
startStreamingSource-script.

The second argument is the port-number used for streaming. The streaming-sink
will listen on this port. The only requirement is, that this port is not 
used already by another program. Also port-numbers for custom services must 
be in the range 1024...65535. Use 5500, if not sure. This is the port-number
normally used by VNC for reverse-connections, and normally is free. You can
use this port-number also for FFmpeg-streaming. 

## Trouble-shooting / Testing the scripts 

Everything, that the C#-Programs do, is to start and stop the scripts here.
So the whole functionality of the WirelessDisplay can be tested, by "manually"
running the scripts from terminals (bash-terminal, cmd.exe).  If something does 
not work as ecpected, try to do this!

For example: On a Windows-projecting-computer run the following scripts (The 
`START /B` on Windows runs a program/script in background, this is the same
as appending `&` in bash):

```
.\manageScreenResolutions.bat SET 1024x768
STRART /B .\preventDisplayBlanking.bat 3600
.\startStreamingSink.bat VNC 5500
```

Assuming, that the IP-Address of the Windows-projecting-computer is 
192.168.1.119, you can run on a Linux-presentation-computer the following 
scripts (change the screen-resolution in the first line to your needs):

```
bash ./manageScreenResolutions.sh SET 1920x1080
bash ./startStreamingSource.sh VNC 192.168.1.119 5500 1024x768
```

## Add more streaming methods

The functionality of this program can be extended without having to modify or 
recompile the C#-projexts. In many cases, only the scripts and the 
[config.json]-files can be changed.

For example: If another streaming-method using 
[RealVNC](https://www.realvnc.com/) shall be added, only two things need to be 
done:

- Extend the startStreamingSink- and the startStreamingSource-scripts to 
  support a streaming-method "RealVNC", besides the already supported
  methods "VNC" and "FFmpeg".

- In [WirelessDisplayClient/config.json] change the line
  `"StreamingTypes" : "VNC, FFmpeg",` to 
  `"StreamingTypes" : "VNC, FFmpeg, RealVNC",`.

There is no need to recompile the C#-programs! The only knowledge you need is
how to write shell-scripts or batch-files, and how to use RealVNC from the 
command-line.



