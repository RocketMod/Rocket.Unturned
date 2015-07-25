using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rocket.Unturned.Commands
{
    internal class UnturnedAliasBase : Command
    {
        public static bool IsPlayer(Steamworks.CSteamID caller)
        {
            return (caller != null && !String.IsNullOrEmpty(caller.ToString()) && caller.ToString() != "0");
        }

        internal IRocketCommand Command;

        public UnturnedAliasBase(IRocketCommand command, string name)
        {
            Command = command;
            base.commandName = name;
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
                Command.Execute(UnturnedPlayer.FromCSteamID(caller), collection);
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occured while executing command /" + commandName + " " + command + ": " + ex.ToString());
            }
        }
    }
}