@echo off
REM Starts the streaming-source, which sends the stream to 
REM the remote streaming-sink

REM Prerequisites:
REM TODO

REM The command-line-arguments passed to this script
REM STREAMING_TYPE ...is either "VNC-Reverse" or "FFmpeg"
REM SINK_IP ...is the SINK_IP-address of the computer with the streaming-sink.
REM PORT ...is the Port-Number the streaming sink (VNC-Client in 
REM         reverse-connections / FFplay) will listen on.
REM SOURCE_SCREEN_RESOLUTION ...is the screen-resolution of the local computer
REM                             (the streaming-source-computer)
REM STREAM_SCREEN_RESOLUTION ...is the sreen-resolution used for the stream.

SET STREAMING_TYPE=%1
SET SINK_IP=%2
SET PORT=%3
SET SOURCE_SCREEN_RESOLUTION=%4
SET STREAM_SCREEN_RESOLUTION=%5

IF "%STREAMING_TYPE%" == "VNC-Reverse" (
    START /B ..\..\ThirdParty\Windows\VNCopen\winvnc4.exe SecurityTypes=None AcceptPointerEvents=0 AcceptKeyEvents=0 AcceptCutText=0
    timeout /t 2 /nobreak > nul
    ..\..\ThirdParty\Windows\VNCopen\winvnc4.exe -connect %SINK_IP%::%PORT%
    REM Hang her forever
    PAUSE

) ELSE IF "%STREAMING_TYPE%" == "FFmpeg" ( 
    ..\..\ThirdParty\Windows\ffmpeg\bin\ffmpeg.exe -f gdigrab -i desktop -r 30 -vf scale=%STREAM_SCREEN_RESOLUTION% -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts udp://%SINK_IP%:%PORT%

) ELSE (
    ECHO Wrong streaming type: %STREAMING_TYPE%
    EXIT 1
)

