#!/bin/bash
# This script starts a Unturned 3 server on Linux machines
# Syntax: start.sh <instance name>
# Author: fr34kyn01535

#Unity 3D requires a xserver to run, open it with startx in another screen
export DISPLAY=:0.0


INSTANCE_NAME=$1
UNTURNED_HOME="./unturned/"
LAUNCHER="RocketLauncher.exe"

if [ ! -f $UNTURNED_HOME$LAUNCHER ]; then #Well, somebody can't configure bash scripts...
	if [ -f ../../../install_304930.vdf ]; then 
		UNTURNED_HOME="../../../"
		if [ ! -f $UNTURNED_HOME$LAUNCHER ]; then
			if [ -f RocketLauncher ]; then
				mv RocketLauncher $UNTURNED_HOME$LAUNCHER
			fi
		fi
	fi
fi


if [ -f $UNTURNED_HOME$LAUNCHER ]; then
	cd $UNTURNED_HOME
	ulimit -n 2048
	mono RocketLauncher.exe $INSTANCE_NAME
else
	echo "RocketLauncher not found"
fi
