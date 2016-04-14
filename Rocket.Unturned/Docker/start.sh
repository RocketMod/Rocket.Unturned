#!/bin/bash
if ! screen -list | grep -q "Rocket"; then
    screen -S Rocket -d -m bash -c 'ulimit -n 2048 && mono RocketLauncher.exe rocket'
fi