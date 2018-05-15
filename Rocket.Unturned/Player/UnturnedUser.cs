using System;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Unturned.Player {
    public class UnturnedUser : IPlayerUser
    {
        private readonly UnturnedPlayerManager manager;

        public UnturnedUser(UnturnedPlayerManager manager, UnturnedPlayer unturnedPlayer)
        {
            this.manager = manager;
            UnturnedPlayer = unturnedPlayer;
        }

        public string Id => UnturnedPlayer.Id;
        public string Name => UnturnedPlayer.Name;
        public IdentityType Type => IdentityType.Player;
        public IUserManager UserManager => manager;
        public bool IsOnline => manager.GetOnlinePlayerById(Id) != null;
        public DateTime SessionConnectTime => throw new NotImplementedException();
        public DateTime? SessionDisconnectTime => throw new NotImplementedException();
        public DateTime? LastSeen => throw new NotImplementedException();
        public string UserType => "User";
        public IPlayer Player => UnturnedPlayer;
        public UnturnedPlayer UnturnedPlayer { get; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (!(o is UnturnedUser uPlayer))
                return false;

            return uPlayer.GetHashCode() == GetHashCode();
        }

    }
}