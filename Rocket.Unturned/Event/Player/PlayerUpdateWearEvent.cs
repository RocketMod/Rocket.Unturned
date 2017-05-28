using Rocket.API.Event.Player;
using Rocket.API.Player;
using Rocket.Unturned.Events;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateWearEvent :  PlayerEvent
    {
        public Wearable Wearable { get; }
        public ushort WearableId { get; }

        public PlayerUpdateWearEvent(IRocketPlayer player, Wearable wearable, ushort id) : base(player)
        {
            Wearable = wearable;
            WearableId = id;
        }
    }
}