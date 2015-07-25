using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System;

namespace Rocket.Unturned.Commands
{
    public class CommandExit : IRocketCommand
    {
        public bool AllowFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "exit"; }
        }

        public string Help
        {
            get { return "Exit the game without cooldown";}
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.exit" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            Steam.kick(player.CSteamID, "you exited");
        }
    }
}