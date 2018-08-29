using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandMore : ICommand
    {
        public bool SupportsUser(API.User.UserType userType) => userType == API.User.UserType.Player;

        public void Execute(ICommandContext context)
        {
            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            if(context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            byte amount = context.Parameters.Get<byte>(0);

            UnturnedPlayer player = (UnturnedPlayer)context.Player;
            ushort itemId = player.NativePlayer.equipment.itemID;

            if (itemId == 0)
            {
                context.User.SendLocalizedMessage(translations, "command_more_dequipped");
                return;
            }

            context.User.SendLocalizedMessage(translations, "command_more_give", null, amount, itemId);
            player.GiveItem(itemId, amount);
        }

        public string Name => "More";
        public string Summary => "Gives more of an item that you have in your hands.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.More";
        public string Syntax => "<amount>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}