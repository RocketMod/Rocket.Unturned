#!/bin/bash
# This script installs / updates steamcmd and Unturned 3 on Linux machines
# Syntax: update.sh <steam username> <steam password>
# Author: fr34kyn01535
# Note: To make sure Steam Guard is not bugging you better create a new Steam account and disable Steam Guard
# Install the prerequisites for Unturned 3 on Linux with apt-get install libglu1-mesa libxcursor1 libxrandr2 unzip 

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

./steamcmd.sh +login $STEAM_USERNAME $STEAM_PASSWORD +force_install_dir ../unturned +app_update 304930 validate +exit