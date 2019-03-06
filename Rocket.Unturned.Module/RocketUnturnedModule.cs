using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using SDG.Framework.Modules;
using SDG.Unturned;
using Debug = UnityEngine.Debug;

namespace Rocket.Unturned.Module
{
    public class RocketUnturnedModule : IModuleNexus
    {
        private Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();
        public void initialize()
        {
            InstallTlsWorkaround();
            InstallAssemblyResolver();

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

            var task = Task.Run(async () =>
            {
                await Task.Yield();
                await bootrapper.BootstrapAsync(rocketDirectory, "Rocket.Unturned", false,
                        RocketDynamicBootstrapper.DefaultNugetRepository, logger);
            });
            task.GetAwaiter().GetResult();

#else
            //Thank you Unturned for providing a very old Newtonsoft.Json...we better should load our own one
            LoadAssembly("Newtonsoft.Json.dll");

            LoadAssembly("Rocket.Unturned.dll");           
            LoadAssembly("Rocket.UnityEngine.dll");
 
            var runtime = new Runtime();
            runtime.InitAsync().GetAwaiter().GetResult();
#endif
        }

        private void InstallAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
            {
                var name = GetVersionIndependentName(args.Name, out _);

                if (loadedAssemblies.ContainsKey(name))
                {
                    return loadedAssemblies[name];
                }

                return null;
            };
        }

        public void shutdown()
        {
        }


        private void InstallTlsWorkaround()
        {
            //http://answers.unity.com/answers/1089592/view.html
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationWorkaroundCallback;
        }

        public bool CertificateValidationWorkaroundCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                {
                    if (chainStatus.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }


        private static readonly Regex versionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);
        protected static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = versionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return versionRegex.Replace(fullAssemblyName, "");
        }

        private void LoadAssembly(string dllName)
        {
            //Load the dll from the same directory as this assembly

            var selfLocation = typeof(RocketUnturnedModule).Assembly.Location;
            var currentPath = Path.GetDirectoryName(selfLocation) ?? "";
            var dllFullPath = Path.GetFullPath(Path.Combine(currentPath, dllName));

            Debug.Log(selfLocation);
            Debug.Log(dllFullPath);

            if (string.Equals(selfLocation, dllFullPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var data = File.ReadAllBytes(dllFullPath);
            var asm = Assembly.Load(data);

            loadedAssemblies.Add(GetVersionIndependentName(asm.FullName, out _), asm);
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
