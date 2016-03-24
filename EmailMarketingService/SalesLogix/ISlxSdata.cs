namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using Sage.SData.Client.Atom;

    public interface ISlxSdata
    {
        IList<AtomEntry> GetAllEmailAccounts();

        int CountEmailAddressBookMembers(string emailAddressBookId);

        void UpdateRecord(AtomEntry recordToUpdate, string resourceKind);

        string Sdatapassword { get; set; }

        string SdataUrl { get; set; }

        string SdataUsername { get; set; }

        void BatchCreateRecords<T>(ICollection<T> recordsToCreate)
            where T : Entity;

        void BatchDeleteRecords<T>(ICollection<string> idsOfrecordsToDelete)
            where T : Entity;

        void BatchUpdateRecords<T>(ICollection<T> recordsToUpdate, bool excludeLeadsAndContactsForEmailAddressBookMembers)
            where T : Entity;

        string CreateRecord(Entity recordToCreate);

        void DeleteRecord<T>(string idOfRecordToDelete)
            where T : Entity;

        /// <summary>
        /// Iterate over each Address Book Member so we can handle any amount of records by taking advantage
        /// of the built in paging ability of the AtomFeedReader.
        /// </summary>
        /// <returns>All Address Book Members, one at a time</returns>
        /// <param name="startIndex"></param>
        IEnumerable<AtomEntry> GetAllAddressBookMembers();

        IList<AtomEntry> GetNewMemberEmailCampaigns(string emailAccountId, string emailAddressBookId);

        IList<AtomEntry> GetDataLabelsAddedBetween(string emailAccountId, DateTime startDate, DateTime endDate);

        IList<AtomEntry> GetDataLabelsModifiedBetween(string emailAccountId, DateTime startDate, DateTime endDate);

        IList<AtomEntry> GetDataMappings(string emailAccountId);

        IList<AtomEntry> GetDeletedItemsUnprocessed(string exactFilterForEntityTypeField = "", string likeFilterForDataField = "");

        IList<AtomEntry> GetEmailAccountsLastSyncedBefore(DateTime lastSynced);

        IList<AtomEntry> GetEmailAddressBookMembers(string emailAddressBookId, Dictionary<string, List<string>> subEntitiesAndFields);

        IList<AtomEntry> GetEmailAddressBookMembersAddedBetween(string emailAddressBookId, DateTime startDate, DateTime endDate, Dictionary<string, List<string>> subEntitiesAndFields);

        IList<AtomEntry> GetEmailAddressBookMembersByEmailAddr(QueryEntityType typeOfQuery, string idOfQueryEntity, string emailAddress, Dictionary<string, List<string>> subEntitiesAndFields = null);

        IList<AtomEntry> GetEmailAddressBookMembersByEmailAddresses(QueryEntityType typeOfQuery, string idOfQueryEntity, string[] emailAddresses, Dictionary<string, List<string>> subEntitiesAndFields = null);

        IList<AtomEntry> GetEmailAddressBookMembersFromImport(string emailAddressBookId, Guid importGuid);

        IList<AtomEntry> GetEmailAddressBookMembersModifiedBetween(string emailAddressBookId, DateTime startDate, DateTime endDate, Dictionary<string, List<string>> subEntitiesAndFields);

        IList<AtomEntry> GetEmailAddressBooks(string emailAccountId);

        IList<AtomEntry> GetEmailAddressBooksForEmailCampaign(string emailCampaignId);

        AtomEntry GetEmailAddressBookUnsubscriber(string emailAddressBookId, string emailAddress);

        AtomEntry GetEmailCampaignAddressBookLink(string emailCampaignId, string emailAddressBookId);

        AtomEntry GetEmailCampaignByEmailServiceId(int emailServiceId);

        AtomEntry GetEmailCampaignClick(string emailCampaignId, string emailAddress, DateTime dateClicked);

        IList<AtomEntry> GetEmailCampaignClicksForEmailCampaignBetween(string slxCampaignId, DateTime startDate, DateTime endDate);

        IList<AtomEntry> GetEmailCampaignSends(string emailCampaignId);

        IList<AtomEntry> GetEmailCampaignSendSummaries(string emailCampaignId);

        AtomEntry GetEmailCampaignSendSummary(string emailCampaignId, string emailAddress, DateTime sendDate);

        IList<AtomEntry> GetEmailCampaigns(string emailAccountId);

        IList<AtomEntry> GetEmailCampaignsLinkedToSlxCampaign();

        AtomEntry GetEmailSuppressionByEmailAddr(string emailAccountId, string emailAddress);

        AtomEntry GetFirstEmailAddressBookMemberInCampaign(string emailCampaignId, string emailAddress);

        AtomEntry GetRecord<T>(string slxRecordId)
    where T : Entity, new();

        List<AtomEntry> GetRecords<T>(ICollection<string> slxRecordIds, string fieldsToSelect = null)
    where T : Entity, new();

        AtomEntry GetSlxCampaignTarget(string slxCampaignId, string entityId, string targetType);

        IList<AtomEntry> GetSyncTasksBeforeDate(DateTime endDateUtc, IEnumerable<string> statuses = null);

        IList<AtomEntry> GetTargetsForSlxCampaign(string slxCampaignId);

        AtomEntry GetUserByUserName(string username);

        void UpdateDataLabel(DataLabel dataLabelToUpdate);

        void UpdateDeletedItem(DeletedItem deletedItemToUpdate);

        void UpdateEmailAccount(EmailAccount emailAccountToUpdate);

        void UpdateEmailAddressBook(EmailAddressBook emailAddressBookToUpdate);

        //void UpdateEmailAddressBookMembers(ICollection<EmailAddressBookMember> emailAddressBookMembersToUpdate);

        void UpdateEmailCampaign(EmailCampaign emailCampaignToUpdate);

        void UpdateRecord(Entity recordToUpdate);

        void UpdateSyncTask(SyncTask syncTaskToUpdate);

        IEnumerable<AtomEntry> GetEnumerableAddressBookMembers(string emailAddressBookId);
    }
}