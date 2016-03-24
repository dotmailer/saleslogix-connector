namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using dotMailer.Sdk.AddressBook;
    using dotMailer.Sdk.Campaigns;
    using dotMailer.Sdk.Collections;
    using Entities;

    public interface IDotMailerConnector
    {
        void DeleteAddressBookContacts(int addressBookId, ICollection<string> emailAddresses);

        DmAddressBook GetAddressBook(string addressBookName);

        DmContactCollection GetContactsInAddressBookModifiedSince(int addressBookId, DateTime sinceDate);

        DmDataFieldDefinitionCollection GetDataFields();

        string Password { get; set; }

        string Username { get; set; }

        DmAddressBook CreateAddressBook(string name);

        void CreateDataField(string dataFieldName, string dataType, string defaultValue);

        void DeleteAddressBook(string addressBookName);

        void DeleteAddressBookContact(int addressBookId, string emailAddress);

        void DeleteDataField(string dataFieldName);

        DmAddressBookCollection GetAddressBooks();

        DmCampaign GetEmailCampaign(int campaignId);

        DmCampaignContactActivityCollection GetEmailCampaignActivitySince(int campaignId, DateTime sinceDate);

        DmCampaignContactStatsCollection GetEmailCampaignClickersSince(int campaignId, DateTime sinceDate);

        DmCampaignCollection GetEmailCampaigns();

        DmCampaignContactActivityCollection GetEmailCampaignActivity(int campaignId);

        DmCampaignCollection GetEmailCampaignsWithActivitySince(DateTime sinceDate);

        DmSuppressedContactCollection GetSuppressionsSince(DateTime sinceDate);

        DmContactCollection GetUnsubscribersForAddressBookSince(int addressBookId, DateTime sinceDate);

        void ImportContactsIntoAddressBook(int addressBookId, IEnumerable<EmailServiceContact> contacts, out bool importFinished, out string importResult);

        bool IsCampaignSendComplete(string campaignSendResult);

        bool IsImportComplete(string importContactResult);

        string SendEmailCampaign(int emailCampaignId, string emailAddressBookName, DateTime sendDate, SplitTestMetric splitTestMetric = SplitTestMetric.Unknown, int splitTestPercentage = 0, int splitTestPeriodHours = 0);
    }
}