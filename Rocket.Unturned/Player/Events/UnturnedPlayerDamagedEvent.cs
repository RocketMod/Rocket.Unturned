using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerDamagedEvent : OnlinePlayerEvent, ICancellableEvent //Todo: extend PlayerDamageEvent
    {
        public EDeathCause DeathCause { get; set; }
        public ELimb Limb { get; set; }
        public ICommandCaller DamageDealer { get; }
        public Vector3 Direction { get; set; }
        public float Damage { get; set; }
        public float Times { get; set; }

        public UnturnedPlayerDamagedEvent(IOnlinePlayer player, EDeathCause deathCause, ELimb limb, ICommandCaller damageDealer, Vector3 direction, float damage, float times) : base(player)
        {
            DeathCause = deathCause;
            Limb = limb;
            DamageDealer = damageDealer;
            Direction = direction;
            Damage = damage;
            Times = times;
        }
        public UnturnedPlayerDamagedEvent(IOnlinePlayer player, EDeathCause deathCause, ELimb limb, ICommandCaller damageDealer, Vector3 direction, float damage, float times, bool global = true) : base(player, global)
        {
            DeathCause = deathCause;
            Limb = limb;
            DamageDealer = damageDealer;
            Direction = direction;
            Damage = damage;
            Times = times;
        }
        public UnturnedPlayerDamagedEvent(IOnlinePlayer player, EDeathCause deathCause, ELimb limb, ICommandCaller damageDealer, Vector3 direction, float damage, float times, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, executionTarget, global)
        {
            DeathCause = deathCause;
            Limb = limb;
            DamageDealer = damageDealer;
            Direction = direction;
            Damage = damage;
            Times = times;
        }
        public UnturnedPlayerDamagedEvent(IOnlinePlayer player, EDeathCause deathCause, ELimb limb, ICommandCaller damageDealer, Vector3 direction, float damage, float times, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player, name, executionTarget, global)
        {
            DeathCause = deathCause;
            Limb = limb;
            DamageDealer = damageDealer;
            Direction = direction;
            Damage = damage;
            Times = times;
        }


        public bool IsCancelled { get; set; }
    }
}