using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandCompass : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return typeof(UnturnedUser).IsAssignableFrom(userType);
        }

        public void Execute(ICommandContext context)
        {
            UnturnedUser user = (UnturnedUser) context.User;
            var player = user.UnturnedPlayer;

            var translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;

            float currentDirection = ((UnturnedPlayerEntity)player.Entity).Rotation;

            string targetDirection = context.Parameters.Length > 0 ? context.Parameters.Get<string>(0) : null;

            if (targetDirection != null)
            {
                switch (targetDirection.ToLowerInvariant())
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
                        throw new CommandWrongUsageException();
                }

                ((UnturnedPlayerEntity)player.Entity).Teleport(player.Entity.Position, currentDirection);
            }

            string directionName = "Unknown";

            if (currentDirection > 30 && currentDirection < 60)
            {
                directionName = translations.Get("command_compass_northeast");
            }
            else if (currentDirection > 60 && currentDirection < 120)
            {
                directionName = translations.Get("command_compass_east");
            }
            else if (currentDirection > 120 && currentDirection < 150)
            {
                directionName = translations.Get("command_compass_southeast");
            }
            else if (currentDirection > 150 && currentDirection < 210)
            {
                directionName = translations.Get("command_compass_south");
            }
            else if (currentDirection > 210 && currentDirection < 240)
            {
                directionName = translations.Get("command_compass_southwest");
            }
            else if (currentDirection > 240 && currentDirection < 300)
            {
                directionName = translations.Get("command_compass_west");
            }
            else if (currentDirection > 300 && currentDirection < 330)
            {
                directionName = translations.Get("command_compass_northwest");
            }
            else if (currentDirection > 330 || currentDirection < 30)
            {
                directionName = translations.Get("command_compass_north");
            }

            user.SendLocalizedMessage(translations, "command_compass_facing_private", null, directionName);
        }

        public string Name => "Compass";
        public string Summary => "Shows the direction you are facing.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Compass";
        public string Syntax => "[direction]";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}