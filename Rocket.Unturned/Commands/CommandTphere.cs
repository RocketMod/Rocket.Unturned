using System;
using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandTphere : ICommand
    {
        public bool SupportsCaller(Type commandCaller)
        {
            return typeof(UnturnedPlayer).IsAssignableFrom(commandCaller);
        }

        public void Execute(ICommandContext context)
        {
            ITranslationLocator translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;
            IChatManager chatManager = context.Container.Resolve<IChatManager>();

            UnturnedPlayer player = (UnturnedPlayer)context.Caller;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            UnturnedPlayer otherPlayer = (UnturnedPlayer)context.Parameters.Get<IOnlinePlayer>(0);

            if (otherPlayer.IsInVehicle)
            {
                chatManager.SendLocalizedMessage(translations, player, "command_tphere_vehicle");
                return;
            }

            otherPlayer.Teleport(player);
            chatManager.SendLocalizedMessage(translations, player, "command_tphere_teleport_from_private", otherPlayer.CharacterName);
            chatManager.SendLocalizedMessage(translations, otherPlayer, "command_tphere_teleport_to_private", player.CharacterName);
        }

        public string Name => "Tphere";
        public string Description => "Teleports another player to you";
        public string Permission => "Rocket.Unturned.Tphere";
        public string Syntax => "<player>";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}