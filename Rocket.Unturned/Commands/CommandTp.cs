using SDG.Unturned;
using UnityEngine;
using System.Linq;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.API.Exceptions;
using Rocket.API.Commands;
using Rocket.API.Providers.Logging;
using Rocket.Core;

namespace Rocket.Unturned.Commands
{
    public class CommandTp : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "tp";

        public string Help => "Teleports you to another player or location";

        public string Syntax => "<player | place | x y z>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.tp", "rocket.teleport" };

        public void Execute(ICommandContext ctx)
        {
            var caller = ctx.Caller;
            var command = ctx.Parameters;
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length != 1 && command.Length != 3)
            {
                throw new WrongUsageOfCommandException(ctx);
            }

            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
            {
                ctx.Print(R.Translations.Translate("command_generic_teleport_while_driving_error"));
                return;
            }

            float? x = null;
            float? y = null;
            float? z = null;

            if (command.Length == 3)
            {
                x = command.GetFloatParameter(0);
                y = command.GetFloatParameter(1);
                z = command.GetFloatParameter(2);
            }
            if (x != null && y != null && z != null)
            {
                player.Teleport(new Vector3((float)x, (float)y, (float)z), MeasurementTool.angleToByte(player.Rotation));
                R.Logger.Log(LogLevel.INFO, R.Translations.Translate("command_tp_teleport_console", player.DisplayName, (float)x + "," + (float)y + "," + (float)z));
                ctx.Print(R.Translations.Translate("command_tp_teleport_private", (float)x + "," + (float)y + "," + (float)z));
                return;
            }
            UnturnedPlayer otherplayer = UnturnedPlayer.FromName(command[0]);
            if (otherplayer != null && otherplayer != player)
            {
                player.Teleport(otherplayer);
                R.Logger.Log(LogLevel.INFO, R.Translations.Translate("command_tp_teleport_console", player.DisplayName, otherplayer.DisplayName));
                ctx.Print(R.Translations.Translate("command_tp_teleport_private", otherplayer.DisplayName));
                return;
            }

            Node item = LevelNodes.nodes.FirstOrDefault(n => n.type == ENodeType.LOCATION && ((LocationNode)n).name.ToLower().Contains(command[0].ToLower()));
            if (item != null)
            {
                Vector3 c = item.point + new Vector3(0f, 0.5f, 0f);
                player.Teleport(c, MeasurementTool.angleToByte(player.Rotation));
                R.Logger.Log(LogLevel.INFO, R.Translations.Translate("command_tp_teleport_console", player.DisplayName, ((LocationNode)item).name));
                ctx.Print(R.Translations.Translate("command_tp_teleport_private", ((LocationNode)item).name));
                return;
            }

            ctx.Print(R.Translations.Translate("command_tp_failed_find_destination"));
        }
    }
}