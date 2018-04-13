using Rocket.Unturned.Events;
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

        public void SendMessage(IPlayer player, string message, params object[] bindings)
        {
            var wrappedMessage = WrapMessage(message);
            foreach(var line in wrappedMessage)
                ChatManager.instance.channel.send("tellChat", new CSteamID(ulong.Parse(player.Id)), ESteamPacket.UPDATE_UNRELIABLE_BUFFER, CSteamID.Nil, (byte)EChatMode.GLOBAL, Color.white, line);
        }

        public void SendLocalizedMessage(ITranslations translations, IPlayer player, string translationKey, params object[] bindings)
        {
            var translatedMessage = translations.GetLocalizedMessage(translationKey, bindings);
            SendMessage(player, translatedMessage);
        }

        public void Broadcast(string message, params object[] bindings)
        {
            var wrappedMessage = WrapMessage(message);
            foreach (var line in wrappedMessage)
                foreach (IPlayer player in playerManager.Players)
                    player.SendMessage(line);

            logger.LogInformation("[Broadcast] " + message);
        }

        public void BroadcastLocalized(ITranslations translations, string translationKey, params object[] bindings)
        {
            var translatedMessage = translations.GetLocalizedMessage(translationKey, bindings);
            Broadcast(translatedMessage);
        }

        private void HandleChat(SteamPlayer player, EChatMode mode, ref Color color, ref bool isRich, string message,
                                ref bool isVisible)
        {
            IPlayer p = playerManager.GetPlayer(player.playerID.steamID.m_SteamID.ToString());
            UnturnedPlayerChatEvent @event = new UnturnedPlayerChatEvent(p, mode, color, isRich, message, !isVisible);
            eventManager.Emit(implementation, @event);
            color = @event.Color;
            isRich = @event.IsRich;
            isVisible = !@event.IsCancelled;
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