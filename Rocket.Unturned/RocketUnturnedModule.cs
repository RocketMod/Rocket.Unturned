using System.IO;
using System.Reflection;
using Rocket.API;
using Rocket.API.Plugin;
using Rocket.UnityEngine.Scheduling;
using SDG.Framework.Modules;

namespace Rocket.Unturned
{
    public class RocketUnturnedModule : IModuleNexus
    {
        private IRuntime runtime;

        public void initialize()
        {
            //ghetto way of force loading Rocket.UnityEngine.dll (don'T remove this or the dll won't be loaded for DI!)
            typeof(UnityTaskScheduler).ToString();

            //thank you unturned for providing a very old Newtonsoft.Json
            Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(typeof(RocketUnturnedModule).Assembly.Location), 
                "Newtonsoft.Json.dll"));

            System.Console.WriteLine("Initialzing Rocket...");
            runtime = Runtime.Bootstrap();
        }

        public void shutdown()
        {
            foreach (var provider in runtime.Container.GetAll<IPluginManager>())
                foreach (var plugin in provider)
                    plugin.Unload();
        }
    }
}