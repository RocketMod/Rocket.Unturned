using Rocket.API.Entities;
using Rocket.API.Player;
using Rocket.UnityEngine.Extensions;

namespace Rocket.Unturned.Player {
    public class UnturnedPlayerEntity : IEntity
    {
        private readonly UnturnedPlayer unturnedPlayer;

        public UnturnedPlayerEntity(UnturnedPlayer unturnedPlayer)
        {
            this.unturnedPlayer = unturnedPlayer;
        }

        public System.Numerics.Vector3 Position => unturnedPlayer.Player?.transform?.position.ToSystemVector() ?? throw new PlayerNotOnlineException();

        public string EntityTypeName => "Player";
    }
}