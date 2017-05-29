using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Exceptions;

namespace Rocket.Unturned.Commands
{
    public class CommandEffect : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "effect";

        public string Help => "Triggers an effect at your position";

        public string Syntax => "<id>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.effect" };

        public void Execute(ICommandContext ctx)
        {
            UnturnedPlayer player = (UnturnedPlayer)ctx.Caller;
            ushort? id = ctx.Parameters.GetUInt16Parameter(0);
            if (id == null)
            {
                throw new WrongUsageOfCommandException(ctx);
            }
            player.TriggerEffect(id.Value);
        }
    }
}