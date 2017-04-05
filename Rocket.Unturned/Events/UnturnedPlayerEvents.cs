using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using System.Linq;
using Rocket.API.Extensions;
using Rocket.Core;

namespace Rocket.Unturned.Events
{
    public sealed class UnturnedPlayerEvents : UnturnedPlayerComponent
    {
        protected override void Load()
        {
            Player.Player.life.onStaminaUpdated += onUpdateStamina;
            Player.Player.inventory.onInventoryAdded += onInventoryAdded;
            Player.Player.inventory.onInventoryRemoved += onInventoryRemoved;
            Player.Player.inventory.onInventoryResized += onInventoryResized;
            Player.Player.inventory.onInventoryUpdated += onInventoryUpdated;
        }

        private void Start()
        {
            U.Instance.triggerOnPlayerConnected(Player);
        }

        internal static void TriggerReceive(SteamChannel instance, CSteamID d, byte[] a, int b)
        {
#if DEBUG
            /*ESteamPacket eSteamPacket = (ESteamPacket)a[0];
            int num = a[1];

            if (eSteamPacket != ESteamPacket.UPDATE_VOICE && eSteamPacket != ESteamPacket.UPDATE_UDP_CHUNK && eSteamPacket != ESteamPacket.UPDATE_TCP_CHUNK)
            {
                object[] objects = SteamPacker.getObjects(d, 2, a, instance.Methods[num].Types);

                string o = "";
                foreach (object r in objects)
                {
                    o += r.ToString() + ",";
                }
                Logger.Log("Receive+" + d.ToString() + ": " + o + " - " + b);
            }*/
#endif
            return;
        }

        internal static void TriggerSend(SteamPlayer player, string name, ESteamCall mode, ESteamPacket type,
            params object[] arguments)
        {
            try
            {
                if (player == null || player.player == null || player.player.transform == null || arguments == null)
                    return;
                UnturnedPlayerEvents instance = player.player.transform.GetComponent<UnturnedPlayerEvents>();
                UnturnedPlayer rp = UnturnedPlayer.FromSteamPlayer(player);
#if DEBUG
                //string o = "";
                //foreach (object r in R)
                //{
                //    o += r.ToString();
                //}
                //Logger.Log("Send+" + s.SteamPlayerID.CSteamID.ToString() + ": " + W + " - " + o);
#endif
                if (name.StartsWith("tellWear"))
                {
                    OnPlayerWear.TryInvoke(rp, Enum.Parse(typeof(Wearables), name.Replace("tellWear", "")),
                        (ushort) arguments[0], arguments.Count() > 1 ? (byte?) arguments[1] : null);
                }
                switch (name)
                {
                    case "tellBleeding":
                        OnPlayerUpdateBleeding.TryInvoke(rp, (bool) arguments[0]);
                        instance.OnUpdateBleeding.TryInvoke(rp, (bool) arguments[0]);
                        break;
                    case "tellBroken":
                        OnPlayerUpdateBroken.TryInvoke(rp, (bool) arguments[0]);
                        instance.OnUpdateBroken.TryInvoke(rp, (bool) arguments[0]);
                        break;
                    case "tellLife":
                        OnPlayerUpdateLife.TryInvoke(rp, (byte) arguments[0]);
                        instance.OnUpdateLife.TryInvoke(rp, (byte) arguments[0]);
                        break;
                    case "tellFood":
                        OnPlayerUpdateFood.TryInvoke(rp, (byte) arguments[0]);
                        instance.OnUpdateFood.TryInvoke(rp, (byte) arguments[0]);
                        break;
                    case "tellHealth":
                        OnPlayerUpdateHealth.TryInvoke(rp, (byte) arguments[0]);
                        instance.OnUpdateHealth.TryInvoke(rp, (byte) arguments[0]);
                        break;
                    case "tellVirus":
                        OnPlayerUpdateVirus.TryInvoke(rp, (byte) arguments[0]);
                        instance.OnUpdateVirus.TryInvoke(rp, (byte) arguments[0]);
                        break;
                    case "tellWater":
                        OnPlayerUpdateWater.TryInvoke(rp, (byte) arguments[0]);
                        instance.OnUpdateWater.TryInvoke(rp, (byte) arguments[0]);
                        break;
                    case "tellStance":
                        OnPlayerUpdateStance.TryInvoke(rp, (byte) arguments[0]);
                        instance.OnUpdateStance.TryInvoke(rp, (byte) arguments[0]);
                        break;
                    case "tellGesture":
                        OnPlayerUpdateGesture.TryInvoke(rp,
                            (PlayerGesture) Enum.Parse(typeof(PlayerGesture), arguments[0].ToString()));
                        instance.OnUpdateGesture.TryInvoke(rp,
                            (PlayerGesture) Enum.Parse(typeof(PlayerGesture), arguments[0].ToString()));
                        break;
                    case "tellStat":
                        OnPlayerUpdateStat.TryInvoke(rp, (EPlayerStat) (byte) arguments[0]);
                        instance.OnUpdateStat.TryInvoke(rp, (EPlayerStat) (byte) arguments[0]);
                        break;
                    case "tellExperience":
                        OnPlayerUpdateExperience.TryInvoke(rp, (uint) arguments[0]);
                        instance.OnUpdateExperience.TryInvoke(rp, (uint) arguments[0]);
                        break;
                    case "tellRevive":
                        OnPlayerRevive.TryInvoke(rp, (Vector3) arguments[0], (byte) arguments[1]);
                        instance.OnRevive.TryInvoke(rp, (Vector3) arguments[0], (byte) arguments[1]);
                        break;
                    case "tellDead":
                        OnPlayerDead.TryInvoke(rp, (Vector3) arguments[0]);
                        instance.OnDead.TryInvoke(rp, (Vector3) arguments[0]);
                        break;
                    case "tellDeath":
                        OnPlayerDeath.TryInvoke(rp, (EDeathCause) (byte) arguments[0], (ELimb) (byte) arguments[1],
                            new CSteamID(ulong.Parse(arguments[2].ToString())));
                        instance.OnDeath.TryInvoke(rp, (EDeathCause) (byte) arguments[0], (ELimb) (byte) arguments[1],
                            new CSteamID(ulong.Parse(arguments[2].ToString())));
                        break;
                    default:
#if DEBUG
                        // Logger.Log("Send+" + s.SteamPlayerID.CSteamID.ToString() + ": " + W + " - " + String.Join(",",R.Select(e => e.ToString()).ToArray()));
#endif
                        break;
                }
                return;
            }
            catch (Exception ex)
            {
                R.Logger.Error("Failed to receive packet \"" + name + "\"", ex);
            }
        }
    }
}
