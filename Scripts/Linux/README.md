# Scripts for Linux.

Needed external programs (xrandr, x11vnc, vncviewer and ffmpeg) are 
best installed with the packagemanager of the Linux-distribution:

NOTE: Only X-server based desktops are supported, wayland is not supported
by all needed tools (as of June 2020)! On wayland there seems to be no
possibility to change the screen-resolution (xrandr definitively can't be used
for this purpose). But see [Scripts/README.md] for how to add other 
Streaming-Types. Help on how to support wayland-based systems would be 
appreciated.

No ThirdParty-executables are necessary in the [ThirdParty/Linux]-folder.

# Debian / Ubuntu

The tool `xrandr` to change screen-resolutions should already be installed.

```
sudo apt-get install xdotool
sudo apt-get install x11vnc
sudo apt-get install tigervnc-viewer
sudo apt-get install ffmpeg libavcodec-extra
```

# Fedora

The tool `xrandr` to change screen-resolutions should already be installed.

```
sudo dnf install xdotool
sudo dnf install x11vnc
sudo dnf install tigervnc
sudo dnf install ffmpeg x264-libs
```

The x264-libs package is in the rpmfusion-free repositories.




