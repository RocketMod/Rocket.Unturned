#!/bin/bash
function package::update_steamcmd() {

	if [ ! -d "/data/steamcmd" ]; then
		mkdir /data/steamcmd
	fi

	printf "Updating steamcmd..\n"

	wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz -O /data/steamcmd/steamcmd.tar.gz
	tar -xvzf /data/steamcmd/steamcmd.tar.gz -C /data/steamcmd
	rm -f /data/steamcmd/steamcmd.tar.gz
}

function package::update_rocket() {

	if [ ! -d "/data/rocket" ]; then
		mkdir /data/rocket
	fi

	printf "Updating rocket..\n"

	wget https://ci.rocketmod.net/job/Rocket.Unturned/lastSuccessfulBuild/artifact/Rocket.Unturned/bin/Release/Rocket.zip -O /data/rocket/rocket.zip
	unzip -o /data/rocket/rocket.zip -d /data/rocket
	rm -f /data/rocket/rocket.zip

	if [ ! -d "/data/unturned" ]; then
		package::unturned
	fi

	if [ -d "/data/unturned/Modules" ]; then
		rm -rf /data/unturned/Modules
	fi

	mv -f /data/rocket/Modules /data/unturned
	mv -f /data/rocket/Scripts/Linux/RocketLauncher.exe /data/unturned
	chmod +x /data/unturned/RocketLauncher.exe
}

function package::update_unturned() {

	printf "Updating Unturned..\n"

	package::get_steam_user

	/data/steamcmd/steamcmd.sh +@sSteamCmdForcePlatformBitness 32 +login "${STEAM_USERNAME}" "${STEAM_PASSWORD}" +force_install_dir "/data/unturned" +app_update 304930 +exit
}

function package::start_server() {

	if [ ! -f "/data/unturned/start_server.sh" ]; then
		mv /data/start_server.sh /data/unturned/start_server.sh
	fi

	printf "Starting server now..\n"
	/data/unturned/start_server.sh
}

function package::get_steam_user() {

	if [ -z ${STEAM_USERNAME+x} ]; then
		printf "Error: STEAM_USERNAME not defined!\n";
		exit
	fi

	if [ -z ${STEAM_PASSWORD+x} ]; then
		printf "Error: STEAM_PASSWORD not defined!\n";
		exit
	fi
}