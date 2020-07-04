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

  ../../ThirdParty/macOS/ffmpeg/bin/ffmpeg -f avfoundation -i "Capture screen 0" -capture_cursor -capture_mouse_clicks -vf scale=${STREAM_SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${SINK_IP}:${PORT}"

elif [ ${STREAMING_TYPE} == "OBS" ]
then

  # Preparations for using OBS as streaming-source:
  # Start OBS, and create a new profile named "WirelessDisplayClientSide"
  # 
  # Go to Settings >> Output. Set Output-Mode to "Advanced". Then switch to 
  # "Recording"-Tab (Yes, for streaming via udp one uses "Recording").
  # - Type: Custom output (FFmpeg)
  # - File path or URL: udp://192.168.1.20:5500   ...this ip-Address will be
  #                                                  changed by this sript
  # - Container-Format: mpegts
  # - Video-Bitrate: 2500 kBit/s ...or maybe lower 
  # - Keyframe-interval: 60   ...each 60 frames a keyframe is sent (every 2s)
  # - Video-Encoder: mpeg2video (Default-Encoder)
  # - Audio-Bitrate: 160kBps
  # - Audio-Encoder: mp2 (Default-Encoder)
  #
  # Go to Settings >> Video. 
  # - Observe the values for the Base (Canvas)-Resolution and the 
  #   Output (Scaled) Resolution. These values will be changed by this script.
  # - Set "Common FPS-Values" (framerate) to 30.
  # 
  # Create a new scene-collection also named "WirelessDisplayClientSide".
  # Set up scenes for the presentation (normally just one scene named "main"
  # with one screen-capture-source is sufficient. Adjust audio inputs as needed.
  # 
  # At least, inspect the following file. It should contain all the above
  # settings (You can use File >> Show Profile Folder)
  PROFILE_INI_FILE="${HOME}/Library/Application Support/obs-studio/basic/profiles/WirelessDisplayClientSide/basic.ini"

  # Let's modify the file (Values for keys BaseCX=..., BaseCY=..., OutputCX=..., 
  # OutputCY=..., amd FFURL=... are replaced)
  BASE_CX=$(echo $SOURCE_SCREEN_RESOLUTION | cut -d x -f 1)
  BASE_CY=$(echo $SOURCE_SCREEN_RESOLUTION | cut -d x -f 2)
  OUTPUT_CX=$(echo $STREAM_SCREEN_RESOLUTION | cut -d x -f 1)
  OUTPUT_CY=$(echo $STREAM_SCREEN_RESOLUTION | cut -d x -f 2)
  FFURL="${SINK_IP}:${PORT}"
  
  sed -e 's/^BaseCX=.*/BaseCX='$BASE_CX'/' -i "bak" "$PROFILE_INI_FILE"
  sed -e 's/^BaseCY=.*/BaseCY='$BASE_CY'/' -i "bak" "$PROFILE_INI_FILE"
  sed -e 's/^OutputCX=.*/OutputCX='$OUTPUT_CX'/' -i "bak" "$PROFILE_INI_FILE"
  sed -e 's/^OutputCY=.*/OutputCY='$OUTPUT_CY'/' -i "bak" "$PROFILE_INI_FILE"
  sed -e 's/^FFURL=.*/FFURL=udp:\/\/'$FFURL'/' -i "bak" "$PROFILE_INI_FILE"

  # After a first run you should review the file again, and check, if the
  # values for the above keys have been replaced correctly

  # start OBS using the correct profile and scene-collection and start
  # recording (=UDP-streaming) immediately
  /Applications/OBS.app/Contents/MacOS/obs --profile WirelessDisplayClientSide --collection WirelessDisplayClientSide --startrecording

else
  echo "Script-ERROR: Unknown Streaming Type ${STREAMING_TYPE}"
  exit 1
fi



