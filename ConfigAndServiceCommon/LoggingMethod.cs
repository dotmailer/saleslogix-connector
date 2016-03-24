namespace EmailMarketing.SalesLogix
{
    using System;

    public enum LoggingMethod
    {
        WindowsEventLog,
        DiskFile,
        Advanced
    }

    public static class LoggingMethodExtensions
    {
        public static LoggingMethod Parse(this LoggingMethod val, string valueToParse)
        {
            if (valueToParse == null)
            {
                throw new ArgumentNullException("valueToParse");
            }
            else if (string.Equals(valueToParse, SettingsInitialiser.LoggingMethodWindowsEventLog, StringComparison.OrdinalIgnoreCase))
            {
                return LoggingMethod.WindowsEventLog;
            }
            else if (string.Equals(valueToParse, SettingsInitialiser.LoggingMethodAdvanced, StringComparison.OrdinalIgnoreCase))
            {
                return LoggingMethod.Advanced;
            }
            else if (string.Equals(valueToParse, SettingsInitialiser.LoggingMethodDiskFile, StringComparison.OrdinalIgnoreCase))
            {
                return LoggingMethod.DiskFile;
            }
            else
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid value for LoggingMethod", valueToParse), "valueToParse");
            }
        }
    }
}