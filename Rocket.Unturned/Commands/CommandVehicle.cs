using System;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandVehicle : ICommand
    {
        public bool SupportsUser(IUser user) => user is UnturnedUser;

        public async Task ExecuteAsync(ICommandContext context)
        {
            ITranslationCollection translations = ((RocketUnturnedHost)context.Container.Resolve<IHost>()).ModuleTranslations;

            UnturnedPlayer player = ((UnturnedUser)context.User).Player;

            if (context.Parameters.Length != 1)
            {
                throw new CommandWrongUsageException();
            }

            string param = await context.Parameters.GetAsync<string>(0);

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

            await context.User.SendLocalizedMessageAsync(translations, VehicleTool.giveVehicle(player.NativePlayer, id)
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