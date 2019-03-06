using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Module
{
    public class RocketInitializer
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Initialize()
        {
#if NUGET_BOOTSTRAP
            string rocketDirectory = Path.GetFullPath($"Servers/{Dedicator.serverID}/Rocket/");
            if (!Directory.Exists(rocketDirectory))
            {
                Directory.CreateDirectory(rocketDirectory);
            }

            Debug.Log("Bootstrapping RocketMod for Unturned, this might take a while...");

            var logger = new UnityLoggerAdapter();
            var bootrapper = new RocketDynamicBootstrapper();

            var task = Task.Run(async () =>
            {
                await Task.Yield();
                await bootrapper.BootstrapAsync(rocketDirectory, 
                    "Rocket.Unturned", 
                    false,
                    RocketDynamicBootstrapper.DefaultNugetRepository, 
                    logger);
            });
            task.GetAwaiter().GetResult();

#else
            var runtime = new Runtime();
            runtime.InitAsync().GetAwaiter().GetResult();
#endif
        }
    }
}