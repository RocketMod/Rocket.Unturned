using SDG.Unturned;

namespace Rocket.Unturned.Event.Player
{
    public class PlayerJoinRequestEvent : API.Event.Event
    {
        public SteamPending PendingPlayer { get; }
        public ESteamRejection? Result { get; }
        
        public PlayerJoinRequestEvent(SteamPending pendingPlayer)
        {
            PendingPlayer = pendingPlayer;
        }
    }
}