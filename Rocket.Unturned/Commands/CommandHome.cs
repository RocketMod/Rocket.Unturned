using System;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.UnityEngine.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandHome : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            UnturnedPlayer player = ((UnturnedUser)context.User).Player;

            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            if (!BarricadeManager.tryGetBed(player.CSteamID, out Vector3 pos, out byte rot))
            {
                throw new CommandWrongUsageException(await translations.GetAsync("command_bed_no_bed_found_private"));
            }

            if (player.Entity.Stance == EPlayerStance.DRIVING || player.Entity.Stance == EPlayerStance.SITTING)
            {
                throw new CommandWrongUsageException(await translations.GetAsync("command_generic_teleport_while_driving_error"));
            }

            player.Entity.Teleport(pos.ToSystemVector(), rot);
        }

        public string Name => "Home";
        public string Summary => "Teleports you to your last bed.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Home";
        public string Syntax => "";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}