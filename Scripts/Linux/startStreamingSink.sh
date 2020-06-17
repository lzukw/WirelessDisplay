#!/bin/bash

# The command-line-arguments passed to this script
# STREAMING_TYPE is either "VNC" or "FFmpeg"
# PORT is the Port-Number the streaming sink (VNC-Client in 
#         reverse-connections / FFplay) will listen on.

STREAMING_TYPE=$1
PORT=$2

if [ ${STREAMING_TYPE} == "VNC" ]
then

  # tigervnc is used, as this seems to be available on most Linux-distros:
  # Fedora (31): sudo dnf install tigervnc
  # debian: sudo apt-get install tigervnc-viewer
  #vncviewer -Shared -FullScreen -ViewOnly -listen ${PORT}
  vncviewer -ViewOnly -SecurityTypes=None -FullScreen -AlertOnFatalError=0 -listen ${PORT}

elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then
  ffplay -fs -an -sn -fflags nobuffer -i udp://0.0.0.0:${PORT}

else
  
  # Wrong STREAMING_TYPE
  exit 2
fi

