using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateVirusEvent:PlayerEvent
    {
        public byte NewVirus { get; }

        public PlayerUpdateVirusEvent(IRocketPlayer player, byte newVirus) : base(player)
        {
            NewVirus = newVirus;
        }
    }
}