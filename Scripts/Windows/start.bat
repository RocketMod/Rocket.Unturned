@ECHO OFF 
REM This script starts a Unturned 3 server on Windows machines
REM Syntax: start.bat <instance name>
REM Author: fr34kyn01535
SET INSTANCE_NAME=%1
SET UNTURNED_HOME="..\..\..\"

if "%~1"=="-FIXED_CTRL_C" (
   SHIFT
) ELSE (
   CALL <NUL %0 -FIXED_CTRL_C %*
   GOTO :EOF
)

IF [%1]==[] exit
cd %UNTURNED_HOME%
:unturned
echo [%time%] Unturned started.
Unturned.exe -logFile "Servers\%INSTANCE_NAME%\Rocket\Unturned.log" -nographics -batchmode -silent-crashes +secureserver/%INSTANCE_NAME%
echo [%time%] WARNING: Unturned closed or crashed, restarting.
ping 1.1.1.1 -n 1 -w 5000 >nul
goto unturned