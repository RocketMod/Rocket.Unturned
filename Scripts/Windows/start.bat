@ECHO OFF 
REM This script starts a Unturned 3 server on Windows machines
REM Syntax: start.bat <instance name>
REM Author: fr34kyn01535

IF [%1]==[] goto restart

if "%~1"=="-FIXED_CTRL_C" (
   SHIFT
) ELSE (
   CALL <NUL %0 -FIXED_CTRL_C %*
   GOTO :EOF
)

SET INSTANCE_NAME=%1

if exist "..\..\..\Unturned.exe" SET UNTURNED_HOME="..\..\..\"
if exist "..\..\Unturned.exe" SET UNTURNED_HOME="..\..\"
if exist "..\Unturned.exe" SET UNTURNED_HOME="..\"
if exist "Unturned.exe" SET UNTURNED_HOME=".\"
if exist "%~dp0\Unturned.exe" SET UNTURNED_HOME="%~dp0\"

cd %UNTURNED_HOME%

:unturned
echo [%time%] Unturned started.
Unturned.exe -logFile "Servers\%INSTANCE_NAME%\Rocket\Unturned.log" -nographics -batchmode -silent-crashes +secureserver/%INSTANCE_NAME%
echo [%time%] WARNING: Unturned closed or crashed, restarting.
ping 1.1.1.1 -n 1 -w 5000 >nul
goto unturned

:restart
set /p INSTANCE_NAME= "Instance: "
%0 %INSTANCE_NAME%
