@echo off

REM The command-line-arguments passed to this script
SET STREAMING_TYPE=%1
SET PORT_NO=%2

IF "%STREAMING_TYPE%" == "VNC" (
    REM The call to vncviewer.exe returns immediately but START /W waits for 
	REM the process to finish
    START /W ..\..\ThirdParty\tightvnc-1.3.10_x86\vncviewer.exe /shared /fullscreen /restricted /viewonly /disableclipboard /nocursorshape /listen %PORT_NO%
	
)

IF "%STREAMING_TYPE%" == "FFmpeg" (
	..\..\ThirdParty\ffmpeg-4.2.2-win64-static\bin\ffplay.exe -fs -an -sn -fflags nobuffer -i udp://0.0.0.0:%PORT_NO%
)
