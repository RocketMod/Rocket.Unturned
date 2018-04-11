using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.Ioc;
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
            container.RegisterSingletonType<IPlayerManager, UnturnedPlayerManager>(null, "unturnedplayermanager");
            container.RegisterSingletonType<IChatManager, UnturnedChatManager>(null, "unturnedchat");
            container.RegisterSingletonType<ICommandProvider, VanillaCommandProvider>(null, "vanillacommands");
        }
    }
}