using System;
using System.Collections.Generic;
using System.IO;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Unturned.Console;
using Rocket.Unturned.Player;
using Rocket.Unturned.Utils;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using ILogger = Rocket.API.Logging.ILogger;

namespace Rocket.Unturned
{
    public class UnturnedImplementation : IImplementation
    {
        private GameObject rocketGameObject;
        private ILogger logger;
        private IPlayerManager playerManager;
        private IEventManager eventManager;
        public bool IsAlive => true;

        public void Load(IRuntime runtime)
        {
            rocketGameObject = new GameObject();

            IDependencyContainer container = runtime.Container;
            eventManager = container.Get<IEventManager>();
            playerManager = container.Get<IPlayerManager>("unturnedplayermanager");
            logger = container.Get<ILogger>();
            logger.LogInformation("Loading Rocket Unturned Implementation...");
            container.RegisterSingletonType<AutomaticSaveWatchdog, AutomaticSaveWatchdog>();
            container.Get<AutomaticSaveWatchdog>().Start();

            string rocketDirectory = $"Servers/{Dedicator.serverID}/Rocket/";
            if (!Directory.Exists(rocketDirectory))
                Directory.CreateDirectory(rocketDirectory);

            Directory.SetCurrentDirectory(rocketDirectory);

            Provider.onServerHosted += () =>
            {
                //todo: ImplementationReadyEvent
            };

            if (Environment.OSVersion.Platform == PlatformID.Unix
                || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                var console = rocketGameObject.AddComponent<UnturnedConsole>();
                console.Logger = logger;
            }

            SteamChannel.onTriggerSend += TriggerSend;
        }

        internal void TriggerSend(SteamPlayer player, string method, ESteamCall steamCall, ESteamPacket steamPacket, params object[] data)
        {
            try
            {
                if (player == null 
                    || player.player == null 
                    || player.playerID.steamID == CSteamID.Nil 
                    || player.player.transform == null 
                    || data == null) return;

                UnturnedPlayer unturnedPlayer = 
                    (UnturnedPlayer) playerManager.GetPlayer(player.playerID.steamID.ToString());

                if (method.StartsWith("tellWear"))
                {
                    //PlayerWearEvent method.Replace("tellWear", ""), (ushort)data[0], data.Count() > 1 ? (byte?)data[1] : null)
                    return;
                }

                switch (method)
                {
                    case "tellBleeding":
                        //PlayerBleedingUpdateEvent (bool)data[0]
                        break;
                    case "tellBroken":
                        //PlayerUpdateBrokenEvent (bool)data[0]
                        break;
                    case "tellLife":
                        //PlayerUpdateLifeEvent (byte)data[0]
                        break;
                    case "tellFood":
                        //PlayerUpdateFoodEvent (byte)data[0]
                        break;
                    case "tellHealth":
                        //PlayerUpdateHealthEvent (byte)data[0]
                        break;
                    case "tellVirus":
                        //PlayerUpdateVirusEvent (byte)data[0]
                        break;
                    case "tellWater":
                        //PlayerUpdateWaterEvent (byte)data[0]
                        break;
                    case "tellStance":
                        //PlayerUpdateStanceEvent (byte)data[0]
                        break;
                    case "tellGesture":
                        //PlayerUpdateGestureEvent (byte)data[0].ToString()
                        break;
                    case "tellStat":
                        //PlayerUpdateStatEvent (EPlayerStat)(byte)data[0])
                        break;
                    case "tellExperience":
                        //PlayerUpdateExperienceEvent (uint)data[0])
                        break;
                    case "tellRevive":
                        //OnPlayerReviveEvent (Vector3)data[0], (byte)data[1]
                        break;
                    case "tellDead":
                        //PlayerDeadEvent (Vector3)data[0]
                        break;
                    case "tellDeath":
                        //PlayerDeathEvent (EDeathCause)(byte)data[0], (ELimb)(byte)data[1], new CSteamID(ulong.Parse(data[2].ToString()))
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to receive packet \"" + method + "\"", ex);
            }
        }

        public void Shutdown()
        {
            Provider.shutdown();
        }

        public void Reload() { }

        public IEnumerable<string> Capabilities => new List<string>();
        public string InstanceId => Provider.serverID;
        public string WorkingDirectory => Environment.CurrentDirectory;
        public string Name => "Rocket.Unturned";
    }
}