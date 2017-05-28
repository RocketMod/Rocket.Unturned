using System.Collections.Generic;
using UnityEngine;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.API.Player;
using Rocket.Core.Commands;

namespace Rocket.Unturned.Commands
{
    public class CommandBroadcast : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "broadcast";

        public string Help => "Broadcast a message";

        public string Syntax => "<color> <message>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.broadcast" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Color? color = command.GetColorParameter(0);

            int i = 1;
            if (color == null) i = 0;
            string message = command.GetParameterString(i);

            if (message == null)
            {
                U.Instance.Chat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            U.Instance.Chat.Say(message, (color.HasValue) ? (Color)color : Color.green);
        }
    }
}