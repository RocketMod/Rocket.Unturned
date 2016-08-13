using Rocket.API;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Core;
using Rocket.Core.Extensions;
using Rocket.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
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

namespace Rocket.Unturned
{
    public class U : MonoBehaviour, IRocketImplementation
    {
        private static GameObject rocketGameObject; 
        public static U Instance;

        private static readonly TranslationList defaultTranslations = new TranslationList(){
            { "command_generic_failed_find_player","Failed to find player"},
                { "command_generic_invalid_parameter","Invalid parameter"},
                { "command_generic_target_player_not_found","Target player not found"},
                { "command_generic_teleport_while_driving_error","You cannot teleport while driving or riding in a vehicle."},
                { "command_god_enable_console","{0} enabled Godmode"},
                { "command_god_enable_private","You can feel the strength now..."},
                { "command_god_disable_console","{0} disabled Godmode"},
                { "command_god_disable_private","The godly powers left you..."},
                { "command_vanish_enable_console","{0} enabled Vanishmode"},
                { "command_vanish_enable_private","You are vanished now..."},
                { "command_vanish_disable_console","{0} disabled Vanishmode"},
                { "command_vanish_disable_private","You are no longer vanished..."},

                { "command_duty_enable_console","{0} is in duty"},
                { "command_duty_enable_private","You are in duty now..."},
                { "command_duty_disable_console","{0} is no longer in duty"},
                { "command_duty_disable_private","You are no longer in duty..."},

                { "command_bed_no_bed_found_private","You do not have a bed to teleport to."},
                { "command_i_giving_console","Giving {0} item {1}:{2}"},
                { "command_i_giving_private","Giving you item {0}x {1} ({2})"},
                { "command_z_giving_console","Spawning {1} zombies near {0}"},
                { "command_z_giving_private","Spawning {0} zombies nearby"},
                { "command_i_giving_failed_private","Failed giving you item {0}x {1} ({2})"},
                { "command_v_giving_console","Giving {0} vehicle {1}"},
                { "command_v_giving_private","Giving you a {0} ({1})"},
                { "command_v_giving_failed_private","Failed giving you a {0} ({1})"},
                { "command_tps_tps","TPS: {0}"},
                { "command_tps_running_since","Running since: {0} UTC"},
                { "command_p_reload_private","Reloaded permissions"},
                { "command_p_groups_private","{0} groups are: {1}"},
                { "command_p_permissions_private","{0} permissions are: {1}"},
                { "command_tp_teleport_console","{0} teleported to {1}"},
                { "command_tp_teleport_private","Teleported to {0}"},
                { "command_tp_failed_find_destination","Failed to find destination"},
                { "command_tphere_teleport_console","{0} was teleported to {1}"},
                { "command_tphere_teleport_from_private","Teleported {0} to you"},
                { "command_tphere_teleport_to_private","You were teleported to {0}"},
                { "command_clear_error","There was an error clearing {0} inventory."},
                { "command_clear_private","Your inventory was cleared!"},
                { "command_clear_other","Your inventory was cleared by {0}!"},
                { "command_clear_other_success","You successfully cleared {0} inventory."},
                { "command_investigate_private","{0} SteamID64 is {1}"},
                { "command_heal_success_me","{0} was successfully healed"},
                { "command_heal_success_other","You were healed by {0}"},
                { "command_heal_success","You were healed"},
                { "command_compass_facing_private","You are facing {0}"},
                { "command_compass_north","N"},
                { "command_compass_east","E"},
                { "command_compass_south","S"},
                { "command_compass_west","W"},
                { "command_compass_northwest","NW"},
                { "command_compass_northeast","NE"},
                { "command_compass_southwest","SW"},
                { "command_compass_southeast","SE"},
                { "command_rocket_plugins_loaded","Loaded: {0}"},
                { "command_rocket_plugins_unloaded","Unloaded: {0}"},
                { "command_rocket_plugins_failure","Failure: {0}"},
                { "command_rocket_plugins_cancelled","Cancelled: {0}"},
                { "command_rocket_reload_plugin","Reloading {0}"},
                { "command_rocket_not_loaded","The plugin {0} is not loaded"},
                { "command_rocket_unload_plugin","Unloading {0}"},
                { "command_rocket_load_plugin","Loading {0}"},
                { "command_rocket_already_loaded","The plugin {0} is already loaded"},
                { "command_rocket_reload","Reloading Rocket"},
                { "command_p_group_not_found","Group not found"},
                { "command_p_group_player_added","{0} was added to the group {1}"},
                { "command_p_group_player_removed","{0} was removed from from the group {1}"},
                { "command_p_unknown_error","Unknown error"},
                { "command_p_player_not_found","{0} was not found"},
                { "command_p_group_not_found","{1} was not found"},
                { "command_p_duplicate_entry","{0} is already in the group {1}"},
                { "command_p_permissions_reload","Permissions reloaded"},
                { "command_rocket_plugin_not_found","Plugin {0} not found"},
                { "command_clear_success","You successfully cleared {0} items"},
                { "invalid_character_name","invalid character name"},
                { "command_not_found","Command not found."}
        };
         

