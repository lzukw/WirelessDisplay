# Scripts

The programs WirelessDisplayClient and ScriptingRestApiServer don't start 
third-party-executables directly, but use starting-scripts, which are in the 
folder [Scripts/<operating-system>].

This mechanism enures, that the user can modify the scripts and change the
way, the third-party-programs are executed, without changing or 
recompiling the C#-programs.

On Linux and macOS `bash` is used to execute the scripts, and on 
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
- A script to start the streaming-sink. (VNC-viewer, VNC-viewer in reverse 
  connection or FFplay, but more streaming-methods could easily be added.)
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
and is only used, if the first argument was "SET". Normally they have a form 
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
that is used for streaming (for now: "VNC, VNC-Reverse" or "FFMpeg", but new 
streaming-types could be added by modifying only the scripts and the 
[config.json]-files. Using Open-broadcaster-Studio as streaming-source is
planned at least on Linux-presentation-computers).

The second argument ("SINK_IP") is the IP-Address of remote computer receiving
the stream.

The third argument ("PORT") is the port-number, that the streaming-sink on
the remote computer listens on.

The fourth argument ("SOURCE_SCREEN_RESOLUTION") is used by some 
streaming-sources to grab the desktop. This is a string like "1980x1024" 
indicating the screen-resolution of the presentation-computer 
(streaming-source).

The fifth argument ("STREAM_SCREEN_RESOLUTION") is a string like "1024x768".
ffmpeg and x11vnc first grab the desktop-content, and then scale it to this 
screen-resolution before streaming. On Windows, if VNC is used, this argument
is ignored, because winvnc4.exe does not support scaling of the sent stream.

### Command-line-arguments passed to the startStreamingSink-script

The first command-line-argument ("STREAMING_TYPE") is the same as for the
startStreamingSource-script.

The second command-line-argument ("SOURCE_IP") is the IP-address of the 
remote streaming-source-computer. This parameter is used for normal 
VNC-connections.

The third argument is the port-number used for streaming. The streaming-sink
will listen on this port (For "normal" VNC the VNC-Server, i.e. the 
streaming-source will listen on this port, not the streaming-sink). The only 
requirement is, that this port is not used already by another program. Also 
port-numbers for custom services must be in the range 1024...65535. Use 5500, 
if not sure. This is the port-number normally used by VNC for 
reverse-connections, and normally is free. You can use this port-number also 
for FFmpeg-streaming and VNC in reverse-mode.


## Trouble-shooting / Testing the scripts 

Everything, that the C#-programs do, is to start and stop the scripts here.
So the whole functionality of the WirelessDisplay can be tested, by "manually"
running the scripts from terminals (bash-terminal, cmd.exe).  If something does 
not work as ecpected, try debugginh the scripts by running their lines in a
terminal!

For example: On a Windows-projecting-computer run the following scripts (The 
`START /B` on Windows runs a program/script in background, this is the same
as appending `&` in bash):

```
.\manageScreenResolutions.bat SET 1024x768
STRART /B .\preventDisplayBlanking.bat 3600
.\startStreamingSink.bat VNC-Reverse 5500 dummyarg dummyaeg dummyaeg
```

Assuming, that the IP-Address of the Windows-projecting-computer is 
192.168.1.119, you can run on a Linux-presentation-computer the following 
scripts (change the screen-resolution in the first line to your needs):

```
bash ./manageScreenResolutions.sh SET 1920x1080
bash ./startStreamingSource.sh VNC-Reverse 192.168.1.119 5500 dummyarg  1024x768
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
  methods "VNC", "VNC-Reverse" and "FFmpeg".

- In [WirelessDisplayClient/config.json] change the line for the used 
  operating-system from
  `"StreamingTypes" : "VNC, FFmpeg",` to 
  `"StreamingTypes" : "VNC, FFmpeg, RealVNC",`.

There is no need to recompile the C#-programs! The only knowledge you need is
how to write shell-scripts or batch-files, and how to use RealVNC from the 
command-line.

## Open Broadcaster Software (OBS-Studio) as streaming-source

On Linux and macOS OBS-Studio can be used as streaming-type. But this only
works, if OBS-Studio has been installed and "prepared" correctly:

### Create a new OBS-profile

Start OBS, and create a new profile named "WirelessDisplayClientSide".
 
Go to Settings >> Output. Set Output-Mode to "Advanced". Then switch to 
"Recording"-Tab (Yes, for streaming via udp one uses "Recording").
  
- Type: Custom output (FFmpeg)
- File path or URL: udp://192.168.1.20:5500   ...this ip-Address will be
  changed by the startStreamingSource.sh-script
- Container-Format: mpegts
- Video-Bitrate: 2500 kBit/s ...or maybe lower 
- Keyframe-interval: 60   ...each 60 frames a keyframe is sent (every 2s)
- Video-Encoder: mpeg2video (Default-Encoder)
- Audio-Bitrate: 160kBps
- Audio-Encoder: mp2 (Default-Encoder)

Go to Settings >> Video. 

- Observe the values for the Base (Canvas)-Resolution and the 
  Output (Scaled) Resolution. These values will be changed by by the 
  startStreamingSource.sh-script.
- Set "Common FPS-Values" (framerate) to 30.

### Create a new scene-collection 

Name the new scene-collection also "WirelessDisplayClientSide".
Set up scenes for the presentation-computer (normally just one scene named 
"main" with one screen-capture-source is sufficient. Adjust audio inputs as 
needed.

Closing OBS and reopening it should save all changes to the file
[${HOME}/.config/obs-studio/basic/profiles/WirelessDisplayClientSide/basic.ini].

Inspect this file. It should contain all the above settings (From OBS you can 
use File >> Show Profile Folder).

When the startStreamingSource.sh-script is started (when clicking the 
start-streaming-Button in WirelessDisplayClient), the [basic.ini]-File is
modified by the script. You can observe either this file or the settings
again. They should have been changed to the proper values.


