using System.Collections.Generic;
using System.Linq;
using Rocket.API.Player;
using Rocket.API.Providers.Implementation.Managers;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Manager
{
    public class UnturnedPlayerManager  : IPlayerManager
    {
        //todo: have internal list updated on player connect / disconnect instead (to reduce foreach overload)
        public List<IRocketPlayer> Players
            => Provider.clients.Select(c => (IRocketPlayer) UnturnedPlayer.FromSteamPlayer(c)).ToList();
    }
}