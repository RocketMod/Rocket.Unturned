using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using System.Linq;
using Rocket.Core;
using Rocket.Unturned.Event;
using Rocket.Unturned.Event.Player;
using Rocket.Unturned.Event.Player.Inventory;

namespace Rocket.Unturned.Events
{
    public sealed class UnturnedPlayerEvents : UnturnedPlayerComponent
    {
        protected override void Load()
        {
            Player.Player.life.onStaminaUpdated += delegate(byte newStamina)
            {
                new PlayerUpdateStaminaEvent(Player, newStamina).Fire();
            };
            Player.Player.inventory.onInventoryAdded += delegate(byte page, byte index, ItemJar item)
            {
                new PlayerInventoryAddedEvent(Player, page, index, item).Fire();
            };
            Player.Player.inventory.onInventoryRemoved += delegate (byte page, byte index, ItemJar item)
            {
                new PlayerInventoryRemovedEvent(Player, page, index, item).Fire();
            };

            Player.Player.inventory.onInventoryResized += delegate(byte page, byte newWidth, byte newHeight)
            {
                new PlayerInventoryResizedEvent(Player, page, newWidth, newHeight);
            };

            Player.Player.inventory.onInventoryUpdated += delegate (byte page, byte index, ItemJar item)
            {
                new PlayerInventoryUpdatedEvent(Player, page, index, item).Fire();
            };
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
                    new PlayerUpdateWearEvent(rp, (Wearable)Enum.Parse(typeof(Wearable), name.Replace("tellWear", "")), (ushort)arguments[0]).Fire();
                }
                switch (name)
                {
                    case "tellBleeding":
                        new PlayerUpdateBleedingEvent(rp, (bool) arguments[0]).Fire();
                        break;

                    case "tellBroken":
                        new PlayerUpdateBrokenEvent(rp, (bool) arguments[0]).Fire();
                        break;

                    case "tellLife":
                        new PlayerUpdateLifeEvent(rp, (byte)arguments[0]).Fire();
                        break;
                    case "tellFood":
                        new PlayerUpdateFoodEvent(rp, (byte)arguments[0]).Fire();
                        break;
                    case "tellHealth":
                        new PlayerUpdateHealthEvent(rp, (byte)arguments[0]).Fire();
                        break;
                    case "tellVirus":
                        new PlayerUpdateVirusEvent(rp, (byte)arguments[0]).Fire();
                        break;
                    case "tellWater":
                        new PlayerUpdateWaterEvent(rp, (byte)arguments[0]).Fire();
                        break;
                    case "tellStance":
                        new PlayerUpdateStanceEvent(rp, (byte) arguments[0]);
                        break;
                    case "tellGesture":
                        new PlayerUpdateGestureEvent(rp, (EPlayerGesture) Enum.Parse(typeof(EPlayerGesture), arguments[0].ToString())).Fire();
                        break;
                    case "tellStat":
                        new PlayerUpdateStatEvent(rp, (EPlayerStat)(byte)arguments[0]).Fire();
                        break;
                    case "tellExperience":
                        new PlayerUpdateExperienceEvent(rp, (uint) arguments[0]).Fire();
                        break;
                    case "tellRevive":
                        new PlayerReviveEvent(rp, (Vector3)arguments[0], (byte)arguments[1]).Fire();
                        break;
                    case "tellDead":
                        new PlayerDeadEvent(rp, (Vector3)arguments[0]).Fire();
                        break;
                    case "tellDeath":
                        new PlayerDeathEvent(rp, (EDeathCause)(byte)arguments[0], (ELimb)(byte)arguments[1],
                            new CSteamID(ulong.Parse(arguments[2].ToString()))).Fire();
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

    public enum Wearable
    {
        Backpack,
        Glasses,
        Hat,
        Mask,
        Pants,
        Shirt,
        Vest
    }
}
