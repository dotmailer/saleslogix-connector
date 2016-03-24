namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.ServiceProcess;
    using log4net;

    public partial class EmailMarketingServiceController : ServiceBase
    {
        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Holds the reference to the Email Marketing service
        /// </summary>
        private EmailMarketingService svc;

        public EmailMarketingServiceController()
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogging(
                            ObjectFactory.Instance.Settings.LoggingMethod,
                            ObjectFactory.Instance.Settings.LoggingLevel,
                            Path.Combine(ObjectFactory.Instance.Settings.LogFileDirectory, "EmailMarketingServiceLog.txt"));
        }

        protected override void OnStart(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            logger.InfoFormat("Service Starting, version: {0}", version);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            svc = new EmailMarketingService();
            svc.Start();
            logger.Info("Service Started Successfully");
        }

        protected override void OnStop()
        {
            logger.Info("Service Stopping");
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            svc.Stop();
            svc = null;
            logger.Info("Service Stopped");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error("An unhandled exception has occurred.", (Exception)e.ExceptionObject);
        }
    }
}