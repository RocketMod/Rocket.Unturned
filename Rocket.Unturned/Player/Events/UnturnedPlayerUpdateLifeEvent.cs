using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateLifeEvent : PlayerEvent
    {
        public byte Life { get; }
        public UnturnedPlayerUpdateLifeEvent(IPlayer player, byte life) : base(player)
        {
            Life = life;
        }
        public UnturnedPlayerUpdateLifeEvent(IPlayer player, byte life, bool global = true) : base(player, global)
        {
            Life = life;
        }
        public UnturnedPlayerUpdateLifeEvent(IPlayer player, byte life, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Life = life;
        }
    }
}