using Rocket.Unturned.Events;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public sealed class UnturnedPlayerFeatures : UnturnedPlayerComponent
    {
        private bool godMode = false;

        public bool VanishMode
        {
            get { return vanishMode; }
            set { vanishMode = value; }
        }

        private bool vanishMode = false;

        public bool GodMode
        {
            set
            {
                if (value)
                {
                    Player.Events.OnUpdateHealth += e_OnPlayerUpdateHealth;
                    Player.Events.OnUpdateWater += e_OnPlayerUpdateWater;
                    Player.Events.OnUpdateFood += e_OnPlayerUpdateFood;
                    Player.Events.OnUpdateVirus += e_OnPlayerUpdateVirus;
                }
                else
                {
                    Player.Events.OnUpdateHealth -= e_OnPlayerUpdateHealth;
                    Player.Events.OnUpdateWater -= e_OnPlayerUpdateWater;
                    Player.Events.OnUpdateFood -= e_OnPlayerUpdateFood;
                    Player.Events.OnUpdateVirus -= e_OnPlayerUpdateVirus;
                }
                godMode = value;
            }
            get
            {
                return godMode;
            }
        }


        private void FixedUpdate()
        {
            if (this.vanishMode)
            {
                Player.Player.SteamChannel.send("tellPosition", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_UDP_BUFFER, new object[] { new Vector3(Player.Position.x, -3, Player.Position.z) });
            }
        }

        protected override void Load()
        {
            if (godMode)
            {
                Player.Events.OnUpdateHealth += e_OnPlayerUpdateHealth;
                Player.Events.OnUpdateWater += e_OnPlayerUpdateWater;
                Player.Events.OnUpdateFood += e_OnPlayerUpdateFood;
                Player.Events.OnUpdateVirus += e_OnPlayerUpdateVirus;
                Player.Heal(100);
                Player.Infection = 0;
                Player.Hunger = 0;
                Player.Thirst = 0;
                Player.Bleeding = false;
                Player.Broken = false;
            }
        }

        private void e_OnPlayerUpdateVirus(UnturnedPlayer player, byte virus)
        {
            if (virus < 95) Player.Infection = 0;
        }

        private void e_OnPlayerUpdateFood(UnturnedPlayer player, byte food)
        {
            if (food < 95) Player.Hunger = 0;
        }

        private void e_OnPlayerUpdateWater(UnturnedPlayer player, byte water)
        {
            if (water < 95) Player.Thirst = 0;
        }

        private void e_OnPlayerUpdateHealth(UnturnedPlayer player, byte health)
        {
            if (health < 95)
            {
                Player.Heal(100);
                Player.Bleeding = false;
                Player.Broken = false;
            }
        }
    }
}