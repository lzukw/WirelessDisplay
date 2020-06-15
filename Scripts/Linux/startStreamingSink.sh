#!/bin/bash

# The command-line-arguments passed to this script
# STREAMING_TYPE is either "VNC" or "FFmpeg"
# PORT is the Port-Number the streaming sink (VNC-Client in 
#         reverse-connections / FFplay) will listen on.

STREAMING_TYPE=$1
PORT=$2

if [ ${STREAMING_TYPE} == "VNC" ]
then

  # Start VNC-viewer in reverse connection, listening on PORT
  # On Debian-based-systems xtightvncviewer is used
  xtightvncviewer -fullscreen -nocursorshape -viewonly -listen $(expr ${PORT} - 5500)

  # On Fedora, xtightvncviewer is not available, so use tigervnc
  # tigervnc is used (Fedora 31), but the arguments seem to be valid for other 
  # vncviewers as well
  #vncviewer -Shared -FullScreen -ViewOnly -listen ${PORT}

elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then
  ffplay -fs -an -sn -fflags nobuffer -i udp://0.0.0.0:${PORT}

else
  
  # Wrong STREAMING_TYPE
  exit 2
fi

