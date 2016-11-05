using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using UnityEngine;


namespace Rocket.Unturned
{
    public class HUD : MonoBehaviour
    {
        SteamChannel channel = null;
        UnturnedPlayer player = null;
        bool visible = false;

        private void Awake()
        {
            channel = GetComponent<SteamChannel>();
            //channel.build();
            player = UnturnedPlayer.FromPlayer(gameObject.transform.GetComponent<SDG.Unturned.Player>());
        }

        private void Start()
        {
        try{
            channel.send("tellToggleHud", player.CSteamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
            }catch(Exception e){
            //
            }
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
