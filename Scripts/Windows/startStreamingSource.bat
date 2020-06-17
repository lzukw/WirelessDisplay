REM Starts the streaming-source, which sends the stream to 
REM the remote streaming-sink

REM Prerequisites:
REM TODO

REM The command-line-arguments passed to this script
REM STREAMING_TYPE ...is either "VNC" or "FFmpeg"
REM IP ...is the IP-address of the computer with the streaming-sink.
REM PORT ...is the Port-Number the streaming sink (VNC-Client in 
REM         reverse-connections / FFplay) will listen on.
REM SCREEN_RESOLUTION ...is the sreen-resolution used for the stream.

STREAMING_TYPE=%1
IP=%2
PORT=%3
SCREEN_RESOLUTION=%4

IF "%STREAMING_TYPE%" == "VNC" (

)

ELSE IF "%STREAMING_TYPE%" == "FFmpeg" (


)

ELSE (
    ECHO Wrong streaming type: %STREAMING_TYPE%
    EXIT 1
)

