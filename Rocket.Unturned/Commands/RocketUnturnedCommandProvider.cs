using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core.DependencyInjection;
using Rocket.Core.Extensions;

namespace Rocket.Unturned.Commands
{
    public class RocketUnturnedCommandProvider : ICommandProvider
    {
        private readonly IImplementation rocketUnturned;

        public RocketUnturnedCommandProvider(IImplementation rocketUnturned)
        {
            this.rocketUnturned = rocketUnturned;
            var types = (typeof(RocketUnturnedCommandProvider).Assembly.FindTypes<ICommand>())
                .Where(c => c.GetCustomAttributes(typeof(DontAutoRegisterAttribute), true).Length == 0)
                .Where(c => !typeof(IChildCommand).IsAssignableFrom(c));

            List<ICommand> list = new List<ICommand>();
            foreach (Type type in types)
                list.Add((ICommand)Activator.CreateInstance(type, new object[0]));
            Commands = list;
        }

        public ILifecycleObject GetOwner(ICommand command) => rocketUnturned;

        public IEnumerable<ICommand> Commands { get; }
        public string ServiceName => "Rocket.Unturned";
    }
}