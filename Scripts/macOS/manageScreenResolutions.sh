#!/bin/bash

# The two command-line arguments passed to this script, when it is started
# from the C#-program. 
# ACTION ... what to do (GET the current screen-resolution, SET it, list
#            ALL available screen-resolutions on the primary display.
# SCREEN_RESOLUTION ...only used for ACTION == "SET". A String like 1024x768
ACTION=$1
SCREEN_RESOLUTION=$2

if [ $ACTION == "GET" ]
then
    # GET the current screen-resolution using xrandr
    # In the outpout of xrandr, an asterisk * denotes the current
    # screen-resolution, the returned line must be processed further,
    # with a regular expression to return something like "1980x1040"
    CURRENT_RES=$(xrandr | grep '*' | grep -oP '\K(\d*x\d*)')
    
    # Couldn't find a valid resolution? If yes, exit with error-code
    if [ -z "$CURRENT_RES" ]
    then
      exit 2
    fi

    echo ${CURRENT_RES}
    exit 0

elif [ $ACTION == "SET" ]
then
    # SET the screen-resolution of the primary display using xrandr.
    PRIMARY_DISP=$(xrandr | grep primary | cut -d " " -f 1)
    xrandr --output ${PRIMARY_DISP} --mode "${SCREEN_RESOLUTION}"
    # return with the same exit-code as the last xrandr-command
    exit $?

elif [ $ACTION == "ALL" ]
then
    # echo ALL available screen-resolutions ofd the primary display to stdout.
    
    # loop over the output of xrandr line by line. The screen-resolutions 
    # found beneath the primary-screen until the next display are echoed to 
    # stdout.

    PRIMARY_FOUND=0
    NEXT_FOUND=0
    RESOLUTION_COUNT=0

    # this loop iterates over the xrandr-output line by line
    while read -r LINE
    do
      if [ $PRIMARY_FOUND -eq 1 ] && [ $NEXT_FOUND -eq 0 ]
      then
	# find and print the screen-resoltion, that $LINE contains
        RESOLUTION=$(echo "${LINE}" | grep -oP '\K(\d+x\d+)')
        if [ -n "$RESOLUTION" ]
        then
          echo "$RESOLUTION"
	  let "RESOLUTION_COUNT++"
        else
	  # $LINE doesn't contain a resolution, so it is the xrandr-output-line
	  # for the next display
          NEXT_FOUND=1
        fi

      elif [ $PRIMARY_FOUND -eq 0 ]
      then
	# The line for the primary screen hasn't been found yet. Is this
	# line the one containting the word 'primary'?
        RESULT=$(echo "$LINE" | grep "primary" | wc -l )
        if [ $RESULT -eq 1 ]
        then
          PRIMARY_FOUND=1
        fi
      fi
    done <<< "$(xrandr)"
   
    # finally check, if there has been some output
    if [ $RESOLUTION_COUNT -eq 0 ]
    then
      exit 2
    fi
    exit 0

else
    # Invalid ACTION
    exit 1
fi



