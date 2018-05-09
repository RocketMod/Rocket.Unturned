using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.I18N;
using Rocket.Core.User;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandHeal : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return typeof(IPlayerUser).IsAssignableFrom(userType);
        }

        public void Execute(ICommandContext context)
        {
            IPermissionProvider permissions = context.Container.Resolve<IPermissionProvider>();
            ITranslationCollection translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;

            IPlayer target;
            if (permissions.CheckPermission(context.User, Permission + ".Others") == PermissionResult.Grant 
                && context.Parameters.Length >= 1)
                target = context.Parameters.Get<IPlayer>(0);
            else
                target = ((IPlayerUser)context.User).Player;

            if (!(target is UnturnedPlayer uPlayer))
            {
                context.User.SendMessage($"Could not heal {target.Name}", ConsoleColor.Red);
                return;
            }

            uPlayer.Heal(100);
            uPlayer.Bleeding = false;
            uPlayer.Broken = false;
            uPlayer.Infection = 0;
            uPlayer.Hunger = 0;
            uPlayer.Thirst = 0;

            if (target == context.User)
            {
                context.User.SendLocalizedMessage(translations, "command_heal_success");
                return;
            }

            context.User.SendLocalizedMessage(translations, "command_heal_success_me", null, target.Name);
            target.User.SendLocalizedMessage(translations, "command_heal_success_other", null, context.User.Name);
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