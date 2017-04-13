using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using Rocket.Unturned.Items;
using Rocket.API.Exceptions;
using Rocket.API.Commands;
using Rocket.API.Player;
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

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length == 0 || command.Length > 2)
            {
                U.Instance.Chat.Say(player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            ushort id = 0;
            byte amount = 1;

            string itemString = command[0].ToString();

            if (!ushort.TryParse(itemString, out id))
            {
                ItemAsset asset = UnturnedItems.GetItemAssetByName(itemString.ToLower());
                if (asset != null) id = asset.id;
                if (String.IsNullOrEmpty(itemString.Trim()) || id == 0)
                {
                    U.Instance.Chat.Say(player, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }
            }

            Asset a = SDG.Unturned.Assets.find(EAssetType.ITEM,id);

            if (command.Length == 2 && !byte.TryParse(command[1].ToString(), out amount) || a == null)
            {
                U.Instance.Chat.Say(player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            string assetName = ((ItemAsset)a).itemName;

            if (player.GiveItem(id, amount))
            {
                R.Logger.Info(U.Translate("command_i_giving_console", player.DisplayName, id, amount));
                U.Instance.Chat.Say(player, U.Translate("command_i_giving_private", amount, assetName, id));
            }
            else
            {
                U.Instance.Chat.Say(player, U.Translate("command_i_giving_failed_private", amount, assetName, id));
            }
        }
    }
}