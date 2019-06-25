#!/bin/bash
# This script installs / updates steamcmd and Unturned 3 on Linux machines
# Syntax: update.sh
# Author: fr34kyn01535

STEAMCMD_HOME="$PWD/../steamcmd"
UNTURNED_HOME="$PWD"

mkdir -p $STEAMCMD_HOME
mkdir -p $UNTURNED_HOME

cd $STEAMCMD_HOME
if [ ! -f "steamcmd.sh" ]; then
	wget http://media.steampowered.com/client/steamcmd_linux.tar.gz
	tar -xvzf steamcmd_linux.tar.gz
	rm steamcmd_linux.tar.gz
fi

./steamcmd.sh +login anonymous +force_install_dir $UNTURNED_HOME +app_update 1110390 validate +exit
