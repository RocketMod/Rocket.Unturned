using Rocket.API.Event.Player;
using Rocket.API.Player;
using SDG.Unturned;
using Steamworks;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerDeathEvent : PlayerEvent
    {
        public EDeathCause DeathCause { get; }
        public ELimb Limb { get; }
        public CSteamID Killer { get; }

        public PlayerDeathEvent(IRocketPlayer player, EDeathCause deathCause, ELimb limb, CSteamID killer) : base(player)
        {
            DeathCause = deathCause;
            Limb = limb;
            Killer = killer;
        }
    }
}