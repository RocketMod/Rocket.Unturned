using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Commands.Events;
using Rocket.Core.Eventing;
using Rocket.Core.Extensions;
using Rocket.Unturned.Console;
using SDG.Unturned;

namespace Rocket.Unturned.Commands
{
    public class VanillaCommandProvider : ICommandProvider
    {
        public VanillaCommandProvider(IDependencyContainer container, IPlayerManager manager)
        {
            IEventManager eventManager = container.Get<IEventManager>();
            IImplementation impl = container.Get<IImplementation>();
            ICommandHandler cmdHandler = container.Get<ICommandHandler>();

            ChatManager.onCheckPermissions += (SteamPlayer player, string commandLine, ref bool shouldExecuteCommand, ref bool shouldList) =>
            {
                if (commandLine.StartsWith("/"))
                {
                    commandLine = commandLine.Substring(1);
                    var caller = manager.GetOnlinePlayer(player.playerID.steamID.ToString());
                    PreCommandExecutionEvent @event = new PreCommandExecutionEvent(caller, commandLine);
                    eventManager.Emit(impl, @event);

                    cmdHandler.HandleCommand(caller, commandLine, "/");

                    shouldList = false;
                }
                shouldExecuteCommand = false;
            };

            CommandWindow.onCommandWindowInputted += (string commandline, ref bool shouldExecuteCommand) =>
            {
                if (commandline.StartsWith("/"))
                    commandline = commandline.Substring(1);

                PreCommandExecutionEvent @event = new PreCommandExecutionEvent(UnturnedConsoleCaller.Instance, commandline);
                eventManager.Emit(impl, @event);

                cmdHandler.HandleCommand(UnturnedConsoleCaller.Instance, commandline, "");

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