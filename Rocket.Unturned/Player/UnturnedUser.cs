using System;
using System.Collections.Generic;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.API.User;
using SDG.Unturned;
using Steamworks;

namespace Rocket.Unturned.Player
{
    public class UnturnedUser : IPlayerUser<UnturnedPlayer>
    {
        public IDependencyContainer Container { get; }

        public string Id { get; }
        public CSteamID CSteamID { get; }

        private IPlayerManager playerManager;

        public UnturnedUser(IDependencyContainer container, SteamPlayer player) : this(container, player.playerID.steamID)
        {
        }

        public UnturnedUser(IDependencyContainer container, SteamPending player) : this(container, player.playerID.steamID)
        {
        }

        public UnturnedUser(IDependencyContainer container, CSteamID id) 
        {
            Id = id.ToString();
            CSteamID = id;
            Container = container;
            UserManager = playerManager = container.Resolve<IPlayerManager>();
        }

        public UnturnedPlayer Player => (UnturnedPlayer)playerManager.GetPlayerByIdAsync(Id).GetAwaiter().GetResult();

        public DateTime? LastSeen { get; }

        public IUserManager UserManager { get; }

        public string UserName => Player.SteamName;

        public string DisplayName => Player.DisplayName;

        public List<IIdentity> Identities { get; } = new List<IIdentity>();

        public string UserType => "Unturned";


        public override int GetHashCode()
        {
            return CSteamID.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (!(o is UnturnedUser uPlayer))
                return false;

            return uPlayer.GetHashCode() == GetHashCode();
        }

        IPlayer IPlayerUser.Player => Player;
    }
}