#!/usr/bin/bash

ARG1=$1
ARG2=$2
ARG3=$3
ARG4=$4

# Write two lines to stdout
echo "$ARG1"
echo "$ARG2"

read fromstdin
echo $fromstdin

# Write two lines to stderr
echo "$ARG3" 1>&2
echo "$ARG4" 1>&2

# simulate starting infinitly long lasting program
sleep 10000

# For testing, don't use standard exit code
exit 5

