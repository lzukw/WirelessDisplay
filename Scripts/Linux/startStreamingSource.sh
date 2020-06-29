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
  #x0vncserver -SecurityTypes=None -AcceptKeyEvents=0 -AcceptPointerEvents=0 -AcceptCutText=0 -rfbport=${PORT}

elif [ ${STREAMING_TYPE} == "VNC-Reverse" ]
then

  #x11vnc -viewonly -scale ${STREAM_SCREEN_RESOLUTION} -nopw -noxdamage -cursor arrow -scale_cursor 1 -connect ${SINK_IP}:${PORT}
  x11vnc -viewonly -scale ${STREAM_SCREEN_RESOLUTION} -nopw -noxdamage -nocursorshape -connect ${SINK_IP}:${PORT}

  # With x0vncserver no reverse-connection is possible  

elif [ ${STREAMING_TYPE} == "FFmpeg" ]
then
 
  ffmpeg -f x11grab -s ${SOURCE_SCREEN_RESOLUTION} -r 30 -i :0.0 -vf scale=${STREAM_SCREEN_RESOLUTION} -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts "udp://${SINK_IP}:${PORT}"

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
  PROFILE_INI_FILE="${HOME}/.config/obs-studio/basic/profiles/WirelessDisplayClientSide/basic.ini"

  # Let's modify the file (Values for keys BaseCX=..., BaseCY=..., OutputCX=..., 
  # OutputCY=..., amd FFURL=... are replaced)
  BASE_CX=$(echo $SOURCE_SCREEN_RESOLUTION | cut -d x -f 1)
  BASE_CY=$(echo $SOURCE_SCREEN_RESOLUTION | cut -d x -f 2)
  OUTPUT_CX=$(echo $STREAM_SCREEN_RESOLUTION | cut -d x -f 1)
  OUTPUT_CY=$(echo $STREAM_SCREEN_RESOLUTION | cut -d x -f 2)
  FFURL="${SINK_IP}:${PORT}"
  
  sed -i 's/^BaseCX=.*/BaseCX='$BASE_CX'/' $PROFILE_INI_FILE
  sed -i 's/^BaseCY=.*/BaseCY='$BASE_CY'/' $PROFILE_INI_FILE
  sed -i 's/^OutputCX=.*/OutputCX='$OUTPUT_CX'/' $PROFILE_INI_FILE
  sed -i 's/^OutputCY=.*/OutputCY='$OUTPUT_CY'/' $PROFILE_INI_FILE
  sed -i 's/^FFURL=.*/FFURL=udp:\/\/'$FFURL'/' $PROFILE_INI_FILE

  # After a first run you should review the file again, and check, if the
  # values for the above keys have been replaced correctly

  # start OBS using the correct profile and scene-collection and start
  # recording (=UDP-streaming) immediately
  obs --profile WirelessDisplayClientSide --collection WirelessDisplayClientSide --startrecording

else
  echo "Script-ERROR: Unknown Streaming Type ${STREAMING_TYPE}"
  exit 1
fi



