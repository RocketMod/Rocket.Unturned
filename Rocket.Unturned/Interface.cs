using Rocket.API;
using Rocket.Core;
using Rocket.Logging;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.ComponentModel;

namespace Rocket.Unturned
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Interface
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ExternalLog(string message, ConsoleColor color)
        {
            if(message != "NullReferenceException: Object reference not set to an instance of an object - ")
            Logger.Warn(message);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TriggerReceive(SteamChannel instance, CSteamID d, byte[] a, int b)
        {
            UnturnedPlayerEvents.TriggerReceive(instance, d, a, b);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TriggerSend(SteamPlayer s, string W, ESteamCall X, ESteamPacket l, params object[] R)
        {
            UnturnedPlayerEvents.TriggerSend(s, W, X, l, R);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool CheckPermissions(SteamPlayer player, string command)
        {
            if (UnturnedPermissions.CheckPermissions(player, command))
            {
                R.Instance.Execute(UnturnedPlayer.FromSteamPlayer(player), command);
            }
            return false;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Execute(CSteamID player, string command)
        {
            return R.Instance.Execute(UnturnedPlayer.FromCSteamID(player), command);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool CheckValid(ValidateAuthTicketResponse_t r)
        {
            return UnturnedPermissions.CheckValid(r);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Splash()
        {
            U.Splash();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RegisterRocketEffect(Bundle b, Data q, ushort k)
        {
            //
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InstantiateUI()
        {
            UI.Instantiate();
        }
    }
}
