using System;
using System.Threading.Tasks;
using Rocket.API.Drawing;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandBroadcast : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            if (!(context.Container.Resolve<IPlayerManager>("unturned") is UnturnedPlayerManager playerManager))
                return;

            string colorName = await context.Parameters.GetAsync<string>(0);

            Color? color = playerManager.GetColorFromName(colorName);

            int i = 1;
            if (color == null) i = 0;
            string message = await context.Parameters.GetAsync<string>(i);

            if (message == null)
                throw new CommandWrongUsageException();

            await playerManager.BroadcastAsync(null, message, color ?? Color.Green);
        }

        public string Name => "Broadcast";
        public string Summary => "Broadcasts a message.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Broadcast";
        public string Syntax => "[color] <message>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}