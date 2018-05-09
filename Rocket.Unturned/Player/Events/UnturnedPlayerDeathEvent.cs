using Rocket.API.Entities;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerDeathEvent : PlayerDeathEvent
    {
        public ELimb Limb { get; }
        public EDeathCause DeathCause { get; }

        public UnturnedPlayerDeathEvent(IPlayer player, ELimb limb, EDeathCause deathCause, IEntity killer = null) : base(player, killer)
        {
            Limb = limb;
            DeathCause = deathCause;
        }
        public UnturnedPlayerDeathEvent(IPlayer player, ELimb limb, EDeathCause deathCause, IEntity killer = null, bool global = true) : base(player, killer, global)
        {
            Limb = limb;
            DeathCause = deathCause;
        }
        public UnturnedPlayerDeathEvent(IPlayer player, ELimb limb, EDeathCause deathCause, IEntity killer = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, killer, executionTarget, global)
        {
            Limb = limb;
            DeathCause = deathCause;
        }
        public UnturnedPlayerDeathEvent(IPlayer player, ELimb limb, EDeathCause deathCause, IEntity killer = null, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, killer, name, executionTarget, global)
        {
            Limb = limb;
            DeathCause = deathCause;
        }
    }
}