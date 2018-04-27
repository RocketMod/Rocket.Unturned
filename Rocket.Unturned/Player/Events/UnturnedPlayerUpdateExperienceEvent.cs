using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateExperienceEvent : OnlinePlayerEvent
    {
        public uint Experience { get; }
        public UnturnedPlayerUpdateExperienceEvent(IOnlinePlayer player, uint experience) : base(player)
        {
            Experience = experience;
        }
        public UnturnedPlayerUpdateExperienceEvent(IOnlinePlayer player, uint experience, bool global = true) : base(player, global)
        {
            Experience = experience;
        }
        public UnturnedPlayerUpdateExperienceEvent(IOnlinePlayer player, uint experience, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Experience = experience;
        }
        public UnturnedPlayerUpdateExperienceEvent(IOnlinePlayer player, uint experience, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Experience = experience;
        }
    }
}