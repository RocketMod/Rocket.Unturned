using Rocket.API.Commands;
using Rocket.Core;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Rocket.API.Providers.Logging;
using UnityEngine;
using Rocket.API.Serialisation;
using Rocket.Unturned.Event.Player;

namespace Rocket.Unturned.Permissions
{
    public class UnturnedPermissions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CheckPermissions(SteamPlayer caller, string permission)
        {
            UnturnedPlayer player = caller.ToUnturnedPlayer();

            Regex r = new Regex("^\\/[a-zA-Z]*");
            string requestedCommand = r.Match(permission.ToLower()).Value.TrimStart('/').ToLower();

            IRocketCommand command = R.Commands.Commands.FirstOrDefault(c => c.Name.Equals(requestedCommand, StringComparison.OrdinalIgnoreCase));

            if (command != null)
            {
                if ((command.Permissions != null && command.Permissions.Any(c => R.Permissions.HasPermission(player, c))) || R.Permissions.HasPermission(player, command.Name) || (command.Aliases != null && command.Aliases.Any(c => R.Permissions.HasPermission(player, c))))
                {
                    return true;
                }

                string language = R.Translations.GetCurrentLanguage();
                R.Implementation.Chat.Say(player, R.Translations.Translate("command_no_permission", language), Color.red);
                return false;
            }
            else
            {
                R.Implementation.Chat.Say(player, R.Translations.Translate("command_not_found"), Color.red);
                return false;
            }
        }

        internal static bool CheckValid(ValidateAuthTicketResponse_t r)
        {
            ESteamRejection? reason = null;

            try
            {
                RocketPermissionsGroup g = R.Permissions.GetGroups(r.m_SteamID.ToString()).FirstOrDefault();
                if (g != null)
                {
                    SteamPending steamPending = Provider.pending.FirstOrDefault(x => x.playerID.steamID == r.m_SteamID);
                    if (steamPending != null)
                    {
                        string prefix = g.Properties[BuiltinProperties.PREFIX] == null ? "" : g.Properties[BuiltinProperties.PREFIX];
                        string suffix = g.Properties[BuiltinProperties.SUFFIX] == null ? "" : g.Properties[BuiltinProperties.SUFFIX];
                        if (prefix != "" && !steamPending.playerID.characterName.StartsWith(prefix))
                        {
                            steamPending.playerID.characterName = prefix + steamPending.playerID.characterName;
                        }
                        if (suffix != "" && !steamPending.playerID.characterName.EndsWith(suffix))
                        {
                            steamPending.playerID.characterName = steamPending.playerID.characterName + suffix;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                R.Logger.Log(LogLevel.INFO, "Failed adding prefix/suffix to player " + r.m_SteamID + ": " + ex.ToString());
            }
            var pendings = Provider.pending.ToList(); // use ToList to create a copy of the original list
            PlayerJoinRequestEvent @event = new PlayerJoinRequestEvent(pendings.First(c => c.playerID.steamID == r.m_SteamID));
            @event.Fire();

            if (@event.Result != null)
            {
                Provider.reject(r.m_SteamID, @event.Result.Value);
                return false;
            }

            return true;
        }
    }
}
