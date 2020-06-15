# Scripts for Linux.

Needed external programs (xrandr, x11vnc, vncviewer and ffmpeg) are 
best installed with the packagemanager of the Linux-distribution:

Note: Only X-server based desktops are supported, wayland is not supported
by all needed tools (as of June 2020)!

No ThirdParty-executables are necessary.

# Debian / Ubuntu

```
xrandr should be installed.
sudo apt-get install xdotool
sudo apt-get install x11vnc
sudo apt-get install xtightvncviewer
```

# Fedora

```
xrandr should be installed
sudo dnf install xdotool
sudo dnf install x11vnc
sudo dnf install tigervnc
```
xtightvncviewer does not seem to be available on Feroda, so tignervnc is used
as streaming-sink. The big disadvantage is, that the mouse-cursor is not shown
by tigervnc (vncviewer). So a debian-based system is definitifely better suited
as presentation-computer. 

The script [Scripts/Linux/startStreamingSink.sh] has to be modified: The line
containing `xtightvncviewer` must be commented out, and the comment-hash-tag
must be removed from the line containing `vncviewer`.


