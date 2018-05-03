using System.Collections.Generic;
using Rocket.API.Commands;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    public class VanillaCommandProvider : ICommandProvider
    {
        private List<ICommand> commands;

        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (commands != null)
                    return commands;

                commands = new List<ICommand>();

                foreach (var cmd in Commander.commands)
                    commands.Add(new VanillaCommand(cmd));

                return commands;
            }
        }
    }
}