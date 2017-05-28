using Rocket.API.Event.Player;
using Rocket.API.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Event.Player.Inventory
{
    public class PlayerInventoryRemovedEvent : PlayerEvent
    {
        public byte Page { get; }
        public byte Index { get; }
        public ItemJar Item { get; }

        public PlayerInventoryRemovedEvent(IRocketPlayer player, byte page, byte index, ItemJar item) : base(player)
        {
            Page = page;
            Index = index;
            Item = item;
        }
    }
}