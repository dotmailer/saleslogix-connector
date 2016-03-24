namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using dotMailer.Sdk;
    using dotMailer.Sdk.AddressBook;
    using dotMailer.Sdk.Campaigns;
    using dotMailer.Sdk.Collections;
    using dotMailer.Sdk.Contacts.Importing;
    using dotMailer.Sdk.Enums;
    using dotMailer.Sdk.Interfaces;
    using dotMailer.Sdk.Sending;
    using Entities;
    using log4net;

    public class DotMailerConnector : IDotMailerConnector
    {
        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Password { get; set; }

        public string Username { get; set; }

        public DmAddressBook CreateAddressBook(string name)
        {
            var service = InitialiseService();
            DmAddressBook book = service.AddressBooks.CreateAddressBook(name);
            return book;
        }

        public void CreateDataField(string dataFieldName, string dataType, string defaultValue)
        {
            var service = InitialiseService();
            DmDataFieldTypes dt;
            object castedDefault = null;
            switch (dataType.ToUpperInvariant())
            {
                case "DATE":
                    dt = DmDataFieldTypes.Date;
                    if (defaultValue != null)
                    {
                        castedDefault = (object)DateTime.Parse(defaultValue);
                    }

                    break;

                case "BOOLEAN":
                    dt = DmDataFieldTypes.Boolean;
                    break;

                case "NUMERIC":
                    dt = DmDataFieldTypes.Decimal;
                    break;

                case "STRING":
                    dt = DmDataFieldTypes.String;
                    break;

                default:
                    throw new InvalidOperationException(string.Format("{0} is not a valid data type.", dataType));
            }

            service.Contacts.CreateDataField(dataFieldName, dt, castedDefault);
        }

        public void DeleteAddressBook(string addressBookName)
        {
            var service = InitialiseService();
            var book = OldSdkGetAddressBookByName(service, addressBookName);
            if (book == null)
            {
                throw new InvalidOperationException("Address book does not exist in email service");
            }

            logger.DebugFormat("Removing email service address book ({0}).", addressBookName);
            service.AddressBooks.DeleteAddressBook(book.Id);
        }

        public void DeleteAddressBookContact(int addressBookId, string emailAddress)
        {
            var service = InitialiseService();
            logger.DebugFormat("Removing Contact ({0}) from email service address book ({1}).", emailAddress, addressBookId);
            service.AddressBooks.RemoveAddressBookContact(addressBookId, emailAddress, false, false);
        }

        public void DeleteAddressBookContacts(int addressBookId, ICollection<string> emailAddresses)
        {
            int batchRemovalThreshold = 5;
            if (emailAddresses.Count <= batchRemovalThreshold)
            {
                logger.DebugFormat("Removing ({0}) Contacts individually from email service address book ({1}).", emailAddresses.Count, addressBookId);
                foreach (string emailAddress in emailAddresses)
                {
                    try
                    {
                        DeleteAddressBookContact(addressBookId, emailAddress);
                    }
                    catch (DmException ex)
                    {
                        if (ex.Code == DMErrorCodes.ERROR_CONTACT_NOT_FOUND)
                        {
                            // log it and ignore it
                            logger.InfoFormat("Could not find member ({0}) to delete it from email service address book ({1})", emailAddress, addressBookId);
                        }
                    }
                }
            }
            else
            {
                var service = InitialiseService();

                logger.DebugFormat("Removing batch of ({0}) Contacts from email service address book ({1}).", emailAddresses.Count, addressBookId);
                foreach (string emailAddress in emailAddresses)
                {
                    logger.DebugFormat("Email address to remove ({0})", emailAddress);
                }

                logger.Debug("Starting batch removal");

                // N.B. Unlike the RemoveAddressBookContact method, this does not throw an exception
                // if the contact does not exist.
                service.AddressBooks.RemoveAddressBookContacts(addressBookId, emailAddresses);
                logger.Debug("Finished batch removal");
            }
        }

        public void DeleteDataField(string dataFieldName)
        {
            var service = InitialiseService();
            if (service.Contacts.DataFieldDefinitions.Contains(dataFieldName))
            {
                try
                {
                    service.Contacts.DeleteDataField(dataFieldName);
                }
                catch (DmException ex)
                {
                    if (ex.Code == DMErrorCodes.ERROR_DATAFIELD_NOTFOUND)
                    {
                        logger.InfoFormat("Couldn't delete data label {0} because it does not exist.", dataFieldName);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                logger.InfoFormat("Couldn't delete data label {0} because it does not exist.", dataFieldName);
            }
        }

        public DmAddressBook GetAddressBook(string addressBookName)
        {
            var service = InitialiseService();
            var book = OldSdkGetAddressBookByName(service, addressBookName);
            return book;
        }

        /// <summary>
        /// Provides an equivalent implementation to the older SDK GetAddressBookByName method - returns null where the address book cannot be found rather than throwing an exception.
        /// </summary>
        private DmAddressBook OldSdkGetAddressBookByName(DmService service, string addressBookName)
        {
            try
            {
                return service.AddressBooks.GetAddressBookByName(addressBookName);
            }
            catch (DmException dotMailerException)
            {
                if (dotMailerException.Code == DMErrorCodes.ERROR_ADDRESSBOOK_NOT_FOUND)
                    return null;
                else
                    throw;
            }
        }

        public DmAddressBookCollection GetAddressBooks()
        {
            var service = InitialiseService();
            var books = service.AddressBooks.ListAddressBooks();
            return books;
        }

        public DmDataFieldDefinitionCollection GetDataFields()
        {
            var service = InitialiseService();
            return service.Contacts.DataFieldDefinitions;
        }

        public DmCampaign GetEmailCampaign(int campaignId)
        {
            var service = InitialiseService();
            DmCampaign camp = service.Campaigns.GetCampaign(campaignId);

            return camp;
        }

        public DmCampaignContactActivityCollection GetEmailCampaignActivitySince(int campaignId, DateTime sinceDate)
        {
            var service = InitialiseService();
            DmCampaignContactActivityCollection activities = service.Campaigns.ListCampaignActivitiesSinceDate(campaignId, sinceDate);
            return activities;
        }

        public DmCampaignContactActivityCollection GetEmailCampaignActivity(int campaignId)
        {
            var service = InitialiseService();
            DmCampaignContactActivityCollection activities = service.Campaigns.ListCampaignActivities(campaignId);
            return activities;
        }

        public DmCampaignContactStatsCollection GetEmailCampaignClickersSince(int campaignId, DateTime sinceDate)
        {
            var service = InitialiseService();
            DmCampaignContactStatsCollection clickers = service.Campaigns.ListCampaignClickers(campaignId, sinceDate);
            return clickers;
        }

        public DmCampaignCollection GetEmailCampaigns()
        {
            var service = InitialiseService();
            DmCampaignCollection campaigns = service.Campaigns.ListCampaigns();

            return campaigns;
        }

        public DmCampaignCollection GetEmailCampaignsWithActivitySince(DateTime sinceDate)
        {
            var service = InitialiseService();
            DmCampaignCollection campaigns = service.Campaigns.ListSentCampaignsWithActivitySinceDate(sinceDate);

            return campaigns;
        }

        public DmContactCollection GetContactsInAddressBookModifiedSince(int addressBookId, DateTime sinceDate)
        {
            var service = InitialiseService();
            var modifiedContacts = service.AddressBooks.ListContactsInAddressBookModifiedSince(addressBookId, sinceDate);
            return modifiedContacts;
        }

        public DmSuppressedContactCollection GetSuppressionsSince(DateTime sinceDate)
        {
            var service = InitialiseService();
            var suppressions = service.Contacts.ListSuppressedContactsSinceDate(sinceDate);
            return suppressions;
        }

        public DmContactCollection GetUnsubscribersForAddressBookSince(int addressBookId, DateTime sinceDate)
        {
            var service = InitialiseService();
            var unsubscribers = service.AddressBooks.ListUnsubscribedContactsInAddressBookSinceDate(addressBookId, sinceDate);
            return unsubscribers;
        }

        /// <summary>
        /// Attempts to import contacts into the specified dotMailer address book
        /// and waits a limited time for that import to complete.
        /// </summary>
        /// <param name="addressBookId"></param>
        /// <param name="contacts"></param>
        /// <param name="importFinished"></param>
        /// <param name="importResult"></param>
        public void ImportContactsIntoAddressBook(
            int addressBookId,
            IEnumerable<EmailServiceContact> contacts,
            out bool importFinished,
            out string importResult)
        {
            if (contacts == null)
            {
                throw new ArgumentNullException("contacts");
            }

            var service = InitialiseService();

            foreach (var con in contacts)
            {
                con.AssignDmDataFields(service.Contacts.GetDefaultEmptyDataFieldValues());
            }

            var ico = new ImportContactOptions();
            ImportContactResult result = service.AddressBooks.ImportContactsIntoAddressBook(ico, addressBookId, contacts);

            importFinished = false;
            const int NumTries = 3;
            TimeSpan checkingInterval = new TimeSpan(0, 0, 10);
            for (int i = 0; i < NumTries; i++)
            {
                // Wait and see if the import has finished
                Thread.Sleep(checkingInterval);
                try
                {
                    service.AddressBooks.UpdateContactsResult(result);
                    importFinished = result.Completed;
                    if (importFinished)
                    {
                        break;
                    }
                }
                catch (DmException ex)
                {
                    if (ex.Code == DMErrorCodes.ERROR_IMPORT_PROGRESSUPDATE_TIMEDOUT)
                    {
                        // Ignore it
                        if (i < NumTries - 1)
                        {
                            logger.DebugFormat("Checking import status timed out. Trying again in {0}", checkingInterval);
                        }
                        else
                        {
                            logger.DebugFormat("Checking import status timed out. Will be processed during next sync cycle.");
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            importResult = service.Serialisation.Serialise(result, SerialisationType.Xml);
        }

        public bool IsCampaignSendComplete(string campaignSendResult)
        {
            var service = InitialiseService();
            DataContractSerializer dcs = new DataContractSerializer(typeof(SendResult));
            XmlReader xr = XmlReader.Create(new StringReader(campaignSendResult));
            SendResult sr = (SendResult)dcs.ReadObject(xr);

            try
            {
                service.Campaigns.UpdateSendResult(sr);
            }
            catch (DmException ex)
            {
                if (ex.Code == DMErrorCodes.ERROR_IMPORT_PROGRESSUPDATE_TIMEDOUT)
                {
                    logger.DebugFormat("Checking import status timed out.");
                }
                else
                {
                    throw;
                }
            }

            return sr.Status == EmailSendProgress.Complete;
        }

        public bool IsImportComplete(string importContactResult)
        {
            var service = InitialiseService();
            ImportContactResult icr;
            try
            {
                icr = (ImportContactResult)service.Serialisation.Deserialise(importContactResult, SerialisationType.Xml);
            }
            catch (Exception e)
            {
                logger.DebugFormat("({0}) DotMailerConnector.IsImportComplete failed to deserialise the following XML data using SerialisationFactory.Deserialise: {1}", e.GetType().Name, importContactResult);
                throw;
            }
            service.AddressBooks.UpdateContactsResult(icr);

            return icr.Completed;
        }

        public string SendEmailCampaign(
            int emailCampaignId,
            string emailAddressBookName,
            DateTime sendDate,
            SplitTestMetric splitTestMetric = SplitTestMetric.Unknown,
            int splitTestPercentage = 0,
            int splitTestPeriodHours = 0)
        {
            var service = InitialiseService();

            var addrBook = OldSdkGetAddressBookByName(service, emailAddressBookName);
            var addrBooks = new List<IDmAddressBook>() { addrBook };

            SplitTestSendOptions sendOpts = new SplitTestSendOptions();

            switch (splitTestMetric)
            {
                case SplitTestMetric.Unknown:
                    sendOpts.SendMode = SplitTestSendMode.Unknown;
                    break;

                case SplitTestMetric.Clicks:
                    sendOpts.SendMode = SplitTestSendMode.Clicks;
                    break;

                case SplitTestMetric.Opens:
                    sendOpts.SendMode = SplitTestSendMode.Opens;
                    break;
            }

            sendOpts.SplitTestPercentage = splitTestPercentage;
            sendOpts.SplitTestPeriodHours = splitTestPeriodHours;

            SendResult sendResult = service.Campaigns.SendCampaignToAddressBooks(sendOpts, emailCampaignId, addrBooks, sendDate);

            // N.B. Use DataContractSerializer instead of XmlSerializer because XmlSerializer cannot handle
            // properties that have interface data types e.g. SendResult.ObjectIds => IEnumerable<int>
            DataContractSerializer dcs = new DataContractSerializer(typeof(SendResult));
            var sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb);
            dcs.WriteObject(xw, sendResult);
            xw.Flush();

            return sb.ToString();
        }

        private DmService InitialiseService()
        {
            DmServiceFactory.Logging = true;
            return DmServiceFactory.Create(Username, Password);
        }
    }
}