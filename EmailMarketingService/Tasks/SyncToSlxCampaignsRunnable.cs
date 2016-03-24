using System.Reflection;

namespace EmailMarketing.SalesLogix.Tasks
{
    using System;
    using log4net;
    using Sage.SData.Client.Core;

    public class SyncToSlxCampaignsRunnable : IRunnable
    {
        /// <summary>log4net logger object</summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Run(DateTime lastRunAtUtc)
        {
            Logger.Info("SyncToSlxCampaigns Started");

            try
            {
                var slxSyncer = new SlxCampaignSynchroniser(ObjectFactory.Instance.GetSlxConnector());
                slxSyncer.SynchroniseAllEmailCampaigns();
            }
            catch (SDataClientException ex)
            {
                LoggingAction actionCompleted = SDataClientExceptionLogger.OutputToLog(ex, null, Logger);
                if (actionCompleted == LoggingAction.ErrorLogged)
                    return;
                throw;
            }

            Logger.Info("SyncToSlxCampaigns Completed");
        }
    }
}