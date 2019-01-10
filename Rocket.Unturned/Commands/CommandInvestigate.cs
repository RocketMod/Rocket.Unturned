using System;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.User;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    public class CommandInvestigate : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            UnturnedPlayer target = (UnturnedPlayer) await context.Parameters.GetAsync<IPlayer>(0);

            SteamPlayer otherPlayer = target.SteamPlayer;
            await context.User.SendMessageAsync(await translations.GetAsync("command_investigate_private", otherPlayer.playerID.characterName, otherPlayer.playerID.steamID.ToString()));
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