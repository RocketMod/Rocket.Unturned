using System;
using System.Collections.Generic;
using System.IO;
using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;
using Rocket.API.Logging;
using Rocket.Unturned.Utils;
using SDG.Unturned;

namespace Rocket.Unturned
{
    public class UnturnedImplementation : IImplementation, IEventEmitter
    {
        public bool IsAlive => true;

        public void Load(IRuntime runtime)
        {
            IDependencyContainer container = runtime.Container;

            ILogger logger = container.Get<ILogger>();
            logger.LogInformation("Loading Rocket Unturned Implementation...");
            container.RegisterSingletonType<AutomaticSaveWatchdog, AutomaticSaveWatchdog>();
            container.Get<AutomaticSaveWatchdog>().Start();

            string rocketDirectory = $"Servers/{Dedicator.serverID}/Rocket/";
            if (!Directory.Exists(rocketDirectory))
                Directory.CreateDirectory(rocketDirectory);

            Directory.SetCurrentDirectory(rocketDirectory);
        }

        public void Shutdown()
        {
            Provider.shutdown();
        }

        public void Reload() { }

        public IEnumerable<string> Capabilities => new List<string>();
        public string InstanceId => Provider.serverID;
        public string WorkingDirectory => Environment.CurrentDirectory;
        public string Name => "Rocket.Unturned";
    }
}