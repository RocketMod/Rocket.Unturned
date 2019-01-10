using System;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandTphere : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            UnturnedPlayer player = ((UnturnedUser)context.User).Player;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            UnturnedPlayer otherPlayer = (UnturnedPlayer) await context.Parameters.GetAsync<IPlayer>(0);

            if (otherPlayer.IsInVehicle)
            {
                await context.User.SendLocalizedMessage(translations, "command_tphere_vehicle");
                return;
            }

            otherPlayer.Entity.Teleport(player);
            await context.User.SendLocalizedMessage(translations, "command_tphere_teleport_from_private", null, otherPlayer.CharacterName);
            await otherPlayer.User.SendLocalizedMessage(translations, "command_tphere_teleport_to_private", null, player.CharacterName);
        }

        public string Name => "Tphere";
        public string Summary => "Teleports another player to you.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Tphere";
        public string Syntax => "<player>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}