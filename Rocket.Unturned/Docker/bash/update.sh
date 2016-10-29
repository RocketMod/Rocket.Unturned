#!/bin/bash
# This script installs / updates steamcmd and Unturned 3 on Linux machines
# Syntax: update.sh
# Author: fr34kyn01535 & Cory Redmond
# Note: To make sure Steam Guard is not bugging you better create a new Steam account and disable Steam Guard

STEAM_USER=$( cat ~/.steam_user )
STEAM_PASS=$( cat ~/.steam_pass )
ROCKET_UUID=$( cat ~/.rocket_id )
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

./steamcmd.sh +@sSteamCmdForcePlatformBitness 32 +login "$STEAM_USER" "$STEAM_PASS" +force_install_dir ../unturned +app_update 304930 validate +exit
# The Unturned 64 bit build seems to not work for some reason...

rm -fR .update_rocket
mkdir .update_rocket
cd .update_rocket/
wget -N -q "http://api.rocketmod.net/download/unturned-linux/latest/$ROCKET_UUID/Rocket.zip"
unzip Rocket.zip
mv *.dll ../../unturned/Unturned_Headless_Data/Managed/
mv *.exe ../../unturned/
