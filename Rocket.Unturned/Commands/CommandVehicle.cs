using System;
using Rocket.API;
using Rocket.API.Chat;
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
        public bool SupportsCaller(Type commandCaller)
        {
            return typeof(UnturnedPlayer).IsAssignableFrom(commandCaller);
        }

        public void Execute(ICommandContext context)
        {
            ITranslationLocator translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;
            IChatManager chatManager = context.Container.Resolve<IChatManager>();

            UnturnedPlayer player = (UnturnedPlayer)context.Caller;
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

            chatManager.SendLocalizedMessage(translations, player,
                VehicleTool.giveVehicle(player.Player, id)
                    ? "command_v_giving_private"
                    : "command_v_giving_failed_private", assetName, id);
        }

        public string Name => "Vehicle";
        public string Summary => "Gives yourself a vehicle.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Vehicle";
        public string Syntax => "<id or name>";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => new [] { "V" };
    }
}