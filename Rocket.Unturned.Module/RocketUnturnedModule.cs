using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NuGet.Common;
using SDG.Framework.Modules;
using SDG.Unturned;
using Debug = UnityEngine.Debug;

namespace Rocket.Unturned.Module
{
    public class RocketUnturnedModule : IModuleNexus
    {
        private const string DefaultNugetRepository = "https://api.nuget.org/v3/index.json";

        public void initialize()
        {
            //Thank you Unturned for providing a very old Newtonsoft.Json...we better should load our own one
            LoadAssembly("Newtonsoft.Json.dll");

#if NUGET_BOOTSTRAP
            var assemblyLocation = typeof(RocketUnturnedModule).Assembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            foreach (var file in Directory.GetFiles(assemblyDirectory))
            {
                if (file.EndsWith(".dll"))
                {
                    LoadAssembly(file);
                }
            }

            string rocketDirectory = Path.GetFullPath($"Servers/{Dedicator.serverID}/Rocket/");
            if (!Directory.Exists(rocketDirectory))
            {
                Directory.CreateDirectory(rocketDirectory);
            }

            Debug.Log("Bootstrapping RocketMod for Unturned...");

            var logger = new UnityLoggerAdapter();
            var bootrapper = new RocketDynamicBootstrapper();
            var bootTask = bootrapper.BootstrapAsync(rocketDirectory, "Rocket.Unturned", false, DefaultNugetRepository, logger);
#else
            LoadAssembly("Rocket.Unturned.dll");           
            LoadAssembly("Rocket.UnityEngine.dll");
 
            var runtime = new Runtime();
            var bootTask = runtime.InitAsync();
#endif

            bootTask.GetAwaiter().GetResult();
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

    public class UnityLoggerAdapter : LoggerBase
    {
        public override void Log(ILogMessage message)
        {
            Debug.Log($"[{message.Level}] [NuGet] {message.Message}");
        }

        public override async Task LogAsync(ILogMessage message)
        {
            Log(message);
        }
    }
}
