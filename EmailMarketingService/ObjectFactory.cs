using System.Reflection;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using log4net;
    using log4net.Repository.Hierarchy;
    using Tasks;

    /// <summary>
    /// Responsible for creating and building the object graph
    /// </summary>
    internal class ObjectFactory
    {
        /// <summary>log4net logger object</summary>
        private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Used for locking creation of this singleton</summary>
        private static readonly object Padlock = new object();

        /// <summary>The singleton instance of this class</summary>
        private static ObjectFactory _instance;

        /// <summary>Watches the config file for changes</summary>
        private FileSystemWatcher configWatcher;

        /// <summary>Stores the time that the last config file change was detected</summary>
        private DateTime _lastDetectedConfigWriteTime = DateTime.MinValue;

        internal event EventHandler ConfigurationChanged;

        public Settings Settings { get; private set; }

        /// <summary>
        /// Prevents a default instance of the ObjectFactory class from being created
        /// </summary>
        private ObjectFactory()
        {
            var initialiser = new SettingsInitialiser();
            Settings = initialiser.Initialise();

            configWatcher = new FileSystemWatcher(
                Path.GetDirectoryName(Settings.LoadedFromFilename),
                Path.GetFileName(Settings.LoadedFromFilename));
            configWatcher.Changed += new FileSystemEventHandler(ConfigWatcherChanged);
            configWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Create a ScheduledTaskRunner object with its task list ready to run
        /// </summary>
        /// <returns>The populated ScheduledTaskRunner</returns>
        internal ScheduledTaskRunner GetScheduledTaskRunner()
        {
            return new ScheduledTaskRunner(GetTasks(), Settings.SchedulerIntervalMillisecs);
        }

        internal void RefreshTaskSchedules(List<IScheduledTask> tasks)
        {
            foreach (var task in tasks)
            {
                switch (task.TaskName.ToUpperInvariant())
                {
                    case "SYNCTOEMAILSERVICE":
                        task.Interval = new TimeSpan(0, Settings.SyncToEmailServiceFrequencyMinutes, 0);
                        break;

                    case "SALESLOGIXUSERTASKS":
                        task.Interval = new TimeSpan(0, 0, Settings.CheckSlxTasksFrequencySeconds);
                        break;

                    case "SYNCTOSALESLOGIXCAMPAIGNS":
                        task.Interval = new TimeSpan(0, Settings.SyncToSlxCampaignsFrequencyMinutes, 0);
                        break;

                    case "SCANFORDELETEDITEMS":
                        task.Interval = new TimeSpan(0, Settings.ScanDeletedItemsFrequencyMinutes, 0);
                        break;
                }
            }
        }

        /// <summary>Gets a singleton instance of this class</summary>
        public static ObjectFactory Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                        _instance = new ObjectFactory();

                    return _instance;
                }
            }
        }

        public ISlxConnector GetSlxConnector()
        {
            var sdata = new SlxSdata(Settings.SdataUrl, Settings.SdataUsername, Settings.SdataPassword);
            var slx = new SlxConnector(sdata);
            return slx;
        }

        public IDotMailerConnector GetDotMailerConnector()
        {
            return new DotMailerConnector();
        }

        protected virtual void OnConfigurationChanged(object sender, EventArgs e)
        {
            EventHandler handler = ConfigurationChanged;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Get the list of task that we want to run
        /// </summary>
        /// <returns>The list of tasks</returns>
        private List<IScheduledTask> GetTasks()
        {
            var tasks = new List<IScheduledTask>();

            IScheduledTask task = new ScheduledTask(
                "SyncToEmailService", new SyncToEmailServiceRunnable(),
                new TimeSpan(0, Settings.SyncToEmailServiceFrequencyMinutes, 0), //In the configuration tool this is "Synchronise to Email Service every... minutes" and is the periodic synchronisation of all email accounts and their various details.
                DateTime.MinValue);
            tasks.Add(task);

            task = new ScheduledTask(
                "SalesLogixUserTasks", new SalesLogixUserTasksRunnable(),
                new TimeSpan(0, 0, Settings.CheckSlxTasksFrequencySeconds),
                DateTime.MinValue);
            tasks.Add(task);

            task = new ScheduledTask(
                "SyncToSalesLogixCampaigns", new SyncToSlxCampaignsRunnable(),
                new TimeSpan(0, Settings.SyncToSlxCampaignsFrequencyMinutes, 0),
                DateTime.MinValue);
            tasks.Add(task);

            task = new ScheduledTask(
                "ScanForDeletedItems", new SalesLogixDeletionScanRunnable(),
                new TimeSpan(0, Settings.ScanDeletedItemsFrequencyMinutes, 0),
                DateTime.MinValue);
            tasks.Add(task);

            return tasks;
        }

        private void ConfigWatcherChanged(object sender, FileSystemEventArgs e)
        {
            _logger.DebugFormat(
                "Config file change detected. LastDetectedConfigWriteTime: {0}",
                _lastDetectedConfigWriteTime);
            DateTime configFileLastWriteTime = File.GetLastWriteTime(Settings.LoadedFromFilename);
            if (configFileLastWriteTime > _lastDetectedConfigWriteTime)
            {
                _logger.Info("Config File Changed");
                _lastDetectedConfigWriteTime = configFileLastWriteTime;
                var initialiser = new SettingsInitialiser();
                Settings = initialiser.Initialise();

                // Reload logger
                var repo = (Hierarchy)LogManager.GetRepository();
                repo.Root.RemoveAllAppenders();
                LoggingHelper.ConfigureLogging(
                    Settings.LoggingMethod,
                    Settings.LoggingLevel,
                    Path.Combine(Instance.Settings.LogFileDirectory, "EmailMarketingServiceLog.txt"));
                _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

                OnConfigurationChanged(this, new EventArgs());
            }
            else
            {
                _logger.Debug("Duplicate change event.  Config file change ignored.  Not enough time passed since last change.");
            }
        }
    }
}