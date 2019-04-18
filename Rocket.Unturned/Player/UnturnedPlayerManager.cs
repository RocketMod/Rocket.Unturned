using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player.Events;
using Rocket.Core.User.Events;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Core.Logging;
using Rocket.Core.Player;
using Color = UnityEngine.Color;
using ILogger = Rocket.API.Logging.ILogger;

namespace Rocket.Unturned.Player
{
    public class UnturnedPlayerManager : IPlayerManager
    {
        private readonly IHost host;
        private readonly IEventBus eventManager;
        private readonly IDependencyContainer container;
        private readonly ILogger logger;

        public UnturnedPlayerManager(IHost host, IEventBus @eventManager,
                                     IDependencyContainer container, ILogger logger)
        {
            this.host = host;
            this.eventManager = eventManager;
            this.container = container;
            this.logger = logger;
        }

        public async Task<bool> KickAsync(IUser user, IUser kickedBy = null, string reason = null)
        {
            var target = ((UnturnedUser)user).Player;

            PlayerKickEvent @event = new PlayerKickEvent(target, kickedBy, reason, true);
            eventManager.Emit(host, @event);
            if (@event.IsCancelled)
                return false;

            if (target.IsOnline)
                Provider.kick(target.CSteamID, reason ?? string.Empty);
            return true;
        }

        public async Task<bool> BanAsync(IUser user, IUser bannedBy = null, string reason = null, TimeSpan? duration = null)
        {
            UserBanEvent @event = new UserBanEvent(user, bannedBy, reason, duration, true);
            eventManager.Emit(host, @event);
            if (@event.IsCancelled)
                return false;

            var callerId = (bannedBy is UnturnedUser up) ? up.CSteamID : CSteamID.Nil;

            //if (user.IsOnline)
            //{
            //    SteamBlacklist.ban(player.CSteamID, 0, callerId, reason, (uint)(duration?.TotalSeconds ?? uint.MaxValue));
            //    return true;
            //}

            var steamId = new CSteamID(ulong.Parse(user.Id));
            SteamBlacklist.ban(steamId, 0, callerId, reason ?? string.Empty, (uint)(duration?.TotalSeconds ?? uint.MaxValue));

            var target = ((UnturnedUser)user).Player;

            if (target.IsOnline)
                Provider.kick(steamId, reason ?? string.Empty);

            return true;
        }

        public async Task<bool> UnbanAsync(IUser target, IUser bannedBy = null)
        {
            UserUnbanEvent @event = new UserUnbanEvent(target, bannedBy);
            eventManager.Emit(host, @event);
            if (@event.IsCancelled)
                return false;

            var steamId = new CSteamID(ulong.Parse(target.Id));
            return SteamBlacklist.unban(steamId);
        }

        public async Task SendMessageAsync(IUser sender, IUser receiver, string message, System.Drawing.Color? color = null,
                                     params object[] arguments)
        {
            var uColor = color == null
                ? Color.white
                : new Color((1f / 255f) * color.Value.R, (1f / 255f) * color.Value.G, (1f / 255f) * color.Value.B, (1f / 100f) * color.Value.A);

            if (receiver is IConsole console)
            {
                console.WriteLine(message, color, arguments);
                return;
            }

            if (!(receiver is UnturnedUser unturnedUser))
                throw new Exception("Could not cast " + receiver.GetType().FullName + " to UnturnedUser!");

            ChatManager.say(unturnedUser.CSteamID, message, uColor, true);
        }

        public async Task BroadcastAsync(IUser sender, IEnumerable<IUser> receivers, string message, System.Drawing.Color? color = null,
                                   params object[] arguments)
        {
            var wrappedMessage = WrapMessage(string.Format(message, arguments));
            foreach (IUser user in receivers)
                foreach (var line in wrappedMessage)
                    await user.UserManager.SendMessageAsync(sender, user, line, color);
        }

        public async Task BroadcastAsync(IUser sender, string message, System.Drawing.Color? color = null,
                                   params object[] arguments)
        {
            await BroadcastAsync(sender, Players.Select(d => d.User), message, color, arguments);
            logger.LogInformation("[Broadcast] " + message);
        }

        public async Task<IUser> GetUserAsync(string idOrName)
        {
            var player = (UnturnedPlayer) await GetPlayerAsync(idOrName);

            if(player == null)
            {
                throw new PlayerNotFoundException(idOrName);
            }

            return new UnturnedUser(player.Container, player.SteamPlayer);
        }

        public System.Drawing.Color? GetColorFromName(string colorName)
        {
            switch (colorName.Trim().ToLower())
            {
                case "black": return System.Drawing.Color.Black;
                case "blue": return System.Drawing.Color.Blue;
                case "cyan": return System.Drawing.Color.Cyan;
                case "gray": return System.Drawing.Color.Gray;
                case "green": return System.Drawing.Color.Green;
                case "grey": return System.Drawing.Color.Gray;
                case "magenta": return System.Drawing.Color.Magenta;
                case "red": return System.Drawing.Color.Red;
                case "white": return System.Drawing.Color.White;
                case "yellow": return System.Drawing.Color.Yellow;
                case "rocket": return GetColorFromRGB(90, 206, 205);
            }

            return GetColorFromHex(colorName);
        }

