using System.Collections.Generic;
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

            bootrapper.Bootstrap(rocketDirectory,
                new List<string> { "Rocket.Unturned" },
                false,
                RocketDynamicBootstrapper.DefaultNugetRepository,
                logger);

#else
            var runtime = new Runtime();
            runtime.InitAsync().GetAwaiter().GetResult();
#endif
        }
    }
}