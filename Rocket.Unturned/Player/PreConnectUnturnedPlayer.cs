using System;
using Rocket.API.DependencyInjection;
using SDG.Unturned;
using Steamworks;

namespace Rocket.Unturned.Player
{
    public class PreConnectUnturnedPlayer : UnturnedPlayer
    {
        public SteamPending PendingPlayer { get; }

        public PreConnectUnturnedPlayer(IDependencyContainer container, SteamPending pendingPlayer, UnturnedPlayerManager manager) : base(container, pendingPlayer.playerID.steamID, manager)
        {
            PendingPlayer = pendingPlayer;
        }

        public override bool IsOnline => false;

        public override string Id => PendingPlayer.playerID.steamID.ToString();
        public override string Name => PendingPlayer.playerID.playerName;

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            string f = format;
            if (f != null && PendingPlayer != null)
            {
                string[] subFormats = f.Split(':');

                f = subFormats[0];
                string subFormat = subFormats.Length > 1 ? subFormats[1] : null;

                if (f.Equals("nick", StringComparison.OrdinalIgnoreCase)
                    || f.Equals("nickname", StringComparison.OrdinalIgnoreCase))
                {
                    return PendingPlayer.playerID.nickName.ToString(formatProvider);
                }

                if (f.Equals("playername", StringComparison.OrdinalIgnoreCase))
                {
                    return PendingPlayer.playerID.playerName.ToString(formatProvider);
                }

                if (f.Equals("charachtername", StringComparison.OrdinalIgnoreCase))
                {
                    return PendingPlayer.playerID.characterName.ToString(formatProvider);
                }

                if (f.Equals("group", StringComparison.OrdinalIgnoreCase)
                    || f.Equals("groupname", StringComparison.OrdinalIgnoreCase))
                {
                    var gid = PendingPlayer.playerID.@group;
                    if (gid == CSteamID.Nil)
                        return "no group";

                    GroupInfo group = GroupManager.getGroupInfo(gid);
                    return group.name.ToString(formatProvider);
                }


                if (f.Equals("groupid", StringComparison.OrdinalIgnoreCase))
                {
                    var gid = PendingPlayer.playerID.@group;
                    if (gid == CSteamID.Nil)
                        return "no group";

                    return gid.m_SteamID.ToString(subFormat, formatProvider);
                }
            }

            return base.ToString(format, formatProvider);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (!(o is PreConnectUnturnedPlayer uPlayer))
                return false;

            return uPlayer.GetHashCode() == GetHashCode();
        }
    }
}