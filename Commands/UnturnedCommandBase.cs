using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public static class UnturnedCommandExtensions
    {
        public static string GetStringParameter(this string[] array, int index)
        {
            return (array.Length <= index || String.IsNullOrEmpty(array[index])) ? null : array[index];
        }

        public static int? GetInt32Parameter(this string[] array, int index)
        {
            int output;
            return (array.Length <= index || !Int32.TryParse(array[index].ToString(), out output)) ? null : (int?)output;
        }

        public static uint? GetUInt32Parameter(this string[] array, int index)
        {
            uint output;
            return (array.Length <= index || !uint.TryParse(array[index].ToString(), out output)) ? null : (uint?)output;
        }

        public static byte? GetByteParameter(this string[] array, int index)
        {
            byte output;
            return (array.Length <= index || !Byte.TryParse(array[index].ToString(), out output)) ? null : (byte?)output;
        }

        public static ushort? GetUInt16Parameter(this string[] array, int index)
        {
            ushort output;
            return (array.Length <= index || !UInt16.TryParse(array[index].ToString(), out output)) ? null : (ushort?)output;
        }

        public static float? GetFloatParameter(this string[] array, int index)
        {
            float output;
            return (array.Length <= index || !float.TryParse(array[index].ToString(), out output)) ? null : (float?)output;
        }

        public static UnturnedPlayer GetUnturnedPlayerParameter(this string[] array, int index)
        {
            return (array.Length <= index) ? null : UnturnedPlayer.FromName(array[index]);
        }

        public static Color? GetColorParameter(this string[] array, int index)
        {
            if(array.Length <= index) return null;
            Color output = UnturnedChat.GetColorFromName(array[index], Color.clear);
            return (output == Color.clear) ? null : (Color?)output;
        }
    }

    internal class UnturnedCommandBase : Command
    {
        public static bool IsPlayer(Steamworks.CSteamID caller)
        {
            return (caller != null && !String.IsNullOrEmpty(caller.ToString()) && caller.ToString() != "0");
        }

        internal IRocketCommand Command;

        public UnturnedCommandBase(IRocketCommand command)
        {
            Command = command;
            base.commandName = Command.Name;
            base.commandHelp = Command.Help;
            base.commandInfo = Command.Syntax;
        }

        protected override void execute(Steamworks.CSteamID caller, string command)
        {
            if (!Command.AllowFromConsole && !IsPlayer(caller))
            {
                Logger.Log("This command can't be called from console");
                return;
            }

            string[] collection = Regex.Matches(command, @"[\""](.+?)[\""]|([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Cast<Match>().Select(m => m.Value.Trim('"').Trim()).ToArray();

            try
            {
                IRocketPlayer p = UnturnedPlayer.FromCSteamID(caller);
                if (p == null)
                {
                    p = new ConsolePlayer();
                }
                Command.Execute(p, collection);
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occured while executing command /" + commandName + " " + command + ": " + ex.ToString());
            }
        }
    }
}