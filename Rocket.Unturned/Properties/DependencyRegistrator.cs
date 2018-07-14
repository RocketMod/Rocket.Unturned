using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.User;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IHost, RocketUnturnedHost>(null, "unturned", "host");
            container.RegisterSingletonType<IPlayerManager, UnturnedPlayerManager>(null, "host", "unturned");
            var mgr = container.Resolve<IPlayerManager>("unturned");

            container.RegisterSingletonInstance<IUserManager>(mgr, null, "host", "unturned");
            container.RegisterSingletonType<IUserManager, StdConsoleUserManager>("stdconsole");

            container.RegisterSingletonType<ICommandProvider, VanillaCommandProvider>("unturned_commands");
            container.RegisterSingletonType<ICommandProvider, RocketUnturnedCommandProvider>("rocket_unturned_commands");

            container.RegisterSingletonType<IPermissionProvider, UnturnedAdminPermissionProvider>("unturned_admins");
        }
    }
}