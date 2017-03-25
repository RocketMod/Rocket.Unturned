using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Extensions;
using Rocket.API.Player;
using Rocket.API.Providers.Implementation;
using Rocket.API.Providers.Implementation.Managers;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Rocket.Unturned.Utils;
using SDG.Framework.Translations;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Rocket.Unturned
{
    public class U : IRocketImplementationProvider
    {
        
        internal void triggerOnPlayerConnected(UnturnedPlayer player)
        {
            OnPlayerConnected.TryInvoke(player);
        }

        public static U Instance { get; private set; }

        public string Name { get; private set; } = "Unturned";
        public string InstanceName { get; private set; } = Dedicator.serverID;

        public ushort Port
        {
            get
            {
                return Provider.port;
            }
        }

        IChatManager IRocketImplementationProvider.Chat => throw new NotImplementedException();

        public IPlayerManager Players => throw new NotImplementedException();

        public ReadOnlyCollection<Type> Providers => throw new NotImplementedException();

        public TranslationList DefaultTranslation => throw new NotImplementedException();
     

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
                    R.Logger.Error("Steam can not be initialized: " + ex.Message);
                }
            };

            CommandWindow.onCommandWindowInputted += (string text, ref bool shouldExecuteCommand) =>
            {
                if (text.StartsWith("/")) text.Substring(1);
                R.Commands.Execute(new ConsolePlayer(), text);
                shouldExecuteCommand = false;
            };

            CommandWindow.onCommandWindowOutputted += (object text, ConsoleColor color) =>  {
                R.Logger.Debug(text);
            };

            /*
            SteamChannel.onTriggerReceive += (SteamChannel channel, CSteamID steamID, byte[] packet, int offset, int size) =>
             {
                 UnturnedPlayerEvents.TriggerReceive(channel, steamID, packet, offset, size);
             };
             */

            SteamChannel.onTriggerSend += (SteamPlayer player, string name, ESteamCall mode, ESteamPacket type, object[] arguments) =>
            {
                UnturnedPlayerEvents.TriggerSend(player, name, mode, type, arguments);
            };

            ChatManager.onCheckPermissions += (SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList) =>
            {
                if (text.StartsWith("/"))
                {
                    if (UnturnedPermissions.CheckPermissions(player, text))
                    {
                        R.Commands.Execute(UnturnedPlayer.FromSteamPlayer(player), text);
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
                R.Logger.Error(ex);
            }
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
        
        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }
    }
}
