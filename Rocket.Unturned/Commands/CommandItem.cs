using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    public class CommandItem : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return typeof(UnturnedUser).IsAssignableFrom(userType);
        }

        public void Execute(ICommandContext context)
        {
            ITranslationCollection translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;

            UnturnedPlayer player = ((UnturnedUser)context.User).Player;
            if (context.Parameters.Length != 1 && context.Parameters.Length != 2)
                throw new CommandWrongUsageException();

            byte amount = 1;

            string itemString = context.Parameters.Get<string>(0);

            if (!ushort.TryParse(itemString, out ushort id))
            {
                List<ItemAsset> sortedAssets = new List<ItemAsset>(Assets.find(EAssetType.ITEM).Cast<ItemAsset>());
                ItemAsset asset = sortedAssets.Where(i => i.itemName != null)
                                              .OrderBy(i => i.itemName.Length)
                                              .FirstOrDefault(i => i.itemName.ToLower().Contains(itemString.ToLower()));

                if (asset != null) id = asset.id;
                if (string.IsNullOrEmpty(itemString.Trim()) || id == 0)
                    throw new CommandWrongUsageException();
            }

            Asset a = Assets.find(EAssetType.ITEM, id);

            if(a == null)
                throw new CommandWrongUsageException();

            if (context.Parameters.Length == 2)
                amount = context.Parameters.Get<byte>(1);

            string assetName = ((ItemAsset)a).itemName;

            context.User.SendLocalizedMessage(translations, player.GiveItem(id, amount) ? "command_i_giving_private" : "command_i_giving_failed_private", 
                null, amount, assetName, id);
        }

        public string Name => "Item";
        public string Summary => "Gives yourself an item.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Item";
        public string Syntax => "[item id or name]";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => new[] { "I" };
    }
}