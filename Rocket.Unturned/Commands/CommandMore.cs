using System;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Commands
{
    public class CommandMore : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            if(context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            byte amount = await context.Parameters.GetAsync<byte>(0);

            UnturnedPlayer player = ((UnturnedUser)context).Player;
            ushort itemId = player.NativePlayer.equipment.itemID;

            if (itemId == 0)
            {
                await context.User.SendLocalizedMessageAsync(translations, "command_more_dequipped");
                return;
            }

            await context.User.SendLocalizedMessageAsync(translations, "command_more_give", null, amount, itemId);
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