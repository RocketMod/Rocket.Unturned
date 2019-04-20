using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Rocket.Unturned.Launcher
{
    internal class RocketLauncher
    {
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        private static TextReader consoleReader;
        private static string instanceName;

        public static void Main(string[] args)
        {
            instanceName = args.Length > 0 ? args[0] : "Rocket";

            string executableName = "";
            if (args.Length > 1)
            {
                executableName = args[1];
            }
            else
            {
                foreach (string s in new[] { "Unturned_Headless.x86_64", "Unturned_Headless.x86", "Unturned.x86_64", "Unturned.x86", "Unturned.exe" })
                    if (File.Exists(s))
                    {
                        executableName = s;
                        break;
                    }
            }

            if (string.IsNullOrEmpty(executableName))
                throw new FileNotFoundException("Could not locate Unturned executable");


            string arguments = "-nographics -batchmode -silent-crashes -logfile 'Servers/"
                + instanceName
                + "/unturned.log' +secureserver/"
                + instanceName;

            var startInfo = new ProcessStartInfo(executableName, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = false,
                UseShellExecute = false,
            };

            if (IsLinux)
            {
                var currentDir = Path.GetFullPath(Environment.CurrentDirectory);
                var lib64Dir = Path.Combine(currentDir, "lib64");

                startInfo.Environment.Add("LD_LIBRARY_PATH", lib64Dir);
            }

            string consoleOutput = instanceName + ".console";

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(".", consoleOutput);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;
            consoleReader = new StreamReader(new FileStream(consoleOutput, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite));
            fileSystemWatcher.EnableRaisingEvents = true;

            Console.WriteLine($@"Starting Unturned via: {executableName} {arguments}");
            Process p = new Process
            {
                StartInfo = startInfo
            };

            p.Start();
            p.WaitForExit();
        }

        private static void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Path.GetFileName(e.FullPath).Equals(instanceName + ".console", StringComparison.OrdinalIgnoreCase))
                return;

            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            string newline = consoleReader.ReadToEnd();
            if (!string.IsNullOrEmpty(newline))
                Console.Write(newline);
        }
    }
}
