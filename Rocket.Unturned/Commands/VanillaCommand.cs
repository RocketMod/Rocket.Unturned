using System.Linq;
using Rocket.API.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace Rocket.Unturned.Commands
{
    public class VanillaCommand : ICommand
    {
        public Command NativeCommand { get; }

        public VanillaCommand(Command nativeCommand)
        {
            NativeCommand = nativeCommand;
        }

        public void Execute(ICommandContext context)
        {
            CSteamID id = CSteamID.Nil;
            if (context.Caller is UnturnedPlayer player)
            {
                id = player.CSteamID;
            }
            Commander.commands.FirstOrDefault(c => c.command == Name)?.check(id, Name, string.Join("/", context.Parameters));
        }

        public string Name => NativeCommand.command;
        public string Permission => null; /* default permission */
    }
}