namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using Entities;

    public interface ISlxConnector
    {
        ICollection<EmailAccount> GetAllEmailAccounts();

        int CountEmailAddressBookMembers(string emailAddressBookId);

        string ConnectedUserId { get; }

        ISlxSdata Sdata { get; set; }

        void BatchCreateRecords<T>(ICollection<T> recordsToCreate)
            where T : Entity;

        void BatchDeleteRecords<T>(ICollection<string> idsOfRecordsToDelete)
            where T : Entity;

        void BatchUpdateRecords<T>(ICollection<T> recordsToUpdate, bool excludeLeadsAndContactsForEmailAddressBookMembers = true)
            where T : Entity;

        string CreateRecord(Entity recordToCreate);

        void DeleteRecord<T>(string idOfRecordToDelete)
            where T : Entity;

        IEnumerable<EmailAddressBookMember> GetAllAddressBookMembers();

        ICollection<EmailCampaign> GetNewMemberEmailCampaigns(string slxEmailAccountId, string slxEmailAddressBookId);

        ICollection<DataLabel> GetDataLabelsAddedBetween(string emailAccountId, DateTime? startDate, DateTime endDate);

        ICollection<DataLabel> GetDataLabelsModifiedBetween(string emailAccountId, DateTime? startDate, DateTime endDate);

        ICollection<DataFieldMapping> GetDataMappings(string emailAccountId);

        ICollection<DeletedItem> GetDeletedItemsUnprocessed(string exactFilterForEntityTypeField = "", string likeFilterForDataField = "");

        ICollection<EmailAccount> GetEmailAccountsNeedingSync(int syncIntervalSeconds);

        ICollection<EmailAddressBookMember> GetEmailAddressBookMembers(string emailAddressBookId, Dictionary<string, List<string>> subEntitiesAndFields);

        ICollection<EmailAddressBookMember> GetEmailAddressBookMembersAddedBetween(string emailAddressBookId, DateTime? startDate, DateTime endDate, Dictionary<string, List<string>> subEntitiesAndFields);

        ICollection<EmailAddressBookMember> GetEmailAddressBookMembersByEmailAddr(QueryEntityType typeOfQuery, string idOfQueryEntity, string emailAddress, Dictionary<string, List<string>> subEntitiesAndFields = null);

        ICollection<EmailAddressBookMember> GetEmailAddressBookMembersByEmailAddresses(QueryEntityType typeOfQuery, string idOfQueryEntity, string[] emailAddresses, Dictionary<string, List<string>> subEntitiesAndFields = null);

        ICollection<EmailAddressBookMember> GetEmailAddressBookMembersFromImport(string emailAddressBookId, Guid importGuid);

        ICollection<EmailAddressBookMember> GetEmailAddressBookMembersModifiedBetween(string emailAddressBookId, DateTime? startDate, DateTime endDate, Dictionary<string, List<string>> subEntitiesAndFields);

        ICollection<EmailAddressBook> GetEmailAddressBooks(string emailAccountId);

        ICollection<EmailAddressBook> GetEmailAddressBooksForEmailCampaign(string emailCampaignId);

        EmailAddressBookUnsubscriber GetEmailAddressBookUnsubscriber(string emailAddressBookId, string emailAddress);

        EmailCampaignAddressBookLink GetEmailCampaignAddressBookLink(string emailCampaignId, string emailAddressBookId);

        EmailCampaign GetEmailCampaignByEmailServiceId(int emailServiceCampaignId);

        EmailCampaignClick GetEmailCampaignClick(string emailCampaignId, string emailAddress, DateTime dateClicked);

        ICollection<EmailCampaignClick> GetEmailCampaignClicksForEmailCampaignBetween(string slxCampaignId, DateTime startDate, DateTime endDate);

        ICollection<EmailCampaignSend> GetEmailCampaignSends(string emailCampaignId);

        ICollection<EmailCampaignSendSummary> GetEmailCampaignSendSummaries(string emailCampaignId);

        EmailCampaignSendSummary GetEmailCampaignSendSummary(string emailCampaignId, string emailAddress, DateTime sendDate);

        ICollection<EmailCampaign> GetEmailCampaigns(string SlxEmailAccountId);

        ICollection<EmailCampaign> GetEmailCampaignsLinkedToSlxCampaign();

        EmailSuppression GetEmailSuppressionByEmailAddr(string emailAccountId, string emailAddress);

        EmailAddressBookMember GetFirstEmailAddressBookMemberInCampaign(string emailCampaignId, string emailAddress);

        T GetRecord<T>(string slxRecordId)
    where T : Entity, new();

        ICollection<T> GetRecords<T>(ICollection<string> slxRecordIds, string fieldsToSelect = null)
    where T : Entity, new();

        SlxCampaignTarget GetSlxCampaignTarget(string slxCampaignId, string entityId, string targetType);

        ICollection<SyncTask> GetPendingAndInProgressSyncTasksThatAreDue();

        void UpdateDataLabel(DataLabel dataLabelToUpdate);

        void UpdateDeletedItem(DeletedItem deletedItemToUpdate);

        void UpdateEmailAccount(EmailAccount emailAccountToUpdate);

        void UpdateEmailAddressBook(EmailAddressBook emailAddressBookToUpdate);

        //void UpdateEmailAddressBookMembers(ICollection<EmailAddressBookMember> emailAddressBookMembersToUpdate);

        void UpdateEmailCampaign(EmailCampaign emailCampaignToUpdate);

        void UpdateRecord(Entity recordToUpdate);

        void UpdateSyncTask(SyncTask syncTaskToUpdate);

        IEnumerable<EmailAddressBookMember> GetEnumerableAddressBookMembers(string emailAddressBookId);
    }
}