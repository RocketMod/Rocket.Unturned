using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rocket.Unturned.Commands
{
    internal class UnturnedAliasBase : UnturnedCommandBase {
        public UnturnedAliasBase(IRocketCommand command, string name) : base (command)
        {
            Command = command;
            base.commandName = name;
            base.commandHelp = Command.Help;
            base.commandInfo = Command.Syntax;
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

        protected override void execute(Steamworks.CSteamID caller, string commandString)
        {
            executeCommand(Command,caller, commandString);
        }

        internal static void executeCommand(IRocketCommand command,Steamworks.CSteamID caller, string commandString)
        {
            if (!command.AllowFromConsole && !IsPlayer(caller))
            {
                Logger.Log("This command can't be called from console");
                return;
            }

            string[] collection = Regex.Matches(commandString, @"[\""](.+?)[\""]|([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Cast<Match>().Select(m => m.Value.Trim('"').Trim()).ToArray();

            try
            {
                IRocketPlayer p = UnturnedPlayer.FromCSteamID(caller);
                if (p == null)
                {
                    p = new ConsolePlayer();
                }
                command.Execute(p, collection);
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occured while executing command /" + command.Name + " " + commandString + ": " + ex.ToString());
            }
        }
    }
}