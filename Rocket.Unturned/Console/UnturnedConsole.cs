using System;
using System.IO;
using System.Threading;
using Rocket.Core.Logging;
using SDG.Unturned;
using UnityEngine;
using ILogger = Rocket.API.Logging.ILogger;

namespace Rocket.Unturned.Console
{
    public class UnturnedConsole : MonoBehaviour
    {
        private FileStream fileStream;
        private StreamWriter streamWriter;
        private UnturnedConsoleWriter writer;

        public ILogger Logger { get; set; }

        private void Awake()
        {
            try
            {
                fileStream = new FileStream($"{Dedicator.serverID}.console", 
                    FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8)
                {
                    AutoFlush = true
                };
                writer = new UnturnedConsoleWriter(streamWriter);

                readingThread = new Thread(DoRead);
                readingThread.Start();
            }
            catch (Exception ex)
            {
                Logger.LogError(null, ex);
            }
        }

        private void OnDestroy()
        {
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose();
            }
            if (streamWriter != null)
            {
                streamWriter.Close();
                streamWriter.Dispose();
            }
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
            }
        }

        private static Thread readingThread;

        private void DoRead()
        {
            do
            {
                try
                {
                    string currentLine = System.Console.ReadLine();

                    if (currentLine != null && CommandWindow.input != null && CommandWindow.input.onInputText != null && currentLine.Trim().Length != 0)
                        CommandWindow.input.onInputText(currentLine);
                }
                catch (Exception ex)
                {
                    Logger.LogError(null, ex);
                }
            }
            while (true);
        }
    }
}