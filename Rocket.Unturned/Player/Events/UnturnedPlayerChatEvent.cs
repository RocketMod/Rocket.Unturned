using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Player.Events
{
    public class UnturnedPlayerChatEvent : PlayerChatEvent
    {
        public EChatMode Mode { get; }
        public Color Color { get; set; }
        public bool IsRichText { get; set; }

        public UnturnedPlayerChatEvent(IPlayer player, EChatMode mode, Color color, bool isRichText, string message,
                                       bool cancelled) : base(player, null, message, EventExecutionTargetContext.Sync)
        {
            Mode = mode;
            Color = color;
            IsRichText = isRichText;
            IsCancelled = cancelled;
        }
    }
}