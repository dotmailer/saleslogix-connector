using System.Diagnostics;
using System.Reflection;
using dotMailer.Sdk.AddressBook;
using dotMailer.Sdk.Collections;
using dotMailer.Sdk.Contacts;
using dotMailer.Sdk.Enums;
using EmailMarketing.SalesLogix.Exceptions;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using dotMailer.Sdk;
    using dotMailer.Sdk.Contacts.DataFields;
    using Entities;
    using log4net;
    using Sage.SData.Client.Extensions;
    using Tasks;

    /// <summary>
    /// Synchronises Email Address Book headers and members
    /// </summary>
    public class EmailAddressBookSynchroniser
    {
        public const string StatusImportInProgress = "Import In Progress";
        public const string StatusInProgress = "In Progress";
        public const string StatusSyncComplete = "Sync Complete";

        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressBookSynchroniser"/> class.
        /// </summary>
        public EmailAddressBookSynchroniser(ISlxConnector slx, IDotMailerConnector dotMailer)
        {
            Slx = slx;
            DotMailer = dotMailer;
        }

        public IDotMailerConnector DotMailer { get; private set; }

        public ISlxConnector Slx { get; private set; }

        public void ExtractDataFieldsAndHash(ICollection<DataFieldMapping> dataMappings, EmailAddressBookMember newMember, string emailAddress, SDataPayload baseEntityPayload, out int fieldHash, out Dictionary<string, object> dataFieldValues)
        {
            fieldHash = 17;
            if (emailAddress != null)
            {
                fieldHash = unchecked((fieldHash * 31) + emailAddress.GetHashCode());
            }

            dataFieldValues = new Dictionary<string, object>();
            if (dataMappings != null)
            {
                foreach (var mapping in dataMappings.Where(
                    m => string.Equals(m.EntityType, newMember.SlxMemberType, StringComparison.OrdinalIgnoreCase)
                    && m.MapDirection != null && m.MapDirection.ToUpperInvariant().Contains(DataFieldMapping.InformationFlowsFromCrmPartialUpper)))
                {
                    object dataFieldValue = null;
                    if (string.IsNullOrWhiteSpace(mapping.LinkedFieldName))
                    {
                        if (baseEntityPayload.Values.ContainsKey(mapping.FieldName))
                        {
                            dataFieldValue = baseEntityPayload.Values[mapping.FieldName];
                        }
                    }
                    else
                    {
                        if (baseEntityPayload.Values.ContainsKey(mapping.FieldName)
                            && ((SDataPayload)baseEntityPayload.Values[mapping.FieldName]).Values.ContainsKey(mapping.LinkedFieldName))
                        {
                            dataFieldValue = ((SDataPayload)baseEntityPayload.Values[mapping.FieldName]).Values[mapping.LinkedFieldName];
                        }
                    }

                    if (dataFieldValue != null)
                    {
                        string dotMailerDataFieldName = mapping.DataLabel.Values["Name"].ToString();
                        if (dataFieldValues.ContainsKey(dotMailerDataFieldName) == false)
                        {
                            dataFieldValues[dotMailerDataFieldName] = dataFieldValue;
                            fieldHash = unchecked((fieldHash * 31) + dataFieldValue.GetHashCode());
                        }
                    }
                }
            }
        }

        public Dictionary<string, List<string>> GetSubEntitiesAndFields(string emailAccountId, out ICollection<DataFieldMapping> dataMappings)
        {
            Dictionary<string, List<string>> subEntitiesAndFields = BuildBaseEmailAddressBookEntitiesAndFields();
            dataMappings = Slx.GetDataMappings(emailAccountId);
            if (dataMappings != null)
            {
                foreach (var dataMapping in dataMappings.Where(m => !string.IsNullOrWhiteSpace(m.MapDirection)))
                {
                    string entityName;
                    string fieldName;
                    if (string.IsNullOrWhiteSpace(dataMapping.LinkedFieldName))
                    {
                        // The field is directly on Contact/Lead. e.g. Contact.WorkPhone
                        entityName = dataMapping.EntityType;
                        fieldName = dataMapping.FieldName;
                    }
                    else
                    {
                        // This is a field on a sub-entity of Contact/Lead. e.g. Contact.Address.Line1
                        entityName = string.Format("{0}/{1}", dataMapping.EntityType, dataMapping.FieldName);
                        fieldName = dataMapping.LinkedFieldName;
                    }

                    List<string> fieldsList;
                    bool found = subEntitiesAndFields.TryGetValue(entityName, out fieldsList);
                    if (!found)
                    {
                        fieldsList = new List<string>();
                        subEntitiesAndFields.Add(entityName, fieldsList);
                    }

                    fieldsList.Add(fieldName);
                }
            }

            return subEntitiesAndFields;
        }

        /// <summary>
        /// Returns a contact or lead's payload.
        /// </summary>
        /// <param name="newMember">The address book member to examine.</param>
        /// <param name="missingContactOrLeadInformationDetected">Returns true if important lead information was found to be missing in the passed member.</param>
        /// <returns>The sdata payload that was retrieved from the sub-entity (Lead or Contact)</returns>
        public SDataPayload GetSubEntityPayload(EmailAddressBookMember newMember, out bool missingContactOrLeadInformationDetected)
        {
            SDataPayload result;
            string emailAddress = IdentifyEmailAddressAndPayload(newMember, out result, out missingContactOrLeadInformationDetected);
            return result;
        }

        /// <summary>
        /// Depending on whether the member is a Contact or a Lead, gets the email address from the correct sub-entity
        /// and retrieves the sdata payload for that sub-entity.
        /// </summary>
        /// <param name="newMember">The address book member to process</param>
        /// <param name="baseEntityPayload">The sdata payload that was retrieved from the sub-entity (Lead or Contact)</param>
        /// <param name="missingContactOrLeadInformationDetected">Returns true if important lead information was found to be missing in the passed member.</param>
        /// <returns>The email address from the correct sub-entity.</returns>
        public string IdentifyEmailAddressAndPayload(EmailAddressBookMember newMember, out SDataPayload baseEntityPayload, out bool missingContactOrLeadInformationDetected)
        {
            baseEntityPayload = null;
            string emailAddress = string.Empty;

            ReportUnexpectedNullValuesInLog(newMember);

            missingContactOrLeadInformationDetected = false;
            object emailEntry;
            switch (newMember.SlxMemberType.ToUpperInvariant())
            {
                case "CONTACT":
                    if (newMember.Contact == null)
                    {
                        logger.DebugFormat("New contact's ({0}) details could not be found.", newMember.Id ?? "");
                        //The above can happen if a contact is added to an address book, then deleted before the address book is synced.

                        missingContactOrLeadInformationDetected = true;
                        return emailAddress;
                    }

                    emailEntry = newMember.Contact.Values["Email"];
                    if (emailEntry != null)
                        emailAddress = emailEntry.ToString();

                    baseEntityPayload = newMember.Contact;
                    break;

                case "LEAD":
                    if (newMember.Lead == null)
                    {
                        logger.DebugFormat("New lead's ({0}) details could not be found.", newMember.Id ?? "");
                        //At the time of writing, this can happen if a lead is converted to a contact.  While this may not be the ultimate fix,
                        //it allows the caller to omit processing this member.
                        //This can probably also happen in the circumstances detailed above for a contact.

                        missingContactOrLeadInformationDetected = true;
                        return emailAddress;
                    }

                    emailEntry = newMember.Lead.Values["Email"];
                    if (emailEntry != null)
                        emailAddress = emailEntry.ToString();

                    baseEntityPayload = newMember.Lead;
                    break;

                default:
                    throw new InvalidOperationException(string.Format("{0} is not a valid address book member type", newMember.SlxMemberType));
            }

            return emailAddress;
        }

        private static void ReportUnexpectedNullValuesInLog(EmailAddressBookMember newMember)
        {
            if (newMember == null)
                logger.Debug("newMember is null in EmailAddressBookSynchroniser.IdentifyEmailAddressAndPayload method.");
            else
            {
                if (newMember.SlxMemberType == null)
                    logger.Debug("newMember.SlxMemberType is null in EmailAddressBookSynchroniser.IdentifyEmailAddressAndPayload method.");
            }
        }

        public void ProcessSyncEmailAddressBookTask(SyncTask syncTask)
        {
            if (!string.Equals(syncTask.TaskType, "SynchroniseEmailAddressBook", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(string.Format("syncTask must have TaskType of SynchroniseEmailAddressBook, but was ({0}) for task with id ({1})", syncTask.TaskType, syncTask.Id));

            if (string.IsNullOrWhiteSpace(syncTask.TaskData))
                throw new ArgumentException(string.Format("syncTask.TaskData cannot be null or blank for task with id ({0})", syncTask.Id));

            string slxEmailAddressBookId = syncTask.TaskData;
            logger.DebugFormat("Address Book to be synchronised is ({0})", slxEmailAddressBookId);
            syncTask.Status = SyncTask.StatusInProgress;
            Slx.UpdateRecord(syncTask);

            try
            {
                Synchroniser.SyncDataLabelsOnAddressBooksAccount(slxEmailAddressBookId);
                bool addressBookNameWasReservedByEmailService = !SyncSingleAddressBookHeader(slxEmailAddressBookId);
                if (addressBookNameWasReservedByEmailService == false)
                    SyncEmailAddressBookMembers(slxEmailAddressBookId, SyncType.Manual, true);
            }
            catch (KeyEntityNotFoundException)
            {
                // The email address book does not exist.  Log it and ignore it.
                logger.InfoFormat("A Sync Task ({0}) was run for an Email Address Book that does not exist ({1}).  Was the Email Address Book deleted before the sync task was run?", syncTask.Id, slxEmailAddressBookId);
            }

            syncTask.Status = SyncTask.StatusComplete;
            Slx.UpdateRecord(syncTask);
        }

        /// <summary>
        /// Process contacts unsubscribing from a specific address book (not contacts unsubscribing globally)
        /// </summary>
        /// <param name="emailAccount">The email account of the address book to check</param>
        /// <param name="addressBook">The address book to check</param>
        public void ProcessUnsubscribers(EmailAccount emailAccount, EmailAddressBook addressBook)
        {
            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();

            int numProcessed = 0;
            logger.InfoFormat("Processing Unsubscribers for address book ({0})", addressBook.Name);
            DateTime lastSyncTime = emailAccount.LastSynchronised.GetValueOrDefault(DateTime.MinValue);
            DmContactCollection unsubscribers;
            try
            {
                unsubscribers = DotMailer.GetUnsubscribersForAddressBookSince(addressBook.EmailServiceAddressBookId, lastSyncTime);
            }
            catch (DmException ex)
            {
                if (ex.Code == DMErrorCodes.ERROR_ADDRESSBOOK_NOT_FOUND)
                {
                    logger.WarnFormat("Email Address Book does not exist in email service ({0})", addressBook.Name);
                    return;
                }
                else
                {
                    throw;
                }
            }

            if (unsubscribers != null)
            {
                foreach (var unsubscriber in unsubscribers)
                {
                    var bookMembers = Slx.GetEmailAddressBookMembersByEmailAddr(QueryEntityType.EmailAddressBook, addressBook.Id, unsubscriber.Email);
                    foreach (var bookMember in bookMembers)
                    {
                        // Are they already unsubscribed?
                        var existingUnsubscribe = Slx.GetEmailAddressBookUnsubscriber(bookMember.EmailAddressBookId, unsubscriber.Email);

                        if (existingUnsubscribe != null)
                        {
                            logger.DebugFormat("Email address ({0}) is already unsubscribed from email address book ({1}).", unsubscriber.Email, bookMember.EmailAddressBookId);
                        }
                        else
                        {
                            EmailAddressBookUnsubscriber newUnsubscribe = new EmailAddressBookUnsubscriber();
                            newUnsubscribe.EmailAccountId = emailAccount.Id;
                            newUnsubscribe.EmailAddressBookId = bookMember.EmailAddressBookId;
                            newUnsubscribe.SlxMemberType = bookMember.SlxMemberType;
                            newUnsubscribe.SlxContactId = bookMember.SlxContactId;
                            newUnsubscribe.SlxLeadId = bookMember.SlxLeadId;
                            newUnsubscribe.EmailAddress = unsubscriber.Email;
                            newUnsubscribe.EmailServiceContactId = unsubscriber.Id;
                            newUnsubscribe.DateUnsubscribed = DateTime.UtcNow;
                            newUnsubscribe.ReasonForUnsubscribing = "Unknown";

                            logger.DebugFormat("Unsubscribing member ({0}) from address book ({1}) and deleting address book member record", unsubscriber.Email, addressBook.Name);
                            Slx.CreateRecord(newUnsubscribe);
                            Slx.DeleteRecord<EmailAddressBookMember>(bookMember.Id);
                            numProcessed++;
                        }
                    }
                }
            }

            logger.InfoFormat("Finished processing ({1}) unsubscribers for address book ({0})", addressBook.Name, numProcessed);
        }

        public void SyncEmailAddressBookHeaders(EmailAccount emailAccount, SyncType syncType, DateTime syncTime)
        {
            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();

            ProcessDeletedAddressBookHeaders(emailAccount);

            // Get all SLX Email Address books for this email account
            ICollection<EmailAddressBook> slxBooks = Slx.GetEmailAddressBooks(emailAccount.Id);

            // Get all dotMailer address books for this email account
            DmAddressBookCollection dmBooks = DotMailer.GetAddressBooks();

            // Identify address books in slx that have not been sent to dotMailer
            IEnumerable<EmailAddressBook> newSlxBooks;
            switch (syncType)
            {
                case SyncType.Scheduled:
                    newSlxBooks = slxBooks.Where(b => b.EmailServiceAddressBookId == 0 && b.ManualSyncOnly == false);
                    break;

                case SyncType.Manual:
                    newSlxBooks = slxBooks.Where(b => b.EmailServiceAddressBookId == 0);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid SyncType: {0}", syncType));
            }

            List<string> booksCreatedInDmThisRun = new List<string>();

            // Send new slx address books to dotMailer
            if (newSlxBooks != null)
            {
                foreach (EmailAddressBook newSlxBook in newSlxBooks)
                {
                    if (string.Equals(newSlxBook.Name, "test", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(newSlxBook.EmailServiceAddressBookName, "test", StringComparison.OrdinalIgnoreCase))
                    {
                        logger.Warn("Not synchronising an address book called 'Test' from SLx to Email Service.  The 'Test' address book in the Email Service cannot be edited externally.");
                    }

                    LinkOrCreateDotmailerAddressBook(newSlxBook);
                    booksCreatedInDmThisRun.Add(newSlxBook.Name);
                }
            }

            // Look for address books that have been removed from dotMailer
            if (dmBooks != null && slxBooks != null)
            {
                foreach (EmailAddressBook slxBook in slxBooks)
                {
                    // Recreate the address book
                    DmAddressBook dmBook = dmBooks.FirstOrDefault(b => string.Equals(b.Name, slxBook.Name, StringComparison.OrdinalIgnoreCase));
                    if (dmBook == null && !booksCreatedInDmThisRun.Contains(slxBook.Name))
                        LinkOrCreateDotmailerAddressBook(slxBook);
                }
            }
        }

        public void SyncEmailAddressBookMembers(EmailAccount emailAccount, SyncType syncType, DateTime syncTime, bool resyncAll, bool cleanDeletedMembersFirst)
        {
            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();

            if (cleanDeletedMembersFirst)
                SalesLogixDeletionScanRunnable.RunForAllAddressBooks(Slx);

            // Get all SLX Email Address books for this email account
            ICollection<EmailAddressBook> slxBooks = Slx.GetEmailAddressBooks(emailAccount.Id);
            IEnumerable<EmailAddressBook> slxBooksToSync;
            switch (syncType)
            {
                case SyncType.Scheduled:
                    slxBooksToSync = slxBooks.Where(b => b.ManualSyncOnly == false);
                    break;

                case SyncType.Manual:
                    slxBooksToSync = slxBooks;
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid SyncType: {0}", syncType));
            }

            bool errorOccurred = false;
            foreach (var slxBook in slxBooksToSync)
            {
                try
                {
                    SyncEmailAddressBookMembers(emailAccount, slxBook, syncType, syncTime, resyncAll, false);
                }
                catch (Exception ex)
                {
                    string addressBookDescription = "(" + (emailAccount.AccountName ?? "") + "\\" + (slxBook.Name ?? "") + ")";
                    logger.Error("An exception was thrown whilst synchronising address book members " + addressBookDescription, ex);
                    errorOccurred = true;
                }
            }
            if (errorOccurred)
                throw new EmailAddressBookMemberSynchronisationException("The synchronisation of email address book members was not successful for all address books.");
        }

        public void SyncEmailAddressBookMembers(string slxEmailAddressBookId, SyncType syncType, bool cleanDeletedMembersFirst)
        {
            var slxEmailAddressBook = Slx.GetRecord<EmailAddressBook>(slxEmailAddressBookId);
            var emailAccount = Slx.GetRecord<EmailAccount>(slxEmailAddressBook.EmailAccountId);

            SyncEmailAddressBookMembers(emailAccount, slxEmailAddressBook, syncType, DateTime.UtcNow, false, cleanDeletedMembersFirst);
        }

        public virtual void SyncEmailAddressBookMembers(EmailAccount emailAccount, EmailAddressBook slxBook, SyncType syncType,
            DateTime syncTime, bool resyncAll, bool cleanDeletedMembersFirst)
        {
            DateTime startTime = DateTime.Now;
            logger.InfoFormat("Starting syncing address book ID {0}, starting at {1}.", slxBook.Id, startTime);
            try
            {
                if (cleanDeletedMembersFirst)
                    SalesLogixDeletionScanRunnable.RunForAddressBook(Slx, slxBook.Id);

                DotMailer.Username = emailAccount.ApiKey;
                DotMailer.Password = emailAccount.GetDecryptedPassword();

                // Test whether the Address Book actually exists in dotMailer
                bool shouldReturn;
                VerifyDotMailerAddrBook(slxBook, out shouldReturn);
                if (shouldReturn)
                    return;

                if (string.Equals(slxBook.SyncStatus, EmailAddressBook.SyncStatusDeletedFromService,
                    StringComparison.OrdinalIgnoreCase))
                {
                    logger.DebugFormat(
                        "Cannot sync members for email address book ({0}) because it was deleted from the email service",
                        slxBook.Name);
                    return;
                }

                if (string.Equals(slxBook.MemberSyncStatus, StatusImportInProgress, StringComparison.OrdinalIgnoreCase))
                {
                    string importGuidString;
                    string importResult;
                    ExtractImportResultAndGuid(slxBook, out importGuidString, out importResult);
                    if (!DotMailer.IsImportComplete(importResult))
                    {
                        // Do nothing until the import is finished.
                        logger.DebugFormat("Import still in progress for Email Address Book ({0})", slxBook.Name);
                        return;
                    }
                    else
                    {
                        logger.InfoFormat("Import is now complete for Email Address Book ({0})", slxBook.Name);
                        HandleCompletedImport(slxBook, importGuidString);
                    }
                }

                if (slxBook.ManualSyncOnly && syncType == SyncType.Scheduled)
                {
                    logger.DebugFormat(
                        "Not synchronising email address book ({0}).  It is manual sync only and this is a scheduled synchronisation.",
                        slxBook.Name);
                }

                logger.InfoFormat("Starting address book member sync ({0})", slxBook.Name);
                slxBook.MemberSyncStatus = StatusInProgress;
                Slx.UpdateEmailAddressBook(slxBook);

                int numberDeleted = ProcessDeletedAddressBookMembers(slxBook);

                ProcessUnsubscribers(emailAccount, slxBook);

                ICollection<DataFieldMapping> dataMappings;
                Dictionary<string, List<string>> subEntitiesAndFields = GetSubEntitiesAndFields(emailAccount.Id, out dataMappings);

                // Get members added since last sync
                logger.InfoFormat(
                    "Scanning new members added between ({1}) and ({2}) for address book ({0}) - new members will be batched for import in a later step",
                    slxBook.Name, slxBook.LastMemberChangeSynced, syncTime);
                HashSet<string> idsOfMembersLoaded = new HashSet<string>();
                ICollection<EmailAddressBookMember> newMembers = Slx.GetEmailAddressBookMembersAddedBetween(slxBook.Id,
                    slxBook.LastMemberChangeSynced, syncTime, subEntitiesAndFields);
                List<EmailServiceContact> contactsToImport = new List<EmailServiceContact>();
                int numNewMembers = 0;
                if (newMembers != null && newMembers.Count > 0)
                {
                    numNewMembers = newMembers.Count;
                    ProcessNewMembers(dataMappings, idsOfMembersLoaded, newMembers, contactsToImport);
                }

                logger.InfoFormat(
                    "Finished scanning new members for address book ({0}) - ({1}) members batched to be sent",
                    slxBook.Name, idsOfMembersLoaded.Count);

                // Get members modified since last sync
                DateTime? startDate;
                if (resyncAll)
                {
                    // load everything
                    startDate = SlxSdata.MinimumDateTimeValue;
                }
                else
                {
                    // Only load changes
                    startDate = slxBook.LastMemberChangeSynced;
                }

                logger.InfoFormat(
                    "Scanning updated members updated between ({1}) and ({2}) for address book ({0}) - updated members will be batched for import in a later step",
                    slxBook.Name, startDate, syncTime);
                var modifiedMembers = Slx.GetEmailAddressBookMembersModifiedBetween(slxBook.Id, startDate, syncTime,
                    subEntitiesAndFields);
                List<EmailAddressBookMember> modifiedMembersActuallySent = new List<EmailAddressBookMember>();
                List<EmailAddressBookMember> membersChangedToDoNotSolicitOrEmail = new List<EmailAddressBookMember>();
                if (modifiedMembers != null && modifiedMembers.Count > 0)
                {
                    ProcessModifiedMembers(slxBook, dataMappings, idsOfMembersLoaded, contactsToImport,
                        modifiedMembers, modifiedMembersActuallySent, out membersChangedToDoNotSolicitOrEmail);
                }

                logger.InfoFormat(
                    "Finished scanning updated members for address book ({0}) - ({1}) members batched to be sent",
                    slxBook.Name, modifiedMembersActuallySent.Count);

                RemoveDoNotSolicitMembers(slxBook, membersChangedToDoNotSolicitOrEmail);

                // Get modifed dotMailer contacts
                ProcessModifedDotMailerContacts(slxBook, dataMappings, subEntitiesAndFields, startDate);

                if (contactsToImport.Count > 0)
                    PerformImport(slxBook, newMembers, modifiedMembersActuallySent, contactsToImport);
                else
                {
                    // No changes made
                    slxBook.MemberSyncStatus = StatusSyncComplete;
                }

                slxBook.LastMemberChangeSynced = syncTime;
                //slxBook.EmailServiceAddressBookCount = this.Slx.CountEmailAddressBookMembers(slxBook.Id);
                var dmBook = DotMailer.GetAddressBook(slxBook.Name);
                if (dmBook != null)
                    slxBook.EmailServiceAddressBookContactCount = dmBook.NumberOfContacts;

                Slx.UpdateEmailAddressBook(slxBook);

                // if this address book is included in any New Member campaigns, then send to any new members
                if (numNewMembers > 0)
                {
                    var newmemberCampaigns = Slx.GetNewMemberEmailCampaigns(emailAccount.Id, slxBook.Id);
                    if (newmemberCampaigns != null && newmemberCampaigns.Count > 0)
                    {
                        var sender = new EmailCampaignSender(Slx, DotMailer);
                        foreach (var newmemberCampaign in newmemberCampaigns)
                        {
                            // Build the required temp address book
                            CampaignSendData sendData = new CampaignSendData();
                            sendData.CampaignId = newmemberCampaign.Id;
                            sendData.SendDate = DateTime.UtcNow;
                            int numAdded = sender.CreateAndPopulateTemporaryAddressBook(sendData, newMembers);

                            // Create a send task
                            if (numAdded > 0)
                            {
                                logger.InfoFormat("Submitting send to ({0}) members for NewMember campaign ({0})",
                                    numAdded, newmemberCampaign.Name);
                                SyncTask sendTask = new SyncTask();
                                sendTask.ScheduledStartTime = DateTime.UtcNow;
                                sendTask.TaskType = SyncTask.TaskTypeSendNewMemberCampaign;

                                XmlSerializer xs = new XmlSerializer(typeof(CampaignSendData));
                                StringWriter sw = new StringWriter();
                                xs.Serialize(sw, sendData);

                                sendTask.TaskData = sw.ToString();
                                sendTask.Status = "Pending";
                                Slx.CreateRecord(sendTask);

                                newmemberCampaign.LastNewMemberSend = DateTime.UtcNow;
                                Slx.UpdateRecord(newmemberCampaign);
                            }
                        }
                    }
                }
            }
            finally
            {
                DateTime endTime = DateTime.Now;
                logger.InfoFormat("Finished syncing address book ID {0}, finished at {1}, time taken = {2}.", slxBook.Id, endTime, endTime - startTime);
            }
        }

        public virtual void VerifyDotMailerAddrBook(EmailAddressBook slxBook, out bool shouldReturn)
        {
            shouldReturn = false;
            var dotMailerBook = this.DotMailer.GetAddressBook(slxBook.Name);
            if (dotMailerBook == null)
            {
                logger.ErrorFormat("An email address book with name ({0}) does not exist in the Email Service",
                    slxBook.Name);
                shouldReturn = true;
                return;
            }

            // Make sure it has the correct ID
            if (dotMailerBook.Id != slxBook.EmailServiceAddressBookId)
            {
                logger.ErrorFormat(
                    "The email address book called ({0}) has a different ID in the email service ({1}) than expected ({2})",
                    slxBook.Name, dotMailerBook.Id, slxBook.EmailServiceAddressBookId);
                shouldReturn = true;
                return;
            }
        }

        private static void ExtractFieldValue(DmDataFieldValue dmFieldValue, ref string valToAssign)
        {
            if (dmFieldValue.Definition != null)
            {
                switch (dmFieldValue.Definition.Type)
                {
                    case DmDataFieldTypes.Boolean:
                        valToAssign = "False";
                        if (dmFieldValue.Value != null)
                        {
                            bool tempbool = false;
                            if (bool.TryParse(dmFieldValue.Value.ToString(), out tempbool))
                            {
                                valToAssign = tempbool.ToString();
                            }
                        }

                        break;

                    case DmDataFieldTypes.String:
                        if (dmFieldValue.Value == null)
                        {
                            valToAssign = null;
                        }
                        else
                        {
                            valToAssign = dmFieldValue.Value.ToString();
                        }

                        break;

                    case DmDataFieldTypes.Decimal:
                        valToAssign = "0";
                        if (dmFieldValue.Value != null)
                        {
                            decimal tempDec;
                            if (decimal.TryParse(dmFieldValue.Value.ToString(), out tempDec))
                            {
                                valToAssign = decimal.ToInt32(tempDec).ToString();
                            }
                        }

                        break;

                    case DmDataFieldTypes.Date:
                        valToAssign = SlxSdata.MinimumDateTimeValue.ToString("s");
                        if (dmFieldValue.Value != null)
                        {
                            DateTime tempDate;
                            if (DateTime.TryParse(dmFieldValue.Value.ToString(), out tempDate))
                            {
                                valToAssign = tempDate.ToString("s");
                            }
                        }

                        break;
                }
            }
        }

        private DataFieldMapping[] GetDataFieldMappingsForContactsWithSaleslogixAsADestination(IEnumerable<DataFieldMapping> dataMappings)
        {
            if (dataMappings == null)
                return new DataFieldMapping[0];
            IEnumerable<DataFieldMapping> result = dataMappings.Where(m =>
                string.Equals(m.EntityType, "Contact", StringComparison.OrdinalIgnoreCase)
                && m.MapDirection != null &&
                m.MapDirection.ToUpperInvariant()
                    .Contains(DataFieldMapping.InformationFlowsToCrmPartialUpper));
            return result.ToArray();
        }

        private DataFieldMapping[] GetDataFieldMappingsForLeadsWithSaleslogixAsADestination(IEnumerable<DataFieldMapping> dataMappings)
        {
            if (dataMappings == null)
                return new DataFieldMapping[0];
            IEnumerable<DataFieldMapping> result = dataMappings.Where(m =>
                string.Equals(m.EntityType, "Lead", StringComparison.OrdinalIgnoreCase)
                && m.MapDirection != null &&
                m.MapDirection.ToUpperInvariant()
                    .Contains(DataFieldMapping.InformationFlowsToCrmPartialUpper));
            return result.ToArray();
        }

        /// <summary>
        /// Where a contact from this address book has had its data fields modified in dotMailer, this method updates
        /// Saleslogix.
        /// </summary>
        /// <param name="slxBook"></param>
        /// <param name="dataMappings"></param>
        /// <param name="subEntitiesAndFields"></param>
        /// <param name="startDate"></param>
        private void ProcessModifedDotMailerContacts(EmailAddressBook slxBook, ICollection<DataFieldMapping> dataMappings,
            Dictionary<string, List<string>> subEntitiesAndFields, DateTime? startDate)
        {
            logger.InfoFormat("Processing contacts modified in email service for address book ({0})", slxBook.Name);
            DataFieldMapping[] contactDataFieldMappings = GetDataFieldMappingsForContactsWithSaleslogixAsADestination(dataMappings);
            DataFieldMapping[] leadDataFieldMappings = GetDataFieldMappingsForLeadsWithSaleslogixAsADestination(dataMappings);
            if ((contactDataFieldMappings.Length == 0) && (leadDataFieldMappings.Length == 0))
            {
                logger.DebugFormat("Data mappings not found.  Not updating email address book ({0})", slxBook.Name);
                return; //If no data field mappings have Saleslogix as a destination then there is no work to do here.
            }

            DateTime nonNullStartTime = startDate.GetValueOrDefault(SlxSdata.MinimumDateTimeValue);
            logger.DebugFormat("Finding contacts modified in email service since ({0}) UTC", nonNullStartTime);
            DmContactCollection modifiedDotMailerContacts = DotMailer.GetContactsInAddressBookModifiedSince(slxBook.EmailServiceAddressBookId, nonNullStartTime);
            if (modifiedDotMailerContacts != null)
            {
                logger.DebugFormat("Found ({0}) email service contacts with changes", modifiedDotMailerContacts.Count);

                const int numberOfSaleslogixContactsToRetrieveAtATime = 7; //We batch up contacts that dotMailer reports as modified, so that we can ask Saleslogix for them more than one at a time.
                ProcessModifiedDotMailerContactsInBatches(slxBook, subEntitiesAndFields, modifiedDotMailerContacts, numberOfSaleslogixContactsToRetrieveAtATime, contactDataFieldMappings, leadDataFieldMappings);
            }

            logger.InfoFormat("Finished processing contacts modified in email service for address book ({0})", slxBook.Name);
        }

        private void ProcessModifiedDotMailerContactsInBatches(EmailAddressBook slxBook, Dictionary<string, List<string>> subEntitiesAndFields, DmContactCollection modifiedDotMailerContacts,
            int numberOfSaleslogixContactsToRetrieveAtATime, DataFieldMapping[] contactDataFieldMappings, DataFieldMapping[] leadDataFieldMappings)
        {
            using (SaleslogixRecordUpdater<EmailAddressBookMember> saleslogixRecordUpdater = new SaleslogixRecordUpdater<EmailAddressBookMember>(Slx))
            {
                saleslogixRecordUpdater.UseBatchUpdatingMethod = ObjectFactory.Instance.Settings.BatchUpdateSaleslogixDataFields;

                for (int modifiedDmContactIndex = 0; modifiedDmContactIndex < modifiedDotMailerContacts.Count; modifiedDmContactIndex += numberOfSaleslogixContactsToRetrieveAtATime)
                {
                    string[] contactEmailAddresses = CreateListOfEmailAddressesForThisBatch(modifiedDotMailerContacts, numberOfSaleslogixContactsToRetrieveAtATime, modifiedDmContactIndex);
                    Dictionary<string, DmContact> emailAddressToContactDictionary = CreateEmailAddressToContactDictionary(modifiedDotMailerContacts, numberOfSaleslogixContactsToRetrieveAtATime,
                        modifiedDmContactIndex);

                    ICollection<EmailAddressBookMember> slxAddressBookMembersInBatch =
                        Slx.GetEmailAddressBookMembersByEmailAddresses(QueryEntityType.EmailAddressBook, slxBook.Id, contactEmailAddresses, subEntitiesAndFields);

                    if (slxAddressBookMembersInBatch != null)
                    {
                        LogAnyChangedContactsThatDoNotAppearInTheSaleslogixAddressBook(slxBook, contactEmailAddresses, slxAddressBookMembersInBatch);
                        ProcessBatchOfModifiedDotMailerContacts(slxBook, contactDataFieldMappings, leadDataFieldMappings, slxAddressBookMembersInBatch, emailAddressToContactDictionary, saleslogixRecordUpdater);
                    }
                }
            }
        }

        private void ProcessBatchOfModifiedDotMailerContacts(EmailAddressBook slxBook, DataFieldMapping[] contactDataFieldMappings,
            DataFieldMapping[] leadDataFieldMappings, ICollection<EmailAddressBookMember> slxAddressBookMembersInBatch,
            Dictionary<string, DmContact> emailAddressToContactDictionary, SaleslogixRecordUpdater<EmailAddressBookMember> saleslogixRecordUpdater)
        {
            foreach (EmailAddressBookMember slxAddrBookMember in slxAddressBookMembersInBatch)
            {
                //Find the matching dotMailer contact for this Saleslogix address book member.
                DmContact dotMailerContact;
                if (emailAddressToContactDictionary.TryGetValue(slxAddrBookMember.LastSyncedEmailAddress.ToUpperInvariant(), out dotMailerContact))
                {
                    // Only if not null, do an update.  Don't create if null because we wouldn't be able to link it to an SLX Lead/Contact                            {
                    if (slxAddrBookMember == null)
                    {
                        logger.DebugFormat("Address book member is null for email address ({0}) in address book ({1})", dotMailerContact.Email, slxBook.Name);
                        continue;
                    }

                    if (GetLinkedEntityIsValid(slxAddrBookMember))
                        UpdateSaleslogixAddressBookMemberFromDotMailerContactsDataFields(contactDataFieldMappings, leadDataFieldMappings, slxAddrBookMember, dotMailerContact, saleslogixRecordUpdater);
                }
                else
                {
                    logger.DebugFormat("Saleslogix returned an address book member with an unexpected email address ({0}) whilst synchronising address book: ({1})",
                        slxAddrBookMember.LastSyncedEmailAddress, slxBook.Name);
                    continue;
                }
            }
        }

        private void UpdateSaleslogixAddressBookMemberFromDotMailerContactsDataFields(DataFieldMapping[] contactDataFieldMappings, DataFieldMapping[] leadDataFieldMappings, EmailAddressBookMember slxAddrBookMember,
            DmContact dotMailerContact, SaleslogixRecordUpdater<EmailAddressBookMember> saleslogixRecordUpdater)
        {
            bool memberHasMissingInformation;
            SDataPayload baseEntityPayload = GetSubEntityPayload(slxAddrBookMember, out memberHasMissingInformation);
            Debug.Assert(memberHasMissingInformation == false);
            if (memberHasMissingInformation)
                return;

            bool addressBookMemberIsAContact = string.Equals(slxAddrBookMember.SlxMemberType, "Contact", StringComparison.OrdinalIgnoreCase);
            DataFieldMapping[] contactOrLeadMappings = addressBookMemberIsAContact ? contactDataFieldMappings : leadDataFieldMappings; //these mappings are only those whose direction is -into- Saleslogix

            bool addrBookMemberModified = false;
            foreach (DataFieldMapping mapping in contactOrLeadMappings)
                UpdateSaleslogixAddressBookMembersDataFieldFromDotMailerContact(dotMailerContact, mapping, baseEntityPayload, ref addrBookMemberModified);

            if (addrBookMemberModified)
            {
                logger.DebugFormat("Updating address book member ({0}), ({1})", slxAddrBookMember.Id, slxAddrBookMember.LastSyncedEmailAddress);
                saleslogixRecordUpdater.Update(slxAddrBookMember);
            }
            else
            {
                logger.DebugFormat(
                    "Email service contact has not actually changed.  Not updating email address book member ({0}), ({1})",
                    slxAddrBookMember.Id, slxAddrBookMember.LastSyncedEmailAddress);
            }
        }

        private static void UpdateSaleslogixAddressBookMembersDataFieldFromDotMailerContact(DmContact dotMailerContact, DataFieldMapping fieldMapping, SDataPayload baseEntityPayload, ref bool slxAddrBookMemberModified)
        {
            string mappingDataFieldNameInDotMailer = fieldMapping.DataLabel.Values["Name"].ToString();
            DmDataFieldValue dmFieldValue =
                dotMailerContact.DataFields.FirstOrDefault(f => string.Equals(f.Definition.Name, mappingDataFieldNameInDotMailer, StringComparison.OrdinalIgnoreCase));

            string valueToAssign = null;
            ExtractFieldValue(dmFieldValue, ref valueToAssign);

            if (string.IsNullOrWhiteSpace(fieldMapping.LinkedFieldName))
            {
                if (baseEntityPayload.Values.ContainsKey(fieldMapping.FieldName))
                {
                    if (!Equals(baseEntityPayload.Values[fieldMapping.FieldName], valueToAssign))
                    {
                        // Values are different, so update
                        baseEntityPayload.Values[fieldMapping.FieldName] = valueToAssign;
                        slxAddrBookMemberModified = true;
                    }
                }
            }
            else
            {
                if (baseEntityPayload.Values.ContainsKey(fieldMapping.FieldName) && ((SDataPayload)baseEntityPayload.Values[fieldMapping.FieldName]).Values.ContainsKey(fieldMapping.LinkedFieldName))
                {
                    if (!Equals(((SDataPayload)baseEntityPayload.Values[fieldMapping.FieldName]).Values[fieldMapping.LinkedFieldName], valueToAssign))
                    {
                        // Values are different, so update
                        ((SDataPayload)baseEntityPayload.Values[fieldMapping.FieldName])
                            .Values[fieldMapping.LinkedFieldName] = valueToAssign;
                        slxAddrBookMemberModified = true;
                    }
                }
            }
        }

        private static bool GetLinkedEntityIsValid(EmailAddressBookMember slxAddrBookMember)
        {
            bool result = true;

            string invalidEntityMessage = "Invalid Entity";
            switch (slxAddrBookMember.SlxMemberType.ToUpperInvariant())
            {
                case "CONTACT":
                    if (string.IsNullOrWhiteSpace(slxAddrBookMember.SlxContactId) || slxAddrBookMember.Contact == null)
                    {
                        result = false;
                        invalidEntityMessage = string.Format("Address book member ({0}) does not have a valid Contact and will not be updated ({1})",
                            slxAddrBookMember.Id, slxAddrBookMember.LastSyncedEmailAddress);
                    }

                    break;

                case "LEAD":
                    if (string.IsNullOrWhiteSpace(slxAddrBookMember.SlxLeadId) || slxAddrBookMember.Lead == null)
                    {
                        result = false;
                        invalidEntityMessage =
                            string.Format("Address book member ({0}) does not have a valid Lead and will not be updated ({1})", slxAddrBookMember.Id, slxAddrBookMember.LastSyncedEmailAddress);
                    }

                    break;

                default:
                    throw new InvalidOperationException(string.Format("{0} is not a valid address book member type", slxAddrBookMember.SlxMemberType));
            }

            if (!result)
                logger.Debug(invalidEntityMessage);

            return result;
        }

        private static void LogAnyChangedContactsThatDoNotAppearInTheSaleslogixAddressBook(EmailAddressBook slxBook, string[] contactEmailAddresses, ICollection<EmailAddressBookMember> slxAddressBookMembersInBatch)
        {
            foreach (string contactEmailAddress in contactEmailAddresses)
            {
                string uppercaseEmailAddress = contactEmailAddress.ToUpperInvariant();
                bool emailAddressExistsInReturnedSaleslogixMembers =
                    slxAddressBookMembersInBatch.Any(emailAddressBookMember => emailAddressBookMember.LastSyncedEmailAddress.ToUpperInvariant() == uppercaseEmailAddress);
                if (!emailAddressExistsInReturnedSaleslogixMembers)
                    logger.DebugFormat("Saleslogix address book member not found for email address ({0}) in address book ({1})", contactEmailAddress, slxBook.Name);
            }
        }

        private static Dictionary<string, DmContact> CreateEmailAddressToContactDictionary(DmContactCollection modifiedDotMailerContacts, int numberOfSaleslogixContactsToRetrieveAtATime, int modifiedContactIndex)
        {
            Dictionary<string, DmContact> result = new Dictionary<string, DmContact>();
            int loopLimit = Math.Min(modifiedContactIndex + numberOfSaleslogixContactsToRetrieveAtATime, modifiedDotMailerContacts.Count);
            for (int index = modifiedContactIndex; index < loopLimit; index++)
            {
                DmContact modifiedContact = modifiedDotMailerContacts[index];
                string emailAddress = modifiedContact.Email;
                result.Add(emailAddress.ToUpperInvariant(), modifiedContact);
            }
            return result;
        }

        private static string[] CreateListOfEmailAddressesForThisBatch(DmContactCollection modifiedDotMailerContacts, int numberOfSaleslogixContactsToRetrieveAtATime, int modifiedContactIndex)
        {
            List<string> result = new List<string>();
            int loopLimit = Math.Min(modifiedContactIndex + numberOfSaleslogixContactsToRetrieveAtATime, modifiedDotMailerContacts.Count);
            for (int index = modifiedContactIndex; index < loopLimit; index++)
            {
                DmContact modifiedContact = modifiedDotMailerContacts[index];
                string emailAddress = modifiedContact.Email;
                result.Add(emailAddress);
            }
            return result.ToArray();
        }

        private Dictionary<string, List<string>> BuildBaseEmailAddressBookEntitiesAndFields()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Contact", new List<string>() { "email", "donotsolicit", "donotemail" } },
                { "Lead", new List<string>() { "email", "donotsolicit", "donotemail" } }
            };
        }

        private void ExtractImportResultAndGuid(EmailAddressBook slxBook, out string importGuidString, out string importResult)
        {
            if (slxBook.MemberSyncProgressObject == null || slxBook.MemberSyncProgressObject.Length < 38)
            {
                throw new InvalidOperationException("MemberSyncProgressObject must contain a GUID and a serialised object");
            }

            importGuidString = slxBook.MemberSyncProgressObject.Substring(0, 38);
            importResult = slxBook.MemberSyncProgressObject.Substring(38);

            if (!string.IsNullOrWhiteSpace(importResult) && importResult.Substring(0, 1) == "?")
            {
                // For some reason sdata puts a stray '?' on the start of this serialised object when it stores it.
                importResult = importResult.Substring(1);
            }
        }

        private void HandleCompletedImport(EmailAddressBook slxBook, string importGuidString)
        {
            // Update the hashcodes now that the import is complete.
            Guid importGuid = Guid.Parse(importGuidString);
            var membersFromimport = Slx.GetEmailAddressBookMembersFromImport(slxBook.Id, importGuid);
            if (membersFromimport != null)
            {
                foreach (var memberFromImport in membersFromimport)
                {
                    memberFromImport.LastSyncHashCode = memberFromImport.PendingImporthashCode;
                }

                if (membersFromimport.Count > 0)
                {
                    Slx.BatchUpdateRecords<EmailAddressBookMember>(membersFromimport);
                }
            }
        }

        private void PerformImport(EmailAddressBook slxBook, ICollection<EmailAddressBookMember> newMembers, List<EmailAddressBookMember> modifiedMembersActuallySent, List<EmailServiceContact> contactsToImport)
        {
            logger.DebugFormat("Importing contacts to email service for address book ({0})", slxBook.Name);
            bool importComplete;
            string importResult;
            Guid importGuid = Guid.NewGuid();
            string importGuidString = importGuid.ToString("B");
            DotMailer.ImportContactsIntoAddressBook(slxBook.EmailServiceAddressBookId, contactsToImport, out importComplete, out importResult);

            if (importComplete)
            {
                logger.DebugFormat("Contact import complete for address book ({0})", slxBook.Name);
                foreach (var member in newMembers)
                {
                    member.LastSyncHashCode = member.PendingImporthashCode;
                    member.LastEmailServiceImport = importGuidString;
                }

                foreach (var member in modifiedMembersActuallySent)
                {
                    member.LastSyncHashCode = member.PendingImporthashCode;
                    member.LastEmailServiceImport = importGuidString;
                }

                slxBook.MemberSyncStatus = StatusSyncComplete;
            }
            else
            {
                logger.DebugFormat("Contact import with id ({1}) submitted but has not completed for address book ({0})", slxBook.Name, importGuidString);
                if (newMembers != null)
                {
                    foreach (var member in newMembers)
                    {
                        member.LastEmailServiceImport = importGuidString;
                    }
                }

                if (modifiedMembersActuallySent != null)
                {
                    foreach (var member in modifiedMembersActuallySent)
                    {
                        member.LastEmailServiceImport = importGuidString;
                    }
                }

                slxBook.MemberSyncStatus = StatusImportInProgress;
                slxBook.MemberSyncProgressObject = importGuidString + importResult;
            }

            if (modifiedMembersActuallySent != null && modifiedMembersActuallySent.Count > 0)
            {
                logger.DebugFormat("Started flagging ({0}) Modified Email Address Book Members as imported.", modifiedMembersActuallySent.Count);
                Slx.BatchUpdateRecords<EmailAddressBookMember>(modifiedMembersActuallySent);
                logger.DebugFormat("Finished flagging ({0}) Email Address Book Members as imported.", modifiedMembersActuallySent.Count);
            }

            if (newMembers != null && newMembers.Count > 0)
            {
                logger.DebugFormat("Started flagging ({0}) New Email Address Book Members as imported.", newMembers.Count);
                Slx.BatchUpdateRecords<EmailAddressBookMember>(newMembers);
                logger.DebugFormat("Finished flagging ({0}) New Email Address Book Members as imported.", newMembers.Count);
            }
        }

        //Where address books have been deleted in Saleslogix, ensures they have also been deleted in dotMailer.
        private void ProcessDeletedAddressBookHeaders(EmailAccount emailAccount)
        {
            // Get members deleted from this address book
            ICollection<DeletedItem> deletedAddressBooks = Slx.GetDeletedItemsUnprocessed(EmailAddressBook.EntityName);
            if (deletedAddressBooks != null)
            {
                foreach (DeletedItem deletedAddressBook in deletedAddressBooks)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(DeletedAddressBookDetails));
                    StringReader sr = new StringReader(deletedAddressBook.Data);

                    DeletedAddressBookDetails deletedAddressBookDetails = null;
                    try
                    {
                        deletedAddressBookDetails = (DeletedAddressBookDetails)xs.Deserialize(sr);
                    }
                    catch (InvalidOperationException ex)
                    {
                        logger.WarnFormat("Deleted Item Entity ({0}), of type ({1}) relating to ID ({2}) could not be deserialized due to exception:{3}{4}", deletedAddressBook.Id, deletedAddressBook.EntityType, deletedAddressBook.EntityId, Environment.NewLine, ex);
                    }

                    if (deletedAddressBookDetails != null)
                    {
                        if (string.IsNullOrWhiteSpace(deletedAddressBookDetails.EmailAddressBookName))
                            logger.DebugFormat("Not Deleting address book ({0}) because the name was blank/null", deletedAddressBookDetails.EmailAddressBookId);
                        else
                        {
                            logger.DebugFormat("Deleting address book ({0}) from email service", deletedAddressBookDetails.EmailAddressBookName);
                            try
                            {
                                DotMailer.DeleteAddressBook(deletedAddressBookDetails.EmailAddressBookName);
                            }
                            catch (InvalidOperationException)
                            {
                                // log it and ignore it
                                logger.InfoFormat("Address book ({0}) does not exist in email service so cannot be deleted", deletedAddressBookDetails.EmailAddressBookName);
                            }
                        }
                    }

                    deletedAddressBook.ProcessedByEMSync = true;
                    Slx.UpdateDeletedItem(deletedAddressBook);
                }
            }
        }

        private int ProcessDeletedAddressBookMembers(EmailAddressBook slxBook)
        {
            int numberProcessed = 0;

            // Get members deleted from this address book
            logger.InfoFormat("Processing deleted members for address book ({0})", slxBook.Name);
            var deletedMembers = Slx.GetDeletedItemsUnprocessed(EmailAddressBookMember.EntityName, string.Format("%{0}%", slxBook.Id));
            List<string> emailAddressesToDelete = new List<string>();
            if (deletedMembers != null)
            {
                int totalNumRecordsToProcess = deletedMembers.Count;
                foreach (var deletedMember in deletedMembers)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(DeletedMemberDetails));
                    StringReader sr = new StringReader(deletedMember.Data);

                    DeletedMemberDetails deletedMemberDetails = null;
                    try
                    {
                        deletedMemberDetails = (DeletedMemberDetails)xs.Deserialize(sr);
                    }
                    catch (InvalidOperationException ex)
                    {
                        logger.WarnFormat("Deleted Item Entity ({0}), of type ({1}) relating to ID ({2}) could not be deserialized due to exception:{3}{4}", deletedMember.Id, deletedMember.EntityType, deletedMember.EntityId, Environment.NewLine, ex);
                    }

                    if (deletedMemberDetails != null)
                    {
                        numberProcessed++;
                        if (string.IsNullOrWhiteSpace(deletedMemberDetails.EmailAddress))
                        {
                            logger.DebugFormat("Not Deleting member ({0}) from email service address book ({1}) because the email address was blank/null", deletedMemberDetails.MemberId, slxBook.Name);
                        }
                        else
                        {
                            logger.DebugFormat("Batching member #({2}) of ({3}) id = ({0}) to be deleted from email service address book ({1})", deletedMemberDetails.MemberId, slxBook.Name, numberProcessed, totalNumRecordsToProcess);
                            emailAddressesToDelete.Add(deletedMemberDetails.EmailAddress);
                        }
                    }
                }

                if (emailAddressesToDelete.Count > 0)
                {
                    DotMailer.DeleteAddressBookContacts(slxBook.EmailServiceAddressBookId, emailAddressesToDelete);
                }

                // Update deleted items to indicate that they have been processed
                if (deletedMembers.Count > 0)
                {
                    logger.Debug("Batching deleted items to be marked as processed");
                    foreach (var deletedMember in deletedMembers)
                    {
                        deletedMember.ProcessedByEMSync = true;
                    }

                    logger.Debug("Started marking deleted items as processed");
                    Slx.BatchUpdateRecords(deletedMembers);
                    logger.Debug("Finished marking deleted items as processed");
                }
            }

            logger.InfoFormat("Finished processing ({1}) members for address book ({0})", slxBook.Name, numberProcessed);
            return numberProcessed;
        }

        private void ProcessModifiedMembers(EmailAddressBook slxBook, ICollection<DataFieldMapping> dataMappings,
            HashSet<string> idsOfMembersLoaded, List<EmailServiceContact> contactsToImport,
            ICollection<EmailAddressBookMember> modifiedMembers, List<EmailAddressBookMember> modifiedMembersActuallySent,
            out List<EmailAddressBookMember> membersChangedToDoNotSolicitOrEmail)
        {
            membersChangedToDoNotSolicitOrEmail = new List<EmailAddressBookMember>();
            List<string> emailAddressesToDelete = new List<string>();
            int numProcessed = 0;
            foreach (var modifiedMember in modifiedMembers)
            {
                numProcessed++;
                if (idsOfMembersLoaded.Contains(modifiedMember.Id))
                {
                    // Do not try to update a member/contact we just created.
                    logger.InfoFormat("Modified address book member {0} of {1} is already in the list of new members", numProcessed, modifiedMembers.Count);
                    continue;
                }

                EmailServiceContact tempContact = new EmailServiceContact();
                tempContact.DataFieldValues = new Dictionary<string, object>();
                SDataPayload baseEntityPayload = null;
                int fieldHash = 17;
                object emailEntry;
                bool linkedEntityIsMissing = false;
                string missingLinkedEntityMessage = "Linked Entity is Missing";
                switch (modifiedMember.SlxMemberType.ToUpperInvariant())
                {
                    case "CONTACT":
                        if (string.IsNullOrWhiteSpace(modifiedMember.SlxContactId) || modifiedMember.Contact == null)
                        {
                            linkedEntityIsMissing = true;
                            missingLinkedEntityMessage = string.Format("Modified address book member {0} of {1} does not have a valid Contact and will not be sent to the Email Service ({2})", numProcessed, modifiedMembers.Count, modifiedMember.LastSyncedEmailAddress);
                        }
                        else
                        {
                            emailEntry = modifiedMember.Contact.Values["Email"];
                            if (emailEntry != null)
                            {
                                tempContact.EmailAddress = emailEntry.ToString();
                            }

                            baseEntityPayload = modifiedMember.Contact;
                        }

                        break;

                    case "LEAD":
                        if (string.IsNullOrWhiteSpace(modifiedMember.SlxLeadId) || modifiedMember.Lead == null)
                        {
                            linkedEntityIsMissing = true;
                            missingLinkedEntityMessage = string.Format("Modified address book member {0} of {1} does not have a valid Lead and will not be sent to the Email Service ({2})", numProcessed, modifiedMembers.Count, modifiedMember.LastSyncedEmailAddress);
                        }
                        else
                        {
                            emailEntry = modifiedMember.Lead.Values["Email"];
                            if (emailEntry != null)
                            {
                                tempContact.EmailAddress = emailEntry.ToString();
                            }

                            baseEntityPayload = modifiedMember.Lead;
                        }
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("{0} is not a valid address book member type", modifiedMember.SlxMemberType));
                }

                if (linkedEntityIsMissing)
                {
                    logger.Debug(missingLinkedEntityMessage);
                }
                else
                {
                    bool doNotSolicit;
                    bool doNotEmail;
                    EmailCampaignSender.ExtractDoNotSolicitFromPayload(baseEntityPayload, out doNotSolicit, out doNotEmail);

                    if (string.IsNullOrWhiteSpace(tempContact.EmailAddress))
                    {
                        logger.InfoFormat("Modified address book member {1} of {2} ({0}) does not have an email address and will not be sent to the Email Service.", modifiedMember.Id, numProcessed, modifiedMembers.Count);
                    }
                    else if (doNotSolicit || doNotEmail)
                    {
                        if (modifiedMember.LastSyncHashCode == null)
                        {
                            // This addr book member has not been loaded yet (because of doNotXXXXX or no email address).
                            // Do not load it
                            logger.DebugFormat("Modified address book member {2} of {3} ({0} - {1}) is marked 'Do Not Solicit' or 'Do Not Email' and will not be sent to the Email Service", modifiedMember.Id, tempContact.EmailAddress, numProcessed, modifiedMembers.Count);
                        }
                        else
                        {
                            // This addr book member has already been loaded.
                            // We will need to remove it from dotMailer, so add it to the list to be processed later.
                            logger.DebugFormat("Modified address book member {2} of {3} ({0} - {1}) is now marked 'Do Not Solicit' or 'Do Not Email' and will be removed from the Email Service", modifiedMember.Id, tempContact.EmailAddress, numProcessed, modifiedMembers.Count);
                            membersChangedToDoNotSolicitOrEmail.Add(modifiedMember);
                        }
                    }
                    else
                    {
                        if (tempContact.EmailAddress != null)
                        {
                            fieldHash = unchecked((fieldHash * 31) + tempContact.EmailAddress.GetHashCode());
                        }

                        if (dataMappings != null)
                        {
                            foreach (var mapping in dataMappings.Where(
                                m => string.Equals(m.EntityType, modifiedMember.SlxMemberType, StringComparison.OrdinalIgnoreCase)
                                && m.MapDirection != null && m.MapDirection.ToUpperInvariant().Contains(DataFieldMapping.InformationFlowsFromCrmPartialUpper)))
                            {
                                object dataFieldValue;
                                if (string.IsNullOrWhiteSpace(mapping.LinkedFieldName))
                                {
                                    dataFieldValue = baseEntityPayload.Values[mapping.FieldName];
                                }
                                else
                                {
                                    dataFieldValue = ((SDataPayload)baseEntityPayload.Values[mapping.FieldName]).Values[mapping.LinkedFieldName];
                                }

                                if (dataFieldValue != null)
                                {
                                    string dotMailerDataFieldName = mapping.DataLabel.Values["Name"].ToString();
                                    if (tempContact.DataFieldValues.ContainsKey(dotMailerDataFieldName) == false)
                                    {
                                        tempContact.DataFieldValues[dotMailerDataFieldName] = dataFieldValue;
                                        fieldHash = unchecked((fieldHash * 31) + dataFieldValue.GetHashCode());
                                    }
                                }
                            }
                        }

                        // If the email address has changed (and it wasn't empty before), then we must delete the email service contact
                        // before loading because the email address is used as the key.
                        if (!string.Equals(modifiedMember.LastSyncedEmailAddress, tempContact.EmailAddress, StringComparison.OrdinalIgnoreCase)
                            && !String.IsNullOrWhiteSpace(modifiedMember.LastSyncedEmailAddress))
                        {
                            logger.DebugFormat("Modified address book member {3} of {4} ({0} - {1}) has changed email address from ({2}) so has been batched to be deleted from email service and will subsequently be re-added", modifiedMember.Id, tempContact.EmailAddress, modifiedMember.LastSyncedEmailAddress, numProcessed, modifiedMembers.Count);
                            emailAddressesToDelete.Add(modifiedMember.LastSyncedEmailAddress);
                        }

                        // If the fieldhash is the same as when last synced, then the fields sent to the email service
                        // have not changed, so don't bother to send it.
                        // E.g. The WebUrl on a Contact was changed (so the ModifyDate was updated), but only
                        // firstname and lastname data mappings are set up.
                        if (modifiedMember.LastSyncHashCode != fieldHash)
                        {
                            contactsToImport.Add(tempContact);
                            if (!idsOfMembersLoaded.Contains(modifiedMember.Id))
                            {
                                logger.DebugFormat("Modified address book member {2} of {3} ({0} - {1}) has been batched to be sent to the Email Service", modifiedMember.Id, tempContact.EmailAddress, numProcessed, modifiedMembers.Count);
                                idsOfMembersLoaded.Add(modifiedMember.Id);
                            }

                            modifiedMember.PendingImporthashCode = fieldHash;
                            modifiedMember.LastSyncedEmailAddress = tempContact.EmailAddress;
                            modifiedMembersActuallySent.Add(modifiedMember);
                        }
                    }
                }
            }

            // Process batch of Email Contacts to be deleted (due to having email address changed - see above)
            if (emailAddressesToDelete.Count > 0)
            {
                DotMailer.DeleteAddressBookContacts(slxBook.EmailServiceAddressBookId, emailAddressesToDelete);
            }
        }

        /// <summary>
        /// Adds contacts representing the items in the newMembers parameter to the contactsToImport list,
        /// but ensuring each is added once only (idsOfMembersLoaded is used to track this).
        /// </summary>
        /// <param name="dataMappings"></param>
        /// <param name="idsOfMembersLoaded"></param>
        /// <param name="newMembers"></param>
        /// <param name="contactsToImport"></param>
        private void ProcessNewMembers(ICollection<DataFieldMapping> dataMappings, HashSet<string> idsOfMembersLoaded, ICollection<EmailAddressBookMember> newMembers, List<EmailServiceContact> contactsToImport)
        {
            int numProcessed = 0;
            foreach (var newMember in newMembers)
            {
                numProcessed++;
                EmailServiceContact tempContact = new EmailServiceContact();
                tempContact.DataFieldValues = new Dictionary<string, object>();
                SDataPayload baseEntityPayload;
                bool memberHasMissingInformation;
                string emailAddress = IdentifyEmailAddressAndPayload(newMember, out baseEntityPayload, out memberHasMissingInformation);

                if (!memberHasMissingInformation)
                {
                    bool doNotSolicit;
                    bool doNotEmail;
                    EmailCampaignSender.ExtractDoNotSolicitFromPayload(baseEntityPayload, out doNotSolicit,
                                                                       out doNotEmail);

                    if (string.IsNullOrWhiteSpace(emailAddress))
                    {
                        logger.DebugFormat(
                            "New address book member {1} of {2} ({0}) does not have an email address and will not be sent to the Email Service.",
                            newMember.Id, numProcessed, newMembers.Count);
                    }
                    else if (doNotSolicit || doNotEmail)
                    {
                        logger.DebugFormat(
                            "New address book member {1} of {2} ({0}) is marked 'Do Not Solicit' or 'Do Not Email' and will not be sent to the Email Service",
                            newMember.Id, numProcessed, newMembers.Count);
                    }
                    else
                    {
                        tempContact.EmailAddress = emailAddress;
                        int fieldHash;
                        Dictionary<string, object> dataFieldValues;
                        ExtractDataFieldsAndHash(dataMappings, newMember, emailAddress, baseEntityPayload,
                                                      out fieldHash, out dataFieldValues);

                        tempContact.DataFieldValues = dataFieldValues;
                        contactsToImport.Add(tempContact);
                        if (idsOfMembersLoaded.Contains(newMember.Id))
                        {
                            logger.DebugFormat(
                                "New address book member {2} of {3} ({0} - {1}) has already been batched", newMember.Id,
                                tempContact.EmailAddress, numProcessed, newMembers.Count);
                        }
                        else
                        {
                            logger.DebugFormat(
                                "New address book member {2} of {3} ({0} - {1}) has been batched to be sent to the Email Service",
                                newMember.Id, tempContact.EmailAddress, numProcessed, newMembers.Count);
                            idsOfMembersLoaded.Add(newMember.Id);
                        }

                        newMember.PendingImporthashCode = fieldHash;
                        newMember.LastSyncedEmailAddress = tempContact.EmailAddress;
                    }
                }
            }
        }

        /// <summary>
        /// Simply creates this address book in dotMailer if it hasn't been previously
        /// created.
        /// </summary>
        /// <param name="slxEmailAddressBookId"></param>
        /// <param name="addressBookNameWasReservedByEmailService"></param>
        /// <returns>Returns true if the address book header was synced, or false if it wasn't synced because the address book used a reserved dotmailer address book name</returns>
        public bool SyncSingleAddressBookHeader(string slxEmailAddressBookId)
        {
            bool addressBookNameWasReservedByEmailService = false;

            EmailAddressBook slxEmailAddressBook;
            try
            {
                slxEmailAddressBook = Slx.GetRecord<EmailAddressBook>(slxEmailAddressBookId);
            }
            catch (EntityNotFoundException ex)
            {
                throw new KeyEntityNotFoundException(
                    string.Format("An entity that is key to the current execution was not found.  The entity is of type ({0}) with id ({1})", ex.EntityName, ex.EntityId),
                    ex.EntityName,
                    ex.EntityId,
                    ex.InnerException);
            }

            EmailAccount emailAccount = Slx.GetRecord<EmailAccount>(slxEmailAddressBook.EmailAccountId);

            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();

            if (slxEmailAddressBook.EmailServiceAddressBookId == 0)
                LinkOrCreateDotmailerAddressBook(slxEmailAddressBook, out addressBookNameWasReservedByEmailService);

            return !addressBookNameWasReservedByEmailService;
        }

        /// <summary>
        /// Simply creates this address book in dotMailer if it hasn't been previously
        /// created.  This overload also updates the address book object with a
        /// EmailServiceAddressBookId, which helps where the address book object is
        /// going to continue to be used.
        /// </summary>
        /// <returns>Returns true if the address book header was synced, or false if it wasn't synced because the address book used a reserved dotmailer address book name</returns>
        public bool SyncSingleAddressBookHeader(EmailAddressBook slxEmailAddressBook)
        {
            bool addressBookNameWasReservedByEmailService = false;

            try
            {
                EmailAccount emailAccount = Slx.GetRecord<EmailAccount>(slxEmailAddressBook.EmailAccountId);

                DotMailer.Username = emailAccount.ApiKey;
                DotMailer.Password = emailAccount.GetDecryptedPassword();

                if (slxEmailAddressBook.EmailServiceAddressBookId == 0)
                    LinkOrCreateDotmailerAddressBook(slxEmailAddressBook, out addressBookNameWasReservedByEmailService);
            }
            catch (Exception)
            {
                logger.Error("Exception occurred in SyncSingleAddressBookHeader whilst processing " + (slxEmailAddressBook.Name ?? ""));
                throw;
            }

            return !addressBookNameWasReservedByEmailService;
        }

        private bool IsDotmailerReservedAddressBookName(string addressBookName)
        {
            bool result = string.Equals(addressBookName, "all contacts", StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(addressBookName, "test", StringComparison.OrdinalIgnoreCase);

            return result;
        }

        private void LinkOrCreateDotmailerAddressBook(EmailAddressBook bookToLinkOrCreate)
        {
            bool addressBookNameWasReservedByEmailService;
            LinkOrCreateDotmailerAddressBook(bookToLinkOrCreate, out addressBookNameWasReservedByEmailService);
        }

        private void LinkOrCreateDotmailerAddressBook(EmailAddressBook bookToLinkOrCreate, out bool addressBookNameWasReservedByEmailService)
        {
            addressBookNameWasReservedByEmailService = IsDotmailerReservedAddressBookName(bookToLinkOrCreate.Name);

            if (addressBookNameWasReservedByEmailService)
            {
                logger.WarnFormat("SalesLogix Address Book ({0}, {1}) cannot be linked to an email service address book because its name is reserved for special use by the email service.",
                    bookToLinkOrCreate.Id,
                    bookToLinkOrCreate.Name);
                bookToLinkOrCreate.SyncStatus = EmailAddressBook.SyncStatusAddressBookNameReservedByEmailService;
                Slx.UpdateRecord(bookToLinkOrCreate);
                return;
            }

            // Does it already exist in dotMailer?
            DmAddressBook dmAddrBook = DotMailer.GetAddressBook(bookToLinkOrCreate.Name);
            if (dmAddrBook != null)
            {
                // Yes it does.  Ensure that the address book has not already been linked into SLX
                ICollection<EmailAddressBook> allBooks = Slx.GetEmailAddressBooks(bookToLinkOrCreate.EmailAccountId);
                if (allBooks != null)
                {
                    EmailAddressBook existingBook = allBooks.FirstOrDefault(b => b.EmailServiceAddressBookId == dmAddrBook.Id);
                    if (existingBook != null)
                    {
                        logger.WarnFormat("SalesLogix Address Book ({0}, {1}) cannot be linked to email service address book ({2}) because it is already linked to SalesLogix address book ({3})",
                            bookToLinkOrCreate.Id,
                            bookToLinkOrCreate.Name,
                            dmAddrBook.Id,
                            existingBook.Id);
                        bookToLinkOrCreate.SyncStatus = EmailAddressBook.SyncStatusSyncError;
                        Slx.UpdateRecord(bookToLinkOrCreate);
                        return;
                    }
                }

                logger.InfoFormat("Linking SalesLogix AddressBook ({0}) to existing address book in email service ({1})", bookToLinkOrCreate.Name, dmAddrBook.Id);
                bookToLinkOrCreate.EmailServiceAddressBookId = dmAddrBook.Id;
                bookToLinkOrCreate.EmailServiceAddressBookName = dmAddrBook.Name;
                bookToLinkOrCreate.SyncStatus = EmailAddressBook.SyncStatusLinkedWithEmailService;

                // Set addrBook so that all members re-sync into linked dotMailer address book
                bookToLinkOrCreate.LastMemberChangeSynced = null;
            }
            else
                CreateAddressBookHeaderInDotMailer(bookToLinkOrCreate);

            Slx.UpdateRecord(bookToLinkOrCreate);
        }

        private void CreateAddressBookHeaderInDotMailer(EmailAddressBook bookToLinkOrCreate)
        {
            logger.InfoFormat("Creating AddressBook header in dotMailer ({0})", bookToLinkOrCreate.Name);
            var createdDmBook = DotMailer.CreateAddressBook(bookToLinkOrCreate.Name);
            if (createdDmBook != null)
            {
                bookToLinkOrCreate.EmailServiceAddressBookId = createdDmBook.Id;
                bookToLinkOrCreate.EmailServiceAddressBookName = createdDmBook.Name;
                bookToLinkOrCreate.SyncStatus = EmailAddressBook.SyncStatusSyncedToEmailService;

                // Set addrBook's last-synced date/time so that all members re-sync into linked dotMailer address book
                bookToLinkOrCreate.LastMemberChangeSynced = null;
            }
        }

        public void RemoveDoNotSolicitMembers(EmailAddressBook slxBook, List<EmailAddressBookMember> membersChangedToDoNotSolicitOrEmail)
        {
            if (membersChangedToDoNotSolicitOrEmail != null)
            {
                List<string> emailAddressesToRemove = new List<string>();
                foreach (var member in membersChangedToDoNotSolicitOrEmail)
                {
                    if (string.IsNullOrWhiteSpace(member.LastSyncedEmailAddress))
                    {
                        logger.DebugFormat("Cannot remove Address Book Member ({0}) because they do not have an email address", member.Id);
                    }
                    else
                    {
                        logger.DebugFormat("Batching member ({0} - {1}) to be deleted from email service address book ({2}) because they are now 'Do Not Solicit' or 'Do Not Email'", member.Id, member.LastSyncedEmailAddress, slxBook.Name);
                        emailAddressesToRemove.Add(member.LastSyncedEmailAddress);
                    }

                    member.LastSyncHashCode = null;
                }

                if (emailAddressesToRemove.Count > 0)
                    DotMailer.DeleteAddressBookContacts(slxBook.EmailServiceAddressBookId, emailAddressesToRemove);

                Slx.BatchUpdateRecords<EmailAddressBookMember>(membersChangedToDoNotSolicitOrEmail);
            }
        }
    }
}