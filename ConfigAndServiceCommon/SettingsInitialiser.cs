using System.Reflection;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.IO;
    using QGate.Components.Serialization;

    /// <summary>
    /// Initialises a LocalSettings object using a file
    /// </summary>
    public class SettingsInitialiser
    {
        public static readonly string ServerConfigFilename = "EmailMarketingServiceConfig.xml";
        public static readonly string ConfigSectionTitle = "EmailMarketingService";
        public static readonly int DefaultCheckSlxTasksFrequencySeconds = 60;
        public static readonly int DefaultScanDeletedItemsFrequencyMinutes = 60;
        public static readonly int DefaultSyncToEmailServiceFrequencyMinutes = 60;
        public static readonly int DefaultSyncToSlxCampaignsFrequencyMinutes = 5;
        public static readonly bool DefaultScanDeletedItemsEnabled = true;
        public static readonly bool DefaultBatchUpdateSaleslogixDataFieldsSetting = false; //We have tested using batches of 100 contacts, and it seemed to be consistently slower (3m30s vs. 5m)
        public static readonly bool DefaultSyncToEmailServiceRunnableEnabledSetting = true;
        public static readonly string LogFileDirectoryName = "logs";
        public const string LoggingMethodDiskFile = "DiskFile";
        public const string LoggingMethodAdvanced = "Advanced";
        public const string LoggingMethodWindowsEventLog = "WindowsEventLog";

        /// <summary>
        /// Initialise the a settings object from file
        /// </summary>
        /// <returns>An initialised settings object</returns>
        public Settings Initialise()
        {
            string filename = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                ServerConfigFilename);

            ConfigurationFile configFile = new ConfigurationFile(filename, ConfigSectionTitle);
            configFile.Load();

            int schedulerInterval = configFile.GetProperty<int>("SchedulerIntervalMillisecs", 1000);

            // SData
            string sdataUrl = configFile.GetProperty<string>("SDataUrl", "http://server:3333/sdata");
            var helper = new SdataHelper();
            sdataUrl = helper.AppendRequiredUrlSegments(sdataUrl);
            string sdataUsername = configFile.GetProperty<string>("SDataUsername", string.Empty);
            string sdataPassword = configFile.GetProperty<string>("SDataPassword", string.Empty);

            // Schedule
            int checkSlxTasksSeconds = configFile.GetProperty<int>("CheckSlxTasksFrequencySeconds", DefaultCheckSlxTasksFrequencySeconds);
            int syncEmailServiceMinutes = configFile.GetProperty<int>("SyncToEmailServiceFrequencyMinutes", DefaultSyncToEmailServiceFrequencyMinutes);
            int syncSlxCampaignsMinutes = configFile.GetProperty<int>("SyncToSlxCampaignsFrequencyMinutes", DefaultSyncToSlxCampaignsFrequencyMinutes);
            bool enableDeletedItemsScan = configFile.GetProperty<bool>("EnableDeletedItemsScan", DefaultScanDeletedItemsEnabled);
            int scanDeletedItemsMinutes = configFile.GetProperty<int>("ScanDeletedItemsFrequencyMinutes", DefaultScanDeletedItemsFrequencyMinutes);

            // Logging
            string loggingLevelString = configFile.GetProperty<string>("LoggingLevel", "Warning");
            LoggingLevel loggingLevel = default(LoggingLevel);
            loggingLevel = loggingLevel.Parse(loggingLevelString);

            string loggingMethodString = configFile.GetProperty<string>("LoggingMethod", LoggingMethodDiskFile);
            LoggingMethod loggingMethod = default(LoggingMethod);
            loggingMethod = loggingMethod.Parse(loggingMethodString);

            string defaultLogFileDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            defaultLogFileDirectory = Path.Combine(defaultLogFileDirectory, LogFileDirectoryName);
            string logFileDirectory = configFile.GetProperty<string>("LogFileDirectory", defaultLogFileDirectory);

            //Performance tuning
            bool batchUpdateSaleslogixDataFields = configFile.GetProperty<bool>("BatchUpdateSaleslogixDataFields", DefaultBatchUpdateSaleslogixDataFieldsSetting);

            //Developer options
            bool syncToEmailServiceRunnableEnabled = configFile.GetProperty<bool>("SyncToEmailServiceRunnableEnabled", DefaultSyncToEmailServiceRunnableEnabledSetting);

            return new Settings(
                filename,
                DateTime.UtcNow,
                schedulerInterval,
                sdataUrl,
                sdataUsername,
                sdataPassword,
                checkSlxTasksSeconds,
                syncEmailServiceMinutes,
                syncSlxCampaignsMinutes,
                enableDeletedItemsScan,
                scanDeletedItemsMinutes,
                loggingLevel,
                loggingMethod,
                logFileDirectory,
                batchUpdateSaleslogixDataFields,
                syncToEmailServiceRunnableEnabled);
        }
    }
}