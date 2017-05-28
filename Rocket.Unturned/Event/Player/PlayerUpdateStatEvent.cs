using Rocket.API.Event.Player;
using Rocket.API.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateStatEvent: PlayerEvent
    {
        public EPlayerStat PlayerStat { get; }

        public PlayerUpdateStatEvent(IRocketPlayer player, EPlayerStat stat) : base(player)
        {
            PlayerStat = stat;
        }
    }
}