REM Prevent the screensaver from activation

REM Prerequistes:
REM TOD

REM The command-line argument passed to this script, when it is started
REM from the C#-program: 
REM SECONDS_TO_PREVENT ... The program shuts down after that amount of seconds

SET SECONDS_TO_PREVENT=%1

REM This is just a fake, until a program is available
timeout /t %SECONDS_TO_PREVENT% /nobreak > nul
