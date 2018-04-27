using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.Core.Commands;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandBroadcast : ICommand
    {
        public bool SupportsCaller(ICommandCaller caller)
        {
            return true; //anyone can use the command
        }

        public void Execute(ICommandContext context)
        {
            if (!(context.Container.Get<IChatManager>() is UnturnedChatManager chatManager))
                return;

            string colorName = context.Parameters.Get<string>(0);

            Color? color = chatManager.GetColorFromName(colorName);

            int i = 1;
            if (color == null) i = 0;
            string message = context.Parameters.Get<string>(i);

            if (message == null)
                throw new CommandWrongUsageException();

            chatManager.Broadcast(message, color ?? Color.green);
        }

        public string Name => "Broadcast";
        public string Description => "Broadcast a message";
        public string Permission => "Rocket.Unturned.Broadcast";
        public string Syntax => "[color] <message>";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}