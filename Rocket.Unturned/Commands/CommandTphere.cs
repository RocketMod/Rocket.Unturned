using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandTphere : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return typeof(UnturnedUser).IsAssignableFrom(userType);
        }

        public void Execute(ICommandContext context)
        {
            ITranslationCollection translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;
            UnturnedPlayer player = ((UnturnedUser)context.User).UnturnedPlayer;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            UnturnedPlayer otherPlayer = (UnturnedPlayer)context.Parameters.Get<IPlayer>(0);

            if (otherPlayer.IsInVehicle)
            {
                context.User.SendLocalizedMessage(translations, "command_tphere_vehicle");
                return;
            }

            ((UnturnedPlayerEntity)otherPlayer.Entity).Teleport(player);
            context.User.SendLocalizedMessage(translations, "command_tphere_teleport_from_private", null, otherPlayer.CharacterName);
            otherPlayer.User.SendLocalizedMessage(translations, "command_tphere_teleport_to_private", null, player.CharacterName);
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