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

if [ ${STREAMING_TYPE} == "VNC-Reverse" ]
then

  echo "Executing: 'x11vnc -viewonly -scale ${STREAM_SCREEN_RESOLUTION} -nopw -noxdamage -cursor arrow -scale_cursor 1 -connect ${SINK_IP}:${PORT}'"
  x11vnc -viewonly -scale ${STREAM_SCREEN_RESOLUTION} -nopw -noxdamage -cursor arrow -scale_cursor 1 -connect ${SINK_IP}:${PORT}


elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then
 
  # Now start ffmpeg
  echo "Executing: 'fmpeg -f x11grab -s ${SOURCE_SCREEN_RESOLUTION} -r 30 -i :0.0 -vf scale=${STREAM_SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${SINK_IP}:${PORT}"'"

  ffmpeg -f x11grab -s ${SOURCE_SCREEN_RESOLUTION} -r 30 -i :0.0 -vf scale=${STREAM_SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${SINK_IP}:${PORT}"


else
  echo "Script-ERROR: Unknown Streaming Type ${STREAMING_TYPE}"
  exit 1
fi



