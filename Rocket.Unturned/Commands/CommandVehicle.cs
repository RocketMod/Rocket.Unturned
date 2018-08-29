using System;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandVehicle : ICommand
    {
        public bool SupportsUser(API.User.UserType userType) => userType == API.User.UserType.Player;

        public void Execute(ICommandContext context)
        {
            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            UnturnedPlayer player = (UnturnedPlayer)context.Player;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            string param = context.Parameters.Get<string>(0);

            if (!ushort.TryParse(param, out ushort id))
            {    
                bool found = false;
                Asset[] assets = SDG.Unturned.Assets.find(EAssetType.VEHICLE);
                foreach (VehicleAsset ia in assets)
                {
                    if (ia?.vehicleName == null || !ia.vehicleName.ToLower().Contains(param.ToLower()))
                        continue;

                    id = ia.id;
                    found = true;
                    break;
                }

                if (!found)
                    throw new CommandWrongUsageException();
            }

            Asset a = Assets.find(EAssetType.VEHICLE, id);
            string assetName = ((VehicleAsset)a).vehicleName;

            context.User.SendLocalizedMessage(translations, VehicleTool.giveVehicle(player.NativePlayer, id)
                    ? "command_v_giving_private"
                    : "command_v_giving_failed_private", null, assetName, id);
        }

        public string Name => "Vehicle";
        public string Summary => "Gives yourself a vehicle.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Vehicle";
        public string Syntax => "<id or name>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => new [] { "V" };
    }
}