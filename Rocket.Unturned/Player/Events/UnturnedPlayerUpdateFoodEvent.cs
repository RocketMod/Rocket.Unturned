using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateFoodEvent : PlayerEvent
    {
        public byte Food { get; }
        public UnturnedPlayerUpdateFoodEvent(IPlayer player, byte food) : base(player)
        {
            Food = food;
        }
        public UnturnedPlayerUpdateFoodEvent(IPlayer player, byte food, bool global = true) : base(player, global)
        {
            Food = food;
        }
        public UnturnedPlayerUpdateFoodEvent(IPlayer player, byte food, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Food = food;
        }
    }
}