        public static XMLFileAsset<UnturnedSettings> Settings;
        public static XMLFileAsset<TranslationList> Translation;

        public static UnturnedEvents Events;

        public event ImplementationInitialized OnInitialized;
        public event ImplementationShutdown OnShutdown;
        public event ImplementationReload OnReload;

        public static string Translate(string translationKey, params object[] placeholder)
        {
            return Translation.Instance.Translate(translationKey, placeholder);
        }

        public ReadOnlyCollection<IRocketCommand> GetCommands()
        {
            List<IRocketCommand> commands = new List<IRocketCommand>()
            {
                new Commands.CommandAdmin(),
                new Commands.CommandBroadcast(),
                new Commands.CommandCompass(),
                new Commands.CommandEffect(),
                new Commands.CommandGod(),
                new Commands.CommandHome(),
                new Commands.CommandI(),
                new Commands.CommandInvestigate(),
                new Commands.CommandTp(),
                new Commands.CommandTphere(),
                new Commands.CommandUnadmin(),
                new Commands.CommandV()
            };

            foreach (Command vanillaCommand in Commander.Commands)
            {
                commands.Add(new UnturnedVanillaCommand(vanillaCommand));
            }

            return commands.AsReadOnly();
        }

#if LINUX
        public static UnturnedConsole Console;
#endif

        internal static void Splash()
        {
            rocketGameObject = new GameObject("Rocket");
            DontDestroyOnLoad(rocketGameObject);
#if LINUX
            Console = rocketGameObject.AddComponent<UnturnedConsole>();
#endif
            System.Console.Clear();
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("Rocket Unturned v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " for Unturned v" + Provider.Version + "\n");

            R.Instance.OnInitialized += () =>
            {
                Instance.Initialize();
            };

            Provider.onServerHosted += () =>
            {
                rocketGameObject.TryAddComponent<U>();
                rocketGameObject.TryAddComponent<R>();
            };
        }
        
        private void Awake()
        {
            Instance = this;
            Environment.Initialize();
        }

        internal void Initialize()
        {
            try
            {
                Settings = new XMLFileAsset<UnturnedSettings>(Environment.SettingsFile);
                Translation = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, R.Instance.Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry) }, defaultTranslations);
                defaultTranslations.AddUnknownEntries(Translation);
                Events = gameObject.TryAddComponent<UnturnedEvents>();
                
                gameObject.TryAddComponent<UnturnedChat>();


                RocketPluginBase.OnPluginsLoading += (RocketPluginBase plugin, ref bool cancelLoading) =>
                {
                    try
                    {
                        plugin.gameObject.TryAddComponent<PluginUnturnedPlayerComponentManager>();
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("Failed to load plugin " + plugin.Name + ".",ex);
                        cancelLoading = true;
                    }
                };

                RocketPluginBase.OnPluginsUnloading += (RocketPluginBase plugin) =>
                {
                    plugin.gameObject.TryRemoveComponent<PluginUnturnedPlayerComponentManager>();
                };

                try
                {
                    //TODO: Readd rocketplugins

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
        
        public void Reload()
        {
            Translation.Load();
            Settings.Load();
        }

        public void Shutdown()
        {
            Provider.shutdown();
        }

        public ReadOnlyCollection<IRocketPlayer> GetPlayers()
        {
            throw new NotImplementedException();
        }

        public string InstanceId
        {
            get
            {
                return Dedicator.InstanceName;
            } 
        }

        public IChat Chat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return "Unturned v" + Provider.Version;
            }
        }
    }
}
