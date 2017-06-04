using System;
using System.Collections.Generic;
using Rocket.API.Event;
using Rocket.API.Event.Player;
using Rocket.API.Player;
using Rocket.API.Providers.Implementation.Managers;
using Rocket.API.Providers.Logging;
using Rocket.Core;
using Rocket.Core.Player;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Rocket.Unturned.Manager
{
    public class UnturnedChatManager: IChatManager
    {
        internal void handleChat(SteamPlayer steamPlayer, EChatMode chatMode, ref Color incomingColor, string message, ref bool cancel)
        {
            cancel = false;
            Color color = incomingColor;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                PlayerChatEvent @event = new PlayerChatEvent(player, incomingColor, message, (PlayerChatMode)chatMode);
                @event.IsCancelled = cancel;
                EventManager.Instance.CallEvent(@event);
                cancel = @event.IsCancelled;

            }
            catch (Exception ex)
            {
                R.Logger.Log(LogLevel.ERROR, null, ex);
            }
            cancel = !cancel;
            incomingColor = color;
        }

        public void Say(IRocketPlayer player, string message, Color? color = null)
        {
            player.Message(message, color);
        }

        public void Say(string message, Color? color = null)
        {
            R.Logger.Log(LogLevel.INFO, "Broadcast: " + message);
            foreach (string m in WrapMessage(message))
            {
                ChatManager.instance.channel.send("tellChat", ESteamCall.OTHERS, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, CSteamID.Nil, (byte)EChatMode.GLOBAL, color, m);
            }
        }

        public static List<string> WrapMessage(string text)
        {
            if (text.Length == 0) return new List<string>();
            string[] words = text.Split(' ');
            List<string> lines = new List<string>();
            string currentLine = "";
            int maxLength = 90;
            foreach (var currentWord in words)
            {

                if ((currentLine.Length > maxLength) ||
                    ((currentLine.Length + currentWord.Length) > maxLength))
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