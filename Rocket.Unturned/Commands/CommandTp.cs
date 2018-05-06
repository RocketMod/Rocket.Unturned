using System;
using System.Linq;
using Rocket.API;
using Rocket.API.Chat;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public class CommandTp : ICommand
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
            if (context.Parameters.Length != 1 && context.Parameters.Length != 3)
                throw new CommandWrongUsageException();

            if (player.Stance == EPlayerStance.DRIVING || player.Stance == EPlayerStance.SITTING)
                throw new CommandWrongUsageException(
                    translations.GetLocalizedMessage("command_generic_teleport_while_driving_error"));

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
                player.Teleport(new Vector3((float)x, (float)y, (float)z), MeasurementTool.angleToByte(player.Rotation));
                chatManager.SendLocalizedMessage(translations, player, "command_tp_teleport_private", (float)x + "," + (float)y + "," + (float)z);
                return;
            }

            if (context.Parameters.Get<IOnlinePlayer>(0) is UnturnedPlayer otherplayer && otherplayer != player)
            {
                player.Teleport(otherplayer);
                chatManager.SendLocalizedMessage(translations, player, "command_tp_teleport_private", otherplayer.CharacterName);
                return;
            }

            Node item = LevelNodes.nodes.FirstOrDefault(n => n.type == ENodeType.LOCATION && ((LocationNode)n).name.ToLower().Contains(context.Parameters.Get<string>(0).ToLower()));
            if (item != null)
            {
                Vector3 c = item.point + new Vector3(0f, 0.5f, 0f);
                player.Teleport(c, MeasurementTool.angleToByte(player.Rotation));
                chatManager.SendLocalizedMessage(translations, player, "command_tp_teleport_private", ((LocationNode)item).name);
                return;
            }

            chatManager.SendLocalizedMessage(translations, player, "command_tp_failed_find_destination");
        }

        public string Name => "Tp";
        public string Summary => "Teleports you to another player or location.";
        public string Description => null;
        public string Permission => "Rocket.Unturned.Tp";
        public string Syntax => "<player | place | x y z>";
        public ISubCommand[] ChildCommands => null;
        public string[] Aliases => null;
    }
}