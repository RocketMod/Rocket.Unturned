using Rocket.API.Commands;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandEffect : ICommand
    {
        public bool SupportsCaller(ICommandCaller caller)
        {
            return caller is UnturnedPlayer;
        }

        public void Execute(ICommandContext context)
        {
            UnturnedPlayer player = (UnturnedPlayer)context.Caller;
            if(context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            ushort id = context.Parameters.Get<ushort>(0);
            player.TriggerEffect(id);
        }

        public string Name => "Effect";
        public string Description => "Triggers an effect at your position";
        public string Permission => "Rocket.Unturned.Effect";
        public string Syntax => "<id>";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}