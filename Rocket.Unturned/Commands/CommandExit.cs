using System;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandExit : ICommand
    {
        public bool SupportsCaller(Type commandCaller)
        {
            return typeof(IOnlinePlayer).IsAssignableFrom(commandCaller);
        }

        public void Execute(ICommandContext context)
        {
            IOnlinePlayer player = (IOnlinePlayer)context.Caller;
            var playerManager = context.Container.Resolve<IPlayerManager>();
            playerManager.Kick(player, context.Caller, "Exit");
        }

        public string Name => "Exit";
        public string Summary => "Exits the game without cooldown.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Exit";
        public string Syntax => "";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}