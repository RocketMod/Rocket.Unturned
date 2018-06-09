using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateWaterEvent : PlayerEvent
    {
        public byte Water { get; }
        public UnturnedPlayerUpdateWaterEvent(IPlayer player, byte water) : base(player)
        {
            Water = water;
        }
        public UnturnedPlayerUpdateWaterEvent(IPlayer player, byte water, bool global = true) : base(player, global)
        {
            Water = water;
        }
        public UnturnedPlayerUpdateWaterEvent(IPlayer player, byte water, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Water = water;
        }
    }
}