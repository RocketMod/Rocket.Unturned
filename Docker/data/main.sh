#!/bin/bash
printf "RocketMod Unturned server on Docker\n"

# import
. /data/package.sh

# main
while true; do
	package::update_steamcmd
	package::update_rocket
	package::update_unturned
	package::start_server
done