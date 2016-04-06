using Rocket.API;
using Rocket.Core;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rocket.Unturned.Permissions
{
    public class UnturnedPermissions : MonoBehaviour
    {
        public delegate void JoinRequested(CSteamID player, ref ESteamRejection? rejectionReason);
        public static event JoinRequested OnJoinRequested;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CheckPermissions(SteamPlayer caller, string permission)
        {
            if (caller.IsAdmin) return true;
            UnturnedPlayer player = caller.ToUnturnedPlayer();

            Regex r = new Regex("^\\/[a-zA-Z]*");
            string requestedCommand = r.Match(permission.ToLower()).Value.ToString().TrimStart('/').ToLower();

            IRocketCommand command = R.Commands.GetCommand(requestedCommand);

            uint? cooldownLeft;
            if (R.Permissions.HasPermission(player, command, out cooldownLeft))
            {
                return true;
            }
            else
            {
                if(cooldownLeft != null)
                {
                    UnturnedChat.Say(player, R.Translate("command_cooldown",cooldownLeft),Color.red);
                }
                else
                {
                    UnturnedChat.Say(player, R.Translate("command_no_permission"), Color.red);
                }
            }
            return false;
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
                            Provider.Reject(r.m_SteamID, reason.Value);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
            return true;
        }
    }
}
