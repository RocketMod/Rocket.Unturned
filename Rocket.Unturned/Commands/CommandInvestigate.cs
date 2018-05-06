using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    public class CommandInvestigate : ICommand
    {
        public bool SupportsCaller(Type commandCaller)
        {
            return typeof(UnturnedPlayer).IsAssignableFrom(commandCaller);
        }

        public void Execute(ICommandContext context)
        {
            ITranslationLocator translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            UnturnedPlayer target = (UnturnedPlayer) context.Parameters.Get<IOnlinePlayer>(0);

            SteamPlayer otherPlayer = target.SteamPlayer;
            context.Caller.SendMessage(translations.GetLocalizedMessage("command_investigate_private", otherPlayer.playerID.characterName, otherPlayer.playerID.steamID.ToString()));
        }

        public string Name => "Investigate";
        public string Summary => "Shows you the SteamID64 of a player.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Investigate";
        public string Syntax => "<player>";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}