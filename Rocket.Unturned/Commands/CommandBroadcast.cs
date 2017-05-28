using System.Collections.Generic;
using UnityEngine;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.Core;

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

        public void Execute(ICommandContext ctx)
        {
            Color? color = ctx.Parameters.GetColorParameter(0);

            int i = 1;
            if (color == null) i = 0;
            string message = ctx.Parameters.GetParameterString(i);

            if (message == null)
            {
                throw new WrongUsageOfCommandException(ctx);
            }

            R.Implementation.Chat.Say(message, color ?? Color.green);
        }
    }
}