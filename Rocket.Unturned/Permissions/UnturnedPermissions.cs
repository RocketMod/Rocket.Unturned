using Rocket.Core.Extensions;
using Rocket.Core.Logging;
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
        public delegate void PermissionRequested(UnturnedPlayer player, string permission, ref bool permissionGranted);
        public static event PermissionRequested OnPermissionRequested;

        public delegate void JoinRequested(CSteamID player, ref ESteamRejection? rejectionReason);
        public static event JoinRequested OnJoinRequested;

        private void Awake()
        {
            OnPermissionRequested = HandlePermissionRequested;
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CheckPermissions(SteamPlayer player, string permission)
        {
            bool permissionGranted = player.IsAdmin;

            if (OnPermissionRequested != null)
            {
                foreach (var handler in OnPermissionRequested.GetInvocationList().Cast<PermissionRequested>())
                {
                    try
                    {
                        handler(player.ToUnturnedPlayer(), permission, ref permissionGranted);
                        if (permissionGranted) return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
            return permissionGranted;
        }

        public static void HandlePermissionRequested(UnturnedPlayer player, string permission, ref bool permissionGranted)
        {
            Regex r = new Regex("^\\/[a-zA-Z]*");
            String requestedPermission = r.Match(permission.ToLower()).Value.ToString().TrimStart('/').ToLower();

            List<string> permissions = Core.R.Permissions.GetPermissions(player);

            if (permissions.Where(p => p.ToLower() == requestedPermission || p.ToLower().StartsWith(requestedPermission + ".")).Count() != 0 || permissions.Contains("*"))
            {
                permissionGranted = true;
                return;
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
