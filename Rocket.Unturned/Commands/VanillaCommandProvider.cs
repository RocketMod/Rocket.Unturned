using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core.ServiceProxies;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    [ServicePriority(Priority = ServicePriority.Low)] //any other command provider should override it
    public class VanillaCommandProvider : ICommandProvider
    {
        private readonly IImplementation rocketUnturned;

        public VanillaCommandProvider(IImplementation rocketUnturned)
        {
            this.rocketUnturned = rocketUnturned;
        }
        private List<ICommand> commands;

        public ILifecycleObject GetOwner(ICommand command) => rocketUnturned;

        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (commands != null && commands.Count != 0)
                    return commands;

                commands = new List<ICommand>();

                foreach (var cmd in Commander.commands)
                    commands.Add(new VanillaCommand(cmd));

                return commands;
            }
        }
    }
}