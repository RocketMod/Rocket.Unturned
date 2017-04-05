using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Commands;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Events;
using Rocket.Unturned.Plugins;
using Rocket.Unturned.Player;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Utils;
using SDG.Framework.Modules;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Rocket.API.Event;
using Rocket.API.Event.Implementation;
using Rocket.API.Event.Player;
using Rocket.API.Event.Plugin;
using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Implementation;
using Rocket.API.Providers.Implementation.Managers;
using Rocket.API.Providers.Logging;
using Rocket.API.Serialisation;
using Rocket.Core.Player;

namespace Rocket.Unturned
{
    [NoProviderAutoRegistration]
    public class U : MonoBehaviour, IRocketImplementationProvider, IListener
    {
        
        #region Events
        internal void triggerOnPlayerConnected(UnturnedPlayer player)
        {
            PlayerConnectedEvent @event = new PlayerConnectedEvent(player);
            EventManager.Instance.CallEvent(@event);
        }

        #endregion

        #region Static Properties
        public static U Instance { get; private set; }
        #endregion

        #region Static Methods
        public static string Translate(string translationKey, params object[] placeholder)
        {
            return Instance.Translation.Instance.Translate(translationKey, placeholder);
        }

        #endregion

        #region Properties
        public IChatManager Chat { get; private set; }
        public XMLFileAsset<UnturnedSettings> Settings { get; private set; }
        public XMLFileAsset<TranslationList> Translation { get; private set; }
        public string Name { get; private set; } = "Unturned";
        public string InstanceName { get; private set; } = Dedicator.serverID;

        public ushort Port => Provider.port;

        #endregion

        private static GameObject rocketGameObject;
#if DEBUG
        private bool debug = true;
#else
        private bool debug = false;
#endif
        private void Awake()
        {
            Instance = this;
            Environment.Initialize();
        }

        public static void Initialize()
        {
            rocketGameObject = new GameObject("Rocket");
            DontDestroyOnLoad(rocketGameObject);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Rocket Unturned v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " for Unturned v" + Provider.APP_VERSION + "\n");
           
            rocketGameObject.TryAddComponent<U>();
            R.Logger.OnLog += message =>
            {
                if (message == null) return;
                string m = message.Message;
                if (m == null) m = "NULL";
                if (m.StartsWith("[Unturned]")) return;

                ConsoleColor old = Console.ForegroundColor;

                switch (message.LogLevel)
                {
#if DEBUG
                    case LogLevel.DEBUG:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(m);
                        break;
#endif
                    case LogLevel.INFO:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(m);
                        break;
                    case LogLevel.WARN:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(m);
                        break;
                    case LogLevel.ERROR:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(m);
                        if (message.Exception != null)
                            Console.WriteLine(message.Exception);
                        break;

                    case LogLevel.FATAL:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(m);
                        if (message.Exception != null)
                            Console.WriteLine(message.Exception);
                        break;
                }
                Console.ForegroundColor = old;
            };

            Provider.onServerHosted += () =>
            {
                try
                {
                    SteamGameServer.SetKeyValue("rocket", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    SteamGameServer.SetKeyValue("rocketplugins", String.Join(", ", R.Plugins.GetPlugins().Select(p => p.Name).ToArray()));
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
                R.Execute(new ConsolePlayer(), text);
                shouldExecuteCommand = false;
            };

            CommandWindow.onCommandWindowOutputted += (text, color) =>  {
                R.Logger.Debug(text);
            };

            /*
            SteamChannel.onTriggerReceive += (SteamChannel channel, CSteamID steamID, byte[] packet, int offset, int size) =>
             {
                 UnturnedPlayerEvents.TriggerReceive(channel, steamID, packet, offset, size);
             };
             */

            SteamChannel.onTriggerSend += (player, name, mode, type, arguments) =>
            {
                UnturnedPlayerEvents.TriggerSend(player, name, mode, type, arguments);
            };

            ChatManager.onCheckPermissions += (SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList) =>
            {
                if (text.StartsWith("/"))
                {
                    text.Substring(1);
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
            
            R.Bootstrap<U>();
        }

        private void Start()
        {
            try
            {
                Settings = new XMLFileAsset<UnturnedSettings>(Environment.SettingsFile);

                TranslationList defaultTranslations = new TranslationList();
                defaultTranslations.AddRange(new UnturnedTranslations());
                Translation = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, R.Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(PropertyListEntry) }, defaultTranslations);
                Translation.AddUnknownEntries(defaultTranslations);

                Chat = gameObject.TryAddComponent<UnturnedChat>();
                gameObject.TryAddComponent<AutomaticSaveWatchdog>();
                Provider.onServerShutdown += () =>
                {
                    ImplementationShutdownEvent @event = new ImplementationShutdownEvent(this);
                    @event.Fire();
                };
                Provider.onServerDisconnected += r => {
                    PlayerDisconnectedEvent @event = new PlayerDisconnectedEvent(UnturnedPlayer.FromCSteamID(r));
                    @event.Fire();
                };

                Provider.onServerConnected += r =>
                {
                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(r);
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerFeatures>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerMovement>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerEvents>();
                    PrePlayerConnectedEvent @event = new PrePlayerConnectedEvent(p);
                    @event.Fire();
                };

                EventManager.Instance.RegisterEventsInternal(this, null);
#if !DEBUG
                debug = Instance.Settings.Instance.Debug;
#endif
                ImplementationInitializedEvent initedEvent = new ImplementationInitializedEvent(this);
                @initedEvent.Fire();
            }
            catch (Exception ex)
            {
                R.Logger.Error(ex);
            }
        }

        [API.Event.EventHandler]
        public void OnPluginLoaded(PluginLoadedEvent @event)
        {
            var plugin = @event.Plugin;
            if(plugin is RocketPluginBase)
                ((RocketPluginBase)plugin).gameObject.TryAddComponent<PluginUnturnedPlayerComponentManager>();
        }

        [API.Event.EventHandler]
        public void OnPluginUnloaded(PluginUnloadedEvent @event)
        {
            var plugin = @event.Plugin;
            if (plugin is RocketPluginBase)
                ((RocketPluginBase)plugin).TryRemoveComponent<PluginUnturnedPlayerComponentManager>();
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
            Translation.Load();
            Settings.Load();
            ImplementationReloadEvent @event = new ImplementationReloadEvent(this);
            @event.Fire();
        }

        public void Shutdown()
        {
            Provider.shutdown();
        }

        public ReadOnlyCollection<Type> Providers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TranslationList DefaultTranslation => new TranslationList();
        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public IPlayerManager Players => new PlayerManager();
    }

    public class PlayerManager : IPlayerManager
    {
        public List<IRocketPlayer> Players
        {
            get
            {
                return Provider.clients.Select(c => UnturnedPlayer.FromCSteamID(c.playerID.steamID)).ToList();
            }
        }
    }
}
