#!/bin/bash
steamclient="/data/steamcmd/linux32/steamclient.so"
usteamclient="/data/unturned/Unturned_Data/Plugins/x86/steamclient.so"

if [[ -f ${steamclient} && -f ${usteamclient} ]]; then
	if ! diff ${steamclient} ${usteamclient} >/dev/null; then
		cp -f ${steamclient} ${usteamclient}
	fi
fi

if [ -f /data/unturned/RocketLauncher.exe ]; then
	ulimit -n 2048
	export LD_LIBRARY_PATH=/data/unturned/lib:$LD_LIBRARY_PATH

	cd "$(dirname "$0")"

	if [ -z ${SERVER_NAME+x} ]; then
		mono --runtime=v4.0.30319 RocketLauncher.exe server
	else
		mono --runtime=v4.0.30319 RocketLauncher.exe ${SERVER_NAME}
	fi
else
	printf "Failed to find Rocket launcher.\n"
	printf "Try restarting your docker container.\n"
	exit
fi