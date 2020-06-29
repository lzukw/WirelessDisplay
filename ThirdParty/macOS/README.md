# Third-party-tools for macOS

## Prerequisites

### Install developer-tools

Open a terminal and just to try, type `git`. If this prompts you to install
commandline-developer-tools, the do so.

### Install dotnet core 3.1 sdk

This step is only needed on deveolper-machines for building WirelessDisplay
on macOS.

Browse to the [dotnet-core-3.1 download-site](https://dotnet.microsoft.com/download/dotnet-core/3.1)
and download the installer for macOS (64-Bit): [dotnet-sdk-3.1.nnn-osx-x64.pkg].
Install the SDK by double-clicking the downloaded file.


## screenresolution

A modified version of the tool 
[screenresolution)(https://github.com/jhford/screenresolution)
is needed for managing screen-resolutions. It could be installed, but
it can also be used as a "portable" tool.

First build the executable:

```
git clone https://github.com/lzukw/build-screenresolution.git
cd screenresolution
make build
```

Then copy or move the executable file (screenresolution) to the folder
[WirelessDisplay/ThirdParty/macOs/screenresolution/]

## ffmpeg

Excecutables for [ffmpeg](https://ffmpeg.org) can be downloaded from
[https://evermeet.cx/ffmpeg/](https://evermeet.cx/ffmpeg/)

Download the three zipped executables:

- [ffmpeg-4.3.7z](https://evermeet.cx/ffmpeg/ffmpeg-4.3.7z)
- [ffplay-4.3.7z](https://evermeet.cx/ffmpeg/ffplay-4.3.7z)
- [ffprobe-4.3.7z](https://evermeet.cx/ffmpeg/ffprobe-4.3.7z)

Just extract these files (using for example 
[The Unarchiver](https://theunarchiver.com) ) and
move the three executables 'ffmeg', 'ffplay' and 'ffprobe' into the
folder [/path/to/WirelessDisplay/ThirdParty/macOS/ffmpeg/bin/].


