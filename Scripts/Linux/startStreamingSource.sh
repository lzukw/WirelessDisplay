#!/bin/bash

# Starts the streaming-source, which sends the stream to 
# the remote streaming-sink

# Prerequisites: x11vnc must be installed.

# The command-line-arguments passed to this script
# STREAMING_TYPE ...is either "VNC-Reverse" or "FFmpeg"
# SINK_IP ...is the IP-address of the computer with the streaming-sink.
# PORT ...is the Port-Number the streaming sink (VNC-Client in 
#         reverse-connections / FFplay) will listen on.
# SOURCE_SCREEN_RESOLUTION ...is the screen-resolution of the local computer
#                            (the streaming-source-computer)
# STREAM_SCREEN_RESOLUTION ...is the sreen-resolution used for the stream.

STREAMING_TYPE=$1
SINK_IP=$2
PORT=$3
SOURCE_SCREEN_RESOLUTION=$4
STREAM_SCREEN_RESOLUTION=$5

echo "startStreamingSource.sh called with arguments STREAMING_TYPE=${STREAMING_TYPE=}, SINK_IP=${SINK_IP}, PORT=${PORT}, WxH_SENDER=${WxH_SENDER}, STREAM_SCREEN_RESOLUTION=${STREAM_SCREEN_RESOLUTION}"

if [ ${STREAMING_TYPE} == "VNC" ]
then

  # debian: sudo apt-get install x11vnc
  # fedora: sudo apt-get install x11vnc
  x11vnc -viewonly -scale ${STREAM_SCREEN_RESOLUTION} -nopw -noxdamage -nocursorshape -rfbport ${PORT}

  # Alternatively use tigervnc's x0vncserver
  # debian: sudo apt-get install tigervnc-scraping-server
  # fedora: sudo dnf install tigervnc-server
  # x0vncserver -SecurityTypes=None -AcceptKeyEvents=0 -AcceptPointerEvents=0 -AcceptCutText=0 -rfbport=${PORT}

elif [ ${STREAMING_TYPE} == "VNC-Reverse" ]
then

  #x11vnc -viewonly -scale ${STREAM_SCREEN_RESOLUTION} -nopw -noxdamage -cursor arrow -scale_cursor 1 -connect ${SINK_IP}:${PORT}
  x11vnc -viewonly -scale ${STREAM_SCREEN_RESOLUTION} -nopw -noxdamage -nocursorshape -connect ${SINK_IP}:${PORT}

  # With x0vncserver no reverse-connection is possible  

elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then
 
  ffmpeg -f x11grab -s ${SOURCE_SCREEN_RESOLUTION} -r 30 -i :0.0 -vf scale=${STREAM_SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${SINK_IP}:${PORT}"

else
  echo "Script-ERROR: Unknown Streaming Type ${STREAMING_TYPE}"
  exit 1
fi



