using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.RCON;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Unturned.Commands
{
    class CommandRKick : IRocketCommand
    {
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Console; }
        }

        public string Help
        {
            get { return "Kicks a client off of RCON."; }
        }

        public string Name
        {
            get { return "rkick"; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.rkick" }; }
        }

        public string Syntax
        {
            get { return "<ConnectionID>"; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            int? instance = command.GetInt32Parameter(0);
            if (command.Length == 0 || command.Length > 1 || instance == null)
            {
                UnturnedChat.Say(caller, U.Translate("command_rkick_help"));
                return;
            }
            foreach (RCONConnection client in RCONServer.Clients)
            {
                if (client.InstanceID == (int)instance)
                {
                    UnturnedChat.Say(caller, U.Translate("command_rkick_kicked", instance.ToString(), client.Address));
                    client.Close();
                    return;
                }
            }
            UnturnedChat.Say(caller, U.Translate("command_rkick_notfound", instance.ToString()));
        }
    }
}
