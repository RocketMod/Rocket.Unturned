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

            IRocketCommand command = R.Instance.GetCommand(requestedCommand);

            if (command != null)
            {
                if (R.Instance.Permissions.HasPermission(player, command))
                {
                    return true;
                }
                else
                {
                    U.Instance.Chat.Say(player, R.Instance.Translation.Instance.Translate("command_no_permission"), Color.red);
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
