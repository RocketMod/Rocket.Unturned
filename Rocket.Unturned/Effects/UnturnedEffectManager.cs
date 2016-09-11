using Rocket.Core.Logging;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rocket.Unturned.Effects
{

    public class UnturnedEffect
    {
        public UnturnedEffect(string type, ushort effectID, bool global)
        {
            this.Type = type;
            this.EffectID = effectID;
            this.Global = global;
        }
        public string Type;
        public ushort EffectID;
        public bool Global;

        public void Trigger(UnturnedPlayer player)
        {
            if (!Global)
            {
                SDG.Unturned.EffectManager.Instance.SteamChannel.send("tellEffectPoint", player.CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { EffectID, player.Player.transform.position });
            }
            else
            {
                SDG.Unturned.EffectManager.Instance.SteamChannel.send("tellEffectPoint", ESteamCall.CLIENTS, player.Player.transform.position, 1024, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { EffectID, player.Player.transform.position });
            }
        }

        public void Trigger(Vector3 position)
        {
            SDG.Unturned.EffectManager.Instance.SteamChannel.send("tellEffectPoint", ESteamCall.CLIENTS, position, 1024, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { EffectID, position });
        }
    }

    public class UnturnedEffectManager : MonoBehaviour
    {
        private static readonly string joinEffect = "Rocket:Join";
        private static readonly string dieEffect = "Rocket:Die";

        public void Start(){
            U.Events.OnPlayerConnected += (UnturnedPlayer player) =>
             {
                 foreach (UnturnedEffect effect in GetEffectsByType(joinEffect))
                 {
                     effect.Trigger((UnturnedPlayer)player);
                 }
             };
            UnturnedPlayerEvents.OnPlayerDeath += (UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer) => 
            {
                foreach (UnturnedEffect effect in GetEffectsByType(dieEffect))
                {
                    effect.Trigger(player);
                }
            };
        }


        public static UnturnedEffect GetEffectsById(ushort id)
        {
            return effects.Where(k => k.EffectID == id).FirstOrDefault();
        }

        public static List<UnturnedEffect> GetEffectsByType(string type)
        {
            return effects.Where(k => k.Type == type).ToList();
        }

        private static List<UnturnedEffect> effects = new List<UnturnedEffect>();

        internal static void RegisterRocketEffect(Bundle b, Data q, ushort k)
        {
            string s = q.readString("RocketEffect");
            bool global = q.readBoolean("Global");
            effects.Add(new UnturnedEffect(s, k, global));

            Core.Logging.Logger.Log("Registering effect: " + s + " (" + k + ")");
        }
    

    }

}
