using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateStanceEvent : OnlinePlayerEvent
    {
        public EPlayerStance Stance { get; }
        public UnturnedPlayerUpdateStanceEvent(IOnlinePlayer player, EPlayerStance stance) : base(player)
        {
            Stance = stance;
        }
        public UnturnedPlayerUpdateStanceEvent(IOnlinePlayer player, EPlayerStance stance, bool global = true) : base(player, global)
        {
            Stance = stance;
        }
        public UnturnedPlayerUpdateStanceEvent(IOnlinePlayer player, EPlayerStance stance, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Stance = stance;
        }
        public UnturnedPlayerUpdateStanceEvent(IOnlinePlayer player, EPlayerStance stance, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Stance = stance;
        }
    }
}