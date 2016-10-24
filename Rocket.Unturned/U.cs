using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Core;
using Rocket.Core.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Events;
using Rocket.Unturned.Plugins;
using Rocket.Unturned.Serialisation;
using SDG.Unturned;
using Steamworks;
using System;
using System.Reflection;
using UnityEngine;
using Rocket.API.Chat;
using System.Collections.ObjectModel;
using Rocket.API.Commands;
using System.Collections.Generic;
using Logger = Rocket.API.Logging.Logger;
using Rocket.Unturned.Player;

namespace Rocket.Unturned
{
    public class U : MonoBehaviour, IRocketImplementation
    {
        #region Events
        public event ImplementationInitialized OnInitialized;
        public event ImplementationShutdown OnShutdown;
        public event ImplementationReload OnReload;

        public event PlayerConnected OnPlayerConnected;
        public event PlayerDisconnected OnPlayerDisconnected;
        #endregion

        #region Static Properties
        public static U Instance { get; private set; }
        #endregion

        #region Static Methods
        internal static void Splash()
        {
            rocketGameObject = new GameObject("Rocket");
            DontDestroyOnLoad(rocketGameObject);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Rocket Unturned v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " for Unturned v" + Provider.APP_VERSION + "\n");

            Provider.onServerHosted += () =>
            {
                rocketGameObject.TryAddComponent<U>();
                R.OnInitialized += () =>
                {
                    Instance.Initialize();
                };
                rocketGameObject.TryAddComponent<R>();
            };
        }

        #endregion

        #region Properties
        public IChat Chat { get; private set; }
        public XMLFileAsset<UnturnedSettings> Settings { get; private set; }
        public XMLFileAsset<TranslationList> Translation { get; private set; }
        public string Name { get; private set; } = "Unturned";
        public string InstanceName { get; private set; } = Dedicator.InstanceName;
        #endregion

        private static GameObject rocketGameObject;

        private void Awake()
        {
            Instance = this;
            Environment.Initialize();
        }

        private void Initialize()
        {
            try
            {
                Settings = new XMLFileAsset<UnturnedSettings>(Environment.SettingsFile);

                UnturnedTranslations defaultTranslations = new UnturnedTranslations();
                Translation = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, R.Instance.Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, defaultTranslations);
                Translation.AddUnknownEntries(defaultTranslations);

                Chat = (IChat)gameObject.TryAddComponent<UnturnedChat>();

                Provider.onServerShutdown += () => { OnShutdown.TryInvoke(); };
                Provider.onServerDisconnected += (CSteamID r) => { OnPlayerDisconnected?.TryInvoke(UnturnedPlayer.FromCSteamID(r)); };
                Provider.onServerConnected += (CSteamID r) =>
                {
                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(r);
                    OnPlayerDisconnected?.Invoke(p);
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerFeatures>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerMovement>();
                    p.Player.gameObject.TryAddComponent<UnturnedPlayerEvents>();
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

                try
                {
                    SteamGameServer.SetKeyValue("rocket", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    SteamGameServer.SetBotPlayerCount(1);
                }
                catch (Exception ex)
                {
                    Logger.Error("Steam can not be initialized: " + ex.Message);
                }

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
                new Commands.CommandBroadcast(),
                new Commands.CommandCompass(),
                new Commands.CommandEffect(),
                new Commands.CommandGod(),
                new Commands.CommandHome(),
                new Commands.CommandI(),
                new Commands.CommandInvestigate(),
                new Commands.CommandTp(),
                new Commands.CommandTphere(),
                new Commands.CommandV()
            };

            foreach (Command vanillaCommand in Commander.Commands)
            {
                commands.Add(new UnturnedVanillaCommand(vanillaCommand));
            }

            return commands.AsReadOnly();
        }

        public void Reload()
        {
            Translation.Load();
            Settings.Load();
        }

        public void Shutdown()
        {
            Provider.shutdown();
        }
    }
}
