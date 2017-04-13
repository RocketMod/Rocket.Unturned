using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.API.Player;
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

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            Vector3 pos;
            byte rot;
            if (!BarricadeManager.tryGetBed(player.CSteamID, out pos, out rot))
            {
                U.Instance.Chat.Say(caller, U.Translate("command_bed_no_bed_found_private"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            else
            {
                if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
                {
                    U.Instance.Chat.Say(caller, U.Translate("command_generic_teleport_while_driving_error"));
                    throw new WrongUsageOfCommandException(caller, this);
                }
                else
                {
                    player.Teleport(pos, rot);
                }
            }

        }
    }
}