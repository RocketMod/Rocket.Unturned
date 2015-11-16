using Rocket.Unturned.Events;
using SDG.Unturned;
using System;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public sealed class UnturnedPlayerFeatures : UnturnedPlayerComponent
    {

        public DateTime Joined = DateTime.Now;

        internal Color? color = null;
        internal Color? Color
        {
            get { return color; }
            set { color = value; }
        }


        //private bool vanishMode = false;
        //public bool VanishMode
        //{
        //    get { return vanishMode; }
        //    set { vanishMode = value; }
        //}


        private bool godMode = false;
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

        private bool initialCheck;

        private void FixedUpdate()
        {
            if(!initialCheck && (DateTime.Now - Joined).TotalSeconds > 3)
            {
                Check();
            }
        //    if (this.vanishMode)
        //    {
        //        Player.Player.SteamChannel.send("tellPosition", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_UDP_BUFFER, new object[] { new Vector3(Player.Position.x, -3, Player.Position.z) });
        //    }
        }

        private void Check()
        {
            initialCheck = true;
           
            if (U.Settings.Instance.CharacterNameValidation)
            {
                string username = Player.CharacterName;
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(U.Settings.Instance.CharacterNameValidationRule);
                System.Text.RegularExpressions.Match match = regex.Match(username);
                if (match.Groups[0].Length != username.Length)
                {
                    Provider.kick(Player.CSteamID, U.Translate("invalid_character_name"));

                }
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
