using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Unturned.Commands
{
    public class CommandHelp : IRocketCommand
    {
        public bool AllowFromConsole
        {
            get { return true; }
        }

        public string Name
        {
            get { return "help"; }
        }

        public string Help
        {
            get { return "Shows you a specific help";}
        }

        public string Syntax
        {
            get { return "[command]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
                get { return new List<string>() { "rocket.help" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("[Vanilla]");
                System.Console.ForegroundColor = ConsoleColor.White;
                Commander.Commands.Where(c => c.GetType().Assembly == typeof(Commander).Assembly).OrderBy(c => c.commandName).All(c => { System.Console.WriteLine(c.commandName.ToLower().PadRight(20, ' ') + " " + c.commandInfo.Replace(c.commandName, "").TrimStart().ToLower()); return true; });

                System.Console.WriteLine();

                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("[Rocket]");
                System.Console.ForegroundColor = ConsoleColor.White;
                Commander.Commands.Where(c => c is UnturnedCommandBase && ((UnturnedCommandBase)c).Command.GetType().Assembly == Assembly.GetExecutingAssembly()).OrderBy(c => c.commandName).All(c => { System.Console.WriteLine(c.commandName.ToLower().PadRight(20, ' ') + " " + c.commandInfo.ToLower()); return true; });

                System.Console.WriteLine();
                
                foreach (IRocketPlugin plugin in R.Plugins.GetPlugins())
                {
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    System.Console.WriteLine("[" + plugin.GetType().Assembly.GetName().Name + "]");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    Commander.Commands.Where(c => c is UnturnedCommandBase && ((UnturnedCommandBase)c).Command.GetType().Assembly == plugin.GetType().Assembly).OrderBy(c => c.commandName).All(c => { System.Console.WriteLine(c.commandName.ToLower().PadRight(20, ' ') + " " + c.commandInfo.ToLower()); return true; });
                    System.Console.WriteLine();
                }
            }
            else
            {
                Command cmd = Commander.Commands.Where(c => (String.Compare(c.commandName, command[0], true) == 0)).FirstOrDefault();
                if (cmd != null)
                {
                    string commandName = cmd.commandName;
                    if (cmd is UnturnedCommandBase)
                    {
                        commandName = ((UnturnedCommandBase)cmd).Command.GetType().Assembly.GetName().Name + " / " + cmd.commandName;
                    }
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    System.Console.WriteLine("[" + commandName + "]");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine(cmd.commandName + "\t\t" + cmd.commandInfo);
                    System.Console.WriteLine(cmd.commandHelp);
                }
            }
        }
    }
}