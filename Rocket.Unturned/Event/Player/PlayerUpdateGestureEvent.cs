using Rocket.API.Event.Player;
using Rocket.API.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateGestureEvent : PlayerEvent
    {
        public EPlayerGesture Gesture { get; }

        public PlayerUpdateGestureEvent(IRocketPlayer player, EPlayerGesture gesture) : base(player)
        {
            Gesture = gesture;
        }
    }
}