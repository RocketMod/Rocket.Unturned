using System;
using System.Diagnostics;
using System.IO;

namespace Rocket.Unturned.Launcher
{
    class RocketLauncher
    {
        private static TextReader consoleReader;

        public static void Main(string[] args)
        {
            string instanceName = args.Length > 0 ? args[0] : "Rocket";

            string executableName = "";
            string myAppPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            foreach (string s in new string[] { "Unturned_Headless.x86" , "Unturned.x86" , "Unturned.exe"})
            {
                if (File.Exists(Path.Combine(myAppPath,s))) {
                    executableName = s;
                    break;
                }
            }
            if (String.IsNullOrEmpty(executableName)) throw new FileNotFoundException("Could not locate Unturned executable");
            string arguments = "-nographics -batchmode -logfile 'Servers/" + instanceName + "/unturned.log' +secureserver/" + instanceName;

            string consoleOutput = instanceName + ".console";

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(".", consoleOutput);
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;
            consoleReader = new StreamReader(new FileStream(consoleOutput, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite));
            fileSystemWatcher.EnableRaisingEvents = true;
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(executableName, arguments);
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
        }

        private static void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                string newline = consoleReader.ReadToEnd();
                if (!String.IsNullOrEmpty(newline))
                    Console.Write(newline);
                
            }
        }
    }
}

