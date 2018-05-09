using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player.Events;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerChatEvent : UserChatEvent
    {
        public UnturnedPlayer UnturnedPlayer { get; }
        public EChatMode Mode { get; }
        public Color Color { get; set; }
        public bool IsRichText { get; set; }

        public UnturnedPlayerChatEvent(UnturnedPlayer unturnedPlayer, EChatMode mode, Color color, bool isRichText, string message,
                                       bool cancelled) : base(unturnedPlayer.User, null, message, EventExecutionTargetContext.Sync)
        {
            UnturnedPlayer = unturnedPlayer;
            Mode = mode;
            Color = color;
            IsRichText = isRichText;
            IsCancelled = cancelled;
        }
    }
}