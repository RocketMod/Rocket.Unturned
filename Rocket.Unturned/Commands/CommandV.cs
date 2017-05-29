using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.API.Providers.Logging;
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

        public void Execute(ICommandContext ctx)
        {
            var command = ctx.Parameters;
            UnturnedPlayer player = (UnturnedPlayer)ctx.Caller;
            if (command.Length != 1)
            {
                throw new WrongUsageOfCommandException(ctx);
            }

            ushort? id = command.GetUInt16Parameter(0);

            if (!id.HasValue)
            {
                string itemString = command.GetStringParameter(0);

                if (itemString == null)
                {
                    throw new WrongUsageOfCommandException(ctx);
                }

                Asset[] assets = Assets.find(EAssetType.VEHICLE);
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
                    throw new WrongUsageOfCommandException(ctx);
                }
            }

            Asset a = Assets.find(EAssetType.VEHICLE, id.Value);
            string assetName = ((VehicleAsset)a).vehicleName;

            if (VehicleTool.giveVehicle(player.Player, id.Value))
            {
                R.Logger.Log(LogLevel.WARN, R.Translations.Translate("command_v_giving_console", player.DisplayName, id));
                ctx.Print(R.Translations.Translate("command_v_giving_private", assetName, id));
                return;
            }

            ctx.Print(R.Translations.Translate("command_v_giving_failed_private", assetName, id));
        }
    }
}