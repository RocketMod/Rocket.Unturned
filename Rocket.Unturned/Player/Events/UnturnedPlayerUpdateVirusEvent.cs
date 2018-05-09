using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerUpdateVirusEvent : PlayerEvent
    {
        public byte Virus { get; }
        public UnturnedPlayerUpdateVirusEvent(IPlayer player, byte virus) : base(player)
        {
            Virus = virus;
        }
        public UnturnedPlayerUpdateVirusEvent(IPlayer player, byte virus, bool global = true) : base(player, global)
        {
            Virus = virus;
        }
        public UnturnedPlayerUpdateVirusEvent(IPlayer player, byte virus, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            Virus = virus;
        }
        public UnturnedPlayerUpdateVirusEvent(IPlayer player, byte virus, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            Virus = virus;
        }
    }
}