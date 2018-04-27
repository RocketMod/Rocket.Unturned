using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateLifeEvent : OnlinePlayerEvent
    {
        public byte Life { get; }
        public UnturnedPlayerUpdateLifeEvent(IOnlinePlayer player, byte life) : base(player)
        {
            Life = life;
        }
        public UnturnedPlayerUpdateLifeEvent(IOnlinePlayer player, byte life, bool global = true) : base(player, global)
        {
            Life = life;
        }
        public UnturnedPlayerUpdateLifeEvent(IOnlinePlayer player, byte life, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Life = life;
        }
        public UnturnedPlayerUpdateLifeEvent(IOnlinePlayer player, byte life, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Life = life;
        }
    }
}