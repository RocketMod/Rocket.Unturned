using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core;
using Logger = Rocket.API.Logging.Logger;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rocket.API.Serialisation;
using Rocket.API.Providers;
using Rocket.API.Player;

namespace Rocket.Unturned.Permissions
{
    public class UnturnedPermissions
    {
        public delegate void JoinRequested(CSteamID player, ref ESteamRejection? rejectionReason);
        public static event JoinRequested OnJoinRequested;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CheckPermissions(SteamPlayer caller, string permission)
        {
            UnturnedPlayer player = caller.ToUnturnedPlayer();

            Regex r = new Regex("^\\/[a-zA-Z]*");
            string requestedCommand = r.Match(permission.ToLower()).Value.ToString().TrimStart('/').ToLower();

            IRocketCommand command = R.com.GetCommand(requestedCommand);

            if (command != null)
            {
                if (R.Permissions.HasPermission(player, command))
                {
                    return true;
                }
                else
                {
                    U.Instance.Chat.Say(player, R.Translate("command_no_permission"), Color.red);
                    return false;
                }
            }
            else
            {
                U.Instance.Chat.Say(player, U.Translate("command_not_found"), Color.red);
                return false;
            }
        }

        internal static bool CheckValid(ValidateAuthTicketResponse_t r)
        {
            ESteamRejection? reason = null;

            try
            {
                RocketPermissionsGroup g = R.Permissions.GetGroups(new IRocketPlayer(r.m_SteamID.ToString())).FirstOrDefault();
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
                Logger.Info("Failed adding prefix/suffix to player " + r.m_SteamID + ": " + ex.ToString());
            }

            if (OnJoinRequested != null)
            {
                foreach (var handler in OnJoinRequested.GetInvocationList().Cast<JoinRequested>())
                {
                    try
                    {
                        handler(r.m_SteamID, ref reason);
                        if (reason != null)
                        {
                            Provider.reject(r.m_SteamID, reason.Value);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            return true;
        }
    }
}
