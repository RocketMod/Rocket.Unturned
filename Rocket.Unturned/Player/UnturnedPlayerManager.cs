using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Player.Events;
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

        public bool Kick(IOnlinePlayer player, ICommandCaller kicker = null, string reason = null)
        {
            PlayerKickEvent @event = new PlayerKickEvent(player, kicker, reason, true);
            eventManager.Emit(implementation, @event);
            if (@event.IsCancelled)
                return false;

            Provider.kick(((UnturnedPlayer)player).CSteamID, reason);
            return true;
        }

        public bool Ban(IPlayer player, ICommandCaller banner = null, string reason = null, TimeSpan? duration = null)
        {
            PlayerBanEvent @event = new PlayerBanEvent(player, banner, reason, duration, true);
            eventManager.Emit(implementation, @event);
            if (@event.IsCancelled)
                return false;

            Provider.ban(((UnturnedPlayer)player).CSteamID, reason, (uint)(duration?.TotalSeconds ?? uint.MaxValue));
            return true;
        }

        public IPlayer GetPlayer(string id)
        {
            return new UnturnedPlayer(container, new CSteamID(ulong.Parse(id)));
        }

        public IOnlinePlayer GetOnlinePlayer(string nameOrId)
        {
            SteamPlayer player;

            if (ulong.TryParse(nameOrId, out var id))
                player = PlayerTool.getSteamPlayer(new CSteamID(id));
            else
                player = PlayerTool.getSteamPlayer(id);

            if (player == null)
                return null;

            return new UnturnedPlayer(container, player);
        }

        public IOnlinePlayer GetOnlinePlayerByName(string displayName)
        {
            SteamPlayer player = PlayerTool.getSteamPlayer(displayName);
            if (player == null)
                return null;

            return new UnturnedPlayer(container, player);
        }

        public IOnlinePlayer GetOnlinePlayerById(string id)
        {
            var player = PlayerTool.getSteamPlayer(new CSteamID(ulong.Parse(id)));
            if (player == null)
                return null;

            return new UnturnedPlayer(container, player);
        }


        public IPlayer GetPendingPlayer(string uniqueID)
        {
            return PendingPlayers.FirstOrDefault(c => c.Id.Equals(uniqueID));
        }

        public IPlayer GetPendingPlayerByName(string displayName)
        {
            return PendingPlayers.FirstOrDefault(c => c.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryGetOnlinePlayer(string nameOrId, out IOnlinePlayer output)
        {
            output = GetOnlinePlayer(nameOrId);
            if (output == null)
                return false;
            return true;
        }

        public bool TryGetOnlinePlayerById(string id, out IOnlinePlayer output)
        {
            output = GetOnlinePlayerById(id);
            if (output == null)
                return false;
            return true;
        }

        public bool TryGetOnlinePlayerByName(string displayName, out IOnlinePlayer output)
        {
            output = GetOnlinePlayerByName(displayName);
            if (output == null)
                return false;
            return true;
        }

        /// <summary>
        /// Online players which succesfully joined the server.
        /// </summary>
        public IEnumerable<IOnlinePlayer> OnlinePlayers => 
            Provider.clients.Select(c => (IOnlinePlayer)new UnturnedPlayer(container, c));

        /// <summary>
        /// Players which are not authenticated and have not joined yet.
        /// </summary>
        public IEnumerable<IPlayer> PendingPlayers => Provider.pending.Select(c => (IPlayer)new PreConnectUnturnedPlayer(container, c));
    }
}