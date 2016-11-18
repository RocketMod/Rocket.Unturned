using SDG.Framework.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rocket.Unturned
{
    public class RocketNexus : IModuleNexus
    {
        public void initialize()
        {
            Dictionary<string, string> dependencies = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries = new DirectoryInfo("./Modules/Rocket.Unturned").GetFiles("*.dll", SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    Assembly.LoadFile(name.FullName);
                }
                catch { }
            }

            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                Console.WriteLine("Resolving "+args.Name);
                string file;
                if (dependencies.TryGetValue(args.Name, out file))
                {
                    return Assembly.Load(File.ReadAllBytes(file));
                }
                else
                {
                    Console.Write("Could not find dependency: " + args.Name);
                }
                return null;
            };
            try
            {

           var endpoint = new Uri(String.Format("http://localhost:{0}/", 1212));
           var serviceHost = new System.ServiceModel.ServiceHost(typeof(Core.RPC.RocketService), endpoint);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            U.Initialize();
        }

        public void shutdown()
        {
            //
        }
    }
}
