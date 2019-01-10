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
        public UnturnedUser(IDependencyContainer container, SteamPlayer player) : this(container)
        {
            Id = player.playerID.steamID.ToString();
            CSteamID = player.playerID.steamID;
        }

        public UnturnedUser(IDependencyContainer container, SteamPending player) : this(container)
        {
            Id = player.playerID.steamID.ToString();
            CSteamID = player.playerID.steamID;
        }

        public UnturnedUser(IDependencyContainer container, CSteamID id) : this(container)
        {
            Id = id.ToString();
            CSteamID = id;
        }

        public UnturnedUser(IDependencyContainer container)
        {
            Container = container;
        }

        public UnturnedPlayer Player => (UnturnedPlayer) Container.Resolve<IPlayerManager>().GetPlayerByIdAsync(Id).GetAwaiter().GetResult();

        public DateTime? LastSeen { get; }

        public IUserManager UserManager { get; }

        public string UserName { get; }

        public string DisplayName { get; }

        public List<IIdentity> Identities { get; } = new List<IIdentity>();

        public string UserType => "Unturned";

        public IDependencyContainer Container { get; }

        public string Id { get; }
        public CSteamID CSteamID { get; }
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