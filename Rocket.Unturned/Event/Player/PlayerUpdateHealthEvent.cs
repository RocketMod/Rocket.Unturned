using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateHealthEvent: PlayerEvent
    {
        public byte NewHealth { get; }

        public PlayerUpdateHealthEvent(IRocketPlayer player, byte newHealth) : base(player)
        {
            NewHealth = newHealth;
        }
    }
}