using System;
using System.IO;
using System.Threading;
using UnityEngine;
using Rocket.Core.Logging;
using SDG.Unturned;

namespace Rocket.Unturned
{
    public class UnturnedConsole : MonoBehaviour
    {
        FileStream fileStream = null;
        private void Awake()
        {
            try
            {
                fileStream = new FileStream(String.Format(Environment.ConsoleFile, Dedicator.serverID), FileMode.Create,FileAccess.Write,FileShare.ReadWrite);

                StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.ASCII)
                {
                    AutoFlush = true
                };


                System.Console.SetOut(streamWriter);

                readingThread = new Thread(new ThreadStart(DoRead));
                readingThread.Start();
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.Log("Error: " + ex.ToString());
            }
        }

        private void Destroy() {
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        private static Thread readingThread;

        private static void DoRead()
        {
            string x;
            do
            {
                try
                {
                x = System.Console.ReadLine();
                 
                if (x != null && CommandWindow.input != null && CommandWindow.input.onInputText != null && x.Trim().Length != 0) CommandWindow.input.onInputText(x);

                }
                catch (Exception ex)
                {
                    Core.Logging.Logger.LogException(ex);
                }
                }
            while (true);
        }
    }
}
