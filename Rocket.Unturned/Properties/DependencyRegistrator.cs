using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IImplementation, UnturnedImplementation>(null, "unturned");
            container.RegisterSingletonType<IPlayerManager, UnturnedPlayerManager>(null, "unturned_playermanager");
            container.RegisterSingletonType<IChatManager, UnturnedChatManager>(null, "unturned_chat");
            container.RegisterSingletonType<ICommandProvider, VanillaCommandProvider>("unturned_commands");
            container.RegisterSingletonType<ICommandProvider, RocketUnturnedCommandProvider>("rocket_unturned_commands");
        }
    }
}