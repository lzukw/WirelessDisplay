#!/bin/bash

# Prerequistes: xdotool must be installed:
# sudo apt-get install xdotool
# sudo dnf install xdotool

# The command-line argument passed to this script, when it is started
# from the C#-program: 
# SECONDS_TO_PREVENT ... The program shuts down after that amount of seconds

SECONDS_TO_PREVENT=$1

LOOPS=`expr ${SECONDS_TO_PREVENT} / 10`

# move mouse to origin
xdotool mousemove 0 0

# each interation takes 10 seconds, so all iterations last 
# $SECONDS_TO_PREVENT seconds.
for i in `seq 1 $LOOPS`
do
    # move mouse-pounter 1 pixel to the right
    xdotool mousemove_relative 1 0
    sleep 1
    # move mouse-pounter 1 pixel to the left. For negative
    # numbers -- must be used.
    xdotool mousemove_relative -- -1 0
    sleep 9
done