using System.Reflection;
using dotMailer.Sdk.Campaigns;
using dotMailer.Sdk.Collections;
using EmailMarketing.SalesLogix.Exceptions;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using log4net;
    using Sage.SData.Client.Atom;
    using Sage.SData.Client.Extensions;
    using Tasks;

    /// <summary>
    /// Pull back email campaign activity (sends, send summaries, clicks etc. from dotMailer
    /// </summary>
    public class EmailCampaignActivitySynchroniser
    {
        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressBookSynchroniser"/> class.
        /// </summary>
        public EmailCampaignActivitySynchroniser(ISlxConnector slx, IDotMailerConnector dotMailer)
        {
            Slx = slx;
            DotMailer = dotMailer;
        }

        public ISlxConnector Slx { get; private set; }

        public IDotMailerConnector DotMailer { get; private set; }

        public void SyncEachEmailCampaignsActivity(EmailAccount emailAccount, SyncType syncType, DateTime syncTime)
        {
            logger.InfoFormat("Starting Email Campaign Activity sync for Email Account ({0})", emailAccount.AccountName);
            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();

            int numSyncedCampaignsWithActivity = 0;

            DateTime lastSyncTime = emailAccount.LastSynchronised.GetValueOrDefault(DateTime.MinValue);
            DmCampaignCollection dmCamps = DotMailer.GetEmailCampaignsWithActivitySince(lastSyncTime);

            int caughtExceptionsCount = 0;
            foreach (DmCampaign dmCamp in dmCamps)
            {
                try
                {
                    EmailCampaign slxEmailCamp = Slx.GetEmailCampaignByEmailServiceId(dmCamp.Id);
                    if (slxEmailCamp != null)
                    {
                        if (syncType == SyncType.Manual || slxEmailCamp.SyncWithEmailService)
                        {
                            numSyncedCampaignsWithActivity++;
                            SyncActivityForSingleCampaign(dmCamp, slxEmailCamp);
                        }
                    }
                    else
                        logger.WarnFormat("Could not find a unique Email Campaign in SalesLogix with Email Service ID ({0}).  Campaign name in email service is ({1})", dmCamp.Id, dmCamp.Name);
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Exception thrown during synching of the Email Campaign with Email Service ID {0}.  Name in email service is {1}.  Exception details: {2}", dmCamp.Id, dmCamp.Name, e);
                    caughtExceptionsCount++;
                }
            }

            logger.InfoFormat(
                "Finished processing Email Campaign Activities for ({0}) different email campaigns.  Number of Exceptions thrown during processing: {1}",
                numSyncedCampaignsWithActivity, caughtExceptionsCount
                );

            if (caughtExceptionsCount > 0)
                throw new EmailCampaignsFailedToSyncActivityException(caughtExceptionsCount + " exceptions were thrown whilst synchronising Email Campaigns."); //An exception must be thrown or the task cannot be marked as failed.
        }

        public void SyncActivityForSingleCampaign(string slxEmailCampaignId)
        {
            EmailCampaign slxEmailCamp = Slx.GetRecord<EmailCampaign>(slxEmailCampaignId);
            EmailAccount emailAccount = Slx.GetRecord<EmailAccount>(slxEmailCamp.EmailAccountId);

            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();
            DmCampaign dmCampaign = DotMailer.GetEmailCampaign(slxEmailCamp.DotMailerId);

            SyncActivityForSingleCampaign(dmCampaign, slxEmailCamp);
        }

        private void SyncSendsForSingleCampaign(DmCampaign dmCamp, EmailCampaign slxEmailCamp, DateTime lastSyncTimeThisCampaign)
        {
            logger.DebugFormat("Starting send sync for Email Campaign ({0})", slxEmailCamp.Name);
            int numSummariesCreated = 0;
            int numSummariesUpdated = 0;
            DmCampaignContactActivityCollection activities = DotMailer.GetEmailCampaignActivitySince(dmCamp.Id, lastSyncTimeThisCampaign);

            if (activities != null)
            {
                logger.DebugFormat("({0}) activities found", activities.Count);
                ICollection<EmailCampaignSend> sends = Slx.GetEmailCampaignSends(slxEmailCamp.Id);
                logger.DebugFormat("({0}) sends found", sends.Count);
                IOrderedEnumerable<EmailCampaignSend> sendsSorted = sends.OrderByDescending(s => s.DateSent);
                foreach (DmCampaignContactActivity activity in activities)
                {
                    logger.DebugFormat("Processing activity for ({0}) DateSent ({1})", activity.ContactEmail, activity.DateSent);

                    // Find the corresponding sendSummary
                    EmailCampaignSendSummary sendSummary = Slx.GetEmailCampaignSendSummary(slxEmailCamp.Id, activity.ContactEmail, activity.DateSent);

                    if (sendSummary == null)
                    {
                        sendSummary = new EmailCampaignSendSummary();
                        sendSummary.EmailCampaignId = slxEmailCamp.Id;
                        sendSummary.EmailAccountId = slxEmailCamp.EmailAccountId;

                        // Get hold of the send that originated the activity, i.e. the most recent send with
                        // a date before the activity.DateSent (allowing 30 seconds leeway)
                        // N.B. It is possible that a send does not exist (E.g. the campaign was sent before the SLX integration
                        // was turned on so we don't know anything about it).
                        var send = sendsSorted.FirstOrDefault(s => s.DateSent <= activity.DateSent.AddSeconds(30));
                        if (send == null)
                        {
                            logger.InfoFormat("No email campaign send found for email campaign ({0}) on ({0})", slxEmailCamp.Name, activity.DateSent);
                            send = new EmailCampaignSend();
                            send.EmailAccountId = slxEmailCamp.EmailAccountId;
                            send.EmailCampaignId = slxEmailCamp.Id;
                            send.SendStatus = EmailCampaignSend.StatusComplete;
                            send.DateSent = activity.DateSent;
                            send.ScheduledSendDate = activity.DateSent;
                            send.Description = "Non-SalesLogix send";
                            string idCreated = Slx.CreateRecord(send);
                            logger.InfoFormat("Created email campaign send with ID ({0})", idCreated);

                            send.Id = idCreated;
                            sends.Add(send);
                            sendsSorted = sends.OrderByDescending(s => s.DateSent);

                            sendSummary.EmailCampaignSendId = idCreated;
                        }
                        else
                            sendSummary.EmailCampaignSendId = send.Id;
                    }

                    sendSummary.NumberOfClicks = activity.NumClicks;
                    sendSummary.DateFirstOpened = NormaliseEmailServiceDateTime(activity.DateFirstOpened);
                    sendSummary.DateLastOpened = NormaliseEmailServiceDateTime(activity.DateLastOpened);
                    sendSummary.DateSent = NormaliseEmailServiceDateTime(activity.DateSent);
                    sendSummary.EmailAddress = activity.ContactEmail;
                    sendSummary.NumberOfEstimatedForwards = activity.NumEstimatedForwards;
                    sendSummary.FirstOpenIPAddress = activity.FirstOpenIp;
                    // Obsolete property
                    // sendSumm.FirstOpenUserAgent = activity.FirstOpenUserAgent;
                    sendSummary.NumberOfForwardsToFriends = activity.NumForwardToFriend;
                    sendSummary.HardBounced = activity.HardBounced;
                    sendSummary.NumberOfOpens = activity.NumOpens;
                    sendSummary.NumberOfReplies = activity.NumReplies;
                    sendSummary.EmailServiceContactId = activity.ContactId;
                    sendSummary.SoftBounced = activity.SoftBounced;
                    sendSummary.Unsubscribed = activity.Unsubscribed;
                    sendSummary.NumberOfViews = activity.NumViews;

                    // Get hold of the first EmailAddressBookMember which matches by email address
                    EmailAddressBookMember bookMember = Slx.GetFirstEmailAddressBookMemberInCampaign(slxEmailCamp.Id, activity.ContactEmail);
                    if (bookMember != null)
                    {
                        sendSummary.AddressBookMemberId = bookMember.Id;
                        sendSummary.AddressBookId = bookMember.EmailAddressBookId;
                        sendSummary.SlxMemberType = bookMember.SlxMemberType;
                        sendSummary.SlxContactId = bookMember.SlxContactId;
                        sendSummary.SlxLeadId = bookMember.SlxLeadId;
                    }
                    else
                        logger.DebugFormat("Email Address Book Member ({0}) not found for Email Campaign ({1})", activity.ContactEmail, slxEmailCamp.Name);

                    if (string.IsNullOrWhiteSpace(sendSummary.Id))
                    {
                        logger.DebugFormat("Creating Send Summary for ({0}) in Email Campaign ({1})", activity.ContactEmail, slxEmailCamp.Name);
                        Slx.CreateRecord(sendSummary);
                        numSummariesCreated++;

                        // If a send summary is being created, then an email was sent.
                        // If we found an address book member (so we know the Contact/Lead), then create a SalesLogix History record.
                        if (bookMember != null)
                        {
                            CreateSlxHistoryForSentEmail(dmCamp, sendSummary, bookMember);
                        }
                        else
                        {
                            logger.DebugFormat("Not creating SalesLogix History for sent email because no EmailAddressBookMember was found for EMailCampaign ({0}) and email address ({1})", dmCamp.Name, sendSummary.EmailAddress);
                        }
                    }
                    else
                    {
                        logger.DebugFormat("Updating Send Summary for ({0}) in Email Campaign ({1})", activity.ContactEmail, slxEmailCamp.Name);
                        Slx.UpdateRecord(sendSummary);
                        numSummariesUpdated++;
                    }
                }
            }

            logger.DebugFormat("Finished send sync {0} send summaries created and {1} send summaries updated", numSummariesCreated, numSummariesUpdated);
        }

        //full account sync uses this specific method
        public void SyncActivityForSingleCampaign(DmCampaign dmCamp, EmailCampaign slxEmailCamp, bool syncEvenIfCampaignIsNotConfiguredToSyncWithEmailService = false)
        {
            if (slxEmailCamp == null)
                logger.WarnFormat("Email Campaign with activity does not exist in SalesLogix ({0}, {1})", dmCamp.Name, dmCamp.Id);
            else
            {
                if ((slxEmailCamp.SyncWithEmailService) || (syncEvenIfCampaignIsNotConfiguredToSyncWithEmailService))
                {
                    logger.DebugFormat("Starting sync of Activity for Email Campaign ({0})", slxEmailCamp.Name);
                    DateTime lastSyncTimeThisCampaign = slxEmailCamp.LastActivityUpdate.GetValueOrDefault(DateTime.MinValue);
                    DateTime thisCampaignSyncStartedAt = DateTime.UtcNow;

                    SyncSendsForSingleCampaign(dmCamp, slxEmailCamp, lastSyncTimeThisCampaign);

                    SyncClicksForSingleCampaign(dmCamp, slxEmailCamp, lastSyncTimeThisCampaign);

                    slxEmailCamp.CopyStatisticsFrom(dmCamp);
                    slxEmailCamp.LastActivityUpdate = thisCampaignSyncStartedAt;
                    Slx.UpdateRecord(slxEmailCamp);
                    logger.DebugFormat("Finished sync of Activity for Email Campaign ({0})", slxEmailCamp.Name);
                }
                else
                    logger.DebugFormat("Email Campaign ({0}, {1}) is configured to not be synchronised.", dmCamp.Name, dmCamp.Id);
            }
        }

        private void SyncClicksForSingleCampaign(DmCampaign dmCamp, EmailCampaign slxEmailCamp, DateTime lastSyncTimeThisCampaign)
        {
            DmCampaignContactStatsCollection emailCampaignClickersSince = DotMailer.GetEmailCampaignClickersSince(dmCamp.Id, lastSyncTimeThisCampaign);
            if (emailCampaignClickersSince != null)
            {
                List<EmailCampaignClick> slxClicksToAdd = new List<EmailCampaignClick>();
                foreach (DmCampaignContactStats clicker in emailCampaignClickersSince)
                {
                    // Only add the click record if it doesn't already exist
                    EmailCampaignClick slxClick = Slx.GetEmailCampaignClick(slxEmailCamp.Id, clicker.ContactEmail, clicker.DateClicked);
                    if (slxClick == null)
                    {
                        // Get hold of the first EmailAddressBookMember which matches by email address
                        var bookMember = Slx.GetFirstEmailAddressBookMemberInCampaign(slxEmailCamp.Id, clicker.ContactEmail);

                        // Build up the click record
                        slxClick = new EmailCampaignClick();
                        if (bookMember != null)
                        {
                            slxClick.AddressBookMemberId = bookMember.Id;
                            slxClick.AddressBookId = bookMember.EmailAddressBookId;
                            slxClick.SlxMemberType = bookMember.SlxMemberType;
                            slxClick.SlxContactId = bookMember.SlxContactId;
                            slxClick.SlxLeadId = bookMember.SlxLeadId;
                        }

                        slxClick.DateClicked = clicker.DateClicked;
                        slxClick.EmailAddress = clicker.ContactEmail;
                        slxClick.EmailServiceContactID = null;
                        slxClick.EmailAccountID = slxEmailCamp.EmailAccountId;
                        slxClick.Ememailcampaignid = slxEmailCamp.Id;
                        slxClick.IPAddress = clicker.IpAddress;
                        slxClick.Keyword = clicker.Keyword;
                        slxClick.URL = (clicker.Url == null) ? "" : clicker.Url.ToString();
                        slxClick.UserAgent = clicker.UserAgent;

                        logger.DebugFormat(
                            "Batching CampaignClick for creation ({0}) on ({1}) in campaign ({2})",
                            slxClick.EmailAddress,
                            slxClick.DateClicked,
                            slxClick.Ememailcampaignid);
                        slxClicksToAdd.Add(slxClick);
                    }
                }

                if (slxClicksToAdd.Count > 0)
                {
                    logger.DebugFormat("Submitting ({0}) CampaignClicks for creation", slxClicksToAdd.Count);
                    Slx.BatchCreateRecords<EmailCampaignClick>(slxClicksToAdd);
                }
                else
                {
                    logger.Debug("No CampaignClicks to be created");
                }
            }
        }

        public void ProcessSyncCampaignTask(SyncTask syncTask)
        {
            if (!string.Equals(syncTask.TaskType, "SynchroniseEmailCampaign", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(string.Format("syncTask must have TaskType of SynchroniseEmailCampaign, but was ({0}) for task with id ({1})", syncTask.TaskType, syncTask.Id));

            if (string.IsNullOrWhiteSpace(syncTask.TaskData))
                throw new ArgumentException(string.Format("syncTask.TaskData cannot be null or blank for task with id ({0})", syncTask.Id));

            string slxEmailCampaignId = syncTask.TaskData;
            logger.DebugFormat("Email Campaign to be synchronised is ({0})", slxEmailCampaignId);
            syncTask.Status = SyncTask.StatusInProgress;
            syncTask.ActualStartTime = DateTime.UtcNow;
            Slx.UpdateRecord(syncTask);

            SyncActivityForSingleCampaign(slxEmailCampaignId);

            syncTask.Status = SyncTask.StatusComplete;
            Slx.UpdateRecord(syncTask);
        }

        private static DateTime? NormaliseEmailServiceDateTime(DateTime dateTimeToNormalise)
        {
            if (dateTimeToNormalise <= new DateTime(1970, 1, 1))
            {
                return null;
            }
            else
            {
                return dateTimeToNormalise;
            }
        }

        private void CreateSlxHistoryForSentEmail(DmCampaign camp, EmailCampaignSendSummary sendSumm, EmailAddressBookMember bookMember)
        {
            History hist = new History();
            hist.Type = HistoryType.atEMail;
            hist.ContactId = bookMember.SlxContactId;
            hist.LeadId = bookMember.SlxLeadId;

            // Get hold of some data we need to create the history record
            AtomEntry entry = (AtomEntry)bookMember.SourceData;
            SDataPayload bookPayload = entry.GetSDataPayload();

            // if the address book member has a linked Contact, then pull out some data from it
            if (bookPayload.Values.ContainsKey(SlxContact.EntityName)
                && bookPayload.Values[SlxContact.EntityName] != null)
            {
                var contactPayload = (SDataPayload)bookPayload.Values[SlxContact.EntityName];
                var accountPayload = (SDataPayload)contactPayload.Values["Account"];
                hist.AccountId = accountPayload.Key;
                hist.AccountName = (string)contactPayload.Values["AccountName"];
                hist.ContactName = (string)contactPayload.Values["NameLF"];
            }

            // if the address book member has a linked Lead, then pull out some data from it
            if (bookPayload.Values.ContainsKey(SlxLead.EntityName)
                && bookPayload.Values[SlxLead.EntityName] != null)
            {
                var leadPayload = (SDataPayload)bookPayload.Values[SlxLead.EntityName];
                hist.LeadName = (string)leadPayload.Values["LeadNameLastFirst"];
            }

            hist.Priority = "None";
            hist.Category = string.Empty;
            hist.StartDate = (DateTime)sendSumm.DateSent;
            hist.Duration = 0;
            hist.Timeless = false;
            hist.UserId = Slx.ConnectedUserId;
            hist.UserName = ObjectFactory.Instance.Settings.SdataUsername;
            hist.OriginalDate = hist.StartDate;
            hist.Result = "Complete";
            hist.ResultCode = "DON";
            hist.CompletedDate = hist.StartDate;
            hist.CompletedUser = hist.UserId;
            hist.Notes = string.Format("Sent Email Marketing Campaign \"{0}\".", camp.Name);
            hist.LongNotes = hist.Notes;

            logger.DebugFormat("Creating SalesLogix email History record for ({0})", hist.UserName);
            Slx.CreateRecord(hist);
        }
    }
}