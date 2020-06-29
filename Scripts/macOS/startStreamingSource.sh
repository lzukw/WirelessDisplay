#!/bin/bash

# Starts the streaming-source, which sends the stream to 
# the remote streaming-sink

# The command-line-arguments passed to this script
# STREAMING_TYPE ...on macOS only "FFmpeg" is supported for now
# SINK_IP ...is the IP-address of the computer with the streaming-sink.
# PORT ...is the Port-Number the streaming sink (FFplay) will listen on.
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
  echo "Streaming type '${STREAMING_TYPE}' is not supported on macOS."
  exit 1

elif [ ${STREAMING_TYPE} == "VNC-Reverse" ]
then

  echo "Streaming type '${STREAMING_TYPE}' is not supported on macOS."
  exit 1

elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then

  # Screengrabbing on macOS is done with the AVFoundation input device.
  # see https://ffmpeg.org/ffmpeg-devices.html#avfoundation
  # Maybe "Capture screen 0" has to be changed. You can list all
  # AVFoundation supported devices by running:
  # ffmpeg -f avfoundation -list_devices true -i ""

  ../../ThirdParty/macOS/ffmpeg/bin/ffmpeg -f avfoundation -i "Capture screen 0"  -vf scale=${STREAM_SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${SINK_IP}:${PORT}"

else
  echo "Script-ERROR: Unknown Streaming Type ${STREAMING_TYPE}"
  exit 1
fi



