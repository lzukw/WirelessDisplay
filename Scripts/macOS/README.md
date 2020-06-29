# Scripts for macOS

For now, macOS can only be used for presentation-computers (WirelessDisplayClient).
So only the scripts [manageScreenResolution.sh] and [startStreamingSource.sh] are
implemented for now.

As streaming-method VNC and VNC-Reverse are not supported. At the moment, there 
doesn't seem to exist an opensource-VNC-server and -client for macOS. TightVNC
and RealVNC could probably be used, but they are not free.

The VCN-Server, that comes with macOS ("Screen sharing") can also not be used,
because:

- There is no way to use it without a password.
- It opens another login-session instead of sharing the existing screen (however,
  I'm not very sure about this).

The scripts use the following external tools:

- [../../ThirdParty/screenresolution/screenresolution] ...to manage screen-resolutions
- [../../ThirdParty/ffmpeg/bin/ffmpeg] ...as streaming-source
