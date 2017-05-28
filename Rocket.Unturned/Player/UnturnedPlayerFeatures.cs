using Rocket.API.Extensions;
using Rocket.Unturned.Events;
using SDG.Unturned;
using System;
using Rocket.API.Event;
using Rocket.Unturned.Event.Player;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public sealed class UnturnedPlayerFeatures : UnturnedPlayerComponent, IListener
    {
        public DateTime Joined = DateTime.Now;

        internal Color? color = null;
        internal Color? Color
        {
            get { return color; }
            set { color = value; }
        }

        private void Start()
        {
            EventManager.Instance.RegisterEventsInternal(this, null);
        }

        private void Stop()
        {
            EventManager.Instance.UnregisterEventsInternal(this, null);
        }

        private bool vanishMode = false;
        public bool VanishMode
        {
            get { return vanishMode; }
            set
            {
               // Player.GetComponent<UnturnedPlayerMovement>().VanishMode = value;
                PlayerMovement pMovement = Player.GetComponent<PlayerMovement>();
                pMovement.canAddSimulationResultsToUpdates = !value;
                if (vanishMode && !value)
                {
                    pMovement.updates.Add(new PlayerStateUpdate(pMovement.real, Player.Player.look.angle, Player.Player.look.rot));
                    pMovement.isUpdated = true;
                    //PlayerManager.updates++;
                }
                vanishMode = value;
            }
        }

        public bool GodMode { get; set; }

        private bool initialCheck;

        Vector3 oldPosition = new Vector3();

        private void FixedUpdate()
        {
            if (oldPosition != Player.Position)
            {
                new PlayerUpdatePositionEvent(Player, oldPosition, Player.Position).Fire();
                oldPosition = Player.Position;
            }
            if (!initialCheck && (DateTime.Now - Joined).TotalSeconds > 3)
            {
                Check();
            }
        }

        private void Check()
        {
            initialCheck = true;

            //if (U.Instance.Settings.Instance.CharacterNameValidation)
            //{
            //    string username = Player.DisplayName;
            //    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(U.Instance.Settings.Instance.CharacterNameValidationRule);
            //    System.Text.RegularExpressions.Match match = regex.Match(username);
            //    if (match.Groups[0].Length != username.Length)
            //    {
            //        Provider.kick(Player.CSteamID, R.Translations.Translate("invalid_character_name"));
            //    }
            //}
        }

        private static string reverse(string s)
        {
            string r = "";
            for (int i = s.Length; i > 0; i--) r += s[i - 1];
            return r;
        }

        protected override void Load()
        {
            if (GodMode)
            {
                Player.Heal(100);
                Player.Infection = 0;
                Player.Hunger = 0;
                Player.Thirst = 0;
                Player.Bleeding = false;
                Player.Broken = false;
            }
        }

        [API.Event.EventHandler]
        private void e_OnPlayerUpdateVirus(PlayerUpdateVirusEvent @event)
        {
            if (!GodMode ||!@event.Player.Id.Equals(Player.Id))
                return;

            if (@event.NewVirus < 95) Player.Infection = 0;
        }

        [API.Event.EventHandler]
        private void e_OnPlayerUpdateFood(PlayerUpdateFoodEvent @event)
        {
            if (!GodMode || !@event.Player.Id.Equals(Player.Id))
                return;

            if (@event.NewFood < 95) Player.Hunger = 0;
        }

        [API.Event.EventHandler]
        private void e_OnPlayerUpdateWater(PlayerUpdateWaterEvent @event)
        {
            if (!GodMode || !@event.Player.Id.Equals(Player.Id))
                return;

            if (@event.NewWater < 95) Player.Thirst = 0;
        }

        [API.Event.EventHandler]
        private void e_OnPlayerUpdateHealth(PlayerUpdateHealthEvent @event)
        {
            if (!GodMode || !@event.Player.Id.Equals(Player.Id))
                return;

            if (@event.NewHealth < 95)
            {
                Player.Heal(100);
                Player.Bleeding = false;
                Player.Broken = false;
            }
        }
    }
}
