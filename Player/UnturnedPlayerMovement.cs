using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Provider;
using SDG.Provider.Services.Achievements;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rocket.Unturned
{
    public class UnturnedPlayerMovement : UnturnedPlayerComponent
    {
        public bool VanishMode = false;
        DateTime lastUpdate = DateTime.Now;
        Vector3 lastVector = new Vector3(0,-1,0);

        private void FixedUpdate()
        {
            PlayerMovement movement = (PlayerMovement)Player.GetComponent<PlayerMovement>();

            if(VanishMode)
                Player.Player.SteamChannel.send("tellPosition", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new Vector3(movement.real.x, -3, movement.real.z));

            if (U.Settings.Instance.LogSuspiciousPlayerMovement && lastUpdate.AddSeconds(1) < DateTime.Now)
            {
                lastUpdate = DateTime.Now;

                if (lastVector.y != -1)
                {
                    float x = Math.Abs(lastVector.x - movement.real.x);
                    float y = movement.real.y - lastVector.y;
                    float z = Math.Abs(lastVector.z - movement.real.z);
                    if (x > 35 || y > 15 || z > 35)
                    {
                        Logger.Log(Player.DisplayName + " moved x:" + x + " y:" + y + " z:" + z + " in the last second. Could be teleporting or hacking.");
                    }
                }
                lastVector = movement.real;

            }

        }
    }
}