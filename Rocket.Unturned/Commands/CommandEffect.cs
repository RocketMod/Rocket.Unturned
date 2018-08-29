using System;
using Rocket.API.Commands;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandEffect : ICommand
    {
        public bool SupportsUser(API.User.UserType userType) => userType == API.User.UserType.Player;

        public void Execute(ICommandContext context)
        {
            if(context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            ushort id = context.Parameters.Get<ushort>(0);
            ((UnturnedPlayer)context.Player).TriggerEffect(id);
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