using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateBrokenEvent : PlayerEvent
    {
        public bool IsBroken { get; }

        public PlayerUpdateBrokenEvent(IRocketPlayer player, bool isBroken) : base(player)
        {
            IsBroken = isBroken;
        }
    }
}