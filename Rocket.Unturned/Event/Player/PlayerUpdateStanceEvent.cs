using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateStanceEvent :PlayerEvent
    {
        public byte NewStance { get; }

        public PlayerUpdateStanceEvent(IRocketPlayer player, byte newStance) : base(player)
        {
            NewStance = newStance;
        }
    }
}