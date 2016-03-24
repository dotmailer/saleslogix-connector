namespace ConsoleTestHost
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using EmailMarketing.SalesLogix;
    using log4net;

    internal class Program
    {
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.WriteLine("The service will be connecting to the SData portal at ({0})", ObjectFactory.Instance.Settings.SdataUrl);
            Console.WriteLine("Press enter to continue and start the service");
            Console.ReadLine();

            var service = new EmailMarketingService();
            LoggingHelper.ConfigureLogging(
                ObjectFactory.Instance.Settings.LoggingMethod,
                ObjectFactory.Instance.Settings.LoggingLevel,
                Path.Combine(ObjectFactory.Instance.Settings.LogFileDirectory, "EmailMarketingServiceLog.txt"));
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            logger.InfoFormat("Starting service, version: {0}", version);
            service.Start();
            Console.WriteLine("Service is running.  Press enter to stop.");
            Console.ReadLine();

            service.Stop();
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error("An unhandled exception has occurred.", (Exception)e.ExceptionObject);
        }
    }
}