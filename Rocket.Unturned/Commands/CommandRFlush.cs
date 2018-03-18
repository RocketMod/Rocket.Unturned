using Rocket.API;
using Rocket.Core.RCON;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Unturned.Commands
{
    class CommandRFlush : IRocketCommand
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
            get { return "Kicks all RCON clients off of the server."; }
        }

        public string Name
        {
            get { return "rflush"; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.rflush" }; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0 || command.Length > 1 || command[0] != "y")
            {
                UnturnedChat.Say(caller, U.Translate("command_rflush_help"));
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_rflush_total", RCONServer.Clients.Count.ToString()));
                List<RCONConnection> connections = new List<RCONConnection>();
                connections.AddRange(RCONServer.Clients);
                for (int i = 0; i < connections.Count; i++)
                {
                    RCONConnection client = connections[i];
                    if (client.Client.Client.Connected)
                    {
                        UnturnedChat.Say(caller, U.Translate("command_rflush_line", i + 1, client.InstanceID, client.Address));
                        client.Close();
                    }
                }
            }
        }
    }
}