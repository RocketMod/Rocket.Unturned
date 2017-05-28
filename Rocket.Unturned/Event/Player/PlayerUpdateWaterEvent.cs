using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateWaterEvent:PlayerEvent
    {
        public byte NewWater { get; }

        public PlayerUpdateWaterEvent(IRocketPlayer player, byte newWater) : base(player)
        {
            NewWater = newWater;
        }
    }
}