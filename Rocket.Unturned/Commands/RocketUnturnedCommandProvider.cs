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
        private readonly IHost rocketUnturned;

        public RocketUnturnedCommandProvider(IHost rocketUnturned)
        {
            this.rocketUnturned = rocketUnturned;
        }

        public ILifecycleObject GetOwner(ICommand command) => rocketUnturned;
        public void Init()
        {
            var types = (typeof(RocketUnturnedCommandProvider).Assembly.FindTypes<ICommand>())
                        .Where(c => c.GetCustomAttributes(typeof(DontAutoRegisterAttribute), true).Length == 0)
                        .Where(c => !typeof(IChildCommand).IsAssignableFrom(c));

            List<ICommand> list = new List<ICommand>();
            foreach (Type type in types)
                list.Add((ICommand)Activator.CreateInstance(type, new object[0]));
            Commands = list;
        }

        public IEnumerable<ICommand> Commands { get; private set; }
        public string ServiceName => "Rocket.Unturned";
    }
}