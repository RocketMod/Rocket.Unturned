using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.User;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    public class CommandInvestigate : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return typeof(UnturnedUser).IsAssignableFrom(userType);
        }

        public void Execute(ICommandContext context)
        {
            ITranslationCollection translations = ((UnturnedImplementation)context.Container.Resolve<IHost>()).ModuleTranslations;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            UnturnedPlayer target = (UnturnedPlayer) context.Parameters.Get<IPlayer>(0);

            SteamPlayer otherPlayer = target.SteamPlayer;
            context.User.SendMessage(translations.Get("command_investigate_private", otherPlayer.playerID.characterName, otherPlayer.playerID.steamID.ToString()));
        }

        public string Name => "Investigate";
        public string Summary => "Shows you the SteamID64 of a player.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Investigate";
        public string Syntax => "<player>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}