using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IHost, RocketUnturnedHost>(null, "unturned", "game");
            container.RegisterSingletonType<IPlayerManager, UnturnedPlayerManager>(null, "game", "unturned");
            var mgr = container.Resolve<IPlayerManager>("unturned");
            container.RegisterSingletonInstance<IUserManager>(mgr, null, "game", "unturned");

            container.RegisterSingletonType<ICommandProvider, VanillaCommandProvider>("unturned_commands");
            container.RegisterSingletonType<ICommandProvider, RocketUnturnedCommandProvider>("rocket_unturned_commands");
        }
    }
}