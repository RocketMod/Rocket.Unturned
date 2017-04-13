using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.API.Player;
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

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length != 1)
            {
                U.Instance.Chat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer!=null && otherPlayer != caller)
            {
                otherPlayer.Teleport(player);
                R.Logger.Info(U.Translate("command_tphere_teleport_console", otherPlayer.DisplayName, player.DisplayName));
                U.Instance.Chat.Say(caller, U.Translate("command_tphere_teleport_from_private", otherPlayer.DisplayName));
                U.Instance.Chat.Say(otherPlayer, U.Translate("command_tphere_teleport_to_private", player.DisplayName));
            }
            else
            {
                U.Instance.Chat.Say(caller, U.Translate("command_generic_failed_find_player"));
                throw new WrongUsageOfCommandException(caller, this);
            }
        }
    }
}