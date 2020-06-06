#!/usr/bin/bash

IP=127.0.0.1
PORT=6007


echo ""
echo "=================================================="
echo "Testing ScriptingRestApiServer with curl-commands:"
echo "=================================================="
echo ""
echo "Starting Server and waiting 15 seconds for it."

gnome-terminal -- bash helper_script_to_start_server.sh $IP $PORT 
sleep 15

echo ""
echo "1. Testing Run-script-API:"
echo "============================="
echo "The response should be successfull, returning two arguments in stdout"
echo "and two in stderr. Exit-code should be 5."
echo ""

curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ScriptName" : "testscript_for_run", "ScriptArgs" : "arg1 arg2 arg3 arg4", "StdIn" : "\n" }' http://$IP:$PORT/api/ScriptRunner/RunScript

echo ""
echo ""
echo "2. Testing Start-script-API":
echo "==========================="
echo "Should return success and the process-ID of the started shell"
echo ""

RES=$(curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ScriptName" : "testscript_for_start_stop", "ScriptArgs" : "arg1 arg2 arg3 arg4", "StdIn" : "\n" }' http://$IP:$PORT/api/ScriptRunner/StartScript)
echo $RES
PROCESS_ID=$(echo $RES | grep -Eo '[0-9]+')
echo "Found Process-ID: $PROCESS_ID"

echo ""
echo ""
echo "3. Testing IsRunning-API:"
echo "========================="
echo "Should return success and IsRunning==true"
echo ""

curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data "{ \"ProcessId\" : $PROCESS_ID }" http://$IP:$PORT/api/ScriptRunner/IsScriptRunning

echo ""
echo ""
echo "4. Testing StopScript-API:"
echo "=========================="
echo "Should return success and the output of the script (two arguments in"
echo "stdout and two in stderr and and exit-code something like 137 (the "
echo "exit-code from a killed sleep-command)."
echo ""
curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data "{ \"ProcessId\" : $PROCESS_ID }" http://$IP:$PORT/api/ScriptRunner/StopScript

echo ""
echo ""
echo "Tests finished. Verify output, if they were successfull."
echo ""
