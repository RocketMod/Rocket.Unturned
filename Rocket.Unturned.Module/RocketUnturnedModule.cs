using NuGet.Common;
using SDG.Framework.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Harmony;
using Debug = UnityEngine.Debug;

namespace Rocket.Unturned.Module
{
    public class RocketUnturnedModule : IModuleNexus
    {
        private const string HarmonyInstanceId = "net.rocketmod.unturned.module";
        private readonly Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();
        private HarmonyInstance harmonyInstance;

        public void initialize()
        {
            var selfAssembly = typeof(RocketUnturnedModule).Assembly;

            harmonyInstance = HarmonyInstance.Create(HarmonyInstanceId);
            harmonyInstance.PatchAll(selfAssembly);

            InstallNewtonsoftJson();
            InstallTlsWorkaround();
            InstallAssemblyResolver();

            var assemblyLocation = selfAssembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            foreach (var file in Directory.GetFiles(assemblyDirectory))
            {
                if (file.EndsWith(".dll"))
                {
                    LoadAssembly(file);
                }
            }

            RocketInitializer.Initialize();
        }

        private void InstallNewtonsoftJson()
        {
            string managedDir = Path.GetFullPath(Path.Combine("Unturned_Data", "Managed"));
            string rocketDir = Path.GetFullPath(Path.Combine("Modules", "Rocket.Unturned"));

            string unturnedNewtonsoftFile = Path.GetFullPath(Path.Combine(managedDir, "Newtonsoft.Json.dll"));
            string newtonsoftBackupFile = unturnedNewtonsoftFile + ".bak";
            string rocketNewtonsoftFile = Path.GetFullPath(Path.Combine(rocketDir, "Newtonsoft.Json.dll"));

            const string runtimeSerialization = "System.Runtime.Serialization.dll";
            var unturnedRuntimeSerialization = Path.GetFullPath(Path.Combine(managedDir, runtimeSerialization));
            var rocketRuntimeSerialization = Path.GetFullPath(Path.Combine(rocketDir, runtimeSerialization));

            const string xmlLinq = "System.Xml.Linq.dll";
            var unturnedXmlLinq = Path.GetFullPath(Path.Combine(managedDir, xmlLinq));
            var rocketXmlLinq = Path.GetFullPath(Path.Combine(rocketDir, xmlLinq));

            // Copy Libraries of Newtonsoft.Json

            if (!File.Exists(unturnedRuntimeSerialization))
            {
                File.Copy(rocketRuntimeSerialization, unturnedRuntimeSerialization);
            }

            if (!File.Exists(unturnedXmlLinq))
            {
                File.Copy(rocketXmlLinq, unturnedXmlLinq);
            }

            // Copy Newtonsoft.Json
            AssemblyName asm = AssemblyName.GetAssemblyName(unturnedNewtonsoftFile);
            GetVersionIndependentName(asm.FullName, out var version);
            if (version.StartsWith("7.", StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(newtonsoftBackupFile))
                {
                    File.Delete(newtonsoftBackupFile);
                }

                File.Move(unturnedNewtonsoftFile, newtonsoftBackupFile);
                File.Copy(rocketNewtonsoftFile, unturnedNewtonsoftFile);
            }
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
            harmonyInstance.UnpatchAll(HarmonyInstanceId);
        }

        private void InstallTlsWorkaround()
        {
            //http://answers.unity.com/answers/1089592/view.html
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationWorkaroundCallback;
        }

        private bool CertificateValidationWorkaroundCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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

        public void LoadAssembly(string dllName)
        {
            //Load the dll from the same directory as this assembly
            var selfLocation = typeof(RocketUnturnedModule).Assembly.Location;
            var currentPath = Path.GetDirectoryName(selfLocation) ?? "";
            var dllFullPath = Path.GetFullPath(Path.Combine(currentPath, dllName));

            if (string.Equals(selfLocation, dllFullPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var data = File.ReadAllBytes(dllFullPath);
            var asm = Assembly.Load(data);

            var name = GetVersionIndependentName(asm.FullName, out _);

            if (loadedAssemblies.ContainsKey(name))
            {
                return;
            }

            loadedAssemblies.Add(name, asm);
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
