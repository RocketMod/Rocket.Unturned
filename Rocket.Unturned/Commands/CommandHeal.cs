using System;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.I18N;
using Rocket.Core.Player;
using Rocket.Core.User;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandHeal : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();
            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            IPlayer target;
            if (await permissions.CheckPermissionAsync(context.User, Permission + ".Others") == PermissionResult.Grant
                && context.Parameters.Length >= 1)
                target = await context.Parameters.GetAsync<IPlayer>(0);
            else
                target = ((UnturnedUser)context.User).Player;

            if (!(target is UnturnedPlayer uPlayer))
            {
                await context.User.SendMessageAsync($"Could not heal {target.GetUser().DisplayName}", ConsoleColor.Red);
                return;
            }

            uPlayer.Entity.Heal(100);
            uPlayer.Entity.Bleeding = false;
            uPlayer.Entity.Broken = false;
            uPlayer.Entity.Infection = 0;
            uPlayer.Entity.Hunger = 0;
            uPlayer.Entity.Thirst = 0;

            if (target == context.User)
            {
                await context.User.SendLocalizedMessage(translations, "command_heal_success");
                return;
            }

            await context.User.SendLocalizedMessage(translations, "command_heal_success_me", null, target.GetUser().DisplayName);
            await target.GetUser().SendLocalizedMessage(translations, "command_heal_success_other", null, context.User.DisplayName);
        }

        public string Name => "Heal";
        public string Summary => "Heals yourself or somebody else.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Heal";
        public string Syntax => "[player]";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}