using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Unturned.Player {
    public class OfflineUnturnedUserInfo : IUserInfo
    {
        private readonly IPlayer player;

        public OfflineUnturnedUserInfo(IUserManager userManager, IPlayer player)
        {
            UserManager = userManager;
            this.player = player;
        }

        public string Id => player.Id;
        public string Name => player.Name;
        public string IdentityType => player.IdentityType;
        public IUserManager UserManager { get; }
    }
}