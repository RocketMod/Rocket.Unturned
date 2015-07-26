using Rocket.Core.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Rocket.Unturned.Commands;
using Rocket.API;

namespace Rocket.Unturned.Plugins
{
    public sealed class PluginCommandManager : MonoBehaviour
    {
        private Assembly assembly;
        private void OnEnable()
        {
            IRocketPlugin plugin = GetComponent<IRocketPlugin>();
            assembly = plugin.GetType().Assembly;
            RegisterFromAssembly(assembly);
        }

        public static void RegisterFromAssembly(Assembly assembly)
        {
            List<Type> commands = RocketHelper.GetTypesFromInterface(assembly, "IRocketCommand");
            foreach (Type command in commands)
            {
                IRocketCommand rocketCommand = (IRocketCommand)Activator.CreateInstance(command);
                Register((Command)(new UnturnedCommandBase(rocketCommand)));
                foreach (string alias in rocketCommand.Aliases)
                {
                    Register((Command)(new UnturnedAliasBase(rocketCommand, alias)));
                }
            }
        }

        private void OnDisable()
        {
            UnregisterFromAssembly(assembly);
        }

        public static void UnregisterFromAssembly(Assembly assembly)
        {
            foreach (Command c in Commander.Commands.Where(c => (c is UnturnedCommandBase && ((UnturnedCommandBase)c).Command.GetType().Assembly == assembly)).ToList())
            {
                Commander.deregister(c);
            }
        }

        public static void Register(Command command)
        {
            string assemblyName = command.GetType().Assembly.GetName().Name;
            List<Command> existingCommand = Commander.Commands.Where(c => (String.Compare(c.commandName, command.commandName, true) == 0)).ToList();
            Commander.register(command);
        }
    }
}