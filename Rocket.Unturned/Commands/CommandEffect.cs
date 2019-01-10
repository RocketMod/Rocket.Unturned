using System;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandEffect : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            if(context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            ushort id = await context.Parameters.GetAsync<ushort>(0);

            var player = ((UnturnedUser)context.User).Player;
            player.TriggerEffect(id);
        }

        public string Name => "Effect";
        public string Summary => "Triggers an effect at your position.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Effect";
        public string Syntax => "<id>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}