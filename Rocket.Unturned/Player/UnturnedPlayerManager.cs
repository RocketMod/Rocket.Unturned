using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
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

        public bool Kick(IPlayer player, ICommandCaller kicker = null, string reason = null)
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

            Provider.ban(((UnturnedPlayer) player).CSteamID, reason, (uint) (duration?.TotalSeconds ?? uint.MaxValue));
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

        public IPlayer GetPendingPlayer(string uniqueID)
        {
            return PendingPlayers.FirstOrDefault(c => c.Id.Equals(uniqueID));
        }

        public IPlayer GetPendingPlayerByName(string displayName)
        {
            return PendingPlayers.FirstOrDefault(c => c.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase));
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

        /// <summary>
        /// Online players which succesfully joined the server.
        /// </summary>
        public IEnumerable<IPlayer> Players => Provider.clients.Select(c => (IPlayer) new UnturnedPlayer(container, c));
        
        /// <summary>
        /// Players which are not authenticated and have not joined yet.
        /// </summary>
        public IEnumerable<IPlayer> PendingPlayers => Provider.pending.Select(c => (IPlayer) new PreConnectUnturnedPlayer(c));
    }
}