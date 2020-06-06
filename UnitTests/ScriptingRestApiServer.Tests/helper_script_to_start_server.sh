#!/bin/bash

IP=$1
PORT=$2

cd ../../ScriptingRestApiServer; 
dotnet run -- --urls=http://$IP:$PORT
