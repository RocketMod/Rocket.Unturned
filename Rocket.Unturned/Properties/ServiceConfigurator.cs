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
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IDependencyContainer container)
        {
            container.AddSingleton<IHost, RocketUnturnedHost>(null, "unturned", "host");
            container.AddSingleton<IPlayerManager, UnturnedPlayerManager>(null, "host", "unturned");
            var mgr = container.Resolve<IPlayerManager>("unturned");

            container.AddSingleton<IUserManager>(mgr, null, "host", "unturned");
            container.AddSingleton<IUserManager, StdConsoleUserManager>("stdconsole");

            container.AddSingleton<ICommandProvider, VanillaCommandProvider>("unturned_commands");
            container.AddSingleton<ICommandProvider, RocketUnturnedCommandProvider>("rocket_unturned_commands");

            container.AddSingleton<IPermissionChecker, UnturnedAdminPermissionChecker>("unturned_admins");
        }
    }
}