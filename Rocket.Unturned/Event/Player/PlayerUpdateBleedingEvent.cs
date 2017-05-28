using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateBleedingEvent: PlayerEvent
    {
        public bool IsBleeding { get; }

        public PlayerUpdateBleedingEvent(IRocketPlayer player, bool isBleeding) : base(player)
        {
            IsBleeding = isBleeding;
        }
    }
}