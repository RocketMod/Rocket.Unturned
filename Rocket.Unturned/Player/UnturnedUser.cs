using System;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Unturned.Player {
    public class UnturnedUser : IPlayerUser<UnturnedPlayer>
    {
        private readonly UnturnedPlayerManager manager;

        public UnturnedUser(UnturnedPlayerManager manager, UnturnedPlayer unturnedPlayer)
        {
            this.manager = manager;
            Player = unturnedPlayer;
        }

        public string Id => Player.Id;
        public string Name => Player.Name;
        public string IdentityType => IdentityTypes.Player;
        public IUserManager UserManager => manager;
        public bool IsOnline => manager.GetOnlinePlayerById(Id) != null;
        public DateTime SessionConnectTime => throw new NotImplementedException();
        public DateTime? SessionDisconnectTime => throw new NotImplementedException();
        public DateTime? LastSeen => throw new NotImplementedException();
        public string UserType => "User";
        public UnturnedPlayer Player { get; }

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