using Logger = Rocket.API.Logging.Logger;
using SDG.Unturned;
using System;
using UnityEngine;

namespace Rocket.Unturned.Utils
{
    internal class AutomaticSaveWatchdog : MonoBehaviour
    {
        private void FixedUpdate()
        {
            checkTimer();
        }

        private DateTime? nextSaveTime = null;
        public static AutomaticSaveWatchdog Instance;
        private int interval = 30;

        private void Start()
        {
            Instance = this;
            if (U.Instance.Settings.Instance.AutomaticSave.Enabled)
            {
                if (U.Instance.Settings.Instance.AutomaticSave.Interval < interval)
                {
                    Logger.Error("AutomaticSave interval must be atleast 30 seconds, changed to 30 seconds");
                }
                else
                {
                    interval = U.Instance.Settings.Instance.AutomaticSave.Interval;
                }
                Logger.Info(String.Format("This server will automatically save every {0} seconds", interval));
                restartTimer();
            }
        }

        private void restartTimer()
        {
            nextSaveTime = DateTime.Now.AddSeconds(interval);
        }

        private void checkTimer()
        {
            try
            {
                if (nextSaveTime != null)
                {
                    if (nextSaveTime.Value < DateTime.Now)
                    {
                        Logger.Info("Saving server");
                        restartTimer();
                        SaveManager.save();
                    }
                }
            }
            catch (Exception er)
            {
                Logger.Fatal(er);
            }
        }
    }
}