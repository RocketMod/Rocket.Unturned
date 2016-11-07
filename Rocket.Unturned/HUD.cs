using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;


namespace Rocket.Unturned
{
    public class HUD : PlayerCaller
    {
        bool visible = false;

        private void Start()
        {
            //base.channel.build();
            //askToggleHud(true);
        }

        public void askToggleHud(bool visible)
        {     
            base.channel.send("tellToggleHud", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
        }

        //[SteamCall]
        public void tellToggleHud(CSteamID steamID, bool visible)
        {
            if (base.channel.checkServer(steamID))
            {
                Debug.LogError("CALLED HUD");
                this.visible = visible;
            }
        }

        private void OnGUI()
        {
            if (Dedicator.isDedicated) return;
            GUI.Label(new Rect(20, 110, 130, 20), "HUD enabled");
            if (visible)
                GUI.Label(new Rect(20, 210, 130, 20), "HUD visible");

        }
    }
}
