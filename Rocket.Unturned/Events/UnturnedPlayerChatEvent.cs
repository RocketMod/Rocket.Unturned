using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Events.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Events
{
    public class UnturnedPlayerChatEvent : PlayerChatEvent
    {
        public EChatMode Mode { get; }
        public Color Color { get; set; }
        public bool IsRich { get; set; }

        public UnturnedPlayerChatEvent(IPlayer player, EChatMode mode, Color color, bool isRich, string message,
                                       bool cancelled) : base(player, null, message, EventExecutionTargetContext.Sync)
        {
            Mode = mode;
            Color = color;
            IsRich = isRich;
            IsCancelled = cancelled;
        }
    }
}