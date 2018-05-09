using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateBleedingEvent : PlayerEvent
    {
        public bool IsBleeding { get; }
        public UnturnedPlayerUpdateBleedingEvent(IPlayer player, bool isBleeding) : base(player)
        {
            IsBleeding = isBleeding;
        }
        public UnturnedPlayerUpdateBleedingEvent(IPlayer player, bool isBleeding, bool global = true) : base(player, global)
        {
            IsBleeding = isBleeding;
        }
        public UnturnedPlayerUpdateBleedingEvent(IPlayer player, bool isBleeding, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            IsBleeding = isBleeding;
        }
        public UnturnedPlayerUpdateBleedingEvent(IPlayer player, bool isBleeding, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            IsBleeding = isBleeding;
        }
    }
}