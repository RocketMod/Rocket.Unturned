using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
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

        public void Execute(ICommandContext context)
        {
            CSteamID id = CSteamID.Nil;
            switch (context.Caller) {
                case UnturnedPlayer player:
                    id = player.CSteamID;
                    break;
                case IConsoleCommandCaller _:
                    id = CSteamID.Nil;
                    break;

                default:
                    throw new NotSupportedException();
            }

            Commander.commands.FirstOrDefault(c => c.command == Name)?.check(id, Name, string.Join("/", context.Parameters.ToArray()));
        }

        public bool SupportsCaller(Type commandCaller)
        {
            //Thanks to unturned we cant know if console is supported before command execution
            return true;
        }

        public string Name => NativeCommand.command;
        public string Description => NativeCommand.help;
        public string Permission => null; /* default permission */
        public string Syntax => NativeCommand.info.Replace(Name, "").Trim();
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}