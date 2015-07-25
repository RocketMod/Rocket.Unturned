#!/bin/bash
# This script starts a Unturned 3 server on Linux machines
# Syntax: start.sh <instance name>
# Author: fr34kyn01535

# Before you start the Unturned 3 server on linux make sure the following packages are installed:
#htop unzip # Utils                                                                           
#screen Xorg xinit x11-common # Headless X server
#libmono2.0-cil mono-runtime # Mono 2
#libglu1-mesa libxcursor1 libxrandr2 libc6:i386 libgl1-mesa-glx:i386 libxcursor1:i386 libxrandr2:i386 # 32/64 bit prerequisites for unity3d

#Untunred 3 requires a xserver to run
export DISPLAY=:0.0


INSTANCE_NAME=$1
UNTURNED_HOME="./unturned/"
RocketLauncher="RocketLauncher"

if [ ! -f $UNTURNED_HOME$RocketLauncher ]; then #Well, somebody can't configure bash scripts...
	if [ -f ../../../install_304930.vdf ]; then 
		UNTURNED_HOME="../../../"
		if [ ! -f $UNTURNED_HOME$RocketLauncher ]; then
			if [ -f RocketLauncher ]; then
				mv RocketLauncher $UNTURNED_HOME$RocketLauncher
			fi
		fi
	fi
fi


if [ -f $UNTURNED_HOME$RocketLauncher ]; then
	cd $UNTURNED_HOME
	ulimit -n 2048
	mono RocketLauncher $UNTURNED_HOME
else
	echo "RocketLauncher not found"
fi