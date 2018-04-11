using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Ioc;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Events.Player;
using SDG.Unturned;
using Steamworks;

namespace Rocket.Unturned.Player
{
    public class UnturnedPlayerManager : IPlayerManager
    {
        private readonly IImplementation implementation;
        private readonly IEventManager eventManager;
        private readonly IDependencyContainer container;

        public UnturnedPlayerManager(IImplementation implementation, IEventManager @eventManager,
                                     IDependencyContainer container)
        {
            this.implementation = implementation;
            this.eventManager = eventManager;
            this.container = container;
        }

        public bool Kick(IPlayer player, string reason)
        {
            Provider.kick(((UnturnedPlayer) player).CSteamID, reason);
            PlayerKickEvent @event = new PlayerKickEvent(player, reason, true);
            eventManager.Emit((IEventEmitter) implementation, @event);
            return true;
        }

        public bool Ban(IPlayer player, string reason, TimeSpan? timeSpan = null)
        {
            Provider.ban(((UnturnedPlayer) player).CSteamID, reason, (uint) (timeSpan?.TotalSeconds ?? uint.MaxValue));
            //todo ban event
            return true;
        }

        public IPlayer GetPlayer(string uniqueID)
        {
            ulong steamId = ulong.Parse(uniqueID);
            SteamPlayer player = PlayerTool.getSteamPlayer(new CSteamID(steamId));
            if (player == null)
                return null;

            return new UnturnedPlayer(container, player);
        }

        public IPlayer GetPlayerByName(string displayName)
        {
            SteamPlayer player = PlayerTool.getSteamPlayer(displayName);
            if (player == null)
                return null;

            return new UnturnedPlayer(container, player);
        }

        public bool TryGetPlayer(string uniqueID, out IPlayer output)
        {
            output = GetPlayer(uniqueID);
            if (output == null)
                return false;
            return true;
        }

        public bool TryGetPlayerByName(string displayName, out IPlayer output)
        {
            output = GetPlayerByName(displayName);
            if (output == null)
                return false;
            return true;
        }

        public IEnumerable<IPlayer> Players => Provider.clients.Select(c => (IPlayer) new UnturnedPlayer(container, c));
    }
}