using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateStaminaEvent : PlayerEvent
    {
        public byte NewStamine { get; }

        public PlayerUpdateStaminaEvent(IRocketPlayer player, byte newStamine) : base(player)
        {
            NewStamine = newStamine;
        }
    }
}