using System;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.User;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandAdmin : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return true; //anyone can use the command
        }

        public void Execute(ICommandContext context)
        {
            if(context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            IPlayer target = context.Parameters.Get<IPlayer>(0);

            if (target.IsOnline && target is UnturnedPlayer uPlayer && !uPlayer.IsAdmin)
            {
                uPlayer.Admin(true);
                return;
            }

            context.User.SendMessage($"Could not admin {target.Name}" , ConsoleColor.Red);
        }

        public string Name => "Admin";
        public string Summary => "Gives admin to a player.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Admin";
        public string Syntax => "<target player>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}