using Rocket.API.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.Core;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandHome : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "home";

        public string Help => "Teleports you to your last bed";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.home" };

        public void Execute(ICommandContext ctx)
        {
            UnturnedPlayer player = (UnturnedPlayer)ctx.Caller;
            Vector3 pos;
            byte rot;
            if (!BarricadeManager.tryGetBed(player.CSteamID, out pos, out rot))
            {
                ctx.Print(R.Translations.Translate("command_bed_no_bed_found_private"));
                return;
            }

            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
            {
                ctx.Print(R.Translations.Translate("command_generic_teleport_while_driving_error"));
                return;
            }
            player.Teleport(pos, rot);
        }
    }
}