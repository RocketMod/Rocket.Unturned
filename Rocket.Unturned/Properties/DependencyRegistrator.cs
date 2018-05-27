using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IHost, UnturnedImplementation>(null, "unturned");
            container.RegisterSingletonType<IPlayerManager, UnturnedPlayerManager>(null, "unturned");
            container.RegisterSingletonType<ICommandProvider, VanillaCommandProvider>("unturned_commands");
            container.RegisterSingletonType<ICommandProvider, RocketUnturnedCommandProvider>("rocket_unturned_commands");
        }
    }
}