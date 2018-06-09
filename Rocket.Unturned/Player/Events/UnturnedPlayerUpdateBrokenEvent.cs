using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateBrokenEvent : PlayerEvent
    {
        public bool IsBroken { get; }
        public UnturnedPlayerUpdateBrokenEvent(IPlayer player, bool isBroken) : base(player)
        {
            IsBroken = isBroken;
        }
        public UnturnedPlayerUpdateBrokenEvent(IPlayer player, bool isBroken, bool global = true) : base(player, global)
        {
            IsBroken = isBroken;
        }
        public UnturnedPlayerUpdateBrokenEvent(IPlayer player, bool isBroken, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            IsBroken = isBroken;
        }
    }
}