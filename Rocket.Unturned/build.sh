cp ~/dev/Managed/Assembly-CSharp-firstpass.dll lib/Assembly-CSharp-firstpass.dll
cp ~/dev/Managed/Assembly-CSharp.dll lib/Assembly-CSharp.dll
cp ~/dev/Managed/UnityEngine.dll lib/UnityEngine.dll
cp /Release/Rocket.Unturned/Rocket.API.dll lib/Rocket.API.dll
cp /Release/Rocket.Unturned/Rocket.Core.dll lib/Rocket.Core.dll
/usr/lib/mono/4.5/xbuild.exe Rocket.Unturned.csproj /p:Configuration=Release /p:DebugSymbols=false /p:PreBuildEvent=;PostBuildEvent=
