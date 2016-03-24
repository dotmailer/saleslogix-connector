using System.Reflection;
using System.Threading;
using dotMailer.Sdk.Campaigns;
using dotMailer.Sdk.Collections;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Entities;
    using log4net;
    using Sage.SData.Client.Extensions;
    using Tasks;

    /// <summary>
    /// Sends Email Campaigns
    /// </summary>
    public class EmailCampaignSender
    {
        public const string TempAddressBookNamePrefix = "TempSalesLogixIntegration-";

        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailCampaignSender"/> class.
        /// </summary>
        public EmailCampaignSender(ISlxConnector slx, IDotMailerConnector dotMailer)
        {
            Slx = slx;
            DotMailer = dotMailer;
        }

        public ISlxConnector Slx { get; private set; }

        public IDotMailerConnector DotMailer { get; private set; }

        public void ProcessSendCampaignTask(SyncTask syncTask)
        {
            ValidateSyncTaskTaskType(syncTask);
            CheckTaskDataHasBeenProvided(syncTask);

            CampaignSendData sendData = DeserialiseCampaignSendData(syncTask);
            EmailCampaign emailCampaign = Slx.GetRecord<EmailCampaign>(sendData.CampaignId);
            EmailAccount emailAccount = Slx.GetRecord<EmailAccount>(emailCampaign.EmailAccountId);

            try
            {
                DotMailer.Username = emailAccount.ApiKey;
                DotMailer.Password = emailAccount.GetDecryptedPassword();

                logger.DebugFormat("Processing send for email campaign ({0})", emailCampaign.Name);

                syncTask.ActualStartTime = DateTime.UtcNow;
                syncTask.Status = SyncTask.StatusInProgress;
                Slx.UpdateSyncTask(syncTask);

                bool isSplitTestSend;
                SplitTestMetric metric;
                GetSplitTestDetails(sendData, out isSplitTestSend, out metric);

                if (string.IsNullOrEmpty(sendData.TempAddressBookName))
                {
                    emailCampaign.Status = EmailCampaign.StatusSendInProgress;
                    Slx.UpdateRecord(emailCampaign);

                    //Targeted address books must be synchronised to ensure that a later campaign sync can populate the contact
                    //and lead in the send summaries.  Only the targeted address books used in this send operation are synced.
                    SynchroniseTargetedAddressBooks(emailCampaign, emailAccount, sendData);

                    // Temp address book import has not been started yet
                    int numberOfTargetsLoaded = CreateAndPopulateTemporaryAddressBook(sendData);

                    if (numberOfTargetsLoaded == 0)
                    {
                        logger.InfoFormat("No valid targets for email campaign send ({0})", emailCampaign.Name);
                        syncTask.SetTaskData(sendData);
                        syncTask.Status = SyncTask.StatusComplete;
                        Slx.UpdateSyncTask(syncTask);
                        return;
                    }

                    syncTask.SetTaskData(sendData);
                    Slx.UpdateSyncTask(syncTask);
                }

                // Do not be tempted to put an 'else' just here. sendData.TempAddressBookName should be set
                // in the above block of code, thereby causing this next block to run straight away to check if the
                // addressbook import is complete.
                if (!string.IsNullOrEmpty(sendData.TempAddressBookName)
                    && string.IsNullOrEmpty(sendData.CampaignSendResult))
                {
                    // Campaign has not been sent, but Temp address book import has been started, has it finished?
                    // N.B. This could happen straight after the Import was submitted above,
                    // or it could happen because the task has gone back into the task list to wait
                    // for the import to finish.
                    bool isImportFinished = DotMailer.IsImportComplete(sendData.ImportContactResult);

                    if (isImportFinished)
                    {
                        logger.Info("Import complete");

                        // QTIP3610
                        // It is possible that the AddressBook import is not completely finished.
                        // Wait a short while to allow it time to finish.
                        Thread.Sleep(5000);
                        logger.InfoFormat("Sending email campaign ({0}) to temp address book ({1})", emailCampaign.Name, sendData.TempAddressBookName);

                        string result;
                        if (isSplitTestSend)
                        {
                            result = DotMailer.SendEmailCampaign(emailCampaign.DotMailerId, sendData.TempAddressBookName, sendData.SendDate, metric, (int)sendData.TestPercentage,
                                (int)sendData.TestPeriodHours);
                        }
                        else
                            result = DotMailer.SendEmailCampaign(emailCampaign.DotMailerId, sendData.TempAddressBookName, sendData.SendDate);
                        sendData.CampaignSendResult = result;

                        // sendData has been updated, so write it back into the SyncTask
                        sendData.ActualDateSent = DateTime.UtcNow;
                        syncTask.SetTaskData(sendData);
                        Slx.UpdateSyncTask(syncTask);

                        emailCampaign.LastSent = sendData.ActualDateSent;
                        Slx.UpdateRecord(emailCampaign);
                    }
                    else
                        logger.DebugFormat("Waiting for Contact Import to finish for task ({0})", syncTask.Id);
                }

                if (sendData.CampaignSendResult != null)
                {
                    // Campaign send has been submitted, but has it finished?
                    // Do not check more often than every 60 seconds to avoid too many email service calls.
                    if (sendData.LastCheckForSendComplete == null || DateTime.UtcNow - sendData.LastCheckForSendComplete > new TimeSpan(0, 0, 60))
                    {
                        // Do not start checking a split test campaign until it is nearly due to finish.
                        if (isSplitTestSend && DateTime.UtcNow < sendData.ActualDateSent.Value.AddHours((double)sendData.TestPeriodHours).AddMinutes(-5))
                            logger.DebugFormat("Waiting for split test campaign to be due ({0})", emailCampaign.Name);
                        else
                        {
                            bool isCampaignSendComplete = DotMailer.IsCampaignSendComplete(sendData.CampaignSendResult);
                            if (isCampaignSendComplete)
                            {
                                logger.InfoFormat("Email campaign send complete for email campaign ({0})", emailCampaign.Name);
                                syncTask.Status = SyncTask.StatusComplete;
                                Slx.UpdateSyncTask(syncTask);
                                DotMailer.DeleteAddressBook(sendData.TempAddressBookName);

                                var existingSends = Slx.GetEmailCampaignSends(emailCampaign.Id);
                                int sendNumber = existingSends == null ? 1 : existingSends.Count + 1;
                                EmailCampaignSend campaignSend = new EmailCampaignSend();
                                campaignSend.EmailAccountId = emailCampaign.EmailAccountId;
                                campaignSend.EmailCampaignId = emailCampaign.Id;
                                campaignSend.DateSent = sendData.ActualDateSent;
                                campaignSend.Description = string.Format("Send {0} : Campaign send to {1} Email Address Books", sendNumber, sendData.AddressBookIds.Count);
                                campaignSend.ScheduledSendDate = sendData.SendDate;
                                campaignSend.SendStatus = EmailCampaignSend.StatusComplete;

                                campaignSend.SendType = "Campaign Send";
                                campaignSend.IsSplitTestSend = isSplitTestSend;
                                if (isSplitTestSend)
                                {
                                    campaignSend.SplitTestPercent = sendData.TestPercentage;
                                    campaignSend.SplitTestOpenTime = sendData.TestPeriodHours;
                                    campaignSend.SplitTestMetric = sendData.TestMetric;

                                    emailCampaign.SplitTestPercent = campaignSend.SplitTestPercent;
                                    emailCampaign.SplitTestOpenTime = campaignSend.SplitTestOpenTime;
                                    emailCampaign.SplitTestMetric = campaignSend.SplitTestMetric;
                                }

                                logger.DebugFormat("Creating campaign send record for email campaign ({0})", emailCampaign.Name);
                                Slx.CreateRecord(campaignSend);

                                //Synchronise the campaign to ensure that send summaries etc. are available to the user sooner, rather than later.
                                SynchroniseEmailCampaign(emailCampaign);

                                emailCampaign.Status = EmailCampaign.StatusSendComplete;
                                Slx.UpdateRecord(emailCampaign);
                            }
                            else
                                logger.DebugFormat("Waiting for Campaign Send to finish for task ({0})", syncTask.Id);

                            // Update 'time of last check'
                            sendData.LastCheckForSendComplete = DateTime.UtcNow;
                            syncTask.SetTaskData(sendData);
                            Slx.UpdateSyncTask(syncTask);
                        }
                    }
                }
            }
            catch
            {
                if (emailCampaign != null && Slx != null && emailCampaign.Status != EmailCampaign.StatusSendComplete)
                {
                    emailCampaign.Status = EmailCampaign.StatusSendFailed;
                    Slx.UpdateRecord(emailCampaign);
                }

                throw;
            }
        }

        private void SynchroniseEmailCampaign(EmailCampaign emailCampaign)
        {
            DmCampaign dmCampaign = DotMailer.GetEmailCampaign(emailCampaign.DotMailerId);
            EmailCampaignActivitySynchroniser activitySynchroniser = new EmailCampaignActivitySynchroniser(Slx, DotMailer);
            activitySynchroniser.SyncActivityForSingleCampaign(dmCampaign, emailCampaign, true);
        }

        //based on code in EmailAddressBookSynchroniser.ProcessSyncEmailAddressBookTask
        private void SynchroniseTargetedAddressBooks(EmailCampaign emailCampaign, EmailAccount emailAccount, CampaignSendData sendData)
        {
            logger.Debug("Starting synchronising all address books that are a target of email campaign for current send operation: " + emailCampaign.Name);
            Synchroniser.SyncDataLabelsOnEmailAccount(emailAccount);
            EmailAddressBookSynchroniser synchroniser = new EmailAddressBookSynchroniser(Slx, DotMailer);

            foreach (string addressBookId in sendData.AddressBookIds)
            {
                EmailAddressBook addressBook = Slx.GetRecord<EmailAddressBook>(addressBookId);
                bool addressBookNameWasReservedByEmailService = !synchroniser.SyncSingleAddressBookHeader(addressBook);

                if (addressBookNameWasReservedByEmailService == false)
                {
                    //We state that it is a manual sync, so that any address book marked as manual sync only
                    //will be processed.  Each address book must be synced or campaign synching only works
                    //partially (email send summaries are not completely populated).
                    synchroniser.SyncEmailAddressBookMembers(emailAccount, addressBook, SyncType.Manual, DateTime.UtcNow, false, true);
                }
            }

            logger.Debug("Finished synchronising all address books that are a target of email campaign: " + emailCampaign.Name);
        }

        private CampaignSendData DeserialiseCampaignSendData(SyncTask syncTask)
        {
            StringReader sr = new StringReader(syncTask.TaskData);
            CampaignSendData result = null;

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(CampaignSendData));
                result = (CampaignSendData)xs.Deserialize(sr);
                if (!string.IsNullOrWhiteSpace(result.ImportContactResult) && result.ImportContactResult.Substring(0, 1) == "?")
                {
                    // For some reason sdata puts a stray '?' on the start of this serialised object when it stores it.
                    result.ImportContactResult = result.ImportContactResult.Substring(1);
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.WarnFormat("SendCampaignTask data ({0}) could not be deserialized due to exception:{3}{4}", syncTask.Id, Environment.NewLine, ex);
            }

            return result;
        }

        private static void GetSplitTestDetails(CampaignSendData sendData, out bool isSplitTestSend, out SplitTestMetric metric)
        {
            isSplitTestSend = false;
            metric = SplitTestMetric.Unknown;

            if (sendData.TestMetric != null && sendData.TestPercentage != null && sendData.TestPeriodHours != null)
            {
                isSplitTestSend = true;
                switch (sendData.TestMetric.ToUpperInvariant())
                {
                    case "OPENS":
                        metric = SplitTestMetric.Opens;
                        break;

                    case "CLICKS":
                        metric = SplitTestMetric.Clicks;
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Invalid split test metric ({0})", sendData.TestMetric));
                }

                logger.DebugFormat("Email campaign is a split test with metric ({0}), percentage ({1}), period ({2})", metric, sendData.TestPercentage, sendData.TestPeriodHours);
            }
        }

        private static void ValidateSyncTaskTaskType(SyncTask syncTask)
        {
            if (!string.Equals(syncTask.TaskType, "SendEmailCampaign", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(syncTask.TaskType, "SendSplitEmailCampaign", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(syncTask.TaskType, SyncTask.TaskTypeSendNewMemberCampaign, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(string.Format("syncTask must have TaskType of SendEmailCampaign, SendSplitEmailCampaign or {2}, but was ({0}) for task with id ({1})", syncTask.TaskType,
                    syncTask.Id, SyncTask.TaskTypeSendNewMemberCampaign));
            }
        }

        private static void CheckTaskDataHasBeenProvided(SyncTask syncTask)
        {
            if (string.IsNullOrWhiteSpace(syncTask.TaskData))
            {
                throw new ArgumentException(string.Format("syncTask.TaskData cannot be null or blank for task with id ({0})", syncTask.Id));
            }
        }

        /// <summary>
        /// Determines which address book members should form part of a temporary address book, then
        /// creates and fills that address book, unless no members were needed.
        ///
        /// When the process completes sendData.TempAddressBookName holds the name of the
        /// temporary address book.
        /// </summary>
        /// <param name="sendData">If specificMembers is null, provides the address book IDs which provide that members for the temporary address book.</param>
        /// <param name="specificMembers">As an alternative to sourcing members from address books, if non-null the address book members specified here are used instead.</param>
        /// <returns>The number of contacts in the temporary address book.  If 0 then no address book has been created.</returns>
        public int CreateAndPopulateTemporaryAddressBook(CampaignSendData sendData, ICollection<EmailAddressBookMember> specificMembers = null)
        {
            EmailCampaign emailCampaign = Slx.GetRecord<EmailCampaign>(sendData.CampaignId);
            if (emailCampaign == null)
                throw new InvalidOperationException(string.Format("EmailCampaign with ID ({0}) does not exist", sendData.CampaignId));

            var synchroniser = new EmailAddressBookSynchroniser(Slx, DotMailer);
            ICollection<DataFieldMapping> dataMappings;
            Dictionary<string, List<string>> subEntitiesAndFields = synchroniser.GetSubEntitiesAndFields(emailCampaign.EmailAccountId, out dataMappings);

            // Send to members in the specified address books (not necessarily all address books connected to the email campaign)
            var toSendMemberEmailAddresses = new HashSet<string>();
            var contactIdsAlreadySentTo = new HashSet<string>();
            var leadIdsAlreadySentTo = new HashSet<string>();

            if (emailCampaign.AllowRepeatSendsToRecipient == false)
                PopulateListsOfContactsAndLeadsToWhichThisCampaignHasAlreadyBeenSent(emailCampaign, contactIdsAlreadySentTo, leadIdsAlreadySentTo);

            List<EmailAddressBookMember> allPotentialMembers = CreateListOfAllPotentialAddressBookMembers(sendData, specificMembers, emailCampaign, subEntitiesAndFields);
            List<EmailServiceContact> toSendContacts = FilterOutMembersThatShouldNotBeSentTo(allPotentialMembers, synchroniser, emailCampaign, contactIdsAlreadySentTo, leadIdsAlreadySentTo, toSendMemberEmailAddresses, dataMappings);

            if (emailCampaign.AllowRepeatSendsToRecipient == false)
            {
                //This is a belt-and-braces approach.  We also ask dotMailer who we've ever sent this campaign to and remove them from the list of contacts to send to as well.
                //If for any reason the send summaries in Saleslogix are missing, then this should stop the recipient getting anything.
                HashSet<string> emailAddressesAlreadySentToFromDotMailer = GetAllPreviousCampaignRecipientsFromDotMailer(emailCampaign.DotMailerId);
                RemoveContactsReportedByDotMailerAsAlreadySentTo(toSendContacts, emailAddressesAlreadySentToFromDotMailer);
            }

            if (toSendContacts.Count > 0)
            {
                // Send to the specified contacts
                string tempAddressBookName = TempAddressBookNamePrefix + Guid.NewGuid().ToString("N");
                logger.InfoFormat("Building temp address book ({0}) for ({1}) members", tempAddressBookName, toSendContacts.Count);
                var newAddressBook = DotMailer.CreateAddressBook(tempAddressBookName);
                bool isImportFinished;
                string importResult;
                DotMailer.ImportContactsIntoAddressBook(newAddressBook.Id, toSendContacts, out isImportFinished, out importResult);

                sendData.TempAddressBookName = tempAddressBookName;
                sendData.ImportContactResult = importResult;
            }

            return toSendContacts.Count;
        }

        private static void RemoveContactsReportedByDotMailerAsAlreadySentTo(List<EmailServiceContact> toSendContacts, HashSet<string> emailAddressesAlreadySentToFromDotMailer)
        {
            for (int index = toSendContacts.Count - 1; index >= 0; index--)
            {
                EmailServiceContact toSendContact = toSendContacts[index];
                if (emailAddressesAlreadySentToFromDotMailer.Contains(toSendContact.EmailAddress.ToLowerInvariant()))
                    toSendContacts.RemoveAt(index);
            }
        }

        private HashSet<string> GetAllPreviousCampaignRecipientsFromDotMailer(int dotMailerCampaignId)
        {
            HashSet<string> result = new HashSet<string>();
            DmCampaignContactActivityCollection recipients = DotMailer.GetEmailCampaignActivity(dotMailerCampaignId);
            foreach (DmCampaignContactActivity recipient in recipients)
                result.Add(recipient.ContactEmail.ToLowerInvariant());
            return result;
        }

        private static List<EmailServiceContact> FilterOutMembersThatShouldNotBeSentTo(List<EmailAddressBookMember> allPotentialMembers, EmailAddressBookSynchroniser synchroniser, EmailCampaign emailCampaign, HashSet<string> contactIdsAlreadySentTo,
            HashSet<string> leadIdsAlreadySentTo, HashSet<string> toSendMemberEmailAddresses, ICollection<DataFieldMapping> dataMappings)
        {
            var toSendContacts = new List<EmailServiceContact>();
            foreach (var member in allPotentialMembers)
            {
                SDataPayload baseEntityPayload;
                bool memberHasMissingInformation;
                string emailAddress = synchroniser.IdentifyEmailAddressAndPayload(member, out baseEntityPayload, out memberHasMissingInformation);

                if (!memberHasMissingInformation)
                {
                    bool doNotSolicit;
                    bool doNotEmail;
                    ExtractDoNotSolicitFromPayload(baseEntityPayload, out doNotSolicit, out doNotEmail);

                    if (string.Equals(member.SlxMemberType, "CONTACT", StringComparison.OrdinalIgnoreCase)
                        && !emailCampaign.AllowRepeatSendsToRecipient
                        && contactIdsAlreadySentTo.Contains(member.SlxContactId))
                    {
                        logger.DebugFormat(
                            "Not sending to Contact that has already been sent to ({0}) for address book member id ({1})",
                            member.SlxContactId, member.Id);
                    }
                    else if (string.Equals(member.SlxMemberType, "LEAD", StringComparison.OrdinalIgnoreCase)
                             && !emailCampaign.AllowRepeatSendsToRecipient
                             && leadIdsAlreadySentTo.Contains(member.SlxLeadId))
                    {
                        logger.DebugFormat(
                            "Not sending to Lead that has already been sent to ({0}) for address book member id ({1})",
                            member.SlxLeadId, member.Id);
                    }
                    else if (string.IsNullOrEmpty(emailAddress))
                    {
                        logger.DebugFormat("Not sending to blank email address for address book member id ({0})",
                            member.Id);
                    }
                    else if (toSendMemberEmailAddresses.Contains(emailAddress))
                    {
                        logger.DebugFormat("Already sending to email address ({0}) for address book member id ({1})",
                            emailAddress, member.Id);
                    }
                    else if (doNotSolicit)
                    {
                        logger.DebugFormat("Not sending to address book member id ({0}) due to DoNotSolicit", member.Id);
                    }
                    else if (doNotEmail)
                    {
                        logger.DebugFormat("Not sending to address book member id ({0}) due to DoNotEmail", member.Id);
                    }
                    else
                    {
                        int fieldHash;
                        Dictionary<string, object> dataFieldValues;
                        synchroniser.ExtractDataFieldsAndHash(dataMappings, member, emailAddress, baseEntityPayload,
                            out fieldHash, out dataFieldValues);

                        EmailServiceContact tempContact = new EmailServiceContact();
                        tempContact.EmailAddress = emailAddress;
                        tempContact.DataFieldValues = dataFieldValues;
                        toSendMemberEmailAddresses.Add(emailAddress);

                        logger.DebugFormat("Adding member ({0}) to list of contacts to be sent to", emailAddress);
                        toSendContacts.Add(tempContact);
                    }
                }
            }
            return toSendContacts;
        }

        private List<EmailAddressBookMember> CreateListOfAllPotentialAddressBookMembers(CampaignSendData sendData, ICollection<EmailAddressBookMember> specificMembers, EmailCampaign emailCampaign, Dictionary<string, List<string>> subEntitiesAndFields)
        {
            List<EmailAddressBookMember> result;
            if (specificMembers == null)
            {
                // We have not been given a specific list of members, so get the members from the specified address books
                result = new List<EmailAddressBookMember>();
                foreach (string addressBookId in sendData.AddressBookIds)
                {
                    logger.DebugFormat("Collecting members to send to from address book ({0})", addressBookId);
                    EmailCampaignAddressBookLink link = Slx.GetEmailCampaignAddressBookLink(emailCampaign.Id, addressBookId);
                    if (link == null)
                        logger.WarnFormat("Cannot send email campaign ({0}) to address book ({1}) because that address book is not a target of the campaign", emailCampaign.Id, addressBookId);
                    else
                    {
                        ICollection<EmailAddressBookMember> membersThisAddrBook = Slx.GetEmailAddressBookMembers(addressBookId, subEntitiesAndFields);

                        if (membersThisAddrBook != null)
                            result.AddRange(membersThisAddrBook);

                        if (link.Sent != true)
                        {
                            link.Sent = true;
                            Slx.UpdateRecord(link);
                        }
                    }
                }
            }
            else
            {
                logger.DebugFormat("Using specified list of ({0}) members to send to.", specificMembers.Count);
                result = specificMembers.ToList<EmailAddressBookMember>();
            }

            logger.DebugFormat("There are ({0}) potential address book members to send to.", result.Count);
            return result;
        }

        private void PopulateListsOfContactsAndLeadsToWhichThisCampaignHasAlreadyBeenSent(EmailCampaign emailCampaign, HashSet<string> contactIdsAlreadySentTo, HashSet<string> leadIdsAlreadySentTo)
        {
            // Build a list of contacts/leads that have already been sent to, so we can filter them out later
            logger.Debug("AllowRepeatSendsToRecipient is turned off.  Building list of members already sent to");
            var summaries = Slx.GetEmailCampaignSendSummaries(emailCampaign.Id);
            foreach (var summary in summaries)
            {
                if (summary.SlxMemberType != null)
                {
                    switch (summary.SlxMemberType.ToUpperInvariant())
                    {
                        case "CONTACT":
                            if (!contactIdsAlreadySentTo.Contains(summary.SlxContactId))
                            {
                                contactIdsAlreadySentTo.Add(summary.SlxContactId);
                            }
                            break;

                        case "LEAD":
                            if (!leadIdsAlreadySentTo.Contains(summary.SlxLeadId))
                            {
                                leadIdsAlreadySentTo.Add(summary.SlxLeadId);
                            }
                            break;
                    }
                }
            }

            logger.DebugFormat("Finished building list of ({0}) leads and ({1}) contacts already sent to", leadIdsAlreadySentTo.Count, contactIdsAlreadySentTo.Count);
        }

        public static void ExtractDoNotSolicitFromPayload(SDataPayload baseEntityPayload, out bool doNotSolicit, out bool doNotEmail)
        {
            doNotEmail = false;
            doNotSolicit = false;
            const string FieldnameDoNotSolicit = "DoNotSolicit";
            if (baseEntityPayload.Values.ContainsKey(FieldnameDoNotSolicit) && baseEntityPayload.Values[FieldnameDoNotSolicit] != null)
            {
                bool.TryParse(baseEntityPayload.Values[FieldnameDoNotSolicit].ToString(), out doNotSolicit);
            }

            const string FieldnameDoNotEmail = "DoNotEmail";
            if (baseEntityPayload.Values.ContainsKey(FieldnameDoNotEmail) && baseEntityPayload.Values[FieldnameDoNotEmail] != null)
            {
                bool.TryParse(baseEntityPayload.Values[FieldnameDoNotEmail].ToString(), out doNotEmail);
            }
        }
    }
}