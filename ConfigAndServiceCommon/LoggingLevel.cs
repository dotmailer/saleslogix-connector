namespace EmailMarketing.SalesLogix
{
    using System;

    public enum LoggingLevel
    {
        Off,
        Fatal,
        Error,
        Warning,
        Info,
        Debug,
        All
    }

    public static class LoggingLevelExtensions
    {
        public static LoggingLevel Parse(this LoggingLevel val, string valueToParse)
        {
            if (valueToParse == null)
            {
                throw new ArgumentNullException("valueToParse");
            }

            switch (valueToParse.ToUpperInvariant())
            {
                case ("OFF"):
                    return LoggingLevel.Off;

                case ("FATAL"):
                    return LoggingLevel.Fatal;

                case ("ERROR"):
                    return LoggingLevel.Error;

                case ("WARNING"):
                    return LoggingLevel.Warning;

                case ("INFORMATION"):
                    return LoggingLevel.Info;

                case ("DEBUG"):
                    return LoggingLevel.Debug;

                case ("ALL"):
                    return LoggingLevel.All;

                default:
                    throw new ArgumentException(string.Format("'{0}' is not a valid value for LoggingLevel", valueToParse), "valueToParse");
            }
        }
    }
}