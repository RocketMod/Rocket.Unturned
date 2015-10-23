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
                fileStream = new FileStream(String.Format(Environment.ConsoleFile, U.Instance.GetInstanceID()), FileMode.Create,FileAccess.Write,FileShare.ReadWrite);

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
                Logger.Log("Error: " + ex.ToString());
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
                 
                if (x != null && CommandWindow.ConsoleInput != null && CommandWindow.ConsoleInput.onInputText != null && x.Trim().Length != 0) CommandWindow.ConsoleInput.onInputText(x);

                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                }
            while (true);
        }
    }
}
