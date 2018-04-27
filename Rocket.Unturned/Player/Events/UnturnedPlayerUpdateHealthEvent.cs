using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateHealthEvent : OnlinePlayerEvent
    {
        public byte Health { get; }
        public UnturnedPlayerUpdateHealthEvent(IOnlinePlayer player, byte health) : base(player)
        {
            Health = health;
        }
        public UnturnedPlayerUpdateHealthEvent(IOnlinePlayer player, byte health, bool global = true) : base(player, global)
        {
            Health = health;
        }
        public UnturnedPlayerUpdateHealthEvent(IOnlinePlayer player, byte health, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Health = health;
        }
        public UnturnedPlayerUpdateHealthEvent(IOnlinePlayer player, byte health, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Health = health;
        }
    }
}