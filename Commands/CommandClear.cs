using SDG.Unturned;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.API;
using System;
using Rocket.Unturned.Chat;

namespace Rocket.Unturned.Commands
{
    public class CommandClear : IRocketCommand
    {
        public bool AllowFromConsole
        {
            get { return true; }
        }
        
        public string Name
        {
            get { return "clear"; }
        }

        public string Help
        {
            get { return "Despawns all items"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.clear" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            int itemCount = 0;
            for (int i = 0; i< ItemManager.ItemRegions.GetLength(0); i++)
            {
                for (int j = 0; j < ItemManager.ItemRegions.GetLength(1); j++)
                {
                    ItemRegion region = ItemManager.ItemRegions[i, j];
                    foreach (SDG.Unturned.ItemData item in region.items)
                    {
                        item.LastDropped = float.MinValue;
                        item.IsDropped = true;
                        itemCount++;
                    }
                }
            }
            UnturnedChat.Say(caller, U.Translate("command_clear_success", itemCount));
        }
    }
}