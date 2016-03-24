using System.Collections.Generic;
using System.Reflection;
using EmailMarketing.SalesLogix.Entities;

namespace EmailMarketing.SalesLogix.Tasks
{
    using System;
    using log4net;
    using Sage.SData.Client.Core;

    //Used to synchronise all email accounts (all the various details) periodically (as determined by the configuration tool's "Synchronise to Email Service every... minutes" setting.
    public class SyncToEmailServiceRunnable : IRunnable
    {
        /// <summary>log4net logger object</summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Run(DateTime lastRunAtUtc)
        {
            Logger.Info("SyncToEmailService Started");

            Settings settings = ObjectFactory.Instance.Settings;
            Logger.DebugFormat("Slx sdata url ({0}), Slx username ({1})", settings.SdataUrl, settings.SdataUsername);

            if (!settings.SyncToEmailServiceRunnableEnabled)
            {
                Logger.Info("Not running SyncToEmailService because SyncToEmailServiceRunnableEnabled is set to false in the config file.");
                return; //This is useful for development/testing purposes, provide a value for "SyncToEmailServiceRunnableEnabled" in EmailMarketingServiceConfig.xml, true is default.
            }

            try
            {
                // Setup
                ISlxConnector slx = ObjectFactory.Instance.GetSlxConnector();

                // Get EmailAccounts which need syncing
                ICollection<EmailAccount> emailAccounts = slx.GetEmailAccountsNeedingSync(settings.SyncToEmailServiceFrequencyMinutes * 60);

                var syncer = new Synchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());
                syncer.SyncEmailAccounts(emailAccounts, SyncType.Scheduled);
            }
            catch (SDataClientException ex)
            {
                LoggingAction actionCompleted = SDataClientExceptionLogger.OutputToLog(ex, settings.SdataUrl, Logger);
                if (actionCompleted == LoggingAction.ErrorLogged)
                    return;
                throw;
            }

            Logger.Info("SyncToEmailService Completed");
        }
    }
}