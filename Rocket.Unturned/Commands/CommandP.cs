using Rocket.API;
using Rocket.API.Extensions;
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
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
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
            get { return "<player> [group] | reload"; }
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
            if(command.Length == 1 && command[0].ToLower() == "reload" && caller.HasPermission("p.reload"))
            {
                R.Permissions.Reload();
                UnturnedChat.Say(caller, U.Translate("command_p_permissions_reload"));
                return;
            }
            

            IRocketPlayer player = command.GetUnturnedPlayerParameter(0);
            if (player == null) player = command.GetRocketPlayerParameter(0);
            string groupName = command.GetStringParameter(1);

            if (command.Length == 0 && !(caller is ConsolePlayer))
            {
                UnturnedChat.Say(caller, U.Translate("command_p_groups_private", "Your", string.Join(", ", R.Permissions.GetGroups(caller, true).Select(g => g.DisplayName).ToArray())));
                UnturnedChat.Say(caller, U.Translate("command_p_permissions_private", "Your", string.Join(", ", Core.R.Permissions.GetPermissions(caller).Select(p => p.Name + (p.Cooldown != null ? "(" + p.Cooldown + ")" : "")).ToArray())));
            }
            else if(command.Length == 1 && player != null) {
                UnturnedChat.Say(caller, U.Translate("command_p_groups_private", player.DisplayName+"s", string.Join(", ", R.Permissions.GetGroups(player, true).Select(g => g.DisplayName).ToArray())));
                UnturnedChat.Say(caller, U.Translate("command_p_permissions_private", player.DisplayName + "s", string.Join(", ", Core.R.Permissions.GetPermissions(player).Select(p => p.Name +(p.Cooldown != null? "(" + p.Cooldown + ")" : "")).ToArray())));
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
