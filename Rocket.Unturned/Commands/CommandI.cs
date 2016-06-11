using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Items;

namespace Rocket.Unturned.Commands
{
    public class CommandI : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Name
        {
            get { return "i"; }
        }

        public string Help
        {
            get { return "Gives yourself an item";}
        }

        public string Syntax
        {
            get { return "<id> [amount]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "item" }; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.item" , "rocket.i" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            ushort id = 0;
            byte amount = 1;

            string itemString = command[0].ToString();

            if (!ushort.TryParse(itemString, out id))
            {
                ItemAsset asset = UnturnedItems.GetItemAssetByName(itemString.ToLower());
                if (asset != null) id = asset.Id;
                if (String.IsNullOrEmpty(itemString.Trim()) || id == 0)
                {
                    UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }
            }

            Asset a = SDG.Unturned.Assets.find(EAssetType.ITEM,id);

            if (command.Length == 2 && !byte.TryParse(command[1].ToString(), out amount) || a == null)
            {
                UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            string assetName = ((ItemAsset)a).Name;

            if (player.GiveItem(id, amount))
            {
                Logger.Log(U.Translate("command_i_giving_console", player.DisplayName, id, amount));
                UnturnedChat.Say(player, U.Translate("command_i_giving_private", amount, assetName, id));
            }
            else
            {
                UnturnedChat.Say(player, U.Translate("command_i_giving_failed_private", amount, assetName, id));
            }
        }
    }
}