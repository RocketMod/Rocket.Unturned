#!/bin/bash
# This script starts a Unturned 3 server on Linux machines
# Syntax: start.sh <instance name>
# Author: fr34kyn01535

export DISPLAY=:0.0

INSTANCE_NAME=$1
UNTURNED_HOME="./unturned"

if [ -f $UNTURNED_HOME"/RocketLauncher.exe" ]; then
	cd $UNTURNED_HOME
	ulimit -n 2048
	mono RocketLauncher.exe $INSTANCE_NAME
else
	echo "RocketLauncher not found"
fi
