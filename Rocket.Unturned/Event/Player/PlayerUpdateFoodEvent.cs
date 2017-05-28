using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateFoodEvent : PlayerEvent
    {
        public byte NewFood { get; }

        public PlayerUpdateFoodEvent(IRocketPlayer player, byte newFood) : base(player)
        {
            NewFood = newFood;
        }
    }
}