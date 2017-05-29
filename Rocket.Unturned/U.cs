using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Extensions;
using Rocket.API.Player;
using Rocket.API.Providers.Implementation;
using Rocket.API.Providers.Implementation.Managers;
using Rocket.Core;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Rocket.API.Event.Implementation;
using Rocket.API.Event.Player;
using Rocket.API.Event.Plugin;
using Rocket.API.Providers.Logging;
using Rocket.Core.Player;
using UnityEngine;

namespace Rocket.Unturned
{
    public class U : IRocketImplementationProvider
    {
        
        internal void triggerOnPlayerConnected(UnturnedPlayer player)
        {
            new PlayerConnectedEvent(player).Fire();
        }

        public static U Instance { get; private set; }

        public string Name { get; } = "Unturned";
        public string InstanceName { get; } = Dedicator.serverID;

        public ushort Port => Provider.port;

        IChatManager IRocketImplementationProvider.Chat
        {
            get
            {
                throw new NotImplementedException();
            }
        } 

        public IPlayerManager Players
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ReadOnlyCollection<Type> Providers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TranslationList DefaultTranslation
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public U()
        {
            Provider.onServerHosted += () =>
            {
                try
                {
                    SteamGameServer.SetKeyValue("rocket", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    //Todo: readd SteamGameServer.SetKeyValue("rocketplugins", String.Join(", ", R.GetAllPlugins().Select(p => p.Name).ToArray()));
                    SteamGameServer.SetBotPlayerCount(1);
                }
                catch (Exception ex)
                {
                    R.Logger.Log(LogLevel.ERROR, "Steam can not be initialized: " + ex.Message);
                }
            };

            CommandWindow.onCommandWindowInputted += (string text, ref bool shouldExecuteCommand) =>
            {
                if (text.StartsWith("/")) text.Substring(1);
                R.Execute(new ConsolePlayer(), text);
                shouldExecuteCommand = false;
            };

            CommandWindow.onCommandWindowOutputted += (text, color) =>  {
                foreach (var logging in R.Providers.GetProviders<IRocketLoggingProvider>())
                {
                    if(logging.EchoNativeOutput)
                        logging.Log(LogLevel.INFO, text);
                }
            };

            /*
            SteamChannel.onTriggerReceive += (SteamChannel channel, CSteamID steamID, byte[] packet, int offset, int size) =>
             {
                 UnturnedPlayerEvents.TriggerReceive(channel, steamID, packet, offset, size);
             };
             */

            SteamChannel.onTriggerSend += UnturnedPlayerEvents.TriggerSend;

            ChatManager.onCheckPermissions += (SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList) =>
            {
                if (text.StartsWith("/"))
                {
                    if (UnturnedPermissions.CheckPermissions(player, text))
                    {
                        R.Execute(UnturnedPlayer.FromSteamPlayer(player), text);
                    }
                    shouldList = false;
                }
                shouldExecuteCommand = false;
            };

            Provider.onCheckValid += (ValidateAuthTicketResponse_t callback, ref bool isValid) =>
            {
                isValid = UnturnedPermissions.CheckValid(callback);
            };
            try
            {
               
                //Provider.onServerShutdown += () => { OnShutdown.TryInvoke(); };
                //Provider.onServerDisconnected += (CSteamID r) => {
                //    OnPlayerDisconnected?.TryInvoke(UnturnedPlayer.FromCSteamID(r));
                //};
                //Provider.onServerConnected += (CSteamID r) =>
                //{
                //    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(r);
                //    p.Player.gameObject.TryAddComponent<UnturnedPlayerFeatures>();
                //    p.Player.gameObject.TryAddComponent<UnturnedPlayerMovement>();
                //    p.Player.gameObject.TryAddComponent<UnturnedPlayerEvents>();
                //    OnBeforePlayerConnected.TryInvoke(p);
                //};

                //RocketPluginBase.OnPluginsLoading += (RocketPluginBase plugin, ref bool cancelLoading) =>
                //{
                //    try
                //    {
                //        plugin.gameObject.TryAddComponent<PluginUnturnedPlayerComponentManager>();
                //    }
                //    catch (Exception ex)
                //    {
                //        Logger.Fatal("Failed to load plugin " + plugin.Name + ".", ex);
                //        cancelLoading = true;
                //    }
                //};

                //RocketPluginBase.OnPluginsUnloading += (RocketPluginBase plugin) =>
                //{
                //    plugin.gameObject.TryRemoveComponent<PluginUnturnedPlayerComponentManager>();
                //};
            }
            catch (Exception ex)
            {
                R.Logger.Log(LogLevel.ERROR, ex);
            }
        }

        [API.Event.EventHandler]
        public void OnPluginLoaded(PluginLoadedEvent @event)
        {
            var plugin = @event.Plugin;
            if(plugin is MonoBehaviour)
                ((MonoBehaviour)plugin).gameObject.TryAddComponent<PluginUnturnedPlayerComponentManager>();
        }

        [API.Event.EventHandler]
        public void OnPluginUnloaded(PluginUnloadedEvent @event)
        {
            var plugin = @event.Plugin;
            if (plugin is MonoBehaviour)
                ((MonoBehaviour)plugin).gameObject.TryRemoveComponent<PluginUnturnedPlayerComponentManager>();
        }

        public ReadOnlyCollection<IRocketCommand> GetAllCommands()
        {
            List<IRocketCommand> commands = new List<IRocketCommand>()
            {
                new CommandBroadcast(),
                new CommandCompass(),
                new CommandEffect(),
                new CommandGod(),
                new CommandHome(),
                new CommandI(),
                new CommandInvestigate(),
                new CommandTp(),
                new CommandTphere(),
                new CommandV()
            };

            foreach (Command vanillaCommand in Commander.commands)
            {
                commands.Add(new UnturnedVanillaCommand(vanillaCommand));
            }

            return commands.AsReadOnly();
        }

        public ReadOnlyCollection<IRocketPlayer> GetAllPlayers()
        {
            return Provider.clients.Select(p => (IRocketPlayer)UnturnedPlayer.FromSteamPlayer(p)).ToList().AsReadOnly();
        }
        
        public void Reload()
        {
            ImplementationReloadEvent @event = new ImplementationReloadEvent(this);
            @event.Fire();
        }

        public void Shutdown()
        {
            Provider.shutdown();
        }

        public void Unload(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerManager : IPlayerManager
    {
        public List<IRocketPlayer> Players
        {
            get
            {
                return Provider.clients.Select(c => (IRocketPlayer) UnturnedPlayer.FromCSteamID(c.playerID.steamID)).ToList();
            }
        }
    }
}
