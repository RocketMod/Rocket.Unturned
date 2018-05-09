using Rocket.API.Entities;

namespace Rocket.Unturned.Player {
    public class UnturnedPlayerEntity : IEntity
    {
        private readonly UnturnedPlayer unturnedPlayer;

        public UnturnedPlayerEntity(UnturnedPlayer unturnedPlayer)
        {
            this.unturnedPlayer = unturnedPlayer;
        }

        public string EntityTypeName => "Player";
    }
}