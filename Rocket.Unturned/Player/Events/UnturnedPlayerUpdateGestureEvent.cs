using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateGestureEvent : PlayerEvent
    {
        public EPlayerGesture Gesture { get; }
        public UnturnedPlayerUpdateGestureEvent(IPlayer player, EPlayerGesture gesture) : base(player)
        {
            Gesture = gesture;
        }
        public UnturnedPlayerUpdateGestureEvent(IPlayer player, EPlayerGesture gesture, bool global = true) : base(player, global)
        {
            Gesture = gesture;
        }
        public UnturnedPlayerUpdateGestureEvent(IPlayer player, EPlayerGesture gesture, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Gesture = gesture;
        }
    }
}