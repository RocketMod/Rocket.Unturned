using System;
using System.IO;
using System.Reflection;
using Rocket.API;
using Rocket.API.Plugins;
using SDG.Framework.Modules;

namespace Rocket.Unturned
{
    public class RocketUnturnedModule : IModuleNexus
    {
        public void initialize()
        {
            //Force loading Rocket.UnityEngine.dll as just adding reference wont load it (since no code is referenced)
            LoadAssembly("Rocket.UnityEngine.dll");

            //Thank you Unturned for providing a very old Newtonsoft.Json...we better should load our own one
            LoadAssembly("Newtonsoft.Json.dll");

            System.Console.WriteLine("Initialzing RocketMod...");

            var runtime = new Runtime();
            runtime.BootstrapAsync().GetAwaiter().GetResult();
        }

        private void LoadAssembly(string dllName)
        {
            //Load the dll from the same directory as this assembly
            var currentPath = Path.GetDirectoryName(typeof(RocketUnturnedModule).Assembly.Location) ?? "";
            Assembly.LoadFrom(Path.Combine(currentPath, dllName));
        }

        public void shutdown()
        {
        }
    }
}