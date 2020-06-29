# Third-party-tools for macOS

## Prerequisites

### Install developer-tools

Open a terminal and just to try, type `git`. If this prompts you to install
commandline-developer-tools, the do so.

### Install dotnet core 3.1 sdk

Browse to the [dotnet-core-3.1 download-site](https://dotnet.microsoft.com/download/dotnet-core/3.1)
and download the installer for macOS (64-Bit): [dotnet-sdk-3.1.nnn-osx-x64.pkg].
Install the SDK by double-clicking the downloaded file.


## screenresolution

The tool (screenresolution)[https://github.com/jhford/screenresolution]
is needed for managing screen-resolutions. It could be installed, but
there it can also be used as a "portable" tool.

First build the executable:

```
git clone https://github.com/lzukw/build-screenresolution.git
cd screenresolution
make build
```

Then copy or move the executable file (screenresolution) to the folder
[WirelessDisplay/ThirdParty/macOs/]



