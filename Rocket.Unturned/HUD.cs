using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using UnityEngine;


namespace Rocket.Unturned
{
    public class HUD : MonoBehaviour
    {
        private UnturnedPlayer player = null;
        private SteamChannel channel = null;
        bool visible = false;

        private void Awake()
        {
            player = UnturnedPlayer.FromPlayer(gameObject.GetComponent<SDG.Unturned.Player>());
            channel = GetComponent<SteamChannel>();
        }

        private void Start()
        {
           // channel.build();
            //channel.send("tellToggleHud", player.CSteamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
        }

        [SteamCall]
        public void tellToggleHud(bool visible)
        {
            Debug.LogError("CALLED HUD");
            this.visible = visible;
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(20, 110, 130, 20), "HUD enabled");
            if (visible)
                GUI.Label(new Rect(20, 210, 130, 20), "HUD visible");

        }
    }
}
