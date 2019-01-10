using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using System.Threading.Tasks;

namespace Rocket.Unturned.Commands
{
    public class CommandCompass : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            UnturnedUser user = (UnturnedUser)context.User;

            var translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            var player = ((UnturnedUser)context.User).Player;
            float currentDirection = player.Entity.Rotation;

            string targetDirection = context.Parameters.Length > 0 ? await context.Parameters.GetAsync<string>(0) : null;

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

                player.Entity.Teleport(player.Entity.Position, currentDirection);
            }

            string directionName = "Unknown";

            if (currentDirection > 30 && currentDirection < 60)
            {
                directionName = await translations.GetAsync("command_compass_northeast");
            }
            else if (currentDirection > 60 && currentDirection < 120)
            {
                directionName = await translations.GetAsync("command_compass_east");
            }
            else if (currentDirection > 120 && currentDirection < 150)
            {
                directionName = await translations.GetAsync("command_compass_southeast");
            }
            else if (currentDirection > 150 && currentDirection < 210)
            {
                directionName = await translations.GetAsync("command_compass_south");
            }
            else if (currentDirection > 210 && currentDirection < 240)
            {
                directionName = await translations.GetAsync("command_compass_southwest");
            }
            else if (currentDirection > 240 && currentDirection < 300)
            {
                directionName = await translations.GetAsync("command_compass_west");
            }
            else if (currentDirection > 300 && currentDirection < 330)
            {
                directionName = await translations.GetAsync("command_compass_northwest");
            }
            else if (currentDirection > 330 || currentDirection < 30)
            {
                directionName = await translations.GetAsync("command_compass_north");
            }

            await user.SendLocalizedMessage(translations, "command_compass_facing_private", null, directionName);
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