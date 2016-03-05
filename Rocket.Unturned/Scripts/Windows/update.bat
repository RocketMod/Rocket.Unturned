@ECHO OFF 
REM This script updates a Unturned 3 server on Windows machines
REM Syntax: update.bat (home)
REM Author: fr34kyn01535

SET HOME=%1
IF [%1]==[] SET HOME=%~dp0..\
SET UNTURNEDHOME=%HOME%Unturned
SET STEAMHOME=%HOME%Steam\

ECHO Steam directory: %STEAMHOME%

IF NOT EXIST "%STEAMHOME%" (
ECHO Installing SteamCMD into Steam directory
MKDIR "%STEAMHOME%"
Powershell -c "Invoke-WebRequest -OutFile 'steamcmd.zip' 'https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip'"
Powershell -c "Expand-Archive 'steamcmd.zip' -Destination '%STEAMHOME%'"
DEL steamcmd.zip
)

CD "%STEAMHOME%"
MKDIR "%UNTURNEDHOME%"
steamcmd.exe +login unturnedrocksupdate force_update +force_install_dir "%UNTURNEDHOME%" +app_update 304930 validate +exit


IF EXIST "%HOME%\Assembly-CSharp.dll" COPY "%HOME%\Assembly-CSharp.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"
IF EXIST "%HOME%\Rocket.API.dll" COPY "%HOME%\Rocket.API.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"
IF EXIST "%HOME%\Rocket.Core.dll" COPY "%HOME%\Rocket.Core.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"
IF EXIST "%HOME%\Rocket.Unturned.dll" COPY "%HOME%\Rocket.Unturned.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"
