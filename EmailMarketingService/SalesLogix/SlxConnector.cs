using System.Reflection;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using log4net;
    using Sage.SData.Client.Atom;

    public class SlxConnector : ISlxConnector
    {
        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string cachedUserId = null;

        /// <summary>
        /// Initializes a new instance of the SlxConnector class.
        /// </summary>
        public SlxConnector(ISlxSdata sdata)
        {
            Sdata = sdata;
        }

        public string ConnectedUserId
        {
            get
            {
                if (cachedUserId == null)
                {
                    cachedUserId = GetUserByUserName(Sdata.SdataUsername).Id;
                }

                return cachedUserId;
            }
        }

        public ISlxSdata Sdata { get; set; }

        public void BatchCreateRecords<T>(ICollection<T> recordsToCreate)
            where T : Entity
        {
            // Split into chunks of 100 to avoid processing too many in a batch
            int chunkSize = 100;
            int numProcessedThisChunk = 0;
            int numProcessedSoFar = 0;
            int totalNumRecords = recordsToCreate.Count;
            List<T> chunkedRecords = null;

            foreach (var record in recordsToCreate)
            {
                if (numProcessedThisChunk == 0)
                {
                    chunkedRecords = new List<T>();
                }

                chunkedRecords.Add(record);
                numProcessedThisChunk++;
                numProcessedSoFar++;

                if (numProcessedThisChunk == chunkSize)
                {
                    // Chunk complete; process it.
                    Sdata.BatchCreateRecords<T>(chunkedRecords);
                    numProcessedThisChunk = 0;
                    logger.DebugFormat("Finished creating chunk of {0} records ({1} of {2}).", chunkedRecords.Count, numProcessedSoFar, totalNumRecords);
                }
            }

            if (numProcessedThisChunk != 0)
            {
                // Final chunk was not full; process it.
                Sdata.BatchCreateRecords<T>(chunkedRecords);
                logger.DebugFormat("Finished creating chunk of {0} records ({1} of {2}).", chunkedRecords.Count, numProcessedSoFar, totalNumRecords);
            }
        }

        public void BatchDeleteRecords<T>(ICollection<string> idsOfRecordsToDelete)
            where T : Entity
        {
            // Split into chunks of 100 to avoid processing too many in a batch
            int chunkSize = 100;
            int numProcessedThisChunk = 0;
            int numProcessedSoFar = 0;
            int totalNumRecords = idsOfRecordsToDelete.Count;
            List<string> chunkedIdsOfRecordsToDeleted = null;

            foreach (string idToDelete in idsOfRecordsToDelete)
            {
                if (numProcessedThisChunk == 0)
                {
                    chunkedIdsOfRecordsToDeleted = new List<string>();
                }

                chunkedIdsOfRecordsToDeleted.Add(idToDelete);
                numProcessedThisChunk++;
                numProcessedSoFar++;

                if (numProcessedThisChunk == chunkSize)
                {
                    // Chunk complete; process it.
                    Sdata.BatchDeleteRecords<T>(chunkedIdsOfRecordsToDeleted);
                    numProcessedThisChunk = 0;
                    logger.DebugFormat("Finished deleting chunk of {0} records ({1} of {2}).", chunkedIdsOfRecordsToDeleted.Count, numProcessedSoFar, totalNumRecords);
                }
            }

            if (numProcessedThisChunk != chunkSize)
            {
                // Final chunk was not full; process it.
                Sdata.BatchDeleteRecords<T>(chunkedIdsOfRecordsToDeleted);
                logger.DebugFormat("Finished deleting chunk of {0} records ({1} of {2}).", chunkedIdsOfRecordsToDeleted.Count, numProcessedSoFar, totalNumRecords);
            }
        }

        public void BatchUpdateRecords<T>(ICollection<T> recordsToUpdate, bool excludeLeadsAndContactsForEmailAddressBookMembers = true)
            where T : Entity
        {
            // Split into chunks of 100 to avoid processing too many in a batch
            int chunkSize = 100;
            int numProcessedThisChunk = 0;
            int numProcessedSoFar = 0;
            int totalNumRecords = recordsToUpdate.Count;
            List<T> chunkedRecords = null;

            foreach (var record in recordsToUpdate)
            {
                if (numProcessedThisChunk == 0)
                {
                    chunkedRecords = new List<T>();
                }

                chunkedRecords.Add(record);
                numProcessedThisChunk++;
                numProcessedSoFar++;

                if (numProcessedThisChunk == chunkSize)
                {
                    // Chunk complete; process it.
                    Sdata.BatchUpdateRecords<T>(chunkedRecords, excludeLeadsAndContactsForEmailAddressBookMembers);
                    numProcessedThisChunk = 0;
                    logger.DebugFormat("Finished updating chunk of {0} records ({1} of {2}).", chunkedRecords.Count, numProcessedSoFar, totalNumRecords);
                }
            }

            if (numProcessedThisChunk != 0)
            {
                // Final chunk was not full; process it.
                Sdata.BatchUpdateRecords<T>(chunkedRecords, excludeLeadsAndContactsForEmailAddressBookMembers);
                logger.DebugFormat("Finished updating chunk of {0} records ({1} of {2}).", chunkedRecords.Count, numProcessedSoFar, totalNumRecords);
            }
        }

        public int CountEmailAddressBookMembers(string emailAddressBookId)
        {
            return Sdata.CountEmailAddressBookMembers(emailAddressBookId);
        }

        public string CreateRecord(Entity recordToCreate)
        {
            return Sdata.CreateRecord(recordToCreate);
        }

        public void DeleteRecord<T>(string idOfRecordToDelete)
            where T : Entity
        {
            Sdata.DeleteRecord<T>(idOfRecordToDelete);
        }

        public IEnumerable<EmailAddressBookMember> GetAllAddressBookMembers()
        {
            foreach (var entry in Sdata.GetAllAddressBookMembers())
            {
                var member = SdataEntityParser.Parse<EmailAddressBookMember>(entry);
                yield return member;
            }
        }

        public IEnumerable<EmailAddressBookMember> GetEnumerableAddressBookMembers(string emailAddressBookId)
        {
            foreach (AtomEntry entry in Sdata.GetEnumerableAddressBookMembers(emailAddressBookId))
            {
                EmailAddressBookMember member = SdataEntityParser.Parse<EmailAddressBookMember>(entry);
                yield return member;
            }
        }

        public ICollection<EmailCampaign> GetNewMemberEmailCampaigns(string slxEmailAccountId, string slxEmailAddressBookId)
        {
            var atomEntries = Sdata.GetNewMemberEmailCampaigns(slxEmailAccountId, slxEmailAddressBookId);
            List<EmailCampaign> output = SdataEntityParser.ParseList<EmailCampaign>(atomEntries);
            return output;
        }

        public ICollection<DataLabel> GetDataLabelsAddedBetween(string emailAccountId, DateTime? startDate, DateTime endDate)
        {
            DateTime cleanedStartDate = CleanDate(startDate);
            var atomEntries = Sdata.GetDataLabelsAddedBetween(emailAccountId, cleanedStartDate, endDate);
            List<DataLabel> output = SdataEntityParser.ParseList<DataLabel>(atomEntries);
            return output;
        }

        public ICollection<DataLabel> GetDataLabelsModifiedBetween(string emailAccountId, DateTime? startDate, DateTime endDate)
        {
            DateTime cleanedStartDate = CleanDate(startDate);
            var atomEntries = Sdata.GetDataLabelsModifiedBetween(emailAccountId, cleanedStartDate, endDate);
            List<DataLabel> output = SdataEntityParser.ParseList<DataLabel>(atomEntries);
            return output;
        }

        public ICollection<DataFieldMapping> GetDataMappings(string emailAccountId)
        {
            var atomEntries = Sdata.GetDataMappings(emailAccountId);
            List<DataFieldMapping> output = SdataEntityParser.ParseList<DataFieldMapping>(atomEntries);
            return output;
        }

        public ICollection<DeletedItem> GetDeletedItemsUnprocessed(string exactFilterForEntityTypeField = "", string likeFilterForDataField = "")
        {
            var atomEntries = Sdata.GetDeletedItemsUnprocessed(exactFilterForEntityTypeField, likeFilterForDataField);
            List<DeletedItem> output = SdataEntityParser.ParseList<DeletedItem>(atomEntries);
            return output;
        }

        public ICollection<EmailAccount> GetEmailAccountsNeedingSync(
            int syncIntervalSeconds)
        {
            DateTime lastSyncDate = DateTime.UtcNow.AddSeconds(-syncIntervalSeconds);
            var atomEntries = Sdata.GetEmailAccountsLastSyncedBefore(lastSyncDate);
            List<EmailAccount> output = SdataEntityParser.ParseList<EmailAccount>(atomEntries);
            return output;
        }

        public ICollection<EmailAccount> GetAllEmailAccounts()
        {
            var atomEntries = Sdata.GetAllEmailAccounts();
            List<EmailAccount> output = SdataEntityParser.ParseList<EmailAccount>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBookMember> GetEmailAddressBookMembers(string emailAddressBookId, Dictionary<string, List<string>> subEntitiesAndFields)
        {
            var atomEntries = Sdata.GetEmailAddressBookMembers(emailAddressBookId, subEntitiesAndFields);
            List<EmailAddressBookMember> output = SdataEntityParser.ParseList<EmailAddressBookMember>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBookMember> GetEmailAddressBookMembersAddedBetween(string emailAddressBookId, DateTime? startDate, DateTime endDate, Dictionary<string, List<string>> subEntitiesAndFields)
        {
            DateTime cleanedStartDate = CleanDate(startDate);
            var atomEntries = Sdata.GetEmailAddressBookMembersAddedBetween(emailAddressBookId, cleanedStartDate, endDate, subEntitiesAndFields);
            List<EmailAddressBookMember> output = SdataEntityParser.ParseList<EmailAddressBookMember>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBookMember> GetEmailAddressBookMembersByEmailAddr(QueryEntityType typeOfQuery, string idOfQueryEntity, string emailAddress, Dictionary<string, List<string>> subEntitiesAndFields = null)
        {
            var atomEntries = Sdata.GetEmailAddressBookMembersByEmailAddr(typeOfQuery, idOfQueryEntity, emailAddress, subEntitiesAndFields);
            List<EmailAddressBookMember> output = SdataEntityParser.ParseList<EmailAddressBookMember>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBookMember> GetEmailAddressBookMembersByEmailAddresses(QueryEntityType typeOfQuery, string idOfQueryEntity, string[] emailAddresses, Dictionary<string, List<string>> subEntitiesAndFields = null)
        {
            var atomEntries = Sdata.GetEmailAddressBookMembersByEmailAddresses(typeOfQuery, idOfQueryEntity, emailAddresses, subEntitiesAndFields);
            List<EmailAddressBookMember> output = SdataEntityParser.ParseList<EmailAddressBookMember>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBookMember> GetEmailAddressBookMembersFromImport(string emailAddressBookId, Guid importGuid)
        {
            var atomEntries = Sdata.GetEmailAddressBookMembersFromImport(emailAddressBookId, importGuid);
            List<EmailAddressBookMember> output = SdataEntityParser.ParseList<EmailAddressBookMember>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBookMember> GetEmailAddressBookMembersModifiedBetween(string emailAddressBookId, DateTime? startDate, DateTime endDate, Dictionary<string, List<string>> subEntitiesAndFields)
        {
            DateTime cleanedStartDate = CleanDate(startDate);
            var atomEntries = Sdata.GetEmailAddressBookMembersModifiedBetween(emailAddressBookId, cleanedStartDate, endDate, subEntitiesAndFields);
            List<EmailAddressBookMember> output = SdataEntityParser.ParseList<EmailAddressBookMember>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBook> GetEmailAddressBooks(string emailAccountId)
        {
            var atomEntries = Sdata.GetEmailAddressBooks(emailAccountId);
            List<EmailAddressBook> output = SdataEntityParser.ParseList<EmailAddressBook>(atomEntries);
            return output;
        }

        public ICollection<EmailAddressBook> GetEmailAddressBooksForEmailCampaign(string emailCampaignId)
        {
            var atomEntries = Sdata.GetEmailAddressBooksForEmailCampaign(emailCampaignId);
            List<EmailAddressBook> output = SdataEntityParser.ParseList<EmailAddressBook>(atomEntries);
            return output;
        }

        public EmailAddressBookUnsubscriber GetEmailAddressBookUnsubscriber(string emailAddressBookId, string emailAddress)
        {
            var atomEntry = Sdata.GetEmailAddressBookUnsubscriber(emailAddressBookId, emailAddress);
            if (atomEntry != null)
                return SdataEntityParser.Parse<EmailAddressBookUnsubscriber>(atomEntry);
            return null;
        }

        public EmailCampaignAddressBookLink GetEmailCampaignAddressBookLink(string emailCampaignId, string emailAddressBookId)
        {
            AtomEntry atomEntry = Sdata.GetEmailCampaignAddressBookLink(emailCampaignId, emailAddressBookId);
            if (atomEntry != null)
                return SdataEntityParser.Parse<EmailCampaignAddressBookLink>(atomEntry);
            return null;
        }

        public EmailCampaign GetEmailCampaignByEmailServiceId(int emailServiceCampaignId)
        {
            var atomEntry = Sdata.GetEmailCampaignByEmailServiceId(emailServiceCampaignId);
            if (atomEntry != null)
            {
                return SdataEntityParser.Parse<EmailCampaign>(atomEntry);
            }
            else
            {
                return null;
            }
        }

        public EmailCampaignClick GetEmailCampaignClick(string emailCampaignId, string emailAddress, DateTime dateClicked)
        {
            var atomEntry = Sdata.GetEmailCampaignClick(emailCampaignId, emailAddress, dateClicked);
            if (atomEntry != null)
            {
                return SdataEntityParser.Parse<EmailCampaignClick>(atomEntry);
            }
            else
            {
                return null;
            }
        }

        public ICollection<EmailCampaignClick> GetEmailCampaignClicksForEmailCampaignBetween(
            string slxCampaignId,
            DateTime startDate,
            DateTime endDate)
        {
            var atomEntries = Sdata.GetEmailCampaignClicksForEmailCampaignBetween(slxCampaignId, startDate, endDate);
            List<EmailCampaignClick> output = SdataEntityParser.ParseList<EmailCampaignClick>(atomEntries);
            return output;
        }

        public ICollection<EmailCampaignSend> GetEmailCampaignSends(string emailCampaignId)
        {
            var atomEntries = Sdata.GetEmailCampaignSends(emailCampaignId);
            List<EmailCampaignSend> output = SdataEntityParser.ParseList<EmailCampaignSend>(atomEntries);
            return output;
        }

        public ICollection<EmailCampaignSendSummary> GetEmailCampaignSendSummaries(string emailCampaignId)
        {
            var atomEntries = Sdata.GetEmailCampaignSendSummaries(emailCampaignId);
            List<EmailCampaignSendSummary> output = SdataEntityParser.ParseList<EmailCampaignSendSummary>(atomEntries);
            return output;
        }

        public EmailCampaignSendSummary GetEmailCampaignSendSummary(string emailCampaignId, string emailAddress, DateTime sendDate)
        {
            var atomEntry = Sdata.GetEmailCampaignSendSummary(emailCampaignId, emailAddress, sendDate);
            if (atomEntry != null)
            {
                return SdataEntityParser.Parse<EmailCampaignSendSummary>(atomEntry);
            }
            else
            {
                return null;
            }
        }

        public ICollection<EmailCampaign> GetEmailCampaigns(string slxEmailAccountId)
        {
            var atomEntries = Sdata.GetEmailCampaigns(slxEmailAccountId);
            List<EmailCampaign> output = SdataEntityParser.ParseList<EmailCampaign>(atomEntries);
            return output;
        }

        public ICollection<EmailCampaign> GetEmailCampaignsLinkedToSlxCampaign()
        {
            var atomEntries = Sdata.GetEmailCampaignsLinkedToSlxCampaign();
            List<EmailCampaign> output = SdataEntityParser.ParseList<EmailCampaign>(atomEntries);
            return output;
        }

        public EmailSuppression GetEmailSuppressionByEmailAddr(string emailAccountId, string emailAddress)
        {
            var atomEntry = Sdata.GetEmailSuppressionByEmailAddr(emailAccountId, emailAddress);
            if (atomEntry == null)
                return null;
            else
                return SdataEntityParser.Parse<EmailSuppression>(atomEntry);
        }

        public EmailAddressBookMember GetFirstEmailAddressBookMemberInCampaign(string emailCampaignId, string emailAddress)
        {
            var atomEntry = Sdata.GetFirstEmailAddressBookMemberInCampaign(emailCampaignId, emailAddress);
            if (atomEntry != null)
                return SdataEntityParser.Parse<EmailAddressBookMember>(atomEntry);
            return null;
        }

        public T GetRecord<T>(string slxRecordId)
            where T : Entity, new()
        {
            var atomEntry = Sdata.GetRecord<T>(slxRecordId);
            if (atomEntry != null)
            {
                return SdataEntityParser.Parse<T>(atomEntry);
            }
            else
            {
                return null;
            }
        }

        public ICollection<T> GetRecords<T>(ICollection<string> slxRecordIds, string fieldsToSelect = null)
            where T : Entity, new()
        {
            // Split into chunks of 100 to avoid processing too many in a batch (due to size of url)
            int chunkSize = 100;
            int numProcessedThisChunk = 0;
            List<string> chunkedIds = null;
            List<AtomEntry> atomEntries = new List<AtomEntry>();

            foreach (string id in slxRecordIds)
            {
                if (numProcessedThisChunk == 0)
                {
                    chunkedIds = new List<string>();
                }

                chunkedIds.Add(id);
                numProcessedThisChunk++;

                if (numProcessedThisChunk == chunkSize)
                {
                    // Chunk complete; process it.
                    atomEntries.AddRange(Sdata.GetRecords<T>(chunkedIds));
                    numProcessedThisChunk = 0;
                }
            }

            if (numProcessedThisChunk != 0)
            {
                // Final chunk was not full; process it.
                atomEntries.AddRange(Sdata.GetRecords<T>(chunkedIds));
            }

            List<T> output = SdataEntityParser.ParseList<T>(atomEntries);
            return output;
        }

        public SlxCampaignTarget GetSlxCampaignTarget(string slxCampaignId, string entityId, string targetType)
        {
            var atomEntry = Sdata.GetSlxCampaignTarget(slxCampaignId, entityId, targetType);
            if (atomEntry != null)
            {
                return SdataEntityParser.Parse<SlxCampaignTarget>(atomEntry);
            }
            else
            {
                return null;
            }
        }

        public ICollection<SyncTask> GetPendingAndInProgressSyncTasksThatAreDue()
        {
            List<string> validStatuses = new List<string>() { SyncTask.StatusPending, SyncTask.StatusInProgress };
            var atomEntries = Sdata.GetSyncTasksBeforeDate(DateTime.UtcNow, validStatuses);
            List<SyncTask> output = SdataEntityParser.ParseList<SyncTask>(atomEntries);
            return output;
        }

        public SlxUser GetUserByUserName(string username)
        {
            var atomEntry = Sdata.GetUserByUserName(username);
            if (atomEntry != null)
            {
                return SdataEntityParser.Parse<SlxUser>(atomEntry);
            }
            else
            {
                return null;
            }
        }

        public void UpdateDataLabel(DataLabel dataLabelToUpdate)
        {
            Sdata.UpdateDataLabel(dataLabelToUpdate);
        }

        public void UpdateDeletedItem(DeletedItem deletedItemToUpdate)
        {
            Sdata.UpdateDeletedItem(deletedItemToUpdate);
        }

        public void UpdateEmailAccount(EmailAccount emailAccountToUpdate)
        {
            Sdata.UpdateEmailAccount(emailAccountToUpdate);
        }

        public void UpdateEmailAddressBook(EmailAddressBook emailAddressBookToUpdate)
        {
            Sdata.UpdateEmailAddressBook(emailAddressBookToUpdate);
        }

        //public void UpdateEmailAddressBookMembers(ICollection<EmailAddressBookMember> emailAddressBookMembersToUpdate)
        //{
        //    this.Sdata.UpdateEmailAddressBookMembers(emailAddressBookMembersToUpdate);
        //}

        public void UpdateEmailCampaign(EmailCampaign emailCampaignToUpdate)
        {
            Sdata.UpdateEmailCampaign(emailCampaignToUpdate);
        }

        public void UpdateRecord(Entity recordToUpdate)
        {
            Sdata.UpdateRecord(recordToUpdate);
        }

        public void UpdateSyncTask(SyncTask syncTaskToUpdate)
        {
            Sdata.UpdateSyncTask(syncTaskToUpdate);
        }

        private static DateTime CleanDate(DateTime? nullableDate)
        {
            DateTime cleanedDate = nullableDate == null ? SlxSdata.MinimumDateTimeValue : (DateTime)nullableDate;
            if (cleanedDate < SlxSdata.MinimumDateTimeValue)
                cleanedDate = SlxSdata.MinimumDateTimeValue;

            return cleanedDate;
        }
    }
}