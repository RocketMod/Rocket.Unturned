using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IImplementation, UnturnedImplementation>();
            container.RegisterSingletonType<IPlayerManager, UnturnedPlayerManager>();
            container.RegisterSingletonType<IChatManager, UnturnedChatManager>();
        }
    }
}