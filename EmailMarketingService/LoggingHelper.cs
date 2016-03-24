using System.IO;
using System.Reflection;

namespace EmailMarketing.SalesLogix
{
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    /// <summary>
    /// Various logging helper functions
    /// </summary>
    internal static class LoggingHelper
    {
        /// <summary>The formatting pattern to use for messages</summary>
        private const string LOGPATTERN = "%utcdate [%thread] %-5level %logger [%property{NDC}] - %message%newline";

        /// <summary>
        /// Configure message logging
        /// </summary>
        /// <param name="logType">The type of destination to log to</param>
        /// <param name="logLevel">The logging severity level</param>
        /// <param name="logFilePathAndName">The filename to use if logging to file</param>
        internal static void ConfigureLogging(
            LoggingMethod logType,
            LoggingLevel logLevel,
            string logFilePathAndName)
        {
            switch (logType)
            {
                case LoggingMethod.DiskFile:
                    ConfigureLoggingToFile(
                        logFilePathAndName,
                        logLevel);
                    break;

                case LoggingMethod.WindowsEventLog:
                    ConfigureLoggingToEventLog(logLevel);
                    break;

                case LoggingMethod.Advanced:
                    ConfigureLoggingAccordingToXmlConfig();
                    break;

                default:
                    ConfigureLoggingToEventLog(logLevel);
                    break;
            }
        }

        /// <summary>
        /// Configure logging according to an XML config file (Advanced)
        /// </summary>
        internal static void ConfigureLoggingAccordingToXmlConfig()
        {
            string log4netConfigFilename = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "log4net.config.xml");
            FileInfo fi = new FileInfo(log4netConfigFilename);
            XmlConfigurator.ConfigureAndWatch(fi);
        }

        /// <summary>
        /// Configure logging so that it goes to a file
        /// </summary>
        /// <param name="filename">The name of the file to log to</param>
        /// <param name="logLevel">Logging severity level</param>
        internal static void ConfigureLoggingToFile(string filename, LoggingLevel logLevel)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = LOGPATTERN;
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.Layout = patternLayout;
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 4;
            roller.MaximumFileSize = "10MB";
            roller.StaticLogFileName = true;
            roller.File = filename;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = ParseLevel(logLevel);
            hierarchy.Configured = true;
        }

        /// <summary>
        /// Configure logging so that it goes to the event log
        /// </summary>
        /// <param name="logLevel">Logging severity level</param>
        internal static void ConfigureLoggingToEventLog(LoggingLevel logLevel)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = LOGPATTERN;
            patternLayout.ActivateOptions();

            EventLogAppender eventAppender = new EventLogAppender();
            eventAppender.Layout = patternLayout;
            eventAppender.ActivateOptions();
            hierarchy.Root.AddAppender(eventAppender);

            hierarchy.Root.Level = ParseLevel(logLevel);
            hierarchy.Configured = true;
        }

        /// <summary>
        /// Parse the internal logging level into a log4net logging level
        /// </summary>
        /// <param name="level">Internal logging level</param>
        /// <returns>Parsed log4net level</returns>
        internal static Level ParseLevel(LoggingLevel level)
        {
            switch (level)
            {
                case LoggingLevel.Off:
                    return Level.Off;

                case LoggingLevel.Fatal:
                    return Level.Fatal;

                case LoggingLevel.Error:
                    return Level.Error;

                case LoggingLevel.Warning:
                    return Level.Warn;

                case LoggingLevel.Info:
                    return Level.Info;

                case LoggingLevel.Debug:
                    return Level.Debug;

                case LoggingLevel.All:
                    return Level.All;

                default:
                    return Level.Warn;
            }
        }
    }
}