using Rocket.API.Event.Player;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdatePositionEvent :PlayerEvent
    {
        public Vector3 OldPosition { get; }
        public Vector3 NewPosition { get; }

        public PlayerUpdatePositionEvent(IRocketPlayer player, Vector3 oldPosition, Vector3 newPosition) : base(player)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}