using SDG.Framework.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Rocket.Core;

namespace Rocket.Unturned
{
    public class RocketNexus : IModuleNexus
    {
        public void initialize() {
            Dictionary<string, string> dependencies = new Dictionary<string, string>();
            foreach (FileInfo library in new DirectoryInfo("./Modules/Rocket.Unturned").GetFiles("*.dll", SearchOption.AllDirectories))
            {
                try {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    Assembly.LoadFile(name.FullName);
                }
                catch {}
            }

            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args) {
                Console.WriteLine("Resolving " + args.Name);
                string file;
                if (dependencies.TryGetValue(args.Name, out file)) {
                    return Assembly.Load(File.ReadAllBytes(file));
                }
                else {
                    Console.Write("Could not find dependency: " + args.Name);
                }
                return null;
            };

            R.Bootstrap<U>();
        }

        public void shutdown() {
            R.Shutdown();
        }
    }
}
