using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Unturned.Commands
{
    public class CommandP : IRocketCommand
    {
        public bool AllowFromConsole
        {
            get { return true; }
        }

        public string Name
        {
            get { return "p"; }
        }

        public string Help
        {
            get { return "Sets a Rocket permission group of a specific player"; }
        }

        public string Syntax
        {
            get { return "<player> [group]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "permissions" }; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.p", "rocket.permissions" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            IRocketPlayer player = command.GetUnturnedPlayerParameter(0);
            if (player == null) player = command.GetRocketPlayerParameter(0);
            string groupName = command.GetStringParameter(1);

            if (command.Length == 0 && !(caller is ConsolePlayer))
            {
                UnturnedChat.Say(caller, U.Translate("command_p_groups_private", "Your", string.Join(", ", R.Permissions.GetGroups(caller, true).Select(g => g.DisplayName).ToArray())));
                UnturnedChat.Say(caller, U.Translate("command_p_permissions_private", "Your", string.Join(", ", Core.R.Permissions.GetPermissions(caller).ToArray())));
            }
            else if(command.Length == 1 && player != null) {
                UnturnedChat.Say(caller, U.Translate("command_p_groups_private", player.DisplayName+"s", string.Join(", ", R.Permissions.GetGroups(caller, true).Select(g => g.DisplayName).ToArray())));
                UnturnedChat.Say(caller, U.Translate("command_p_permissions_private", player.DisplayName + "s", string.Join(", ", Core.R.Permissions.GetPermissions(player).ToArray())));
            }
            else if (command.Length == 2 && player != null && !String.IsNullOrEmpty(groupName) && caller.HasPermission("p.set"))
            {
                if (Core.R.Permissions.SetGroup(player, groupName))
                {
                    UnturnedChat.Say(caller, U.Translate("command_p_group_assigned", player.DisplayName, groupName));
                }
                else {
                    UnturnedChat.Say(caller, U.Translate("command_p_group_not_found"));
                }
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                return;
            }

            
         }
    }
}