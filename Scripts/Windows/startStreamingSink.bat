@echo off

REM The command-line-arguments passed to this script
SET STREAMING_TYPE=%1
SET SOURCE_IP=%2
SET PORT=%3
SET SINK_SCREEN_RESOLUTION=%4
SET STREAM_SCREEN_RESOLUTION=%5

IF "%STREAMING_TYPE%" == "VNC-Reverse" (
    REM The call to vncviewer.exe returns immediately but START /W waits for 
	REM the process to finish
    REM START /W ..\..\ThirdParty\tightvnc-1.3.10_x86\vncviewer.exe /shared /fullscreen /restricted /viewonly /disableclipboard /nocursorshape /listen %PORT%
    ..\..\ThirdParty\Windows\VNCopen\vncviewer.exe -console SendKEyEvents=0 SendPointerEvents=0 FullScreen=1 UseLocalCursor=0 Listen=1 %PORT%
	
) ELSE IF "%STREAMING_TYPE%" == "FFmpeg" (
	..\..\ThirdParty\Windows\ffmpeg\bin\ffplay.exe -fs -an -sn -fflags nobuffer -i udp://0.0.0.0:%PORT%
	
) ELSE (
    ECHO "Wrong streaming type"
    EXIT 1
)

