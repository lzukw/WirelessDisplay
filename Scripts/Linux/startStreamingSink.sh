#!/bin/bash

# The command-line-arguments passed to this script
# STREAMING_TYPE is either "VNC-Reverse" or "FFmpeg"
# PORT is the Port-Number the streaming sink (VNC-Client in 
#         reverse-connections / FFplay) will listen on.

STREAMING_TYPE=$1
SOURCE_IP=$2
PORT=$3
SINK_SCREEN_RESOLUTION=$4
STREAM_SCREEN_RESOLUTION=$5

if [ ${STREAMING_TYPE} == "VNC" ]
then

  # tigervnc is used, as this seems to be available on most Linux-distros:
  # Fedora (31): sudo dnf install tigervnc
  # debian: sudo apt-get install tigervnc-viewer
  while true
  do
    # Note: the following line doesn't work with TigerVNC viewer version 1.7.0
    # but it works with verison 1.10.1
    vncviewer -ViewOnly -SecurityTypes=None -FullScreen -AlertOnFatalError=0 ${SOURCE_IP}::${PORT}
    sleep 2
  done

elif [ ${STREAMING_TYPE} == "VNC-Reverse" ]
then

  vncviewer -ViewOnly -SecurityTypes=None -FullScreen -AlertOnFatalError=0 -listen ${PORT}

elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then
  ffplay -fs -an -sn -fflags nobuffer -i udp://0.0.0.0:${PORT}

elif [ ${STREAMING_TYPE} == "OBS" ]
then
  ffplay -fs -sn -fflags nobuffer -i udp://0.0.0.0:${PORT}

else
  
  # Wrong STREAMING_TYPE
  exit 2
fi

