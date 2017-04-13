using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Unturned.Commands
{

    public class UnturnedVanillaCommand : IRocketCommand
    {
        public Command Command;

        public UnturnedVanillaCommand(Command command)
        {
            Command = command;
        }

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Help => Command.help;

        public string Name => Command.command;

        public List<string> Permissions => new List<string>() { "unturned." + Command.command.ToLower() };

        public string Syntax => Command.info.Replace("/", " ");

        public void Execute(IRocketPlayer caller, string[] command)
        {
            CSteamID id = CSteamID.Nil;
            if (caller is UnturnedPlayer)
            {
                id = ((UnturnedPlayer)caller).CSteamID;
            }
            Commander.commands.FirstOrDefault(c => c.command == Name)?.check(id, Name, String.Join("/", command));
        }
    }
}
