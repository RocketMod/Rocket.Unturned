using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.User;
using Rocket.Core.DependencyInjection;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace Rocket.Unturned.Commands
{
    [DontAutoRegister]
    public class VanillaCommand : ICommand
    {
        public Command NativeCommand { get; }

        public VanillaCommand(Command nativeCommand)
        {
            NativeCommand = nativeCommand;
        }

        public bool SupportsUser(IUser user) => true;

        public async Task ExecuteAsync(ICommandContext context)
        {
            CSteamID id = CSteamID.Nil;
            switch (context.User)
            {
                case UnturnedUser user:
                    id = user.Player.NativePlayer.channel.owner.playerID.steamID;
                    break;
                case IConsole _:
                    id = CSteamID.Nil;
                    break;

                default:
                    throw new NotSupportedException();
            }

            Commander.commands.FirstOrDefault(c => c.command == Name)?.check(id, Name, string.Join("/", context.Parameters.ToArray()));
        }

        public string Name => NativeCommand.command;
        public string Summary => NativeCommand.help;
        public string Description => null;
        public string Permission => "Unturned." + Name;
        public string Syntax => NativeCommand.info.Replace(Name, "").Trim();
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}