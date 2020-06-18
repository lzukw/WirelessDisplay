REM @echo off
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

SET STREAMING_TYPE=%1
SET IP=%2
SET PORT=%3
SET SCREEN_RESOLUTION=%4

IF "%STREAMING_TYPE%" == "VNC" (
    START /B ..\..\ThirdParty\Windows\VNC4\winvnc4.exe SecurityTypes=None AcceptPointerEvents=0 AcceptKeyEvents=0 AcceptCutText=0
    timeout /t 2 /nobreak > nul
    ..\..\ThirdParty\Windows\VNC4\winvnc4.exe -connect %IP%::%PORT%
    REM Hang her forever
    PAUSE

) ELSE IF "%STREAMING_TYPE%" == "FFmpeg" ( 
    ..\..\ThirdParty\Windows\ffmpeg\bin\ffmpeg.exe -f gdigrab -i desktop -r 30 -vf scale=%SCREEN_RESOLUTION% -vcodec libx264 -pix_fmt yuv420p -profile:v baseline -tune zerolatency -preset ultrafast -f mpegts udp://%IP%:%PORT%

) ELSE (
    ECHO Wrong streaming type: %STREAMING_TYPE%
    EXIT 1
)

