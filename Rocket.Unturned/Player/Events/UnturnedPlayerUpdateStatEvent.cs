using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateStatEvent : PlayerEvent
    {
        public EPlayerStat Stat { get; }
        public UnturnedPlayerUpdateStatEvent(IPlayer player, EPlayerStat stat) : base(player)
        {
            Stat = stat;
        }
        public UnturnedPlayerUpdateStatEvent(IPlayer player, EPlayerStat stat, bool global = true) : base(player, global)
        {
            Stat = stat;
        }
        public UnturnedPlayerUpdateStatEvent(IPlayer player, EPlayerStat stat, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Stat = stat;
        }
        public UnturnedPlayerUpdateStatEvent(IPlayer player, EPlayerStat stat, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Stat = stat;
        }
    }
}