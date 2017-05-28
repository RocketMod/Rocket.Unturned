using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.Core;

namespace Rocket.Unturned.Commands
{
    internal class CommandTphere : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "tphere";

        public string Help => "Teleports another player to you";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.tphere", "rocket.teleporthere" };

        public void Execute(ICommandContext ctx)
        {
            UnturnedPlayer player = (UnturnedPlayer)ctx.Caller;
            var command = ctx.Parameters;

            if (command.Length != 1)
            {
                throw new WrongUsageOfCommandException(ctx);
            }

            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer!=null && otherPlayer != player)
            {
                otherPlayer.Teleport(player);
                R.Logger.Info(R.Translations.Translate("command_tphere_teleport_console", otherPlayer.DisplayName, player.DisplayName));
                ctx.Print(R.Translations.Translate("command_tphere_teleport_from_private", otherPlayer.DisplayName));
                R.Implementation.Chat.Say(otherPlayer, R.Translations.Translate("command_tphere_teleport_to_private", player.DisplayName));
                return;
            }

            ctx.Print(R.Translations.Translate("command_generic_failed_find_player"));
        }
    }
}