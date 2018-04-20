using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Core.Extensions;
using Rocket.Unturned.Console;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    public class VanillaCommandProvider : ICommandProvider
    {
        public VanillaCommandProvider(IDependencyContainer container, IPlayerManager manager)
        {
            ChatManager.onCheckPermissions += (SteamPlayer player, string commandLine, ref bool shouldExecuteCommand, ref bool shouldList) =>
            {
                //todo: PlayerPreCommandEvent
                if (commandLine.StartsWith("/"))
                {
                    commandLine = commandLine.Substring(1);
                    var caller = manager.GetOnlinePlayer(player.playerID.steamID.ToString());

                    container.Get<ICommandHandler>().HandleCommand(caller, commandLine, "/");

                    shouldList = false;
                }
                shouldExecuteCommand = false;
            };

            CommandWindow.onCommandWindowInputted += (string commandline, ref bool shouldExecuteCommand) =>
            {
                //PlayerPreCommandEvent @event = new PlayerPreCommandEvent(ConsoleCaller.Instance, );
                if (commandline.StartsWith("/"))
                    commandline = commandline.Substring(1);

                container.Get<ICommandHandler>().HandleCommand(ConsoleCaller.Instance, commandline, "");

                shouldExecuteCommand = false;
            };

            var commands = new List<ICommand>();

            foreach (var cmd in Commander.commands)
                commands.Add(new VanillaCommand(cmd));

            Commands = commands;
        }
        public IEnumerable<ICommand> Commands { get; }
    }
}