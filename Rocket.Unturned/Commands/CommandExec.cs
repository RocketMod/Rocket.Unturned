using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System;
using Rocket.Core;

namespace Rocket.Unturned.Commands
{
    public class CommandExec : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
        }

        public string Name
        {
            get { return "exec"; }
        }

        public string Help
        {
            get { return "Execute a command as console";}
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
                return new List<string>() { "rocket.exec" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!R.Commands.Execute(new ConsolePlayer(), String.Join(" ", command)))
            {
                Commander.execute(new Steamworks.CSteamID(0), String.Join(" ", command));
            }
        }
    }
}