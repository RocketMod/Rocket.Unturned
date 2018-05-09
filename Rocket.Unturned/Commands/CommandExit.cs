using System;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Unturned.Commands
{
    public class CommandExit : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return typeof(IPlayerUser).IsAssignableFrom(userType);
        }

        public void Execute(ICommandContext context)
        {
            IPlayerUser player = (IPlayerUser)context.User;
            var playerManager = context.Container.Resolve<IPlayerManager>();
            playerManager.Kick(player, context.User, "Exit");
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