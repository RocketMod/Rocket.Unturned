using System;
using System.Linq;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.UnityEngine.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandTp : ICommand
    {
        public bool SupportsUser(Type userType)
        {
            return typeof(UnturnedUser).IsAssignableFrom(userType);
        }

        public void Execute(ICommandContext context)
        {
            ITranslationCollection translations = ((UnturnedImplementation)context.Container.Resolve<IImplementation>()).ModuleTranslations;
            UnturnedPlayer player = ((UnturnedUser)context.User).UnturnedPlayer;

            if (context.Parameters.Length != 1 && context.Parameters.Length != 3)
                throw new CommandWrongUsageException();

            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
                throw new CommandWrongUsageException(
                    translations.Get("command_generic_teleport_while_driving_error"));

            float? x = null;
            float? y = null;
            float? z = null;

            if (context.Parameters.Length == 3)
            {
                x = context.Parameters.Get<float>(0);
                y = context.Parameters.Get<float>(1);
                z = context.Parameters.Get<float>(2);
            }

            if (x != null)
            {
                player.Teleport(new API.Math.Vector3((float)x, (float)y, (float)z), MeasurementTool.angleToByte(player.Rotation));
                context.User.SendLocalizedMessage(translations, "command_tp_teleport_private", null, (float)x + "," + (float)y + "," + (float)z);
                return;
            }

            if (context.Parameters.Get<IPlayer>(0) is UnturnedPlayer otherplayer && otherplayer != player)
            {
                player.Teleport(otherplayer);
                context.User.SendLocalizedMessage(translations, "command_tp_teleport_private", null, otherplayer.CharacterName);
                return;
            }

            Node item = LevelNodes.nodes.FirstOrDefault(n => n.type == ENodeType.LOCATION && ((LocationNode)n).name.ToLower().Contains(context.Parameters.Get<string>(0).ToLower()));
            if (item != null)
            {
                Vector3 c = item.point + new Vector3(0f, 0.5f, 0f);
                player.Teleport(c.ToRocketVector(), MeasurementTool.angleToByte(player.Rotation));
                context.User.SendLocalizedMessage(translations, "command_tp_teleport_private", null, ((LocationNode)item).name);
                return;
            }

            context.User.SendLocalizedMessage(translations, "command_tp_failed_find_destination");
        }

        public string Name => "Tp";
        public string Summary => "Teleports you to another player or location.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Tp";
        public string Syntax => "<player | place | x y z>";
        public IChildCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}