using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.Core;

namespace Rocket.Unturned.Commands
{
    public class CommandCompass : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "compass";

        public string Help => "Shows the direction you are facing";

        public string Syntax => "[direction]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.compass" };

        public void Execute(ICommandContext ctx)
        {
            UnturnedPlayer player = (UnturnedPlayer)ctx.Caller;
            float currentDirection = player.Rotation;
            var command = ctx.Parameters;

            if (command.Length == 1)
            {
                switch (command[0])
                {
                    case "north":
                        currentDirection = 0;
                        break;
                    case "east":
                        currentDirection = 90;
                        break;
                    case "south":
                        currentDirection = 180;
                        break;
                    case "west":
                        currentDirection = 270;
                        break;
                    default:
                        throw new WrongUsageOfCommandException(ctx);
                }
                player.Teleport(player.Position, currentDirection);
            }
            
            string directionName = "Unknown";

            if (currentDirection > 30 && currentDirection < 60)
            {
                directionName = R.Translations.Translate("command_compass_northeast");
            }
            else if (currentDirection > 60 && currentDirection < 120)
            {
                directionName = R.Translations.Translate("command_compass_east");
            }
            else if (currentDirection > 120 && currentDirection < 150)
            {
                directionName = R.Translations.Translate("command_compass_southeast");
            }
            else if (currentDirection > 150 && currentDirection < 210)
            {
                directionName = R.Translations.Translate("command_compass_south");
            }
            else if (currentDirection > 210 && currentDirection < 240)
            {
                directionName = R.Translations.Translate("command_compass_southwest");
            }
            else if (currentDirection > 240 && currentDirection < 300)
            {
                directionName = R.Translations.Translate("command_compass_west");
            }
            else if (currentDirection > 300 && currentDirection < 330)
            {
                directionName = R.Translations.Translate("command_compass_northwest");
            }
            else if (currentDirection > 330 || currentDirection < 30)
            {
                directionName = R.Translations.Translate("command_compass_north");
            }

            ctx.Print(R.Translations.Translate("command_compass_facing_private", directionName));
        }
    }
}