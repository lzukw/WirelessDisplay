@echo off

REM The command-line-arguments passed to this script
SET ACTION=%1
SET RESOLUTION=%2

REM This script uses screenres.exe to mainpulate screen-resolutions

REM Get current screen-resolution and echo it to stdout. The output of 
REM screenres.exe /V /S is something like 1280x918, 32 bits @ 64 Hz.
REM See https://ss64.com/nt/for_cmd.html for how the FOR /F - loop works
IF "%ACTION%" == "GET" (
    FOR /F "DELIMS=, TOKENS=1" %%G IN ('..\..\ThirdParty\Windows\ScreenRes\screenres.exe /V /S') DO echo %%G
)

REM Echo all available screen-resolitions of the primary screen to stdout.
IF "%ACTION%" == "ALL" (
    FOR /F "DELIMS=, TOKENS=1" %%G IN ('..\..\ThirdParty\Windows\ScreenRes\screenres.exe /V /L') DO echo %%G
)

REM Set screen-resolution to given value
REM The given parameter is like "1024x768" and has to be split using 'x' as 
REM delimiter.
IF "%ACTION%" == "SET" (
    FOR /F "tokens=1,2 delims=x" %%i IN ('echo %RESOLUTION%') DO ..\..\ThirdParty\Windows\ScreenRes\screenres.exe /V /X:%%i /Y:%%j
)

