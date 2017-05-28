using Rocket.API.Event.Player;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerDeadEvent:PlayerEvent
    {
        public Vector3 Ragdoll { get; }

        public PlayerDeadEvent(IRocketPlayer player, Vector3 ragdoll) : base(player)
        {
            Ragdoll = ragdoll;
        }
    }
}