@ECHO OFF 
REM This script updates a Unturned 3 server on Windows machines
REM Syntax: update.bat (home)
REM Author: fr34kyn01535

SET HOME=%1
IF [%1]==[] SET HOME=..\
SET UNTURNEDHOME=%HOME%Unturned
SET STEAMHOME=%HOME%Steam\
ECHO Steam directory: %STEAMHOME%

IF NOT EXIST "%STEAMHOME%"\steamcmd.zip (
ECHO Installing SteamCMD into Steam directory
MKDIR "%STEAMHOME%"
Powershell -c "Invoke-WebRequest -OutFile 'steamcmd.zip' 'https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip'"
CALL :unzip "%~dp0" "%~dp0steamcmd.zip"
DEL steamcmd.zip
MOVE steamcmd.exe %STEAMHOME%
)

CD "%STEAMHOME%"
MKDIR "%UNTURNEDHOME%"
steamcmd.exe +login unturnedrocksupdate force_update +force_install_dir "%UNTURNEDHOME%" +app_update 304930 validate +exit


IF EXIST "%HOME%\Assembly-CSharp.dll" COPY "%HOME%\Assembly-CSharp.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"
IF EXIST "%HOME%\Rocket.API.dll" COPY "%HOME%\Rocket.API.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"
IF EXIST "%HOME%\Rocket.Core.dll" COPY "%HOME%\Rocket.Core.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"
IF EXIST "%HOME%\Rocket.Unturned.dll" COPY "%HOME%\Rocket.Unturned.dll" "%UNTURNEDHOME%\Unturned_Data\Managed"

exit /b

:unzip <destination> <archive>
set vbs="%temp%\_.vbs"
if exist %vbs% del /f /q %vbs%
>%vbs%  echo Set fso = CreateObject("Scripting.FileSystemObject")
>>%vbs% echo If NOT fso.FolderExists(%1) Then
>>%vbs% echo fso.CreateFolder(%1)
>>%vbs% echo End If
>>%vbs% echo set objShell = CreateObject("Shell.Application")
>>%vbs% echo set FilesInZip=objShell.NameSpace(%2).items
>>%vbs% echo objShell.NameSpace(%1).CopyHere(FilesInZip)
>>%vbs% echo Set fso = Nothing
>>%vbs% echo Set objShell = Nothing
cscript //nologo %vbs%
if exist %vbs% del /f /q %vbs%
