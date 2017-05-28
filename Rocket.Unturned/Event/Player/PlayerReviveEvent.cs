using Rocket.API.Event.Player;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerReviveEvent : PlayerEvent
    {
        public Vector3 Position { get; }
        public byte Angle { get; }

        public PlayerReviveEvent(IRocketPlayer player, Vector3 position, byte angle) : base(player)
        {
            Position = position;
            Angle = angle;
        }
    }
}