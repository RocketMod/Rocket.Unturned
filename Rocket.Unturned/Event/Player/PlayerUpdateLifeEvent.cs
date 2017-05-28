using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateLifeEvent :PlayerEvent
    {
        public byte NewLife { get; }

        public PlayerUpdateLifeEvent(IRocketPlayer player, byte newLife) : base(player)
        {
            NewLife = newLife;
        }
    }
}