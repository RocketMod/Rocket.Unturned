using System;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandBroadcast : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return true; //anyone can use the command
        }

        public void Execute(ICommandContext context)
        {
            if (!(context.Container.Resolve<IPlayerManager>("unturned") is UnturnedPlayerManager playerManager))
                return;

            string colorName = context.Parameters.Get<string>(0);

            Color? color = playerManager.GetColorFromName(colorName);

            int i = 1;
            if (color == null) i = 0;
            string message = context.Parameters.Get<string>(i);

            if (message == null)
                throw new CommandWrongUsageException();

            playerManager.Broadcast(null, message, color ?? Color.Green);
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