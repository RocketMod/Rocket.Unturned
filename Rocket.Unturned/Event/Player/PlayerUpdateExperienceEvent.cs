using Rocket.API.Event.Player;
using Rocket.API.Player;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerUpdateExperienceEvent : PlayerEvent
    {
        public uint NewExperience { get; }

        public PlayerUpdateExperienceEvent(IRocketPlayer player, uint newExperience) : base(player)
        {
            NewExperience = newExperience;
        }
    }
}