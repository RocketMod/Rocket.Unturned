using System;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.I18N;
using Rocket.API.Player;
using Steamworks;
using UnityEngine;
using ILogger = Rocket.API.Logging.ILogger;
using Rocket.Unturned.Player.Events;

namespace Rocket.Unturned.Chat
{
    public sealed class UnturnedChatManager : IChatManager
    {
        private readonly ILogger logger;
        private readonly IImplementation implementation;
        private readonly IEventManager eventManager;
        private readonly IPlayerManager playerManager;

        public UnturnedChatManager(ILogger logger, IImplementation implementation, IEventManager eventManager,
                                   IPlayerManager playerManager, IDependencyContainer container)
        {
            this.logger = logger;
            this.implementation = implementation;
            this.eventManager = eventManager;
            this.playerManager = playerManager;

            ChatManager.onChatted += HandleChat;
             
            CommandWindow.onCommandWindowOutputted += (text, color) 
                => logger.LogInformation(text?.ToString());
        }

        public void SendMessage(IOnlinePlayer player, string message, params object[] bindings)
        {
            var wrappedMessage = WrapMessage(message);
            foreach(var line in wrappedMessage)
                ChatManager.instance.channel.send("tellChat", new CSteamID(ulong.Parse(player.Id)), ESteamPacket.UPDATE_UNRELIABLE_BUFFER, CSteamID.Nil, (byte)EChatMode.GLOBAL, Color.white, line);
        }

        public void SendLocalizedMessage(ITranslationLocator translations, IOnlinePlayer player, string translationKey, params object[] bindings)
        {
            var translatedMessage = translations.GetLocalizedMessage(translationKey, bindings);
            SendMessage(player, translatedMessage);
        }

        public void Broadcast(string message, params object[] bindings)
        {
            var wrappedMessage = WrapMessage(message);
            foreach (var line in wrappedMessage)
                foreach (IOnlinePlayer player in playerManager.OnlinePlayers)
                    player.SendMessage(line);

            logger.LogInformation("[Broadcast] " + message);
        }

        public void BroadcastLocalized(ITranslationLocator translations, string translationKey, params object[] bindings)
        {
            var translatedMessage = translations.GetLocalizedMessage(translationKey, bindings);
            Broadcast(translatedMessage);
        }

        private void HandleChat(SteamPlayer player, EChatMode mode, ref Color color, ref bool isRich, string message,
                                ref bool isVisible)
        {
            IOnlinePlayer p = playerManager.GetOnlinePlayerById(player.playerID.steamID.m_SteamID.ToString());
            UnturnedPlayerChatEvent @event = new UnturnedPlayerChatEvent(p, mode, color, isRich, message, !isVisible);
            eventManager.Emit(implementation, @event);
            color = @event.Color;
            isRich = @event.IsRichText;
            isVisible = !@event.IsCancelled;
        }

        public Color? GetColorFromName(string colorName)
        {
            switch (colorName.Trim().ToLower())
            {
                case "black":   return Color.black;
                case "blue":    return Color.blue;
                case "clear":   return Color.clear;
                case "cyan":    return Color.cyan;
                case "gray":    return Color.gray;
                case "green":   return Color.green;
                case "grey":    return Color.grey;
                case "magenta": return Color.magenta;
                case "red":     return Color.red;
                case "white":   return Color.white;
                case "yellow":  return Color.yellow;
                case "rocket":  return GetColorFromRGB(90, 206, 205);
            }

            return GetColorFromHex(colorName);
        }

        public Color? GetColorFromHex(string hexString)
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

        public Color GetColorFromRGB(byte R, byte G, byte B)
        {
            return GetColorFromRGB(R, G, B, 100);
        }

        public Color GetColorFromRGB(byte R, byte G, byte B, short A)
        {
            return new Color((1f / 255f) * R, (1f / 255f) * G, (1f / 255f) * B, (1f / 100f) * A);
        }

        public static List<string> WrapMessage(string text)
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