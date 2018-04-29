using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandHome : ICommand
    {
        public bool SupportsCaller(Type commandCaller)
        {
            return typeof(UnturnedPlayer).IsAssignableFrom(commandCaller);
        }

        public void Execute(ICommandContext context)
        {
            UnturnedPlayer player = (UnturnedPlayer) context.Caller;
            ITranslationLocator translations = ((UnturnedImplementation)context.Container.Get<IImplementation>()).ModuleTranslations;

            if (!BarricadeManager.tryGetBed(player.CSteamID, out Vector3 pos, out byte rot))
            {
                throw new CommandWrongUsageException(translations.GetLocalizedMessage("command_bed_no_bed_found_private"));
            }

            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
            {
                throw new CommandWrongUsageException(translations.GetLocalizedMessage("command_generic_teleport_while_driving_error"));
            }

            player.Teleport(pos, rot);
        }

        public string Name => "Home";
        public string Description => "Teleports you to your last bed";
        public string Permission => "Rocket.Unturned.Home";
        public string Syntax => "";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}