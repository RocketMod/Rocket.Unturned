using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateHealthEvent : PlayerEvent
    {
        public byte Health { get; }
        public UnturnedPlayerUpdateHealthEvent(IPlayer player, byte health) : base(player)
        {
            Health = health;
        }
        public UnturnedPlayerUpdateHealthEvent(IPlayer player, byte health, bool global = true) : base(player, global)
        {
            Health = health;
        }
        public UnturnedPlayerUpdateHealthEvent(IPlayer player, byte health, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Health = health;
        }
        public UnturnedPlayerUpdateHealthEvent(IPlayer player, byte health, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Health = health;
        }
    }
}