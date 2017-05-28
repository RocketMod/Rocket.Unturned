using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player.Inventory
{
    public class PlayerInventoryResizedEvent  :PlayerEvent
    {
        public byte Page { get; }
        public byte NewWidth { get; }
        public byte NewHeight { get; }

        public PlayerInventoryResizedEvent(IRocketPlayer player, byte page, byte newWidth, byte newHeight) : base(player)
        {
            Page = page;
            NewWidth = newWidth;
            NewHeight = newHeight;
        }
    }
}