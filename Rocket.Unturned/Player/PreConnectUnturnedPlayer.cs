using System;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.Core.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Player {
    public class PreConnectUnturnedPlayer : BasePlayer
    {
        public SteamPending PendingPlayer { get; }

        public PreConnectUnturnedPlayer(IDependencyContainer container, SteamPending pendingPlayer) : base(container)
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

        public override Type CallerType => typeof(UnturnedPlayer);
        public override bool IsOnline => false;

        public override DateTime? LastSeen => throw new NotImplementedException();



        public override string Id => PendingPlayer.playerID.steamID.ToString();
        public override string Name => PendingPlayer.playerID.playerName;
    }
}