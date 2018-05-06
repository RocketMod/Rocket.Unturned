using SDG.Unturned;
using System;
using Rocket.API;
using Rocket.API.Logging;
using Rocket.API.Scheduler;
using Rocket.Core.Logging;

namespace Rocket.Unturned.Utils
{
    public class AutomaticSaveWatchdog
    {
        private readonly ILogger logger;
        private DateTime? nextSaveTime;
        public static AutomaticSaveWatchdog Instance;
        private int interval = 30;

        public AutomaticSaveWatchdog(IImplementation implementation, ILogger logger, ITaskScheduler scheduler)
        {
            this.logger = logger;
            scheduler.ScheduleEveryAsyncFrame(implementation, CheckTimer);
        }

        public void Start()
        {
            Instance = this;
            bool autoSaveEnabled = true; //U.Settings.Instance.AutomaticSave.Enabled;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once HeuristicUnreachableCode

            if (!autoSaveEnabled)
                return;

            int i = 300; //;U.Settings.Instance.AutomaticSave.Interval;

            if (i < interval)
                logger.LogError("AutomaticSave interval must be atleast 30 seconds, changed to 30 seconds");
            else
                interval = i;

            logger.LogInformation("This server will automatically save every {0} seconds", interval);
            RestartTimer();
        }

        private void RestartTimer()
        {
            nextSaveTime = DateTime.Now.AddSeconds(interval);
        }

        private void CheckTimer()
        {
            if (nextSaveTime == null)
                return;
            if (nextSaveTime.Value >= DateTime.Now)
                return;

            logger.LogInformation("Saving server");
            RestartTimer();
            try
            {
                SaveManager.save();
            }
            catch (Exception er)
            {
                logger.LogError("Error occured while trying to save: ", er);
            }
        }
    }
}