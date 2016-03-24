namespace EmailMarketing.SalesLogix
{
    using System;

    /// <summary>
    /// Holds settings for the Email Marketing Service.
    /// This class is immutable so that a thread can keep a reference to an object without worrying about it changing.
    /// </summary>
    public class Settings
    {
        public string LoadedFromFilename { get; private set; }
        public DateTime LoadedAtUtc { get; private set; }
        public int SchedulerIntervalMillisecs { get; private set; }
        public string SdataUrl { get; private set; }
        public string SdataUsername { get; private set; }
        public string SdataPassword { get; private set; }
        public int CheckSlxTasksFrequencySeconds { get; private set; }
        public int SyncToEmailServiceFrequencyMinutes { get; private set; }
        public int SyncToSlxCampaignsFrequencyMinutes { get; private set; }
        public bool EnableDeletedItemsScan { get; private set; }
        public int ScanDeletedItemsFrequencyMinutes { get; private set; }
        public LoggingLevel LoggingLevel { get; private set; }
        public LoggingMethod LoggingMethod { get; private set; }
        public string LogFileDirectory { get; private set; }
        public bool BatchUpdateSaleslogixDataFields { get; private set; }
        public bool SyncToEmailServiceRunnableEnabled { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Settings class.
        /// </summary>
        public Settings(
            string loadedFromFilename,
            DateTime loadedAtUtc,
            int schedulerInterval,
            string sdataUrl,
            string sdataUsername,
            string sdataPassword,
            int checkSlxTasksFrequencySeconds,
            int syncToEmailServiceFrequencyMinutes,
            int syncToSlxCampaignsFrequencyMinutes,
            bool enableDeletedItemsScan,
            int scanDeletedItemsFrequencyMinutes,
            LoggingLevel loggingLevel,
            LoggingMethod loggingMethod,
            string logFileDirectory,
            bool batchUpdateSaleslogixDataFields,
            bool syncToEmailServiceRunnableEnabled)
        {
            LoadedFromFilename = loadedFromFilename;
            LoadedAtUtc = loadedAtUtc;
            SchedulerIntervalMillisecs = schedulerInterval;
            SdataUrl = sdataUrl;
            SdataUsername = sdataUsername;
            SdataPassword = sdataPassword;
            CheckSlxTasksFrequencySeconds = checkSlxTasksFrequencySeconds;
            SyncToEmailServiceFrequencyMinutes = syncToEmailServiceFrequencyMinutes;
            SyncToSlxCampaignsFrequencyMinutes = syncToSlxCampaignsFrequencyMinutes;
            EnableDeletedItemsScan = enableDeletedItemsScan;
            ScanDeletedItemsFrequencyMinutes = scanDeletedItemsFrequencyMinutes;
            LoggingLevel = loggingLevel;
            LoggingMethod = loggingMethod;
            LogFileDirectory = logFileDirectory;
            BatchUpdateSaleslogixDataFields = batchUpdateSaleslogixDataFields;
            SyncToEmailServiceRunnableEnabled = syncToEmailServiceRunnableEnabled;
        }
    }
}