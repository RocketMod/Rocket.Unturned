using System.IO;
using System.Reflection;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace Rocket.Unturned.Module
{
    public class RocketUnturnedModule : IModuleNexus
    {
        public void initialize()
        {
            //Thank you Unturned for providing a very old Newtonsoft.Json...we better should load our own one
            LoadAssembly("Newtonsoft.Json.dll");

#if NUGET_BOOTSTRAP
            string rocketDirectory = Path.GetFullPath($"Servers/{Dedicator.serverID}/Rocket/");
            if (!Directory.Exists(rocketDirectory))
            {
                Directory.CreateDirectory(rocketDirectory);
            }

            var bootrapper = new RocketDynamicBootstrapper();
            var bootTask = bootrapper.BootstrapAsync(rocketDirectory, "Rocket.Unturned");
#else
            LoadAssembly("Rocket.Unturned.dll");           
            LoadAssembly("Rocket.UnityEngine.dll");
 
            var runtime = new Runtime();
            var bootTask = runtime.InitAsync();
#endif

            bootTask.RunSynchronously();
        }

        public void shutdown()
        {
        }

        private void LoadAssembly(string dllName)
        {
            //Load the dll from the same directory as this assembly
            var currentPath = Path.GetDirectoryName(typeof(RocketUnturnedModule).Assembly.Location) ?? "";
            Assembly.LoadFrom(Path.Combine(currentPath, dllName));
        }
    }
}
