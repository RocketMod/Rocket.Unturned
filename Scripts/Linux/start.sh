#!/bin/bash
# This script starts a Unturned 3 server on Linux machines
# Syntax: start.sh <instance name>
# Author: fr34kyn01535

export DISPLAY=:0.0

INSTANCE_NAME=$1
STEAMCMD_HOME="./steamcmd"
UNTURNED_HOME="./unturned"

YELLLOW='\033[0;33m'
GREEN='\033[0;32m'
NC='\033[0m'

printf "XServer: "
if ! screen -list | grep -q "XServer"; then
    screen -dmS XServer startx
	printf "${YELLLOW}STARTING${NC}\n"
	sleep 3
else
	printf "${GREEN}RUNNING${NC}\n"
fi


STEAMCMD_API=$STEAMCMD_HOME/linux32/steamclient.so
UNTURNED_API=$UNTURNED_HOME/Unturned_Data/Plugins/x86/steamclient.so

printf "Steam: "
if [ -f $STEAMCMD_API ]; then
	if diff $STEAMCMD_API $UNTURNED_API >/dev/null ; then
		printf "${GREEN}UP TO DATE${NC}\n"
	else
		cp $STEAMCMD_API $UNTURNED_API
		printf "${YELLLOW}UPDATING${NC}\n"
	fi
fi

echo ""

cd $UNTURNED_HOME

if [ -f RocketLauncher.exe ]; then
	ulimit -n 2048
	mono RocketLauncher.exe $INSTANCE_NAME
else
	echo "RocketLauncher not found."
fi
