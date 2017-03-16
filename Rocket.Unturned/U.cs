using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.Core;
using Rocket.Core.Extensions;
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
using Logger = Rocket.API.Logging.Logger;
using Rocket.API.Serialisation;

namespace Rocket.Unturned
{
    public class U : MonoBehaviour, IRocketImplementation
    {
        
        #region Events
        public event ImplementationInitialized OnInitialized;

        internal void triggerOnPlayerConnected(UnturnedPlayer player)
        {
            OnPlayerConnected.TryInvoke(player);
        }

        public event ImplementationShutdown OnShutdown;
        public event ImplementationReload OnReload;

        public event PlayerConnected OnBeforePlayerConnected;
        public event PlayerConnected OnPlayerConnected;
        public event PlayerDisconnected OnPlayerDisconnected;
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
        public IChat Chat { get; private set; }
        public XMLFileAsset<UnturnedSettings> Settings { get; private set; }
        public XMLFileAsset<TranslationList> Translation { get; private set; }
        public string Name { get; private set; } = "Unturned";
        public string InstanceName { get; private set; } = Dedicator.serverID;

        public ushort Port
        {
            get
            {
                return Provider.port;
            }
        }
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
            Logger.OnLog += (API.Logging.LogMessage message) =>
            {
                if (message == null) return;
                string m = message.Message;
                if (m == null) m = "NULL";
                if (m.StartsWith("[Unturned]")) return;

                ConsoleColor old = Console.ForegroundColor;

                switch (message.LogLevel)
                {
#if DEBUG
                    case API.Logging.LogLevel.DEBUG:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(m);
                        break;
#endif
                    case API.Logging.LogLevel.INFO:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(m);
                        break;
                    case API.Logging.LogLevel.WARN:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(m);
                        break;
                    case API.Logging.LogLevel.ERROR:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(m);
                        if (message.Exception != null)
                            Console.WriteLine(message.Exception);
                        break;
                    case API.Logging.LogLevel.FATAL:
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
                    SteamGameServer.SetKeyValue("rocketplugins", String.Join(", ", R.GetAllPlugins().Select(p => p.Name).ToArray()));
                    SteamGameServer.SetBotPlayerCount(1);
                }
                catch (Exception ex)
                {
                    Logger.Error("Steam can not be initialized: " + ex.Message);
                }
            };

            CommandWindow.onCommandWindowInputted += (string text, ref bool shouldExecuteCommand) =>
            {
                if (text.StartsWith("/")) text.Substring(1);
                R.Execute(new ConsolePlayer(), text);
                shouldExecuteCommand = false;
            };

            CommandWindow.onCommandWindowOutputted += (object text, ConsoleColor color) =>  {
                Logger.Debug(text);
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
            rocketGameObject.TryAddComponent<R>();
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
                Provider.onServerShutdown += () => { OnShutdown.TryInvoke(); };
                Provider.onServerDisconnected += (CSteamID r) => {
                    OnPlayerDisconnected?.TryInvoke(UnturnedPlayer.FromCSteamID(r));
                };
                Provider.onServerConnected += (CSteamID r) =>
                {
                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(r);
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerFeatures>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerMovement>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerEvents>();
                    OnBeforePlayerConnected.TryInvoke(p);
                };

                RocketPluginBase.OnPluginsLoading += (RocketPluginBase plugin, ref bool cancelLoading) =>
                {
                    try
                    {
                        plugin.gameObject.TryAddComponent<PluginUnturnedPlayerComponentManager>();
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("Failed to load plugin " + plugin.Name + ".", ex);
                        cancelLoading = true;
                    }
                };

                RocketPluginBase.OnPluginsUnloading += (RocketPluginBase plugin) =>
                {
                    plugin.gameObject.TryRemoveComponent<PluginUnturnedPlayerComponentManager>();
                };
#if !DEBUG
                debug = Instance.Settings.Instance.Debug;
#endif
                OnInitialized.TryInvoke();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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
        
        public void Reload()
        {
            Translation.Load();
            Settings.Load();
            OnReload?.Invoke();
        }

        public void Shutdown()
        {
            Provider.shutdown();
        }
    }
}
