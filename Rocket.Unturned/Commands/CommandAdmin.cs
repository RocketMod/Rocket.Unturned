using System;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.Player;
using Rocket.Core.User;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandAdmin : ICommand
    {
        public async Task ExecuteAsync(ICommandContext context)
        {
            if(context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            IPlayer target = await context.Parameters.GetAsync<IPlayer>(0);

            if (target.IsOnline && target is UnturnedPlayer uPlayer && !uPlayer.IsAdmin)
            {
                uPlayer.Admin(true);
                return;
            }

            await context.User.SendMessageAsync($"Could not admin {target.GetUser().DisplayName}" , ConsoleColor.Red);
        }

        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public string Name => "Admin";
        public string Summary => "Gives admin to a player.";
        public string Description => null;
        public string Syntax => "<target player>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}