        public System.Drawing.Color? GetColorFromHex(string hexString)
        {
            hexString = hexString.Replace("#", "");
            if (hexString.Length == 3)
            {                                                                           // #99f
                hexString = hexString.Insert(1, System.Convert.ToString(hexString[0])); // #999f
                hexString = hexString.Insert(3, System.Convert.ToString(hexString[2])); // #9999f
                hexString = hexString.Insert(5, System.Convert.ToString(hexString[4])); // #9999ff
            }

            if (hexString.Length != 6 || !int.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out int argb))
            {
                return null;
            }
            byte r = (byte)((argb >> 16) & 0xff);
            byte g = (byte)((argb >> 8) & 0xff);
            byte b = (byte)(argb & 0xff);
            return GetColorFromRGB(r, g, b);
        }

        public System.Drawing.Color GetColorFromRGB(byte R, byte G, byte B)
        {
            return GetColorFromRGB(R, G, B, 100);
        }

        public System.Drawing.Color GetColorFromRGB(byte R, byte G, byte B, short A)
        {
            return System.Drawing.Color.FromArgb(A, R, G, B);
        }

        public IPlayer GetPlayer(string nameOrId)
        {
            SteamPlayer player;

            if (ulong.TryParse(nameOrId, out var id))
                player = PlayerTool.getSteamPlayer(new CSteamID(id));
            else
                player = PlayerTool.getSteamPlayer(nameOrId);

            if (player == null)
                throw new PlayerNotFoundException(nameOrId);

            return new UnturnedPlayer(container, player, this);
        }



        public PreConnectUnturnedPlayer GetPendingPlayer(string uniqueID)
        {
            return PendingPlayers.FirstOrDefault(c => c.User.Id.Equals(uniqueID));
        }

        public PreConnectUnturnedPlayer GetPendingPlayerByName(string displayName)
        {
            return PendingPlayers.FirstOrDefault(c => c.User.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<IPlayer>> GetPlayersAsync() => Players;

        public async Task<IPlayer> GetPlayerAsync(string nameOrId)
        {
            SteamPlayer player = null;

            if (ulong.TryParse(nameOrId, out var id))
                player = PlayerTool.getSteamPlayer(new CSteamID(id));
            
            if(player == null)
                player = PlayerTool.getSteamPlayer(nameOrId);

            if (player == null)
                throw new PlayerNotFoundException(nameOrId);

            return new UnturnedPlayer(container, player, this);
        }

        public async Task<IPlayer> GetPlayerByNameAsync(string name)
        {
            SteamPlayer player = PlayerTool.getSteamPlayer(name);
            if (player == null)
                throw new PlayerNameNotFoundException(name);

            return new UnturnedPlayer(container, player, this);
        }

        public async Task<IPlayer> GetPlayerByIdAsync(string id)
        {
            var player = PlayerTool.getSteamPlayer(ulong.Parse(id));
            if (player == null)
                throw new PlayerIdNotFoundException(id);

            return new UnturnedPlayer(container, player, this);
        }

        public bool TryGetOnlinePlayer(string nameOrId, out IPlayer output)
        {
            output = null;
            try
            {
                output = GetPlayerAsync(nameOrId).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryGetOnlinePlayerById(string id, out IPlayer output)
        {
            output = null;
            try
            {
                output = GetPlayerByIdAsync(id).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryGetOnlinePlayerByName(string displayName, out IPlayer output)
        {
            output = null;
            try
            {
                output = GetPlayerByNameAsync(displayName).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Online players which succesfully joined the server.
        /// </summary>
        public IEnumerable<IPlayer> Players =>
            Provider.clients.Select(c => (IPlayer)new UnturnedPlayer(container, c, this));

        /// <summary>
        /// Players which are not authenticated and have not joined yet.
        /// </summary>
        public IEnumerable<PreConnectUnturnedPlayer> PendingPlayers => Provider.pending.Select(c => new PreConnectUnturnedPlayer(container, c, this));
        public string ServiceName => "Unturned";

        public async Task<IIdentity> GetIdentity(string id) => new UnturnedIdentity(ulong.Parse(id));

        protected static List<string> WrapMessage(string text)
        {
            if (text.Length == 0) return new List<string>();
            string[] words = text.Split(' ');
            List<string> lines = new List<string>();
            string currentLine = "";
            int maxLength = 90;
            foreach (string currentWord in words)
            {
                if (currentLine.Length > maxLength || currentLine.Length + currentWord.Length > maxLength)
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                if (currentLine.Length > 0)
                    currentLine += " " + currentWord;
                else
                    currentLine += currentWord;
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine);
            return lines;
        }
    }
}
