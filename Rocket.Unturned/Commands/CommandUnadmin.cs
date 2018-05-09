using System;
using System.ComponentModel.Design;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.User;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandUnadmin : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return true; //anyone can use the command
        }

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            IPlayer targetUser = context.Parameters.Get<IPlayer>(0);

            if (targetUser is UnturnedPlayer uPlayer && uPlayer.IsAdmin)
            {
                uPlayer.Admin(false);
                return;
            }

            context.User.SendMessage($"Could not unadmin {targetUser.Name}", ConsoleColor.Red);
        }

        public string Name => "Unadmin";
        public string Summary => "Removes admin from a player.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Unadmin";
        public string Syntax => "<target player>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}