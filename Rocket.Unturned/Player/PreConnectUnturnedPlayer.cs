using System;
using Rocket.API.Permissions;
using Rocket.API.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Player {
    public class PreConnectUnturnedPlayer : IPlayer
    {
        public SteamPending PendingPlayer { get; }

        public PreConnectUnturnedPlayer(SteamPending pendingPlayer)
        {
            PendingPlayer = pendingPlayer;
        }

        public int CompareTo(IIdentifiable other) => string.Compare(Id, other.Id, StringComparison.Ordinal);

        public bool Equals(IIdentifiable other)
        {
            if (other == null)
                return false;

            return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(string other) => string.Compare(Id, other, StringComparison.Ordinal);

        public bool Equals(string other) => Id.Equals(other, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Id;
        public int CompareTo(object obj)
        {
            return CompareTo((PreConnectUnturnedPlayer) obj);
        }

        public void SendMessage(string message)
        {
            throw new NotSupportedException("Can not send messages to pre connect players");
        }

        public string Name => PendingPlayer.playerID.playerName;

        public Type PlayerType => typeof(UnturnedPlayer);

        public string Id => PendingPlayer.playerID.steamID.ToString();
    }
}