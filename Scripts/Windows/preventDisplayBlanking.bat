REM Prevent the display from blanking

REM Prerequistes:
REM TODO

REM The command-line argument passed to this script, when it is started
REM from the C#-program: 
REM SECONDS_TO_PREVENT ... The program shuts down after that amount of seconds

SET SECONDS_TO_PREVENT=%1

..\..\ThirdParty\Windows\PreventTurnOffDisplay\PreventTurnOffDisplay.exe %SECONDS_TO_PREVENT%
