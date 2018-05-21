using System;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player;
using SDG.Unturned;
using Steamworks;

namespace Rocket.Unturned.Player
{
    public class PreConnectUnturnedPlayer : BasePlayer
    {
        public SteamPending PendingPlayer { get; }

        public PreConnectUnturnedPlayer(IDependencyContainer container, SteamPending pendingPlayer) : base(container)
        {
            PendingPlayer = pendingPlayer;
        }

        public override IUser User => null;
        public override IPlayerEntity Entity => null;
        public override bool IsOnline => false;

        public override string Id => PendingPlayer.playerID.steamID.ToString();
        public override string Name => PendingPlayer.playerID.playerName;

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            if (format != null && PendingPlayer != null)
            {
                string[] subFormats = format.Split(':');

                format = subFormats[0];
                string subFormat = subFormats.Length > 1 ? subFormats[1] : null;

                if (format.Equals("nick", StringComparison.OrdinalIgnoreCase)
                    || format.Equals("nickname", StringComparison.OrdinalIgnoreCase))
                {
                    return PendingPlayer.playerID.nickName.ToString(formatProvider);
                }

                if (format.Equals("playername", StringComparison.OrdinalIgnoreCase))
                {
                    return PendingPlayer.playerID.playerName.ToString(formatProvider);
                }

                if (format.Equals("charachtername", StringComparison.OrdinalIgnoreCase))
                {
                    return PendingPlayer.playerID.characterName.ToString(formatProvider);
                }

                if (format.Equals("group", StringComparison.OrdinalIgnoreCase)
                    || format.Equals("groupname", StringComparison.OrdinalIgnoreCase))
                {
                    var gid = PendingPlayer.playerID.@group;
                    if (gid == CSteamID.Nil)
                        return "no group";

                    GroupInfo group = GroupManager.getGroupInfo(gid);
                    return group.name.ToString(formatProvider);
                }


                if (format.Equals("groupid", StringComparison.OrdinalIgnoreCase))
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