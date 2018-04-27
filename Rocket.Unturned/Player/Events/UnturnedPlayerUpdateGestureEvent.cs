using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateGestureEvent : OnlinePlayerEvent
    {
        public EPlayerGesture Gesture { get; }
        public UnturnedPlayerUpdateGestureEvent(IOnlinePlayer player, EPlayerGesture gesture) : base(player)
        {
            Gesture = gesture;
        }
        public UnturnedPlayerUpdateGestureEvent(IOnlinePlayer player, EPlayerGesture gesture, bool global = true) : base(player, global)
        {
            Gesture = gesture;
        }
        public UnturnedPlayerUpdateGestureEvent(IOnlinePlayer player, EPlayerGesture gesture, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Gesture = gesture;
        }
        public UnturnedPlayerUpdateGestureEvent(IOnlinePlayer player, EPlayerGesture gesture, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Gesture = gesture;
        }
    }
}