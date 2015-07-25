using Rocket.Unturned.Effects;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
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
            Core.Logging.Logger.ExternalLog(message, color);
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
        public static bool CheckPermissions(SteamPlayer player, string permission)
        {
            return UnturnedPermissions.CheckPermissions(player, permission);
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
            UnturnedEffectManager.RegisterRocketEffect(b, q, k);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InstantiateUI()
        {
            UI.Instantiate();
        }
    }
}
