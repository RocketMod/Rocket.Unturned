using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rocket.Unturned.Chat;

namespace Rocket.Unturned.Commands
{
    public class CommandBroadcast : IRocketCommand
    {
        public bool AllowFromConsole
        {
            get { return true; }
        }

        public string Name
        {
            get { return "broadcast"; }
        }

        public string Help
        {
            get { return "Broadcast a message"; }
        }

        public string Syntax
        {
            get { return "<color> <message>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.broadcast" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Color? color = command.GetColorParameter(0);

            int i = 1;
            if (color == null) i = 0;
            string message = command.GetParameterString(i);

            if (message == null)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                return;
            }

            UnturnedChat.Say(message, (color.HasValue) ? (Color)color : Color.green);
        }
    }
}