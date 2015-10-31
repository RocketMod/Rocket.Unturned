#!/bin/bash
# This script starts a Unturned 3 server on Linux machines
# Syntax: start.sh <instance name>
# Author: fr34kyn01535

export DISPLAY=:0.0

INSTANCE_NAME=$1
UNTURNED_HOME="./unturned/"


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

printf "Steam: "
if ! screen -list | grep -q "Steam"; then
	screen -dmS Steam bash -c "export DISPLAY=:0.0 ; steam"
	printf "${YELLLOW}STARTING${NC}\n"
    sleep 10
else
	printf "${GREEN}RUNNING${NC}\n"
fi

echo ""

cd $UNTURNED_HOME

if [ -f RocketLauncher.exe ]; then
	ulimit -n 2048
	mono RocketLauncher.exe $INSTANCE_NAME
else
	echo "RocketLauncher not found."
fi
