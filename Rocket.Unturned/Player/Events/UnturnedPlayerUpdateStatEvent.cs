using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateStatEvent : OnlinePlayerEvent
    {
        public EPlayerStat Stat { get; }
        public UnturnedPlayerUpdateStatEvent(IOnlinePlayer player, EPlayerStat stat) : base(player)
        {
            Stat = stat;
        }
        public UnturnedPlayerUpdateStatEvent(IOnlinePlayer player, EPlayerStat stat, bool global = true) : base(player, global)
        {
            Stat = stat;
        }
        public UnturnedPlayerUpdateStatEvent(IOnlinePlayer player, EPlayerStat stat, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Stat = stat;
        }
        public UnturnedPlayerUpdateStatEvent(IOnlinePlayer player, EPlayerStat stat, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Stat = stat;
        }
    }
}