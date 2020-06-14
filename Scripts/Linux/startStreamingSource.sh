#!/bin/bash

# Starts the streaming-source, which sends the stream to 
# the remote streaming-sink

# Prerequisites: x11vnc must be installed.

# The command-line-arguments passed to this script
# STREAMING_TYPE ...is either "VNC" or "FFmpeg"
# IP ...is the IP-address of the computer with the streaming-sink.
# PORT ...is the Port-Number the streaming sink (VNC-Client in 
#         reverse-connections / FFplay) will listen on.
# SCREEN_RESOLUTION ...is the sreen-resolution used for the stream.

STREAMING_TYPE=$1
IP=$2
PORT=$3
SCREEN_RESOLUTION=$4

echo "startStreamingSource.sh called with arguments STREAMING_TYPE=${STREAMING_TYPE=}, IP=${IP}, PORT=${PORT}, WxH_SENDER=${WxH_SENDER}, SCREEN_RESOLUTION=${SCREEN_RESOLUTION}"

if [ ${STREAMING_TYPE} == "VNC" ]
then

  echo "Executing: 'x11vnc -viewonly -scale ${SCREEN_RESOLUTION} -nopw -noxdamage -cursor arrow -scale_cursor 1 -connect ${IP}:${PORT}'"
  x11vnc -viewonly -scale ${SCREEN_RESOLUTION} -nopw -noxdamage -cursor arrow -scale_cursor 1 -connect ${IP}:${PORT}


elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then
 
  # Find out current screen-resolution. After this command WxH_SENDER is something
  # like "1980x1200". 
  WxH_SENDER=$(xrandr | grep '*' | grep -oP '\K(\d*x\d*)')

  # Now start ffmpeg
  echo "Executing: 'fmpeg -f x11grab -s ${WxH_SENDER} -r 30 -i :0.0 -vf scale=${SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${IP}:${PORT}"'"

  ffmpeg -f x11grab -s ${WxH_SENDER} -r 30 -i :0.0 -vf scale=${SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${IP}:${PORT}"


else
  echo "Script-ERROR: Unknown Streaming Type ${STREAMING_TYPE}"
  exit 1
fi



