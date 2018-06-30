using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.Commands.Events;
using Rocket.Core.Configuration;
using Rocket.Core.Implementation.Events;
using Rocket.Core.Logging;
using Rocket.Core.Player;
using Rocket.Core.Player.Events;
using Rocket.Core.User;
using Rocket.Unturned.Console;
using Rocket.Unturned.Player;
using Rocket.Unturned.Player.Events;
using Rocket.Unturned.Utils;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using ILogger = Rocket.API.Logging.ILogger;
using Object = UnityEngine.Object;
using Version = System.Version;

namespace Rocket.Unturned
{
    public class RocketUnturnedHost : IHost
    {
        public RocketUnturnedHost(IDependencyContainer container)
        {
            string rocketDirectory = $"Servers/{Dedicator.serverID}/Rocket/";
            if (!Directory.Exists(rocketDirectory))
                Directory.CreateDirectory(rocketDirectory);

            Directory.SetCurrentDirectory(rocketDirectory);
            Console = new StdConsole(container);
        }

        private GameObject rocketGameObject;
        private ILogger logger;
        private UnturnedPlayerManager playerManager;
        private IEventManager eventManager;
        private IDependencyContainer container;
        internal ITranslationCollection ModuleTranslations { get; private set; }
        private IRuntime runtime;
        public bool IsAlive => true;
        public ushort ServerPort => Provider.port;
        public IConsole Console { get; set; }
        public string GameName => "Unturned";

        public void Init(IRuntime runtime)
        {
            BaseLogger.SkipTypeFromLogging(typeof(UnturnedPlayerManager));
            ApplyTlsWorkaround();

            this.runtime = runtime;
            rocketGameObject = new GameObject();
            Object.DontDestroyOnLoad(rocketGameObject);

            container = runtime.Container;
            eventManager = container.Resolve<IEventManager>();
            playerManager = (UnturnedPlayerManager) container.Resolve<IUserManager>("host");
            ModuleTranslations = container.Resolve<ITranslationCollection>();
            
            logger = container.Resolve<ILogger>();
            logger.LogInformation("Loading Rocket Unturned Implementation...");

            container.RegisterSingletonType<AutomaticSaveWatchdog, AutomaticSaveWatchdog>();
            container.Resolve<AutomaticSaveWatchdog>().Start();
            LoadTranslations();

            Provider.onServerHosted += OnServerHosted;

            if (Environment.OSVersion.Platform == PlatformID.Unix
                || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                rocketGameObject.SetActive(false); // deactivate object so it doesn't run Awake until all properties were set
                var console = rocketGameObject.AddComponent<UnturnedConsolePipe>();
                console.Logger = logger;
                rocketGameObject.SetActive(true); // reactivate object
            }

            SteamChannel.onTriggerSend += TriggerSend;
            Provider.onCheckValid += OnCheckValid;
            Provider.onServerConnected += OnPlayerConnected;
            Provider.onServerDisconnected += OnPlayerDisconnected;
            DamageTool.playerDamaged += OnPlayerDamaged;
            Provider.onServerShutdown += OnServerShutdown;
            ChatManager.onChatted += (SteamPlayer player, EChatMode mode, ref Color color, ref bool isRich, string message,
                                      ref bool isVisible) =>
            {
                UnturnedPlayer p = (UnturnedPlayer)playerManager.GetOnlinePlayerById(player.playerID.steamID.m_SteamID.ToString());
                UnturnedPlayerChatEvent @event = new UnturnedPlayerChatEvent(p, mode, color, isRich, message, !isVisible);
                eventManager.Emit(this, @event);
                color = @event.Color;
                isRich = @event.IsRichText;
                isVisible = !@event.IsCancelled;
            };

            CommandWindow.onCommandWindowOutputted += (text, color)
                => logger.LogNative(text?.ToString());
        }

        private void ApplyTlsWorkaround()
        {
            //http://answers.unity.com/answers/1089592/view.html
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationWorkaroundCallback;
        }

        public bool CertificateValidationWorkaroundCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                {
                    if (chainStatus.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        private void OnServerShutdown()
        {
            runtime.Shutdown();
        }

        private void OnPlayerDamaged(SDG.Unturned.Player uPlayer, ref EDeathCause cause, ref ELimb limb, ref CSteamID killerId, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            var player = playerManager.GetOnlinePlayerById(uPlayer.channel.owner.playerID.steamID.m_SteamID.ToString());
            var killer = playerManager.GetOnlinePlayerById(killerId.m_SteamID.ToString());

            UnturnedPlayerDamagedEvent @event =
                new UnturnedPlayerDamagedEvent(player, cause, limb, killer.GetUser(), direction, damage, times)
                {
                    IsCancelled = !canDamage
                };

            eventManager.Emit(this, @event);
            cause = @event.DeathCause;
            limb = @event.Limb;
            killerId = @event.DamageDealer != null ? new CSteamID(ulong.Parse(@event.DamageDealer.Id)) : CSteamID.Nil;
            direction = @event.Direction;
            damage = (float)@event.Damage;
            times = @event.Times;
            canDamage = !@event.IsCancelled;
        }

        private void LoadTranslations()
        {
            var context = new ConfigurationContext(this);
            context.ConfigurationName += "Translations";

            ModuleTranslations.Load(context,
            new Dictionary<string, string>
            {
                { "command_compass_facing_private","You are facing {0}"},
                { "command_compass_north","N"},
                { "command_compass_east","E"},
                { "command_compass_south","S"},
                { "command_compass_west","W"},
                { "command_compass_northwest","NW"},
                { "command_compass_northeast","NE"},
                { "command_compass_southwest","SW"},
                { "command_compass_southeast","SE"},
                { "command_heal_success_me","{0} was successfully healed"},
                { "command_heal_success_other","You were healed by {0}"},
                { "command_heal_success","You were healed"},
                { "command_bed_no_bed_found_private","You do not have a bed to teleport to."},
                { "command_i_giving_private","Giving you item {0}x {1} ({2})"},
                { "command_i_giving_failed_private","Failed giving you item {0}x {1} ({2})"},
                { "command_more_dequipped", "No item being held in hands." },
                { "command_more_give", "Giving {0} of item: {1}." },
                { "command_generic_teleport_while_driving_error","You cannot teleport while driving or riding in a vehicle."},
                { "command_tp_failed_find_destination","Failed to find destination"},
                { "command_tphere_vehicle", "The player you are trying to teleport is in a vehicle"},
                { "command_tphere_teleport_from_private","Teleported {0} to you"},
                { "command_tphere_teleport_to_private","You were teleported to {0}"},
                { "command_v_giving_private","Giving you a {0} ({1})"},
                { "command_v_giving_failed_private","Failed giving you a {0} ({1})"},
            });
        }

        private void OnPlayerConnected(CSteamID steamid)
        {
            var player = playerManager.GetOnlinePlayerById(steamid.ToString());
            PlayerConnectedEvent @event = new PlayerConnectedEvent(player);
            eventManager.Emit(this, @event);
        }

        private void OnPlayerDisconnected(CSteamID steamid)
        {
            var player = playerManager.GetOnlinePlayerById(steamid.ToString());
            PlayerDisconnectedEvent @event = new PlayerDisconnectedEvent(player, null);
            eventManager.Emit(this, @event);
        }

        private void OnCheckValid(ValidateAuthTicketResponse_t callback, ref bool isValid)
        {
            var pendingPlayer = Provider.pending.FirstOrDefault(c => c.playerID.steamID.Equals(callback.m_SteamID));
            if (pendingPlayer == null) return;

            PreConnectUnturnedPlayer player = new PreConnectUnturnedPlayer(container, pendingPlayer, playerManager);
            UnturnedPlayerPreConnectEvent @event = new UnturnedPlayerPreConnectEvent(player, callback);
            eventManager.Emit(this, @event);

            if (@event.UnturnedRejectionReason != null)
            {
                Provider.reject(callback.m_SteamID, @event.UnturnedRejectionReason.Value);
                isValid = false;
                return;
            }

            if (@event.IsCancelled)
            {
                Provider.reject(callback.m_SteamID, ESteamRejection.PLUGIN);
                isValid = false;
                return;
            }

            isValid = true;
        }

        private void OnServerHosted()
        {
            //proxied
            var pluginManager = container.Resolve<IPluginManager>();
            pluginManager.Init();

            IEvent @event = new ImplementationReadyEvent(this);
            eventManager.Emit(this, @event);

            ICommandHandler cmdHandler = container.Resolve<ICommandHandler>();
            
            ChatManager.onCheckPermissions += (SteamPlayer player, string commandLine, ref bool shouldExecuteCommand,
                                               ref bool shouldList) =>
            {
                if (commandLine.StartsWith("/"))
                {
                    commandLine = commandLine.Substring(1);
                    var caller = playerManager.GetOnlinePlayer(player.playerID.steamID.ToString());
                    @event = new PreCommandExecutionEvent(caller.GetUser(), commandLine);
                    eventManager.Emit(this, @event);
                    bool success = cmdHandler.HandleCommand(caller.GetUser(), commandLine, "/");
                    if(!success)
                        caller.GetUser().SendMessage("Command not found", ConsoleColor.Red);
                    shouldList = false;
                }

                shouldExecuteCommand = false;
            };

            CommandWindow.onCommandWindowInputted += (string commandline, ref bool shouldExecuteCommand) =>
            {
                if (commandline.StartsWith("/"))
                    commandline = commandline.Substring(1);

                @event = new PreCommandExecutionEvent(Console, commandline);
                eventManager.Emit(this, @event);
                bool success = cmdHandler.HandleCommand(Console, commandline, "");
                if (!success)
                    Console.SendMessage("Command not found", ConsoleColor.Red);

                shouldExecuteCommand = false;
            };
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
                    (UnturnedPlayer)playerManager.GetPlayer(player.playerID.steamID.ToString());

                if (method.StartsWith("tellWear"))
                {
                    //PlayerWearEvent method.Replace("tellWear", ""), (ushort)data[0], data.Count() > 1 ? (byte?)data[1] : null)
                    return;
                }

                IEvent @event = null;
                switch (method)
                {
                    case "tellBleeding":
                        @event = new UnturnedPlayerUpdateBleedingEvent(unturnedPlayer, (bool)data[0]);
                        break;
                    case "tellBroken":
                        @event = new UnturnedPlayerUpdateBrokenEvent(unturnedPlayer, (bool)data[0]);
                        break;
                    case "tellLife":
                        @event = new UnturnedPlayerUpdateLifeEvent(unturnedPlayer, (byte)data[0]);
                        break;
                    case "tellFood":
                        @event = new UnturnedPlayerUpdateFoodEvent(unturnedPlayer, (byte)data[0]);
                        break;
                    case "tellHealth":
                        @event = new UnturnedPlayerUpdateHealthEvent(unturnedPlayer, (byte)data[0]);
                        break;
                    case "tellVirus":
                        @event = new UnturnedPlayerUpdateVirusEvent(unturnedPlayer, (byte)data[0]);
                        break;
                    case "tellWater":
                        @event = new UnturnedPlayerUpdateWaterEvent(unturnedPlayer, (byte)data[0]);
                        break;
                    case "tellStance":
                        @event = new UnturnedPlayerUpdateStanceEvent(unturnedPlayer, (EPlayerStance)(byte)data[0]);
                        break;
                    case "tellGesture":
                        @event = new UnturnedPlayerUpdateGestureEvent(unturnedPlayer, (EPlayerGesture)(byte)data[0]);
                        break;
                    case "tellStat":
                        @event = new UnturnedPlayerUpdateStatEvent(unturnedPlayer, (EPlayerStat)(byte)data[0]);
                        break;
                    case "tellExperience":
                        @event = new UnturnedPlayerUpdateExperienceEvent(unturnedPlayer, (uint)data[0]);
                        break;
                    case "tellRevive":
                        @event = new PlayerRespawnEvent(unturnedPlayer);
                        break;
                    case "tellDead":
                        @event = new UnturnedPlayerDeadEvent(unturnedPlayer, (Vector3)data[0]);
                        break;
                    case "tellDeath":
                        {
                            var deathCause = (EDeathCause)(byte)data[0];
                            var limb = (ELimb)(byte)data[1];
                            var killerId = data[2].ToString();

                            var killer = killerId != "0" ? playerManager.GetOnlinePlayerById(killerId) : null;
                            @event = new UnturnedPlayerDeathEvent(unturnedPlayer, limb, deathCause, killer?.GetEntity());
                            break;
                        }
                }

                if (@event != null)
                    eventManager.Emit(this, @event);
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
        public Version HostVersion => new Version(FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).FileVersion);
        public Version GameVersion => new Version(Provider.APP_VERSION);
        public string ServerName => Provider.serverName;

        public string InstanceId => Provider.serverID;
        public string WorkingDirectory => Directory.GetCurrentDirectory();
        public string ConfigurationName => Name;
        public string Name => "Rocket.Unturned";
    }
}