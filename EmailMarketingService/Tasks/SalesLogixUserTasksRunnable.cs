using System.Reflection;
using dotMailer.Sdk.Enums;

namespace EmailMarketing.SalesLogix.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using dotMailer.Sdk;
    using Entities;
    using log4net;
    using Sage.SData.Client.Core;

    //This class handles tasks queued in the database table.  I believe these are all manually
    //triggered by the user's interaction with the UI, which would explain why they are called
    //"user tasks".

    public class SalesLogixUserTasksRunnable : IRunnable
    {
        /// <summary>log4net logger object</summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Run(DateTime lastRunAtUtc)
        {
            Logger.Info("Process Sync Tasks started");

            // Setup
            var settings = ObjectFactory.Instance.Settings;
            ICollection<SyncTask> syncTasks = null;
            try
            {
                ISlxConnector slx = ObjectFactory.Instance.GetSlxConnector();

                syncTasks = slx.GetPendingAndInProgressSyncTasksThatAreDue();

                foreach (SyncTask syncTask in syncTasks)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(syncTask.TaskType))
                        {
                            Logger.WarnFormat("Null/Blank TaskType for task with id ({0})", syncTask.Id);
                            syncTask.Status = "Failed";
                        }
                        else
                        {
                            try
                            {
                                Logger.InfoFormat("Started processing task type ({0}) with ID ({1})", syncTask.TaskType, syncTask.Id);
                                Synchroniser syncer;
                                switch (syncTask.TaskType.ToUpperInvariant())
                                {
                                    case "SENDEMAILCAMPAIGN":
                                    case "SENDSPLITEMAILCAMPAIGN":
                                    case SyncTask.TaskTypeSendNewMemberCampaignUpper:
                                        var sender = new EmailCampaignSender(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());
                                        sender.ProcessSendCampaignTask(syncTask);
                                        break;

                                    case "SYNCHRONISEEMAILCAMPAIGN":
                                        var activitySyncer = new EmailCampaignActivitySynchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());
                                        activitySyncer.ProcessSyncCampaignTask(syncTask);
                                        break;

                                    case "SYNCHRONISEEMAILADDRESSBOOK":
                                        var bookSyncer = new EmailAddressBookSynchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());
                                        bookSyncer.ProcessSyncEmailAddressBookTask(syncTask);
                                        break;

                                    case SyncTask.TaskTypeSyncEmailAccountUpper:
                                        syncer = new Synchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());
                                        syncer.ProcessSyncEmailAccountTask(syncTask);
                                        break;

                                    case SyncTask.TaskTypeSyncAllEmailCampaignHeadersUpper:
                                        syncer = new Synchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());
                                        syncer.ProcessSyncAllEmailCampaignHeaders(syncTask);
                                        break;

                                    default:
                                        Logger.WarnFormat("Invalid TaskType ({0}) for task with ID ({1})", syncTask.TaskType, syncTask.Id);
                                        break;
                                }

                                Logger.InfoFormat("Finished processing task type ({0}) with ID ({1})", syncTask.TaskType, syncTask.Id);
                            }
                            catch (DmException ex)
                            {
                                if (ex.Code == DMErrorCodes.ERROR_INVALID_LOGIN)
                                {
                                    Logger.WarnFormat("The login credentials for are incorrect for the sync task with ID ({0}). Login failed.", syncTask.Id);
                                    syncTask.Status = SyncTask.StatusFailed;
                                    slx.UpdateRecord(syncTask);
                                    return;
                                }

                                Logger.WarnFormat("An exception was thrown while processing sync task ({0}).  Attempting to mark sync task as failed.{1}{2}", syncTask.Id, Environment.NewLine, ex);
                                syncTask.Status = SyncTask.StatusFailed;
                                slx.UpdateRecord(syncTask);
                                throw;
                            }
                            catch (CryptographicException)
                            {
                                Logger.WarnFormat("The login credentials for the sync task with ID ({0}) are corrupt.", syncTask.Id);
                                syncTask.Status = SyncTask.StatusFailed;
                                slx.UpdateRecord(syncTask);
                                return;
                            }
                            catch (Exception ex)
                            {
                                // Update the sync task status then re-throw the exception
                                Logger.WarnFormat("An exception was thrown while processing sync task ({0}).  Attempting to mark sync task as failed.{1}{2}", syncTask.Id, Environment.NewLine, ex);
                                syncTask.Status = SyncTask.StatusFailed;
                                slx.UpdateRecord(syncTask);
                                throw;
                            }
                        }
                    }
                    catch (SDataClientException ex)
                    {
                        LoggingAction actionCompleted = SDataClientExceptionLogger.OutputToLog(ex, settings.SdataUrl, Logger);
                        if (actionCompleted == LoggingAction.NoErrorLogged)
                            Logger.ErrorFormat("An exception was thrown while processing task {0} ({1}).", syncTask.Id, syncTask.TaskType);
                    }
                    catch (Exception)
                    {
                        Logger.ErrorFormat("An exception was thrown while processing task {0} ({1}).", syncTask.Id, syncTask.TaskType);
                    }
                }
            }
            catch (SDataClientException ex)
            {
                LoggingAction actionCompleted = SDataClientExceptionLogger.OutputToLog(ex, settings.SdataUrl, Logger);
                if (actionCompleted == LoggingAction.ErrorLogged)
                    return;
                throw;
            }

            int count = 0;
            if (syncTasks != null)
                count = syncTasks.Count;

            Logger.InfoFormat("Process Sync Tasks completed. ({0}) tasks processed.", count);
        }
    }
}