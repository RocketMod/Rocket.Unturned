#!/bin/bash

unturnedHome="../unturned"
updatingCache="$unturnedHome/updating.cache"
checkingCache="buildid.cache"

declare -i oldbuild=`cat $checkingCache`
declare -i currentbuild=`./steamcmd.sh +login anonymous +app_info_update 1 +app_info_print 304930 +quit | grep -e "buildid" -m 1 | tr "\"buildid\"" " " | sed -r 's/\s*(.*?)\s*$/\1/'`

if [ ! -f "$updatingCache" ]
then
	touch $updatingCache
	if [ "$oldbuild" -ne "$currentbuild" ]
	then
		screen -S Rocket -X stuff "shutdown $(echo -ne '\r')"
		sleep 10
		wget http://api.rocketmod.net/download/unturned-linux/latest/DOCKER/Rocket.zip
		unzip -o Rocket.zip $unturnedHome/Unturned_Data/Managed
		rm Rocket.zip
		./steamcmd.sh +@sSteamCmdForcePlatformBitness 32 +login unturnedrocksupdate force_update +force_install_dir $unturnedHome +app_update 304930 validate +exit
		echo $currentbuild > $checkingCache
	fi
	rm $updatingCache
fi