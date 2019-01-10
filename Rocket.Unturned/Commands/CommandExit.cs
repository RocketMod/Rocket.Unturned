using System;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandExit : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            var playerManager = context.Container.Resolve<IPlayerManager>();
            await playerManager.KickAsync(context.User, context.User, "Exit");
        }

        public string Name => "Exit";
        public string Summary => "Exits the game without cooldown.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Exit";
        public string Syntax => "";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}