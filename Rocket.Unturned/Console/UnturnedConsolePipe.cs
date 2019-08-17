using System;
using System.IO;
using System.Threading;
using Rocket.Core.Logging;
using SDG.Unturned;
using UnityEngine;
using ILogger = Rocket.API.Logging.ILogger;

namespace Rocket.Unturned.Console
{
    public class UnturnedConsolePipe : MonoBehaviour
    {
        public ILogger Logger { get; set; }
    }
}
