#!/bin/bash

# The two command-line arguments passed to this script, when it is started
# from the C#-program. 
# ACTION ... what to do (GET the current screen-resolution, SET it, list
#            ALL available screen-resolutions on the primary display.
# SCREEN_RESOLUTION ...only used for ACTION == "SET". A String like 1024x768
ACTION=$1
SCREEN_RESOLUTION=$2


SCREENRES="../../ThirdParty/macOS/screenresolution/screenresolution"

if [ $ACTION == "GET" ]
then
    # GET the current screen-resolution using xrandr
    # In the outpout of xrandr, an asterisk * denotes the current
    # screen-resolution, the returned line must be processed further,
    # with a regular expression to return something like "1980x1040"
    CURRENT_RES=$($SCREENRES get | egrep -o '(\d+x\d+)')
    
    # Couldn't find a valid resolution? If yes, exit with error-code
    if [ -z "$CURRENT_RES" ]
    then
      exit 2
    fi

    echo ${CURRENT_RES}
    exit 0

elif [ $ACTION == "SET" ]
then
    # SET the screen-resolution.
    
    # loop over the output of 'screenresolution list' line by line. The 
    # first screen-resolutions whose width and height mathes the given
    # value is set.

    while read -r LINE
    do
        RESOLUTION=$(echo $LINE | egrep -o '(\d+x\d+)')
        if [ -n RESOLUTION ]
        then
            if [ $RESOLUTION == $SCREEN_RESOLUTION ]
            then
                #echo "Executing '$SCREENRES set $LINE'"
                $SCREENRES set $LINE
                exit 0
            fi
        fi
    done <<< "$($SCREENRES list)"
    
    # Resolution to set has not been found in available resolutions   
    exit 2
    
elif [ $ACTION == "ALL" ]
then
    # echo ALL available screen-resolutions ofd the primary display to stdout.
    
    # loop over the output of 'screenresolution list' line by line. The 
    # screen-resolutions are echoed in a modified way to stdout.

    RESOLUTION_COUNT=0

    while read -r LINE
    do
      RESOLUTION=$(echo $LINE | egrep -o '(\d+x\d+)')
      if [ -n RESOLUTION ]
      then
          echo ${RESOLUTION}
          RESOLUTION_COUNT=$(expr ${RESOLUTION_COUNT} + 1)
      fi
    done <<< "$($SCREENRES list)"
   
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



