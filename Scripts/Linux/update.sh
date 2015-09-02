#!/bin/bash
# This script installs / updates steamcmd and Unturned 3 on Linux machines
# Syntax: update.sh <steam username> <steam password>
# Author: fr34kyn01535
# Note: To make sure Steam Guard is not bugging you better create a new Steam account and disable Steam Guard

# Before you install the Unturned 3 server on linux make sure the following packages are installed:

#apt-get install screen htop unzip                    				     # Utils
#apt-get install libmono2.0-cil mono-runtime          				     # Mono                                           
#apt-get install Xorg xinit x11-common                			             # Headless X server
#apt-get install libglu1-mesa libxcursor1 libxrandr2  			             # Native 64 bit Unity 3D prerequisites	
#apt-get install libc6:i386 libgl1-mesa-glx:i386 libxcursor1:i386 libxrandr2:i386    # 32 bit prerequisites for Unity 3D

#Unity 3D requires a xserver to run, start it with:
#screen -S XServer startx

STEAM_USERNAME=$1
STEAM_PASSWORD=$2
STEAMCMD_HOME="./steamcmd"
UNTURNED_HOME="./unturned"

mkdir -p $STEAMCMD_HOME
mkdir -p $UNTURNED_HOME

cd $STEAMCMD_HOME
if [ ! -f "steamcmd.sh" ]; then
	wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz
	tar -xvzf steamcmd_linux.tar.gz
	rm steamcmd_linux.tar.gz
fi

./steamcmd.sh +@sSteamCmdForcePlatformBitness 32 +login $STEAM_USERNAME $STEAM_PASSWORD +force_install_dir ../unturned +app_update 304930 validate +exit
# The Unturned 64 bit build seems to not work for some reason...
