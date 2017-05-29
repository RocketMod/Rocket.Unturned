using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using Rocket.Unturned.Items;
using Rocket.API.Exceptions;
using Rocket.API.Commands;
using Rocket.API.Providers.Logging;
using Rocket.Core;

namespace Rocket.Unturned.Commands
{
    public class CommandI : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "i";

        public string Help => "Gives yourself an item";

        public string Syntax => "<id> [amount]";

        public List<string> Aliases => new List<string>() { "item" };

        public List<string> Permissions => new List<string>() { "rocket.item" , "rocket.i" };

        public void Execute(ICommandContext ctx)
        {
            UnturnedPlayer player = (UnturnedPlayer)ctx.Caller;

            var command = ctx.Parameters;
            if (command.Length == 0 || command.Length > 2)
            {
                throw new WrongUsageOfCommandException(ctx);
            }

            ushort id;
            byte amount = 1;

            string itemString = command[0];

            if (!ushort.TryParse(itemString, out id))
            {
                ItemAsset asset = UnturnedItems.GetItemAssetByName(itemString.ToLower());
                if (asset != null) id = asset.id;
                if (String.IsNullOrEmpty(itemString.Trim()) || id == 0)
                {
                    throw new WrongUsageOfCommandException(ctx);
                }
            }

            Asset a = Assets.find(EAssetType.ITEM,id);

            if (command.Length == 2 && !byte.TryParse(command[1].ToString(), out amount) || a == null)
            {
                throw new WrongUsageOfCommandException(ctx);
            }

            string assetName = ((ItemAsset)a).itemName;

            if (player.GiveItem(id, amount))
            {
                R.Logger.Log(LogLevel.INFO, R.Translations.Translate("command_i_giving_console", player.DisplayName, id, amount));
                ctx.Print(R.Translations.Translate("command_i_giving_private", amount, assetName, id));
                return;

            }
            ctx.Print(R.Translations.Translate("command_i_giving_failed_private", amount, assetName, id));
        }
    }
}