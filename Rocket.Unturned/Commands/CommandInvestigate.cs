using SDG.Unturned;
using System.Collections.Generic;
using Rocket.API.Exceptions;
using Rocket.API.Commands;
using Rocket.Core;

namespace Rocket.Unturned.Commands
{
    public class CommandInvestigate : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "investigate";

        public string Help => "Shows you the SteamID64 of a player";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.investigate" };

        public void Execute(ICommandContext ctx)
        {
            var command = ctx.Parameters;
            var caller = ctx.Caller;

            if (command.Length!=1)
            {
                throw new WrongUsageOfCommandException(ctx);
            }

            SteamPlayer otherPlayer = PlayerTool.getSteamPlayer(command[0]);
            if (otherPlayer == null || (caller != null && otherPlayer.playerID.steamID.ToString() == caller.ToString()))
                throw new WrongUsageOfCommandException(ctx);

            ctx.Print(R.Translations.Translate("command_investigate_private", otherPlayer.playerID.characterName,
                otherPlayer.playerID.steamID.ToString()));
        }
    }
}