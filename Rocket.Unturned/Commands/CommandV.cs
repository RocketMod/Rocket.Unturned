using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.API.Player;
using Rocket.Core;

namespace Rocket.Unturned.Commands
{
    public class CommandV : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "v";

        public string Help => "Gives yourself an vehicle";

        public string Syntax => "<id>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rocket.v", "rocket.vehicle" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length != 1)
            {
                U.Instance.Chat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            ushort? id = command.GetUInt16Parameter(0);

            if (!id.HasValue)
            {
                string itemString = command.GetStringParameter(0);

                if (itemString == null)
                {
                    U.Instance.Chat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }

                Asset[] assets = SDG.Unturned.Assets.find(EAssetType.VEHICLE);
                foreach (VehicleAsset ia in assets)
                {
                    if (ia != null && ia.vehicleName != null && ia.vehicleName.ToLower().Contains(itemString.ToLower()))
                    {
                        id = ia.id;
                        break;
                    }
                }
                if (!id.HasValue)
                {
                    U.Instance.Chat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }
            }

            Asset a = SDG.Unturned.Assets.find(EAssetType.VEHICLE, id.Value);
            string assetName = ((VehicleAsset)a).vehicleName;

            if (VehicleTool.giveVehicle(player.Player, id.Value))
            {
                R.Logger.Warn(U.Translate("command_v_giving_console", player.DisplayName, id));
                U.Instance.Chat.Say(caller, U.Translate("command_v_giving_private", assetName, id));
            }
            else
            {
                U.Instance.Chat.Say(caller, U.Translate("command_v_giving_failed_private", assetName, id));
            }
        }
    }
}