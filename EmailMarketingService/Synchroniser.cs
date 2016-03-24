using System.Reflection;
using dotMailer.Sdk.Collections;
using dotMailer.Sdk.Enums;
using dotMailer.Sdk.Objects.Contacts;
using EmailMarketing.SalesLogix.Exceptions;
using Sage.SData.Client.Atom;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Xml.Serialization;
    using dotMailer.Sdk;
    using Entities;
    using log4net;
    using Sage.SData.Client.Extensions;
    using Tasks;

    /// <summary>
    /// Controls the synchronisation of Email Accounts and sub-entities
    /// </summary>
    public class Synchroniser
    {
        /// <summary>log4net logger object</summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<MappingDefinition> defaultDataMappings = new List<MappingDefinition>()
        {
            new MappingDefinition("Contact", "FIRSTNAME", "FirstName", null),
            new MappingDefinition("Lead", "FIRSTNAME", "FirstName", null),

            new MappingDefinition("Contact", "LASTNAME", "LastName", null),
            new MappingDefinition("Lead", "LASTNAME", "LastName", null),

            new MappingDefinition("Contact", "FULLNAME", "FullName", null),
            new MappingDefinition("Lead", "FULLNAME", "LeadFullName", null),

            new MappingDefinition("Contact", "POSTCODE", "Address", "PostalCode"),
            new MappingDefinition("Lead", "POSTCODE", "Address", "PostalCode")
        };

        /// <summary>
        /// Initializes a new instance of the Synchroniser class.
        /// </summary>
        public Synchroniser(ISlxConnector slx, IDotMailerConnector dotMailer)
        {
            Slx = slx;
            DotMailer = dotMailer;
        }

        public ISlxConnector Slx { get; private set; }

        public IDotMailerConnector DotMailer { get; private set; }

        public void ProcessSyncAllEmailCampaignHeaders(SyncTask syncTask)
        {
            if (!string.Equals(syncTask.TaskType, SyncTask.TaskTypeSyncAllEmailCampaignHeaders, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(string.Format("syncTask must have TaskType of {0}, but was ({1}) for task with id ({2})", SyncTask.TaskTypeSyncAllEmailCampaignHeaders, syncTask.TaskType, syncTask.Id));
            }

            syncTask.Status = SyncTask.StatusInProgress;
            syncTask.ActualStartTime = DateTime.UtcNow;
            Slx.UpdateRecord(syncTask);

            ICollection<EmailAccount> emailAccounts = Slx.GetAllEmailAccounts();

            var syncer = new Synchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());

            if (emailAccounts != null)
            {
                bool errorOccurredSyncingAccountHeaders = false;
                foreach (EmailAccount emailAccount in emailAccounts)
                {
                    try
                    {
                        syncer.SyncEmailCampaignHeaderInfo(emailAccount, DateTime.UtcNow);
                    }
                    catch (Exception ex)
                    {
                        string emailAccountDescription = (emailAccount.AccountName ?? "") + "/" + (emailAccount.ApiKey ?? "");
                        Logger.Error("Exception thrown whilst synchronising campaign header information for email account (" + emailAccountDescription + ").", ex);
                        errorOccurredSyncingAccountHeaders = true;
                    }
                }
                if (errorOccurredSyncingAccountHeaders)
                    throw new EmailCampaignHeaderSynchronisationException("Synchronising headers for some email accounts failed.");
            }

            syncTask.Status = SyncTask.StatusComplete;
            Slx.UpdateRecord(syncTask);
        }

        public void ProcessSyncEmailAccountTask(SyncTask syncTask)
        {
            if (!string.Equals(syncTask.TaskType, SyncTask.TaskTypeSyncEmailAccount, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(string.Format("syncTask must have TaskType of {0}, but was ({1}) for task with id ({2})", SyncTask.TaskTypeSyncEmailAccount, syncTask.TaskType, syncTask.Id));

            if (string.IsNullOrWhiteSpace(syncTask.TaskData))
                throw new ArgumentException(string.Format("syncTask.TaskData cannot be null or blank for task with id ({0})", syncTask.Id));

            string slxEmailAccountId = syncTask.TaskData;
            Logger.DebugFormat("Email Account to be synchronised is ({0})", slxEmailAccountId);
            syncTask.Status = SyncTask.StatusInProgress;
            Slx.UpdateRecord(syncTask);

            SyncEmailAccount(slxEmailAccountId, SyncType.Manual);

            syncTask.Status = SyncTask.StatusComplete;
            Slx.UpdateRecord(syncTask);
        }

        public void SyncEmailAccounts(IEnumerable<EmailAccount> emailAccounts, SyncType syncType)
        {
            bool failed = false;

            foreach (EmailAccount emailAccount in emailAccounts)
            {
                try
                {
                    SyncEmailAccount(emailAccount, syncType);
                }
                catch (Exception)
                {
                    string emailAccountDescription = (emailAccount.AccountName ?? "") + "/" + (emailAccount.ApiKey ?? "");
                    Logger.Error("An unhandled exception was thrown whilst synchronising email account (" + emailAccountDescription + ")");
                    failed = true;
                }
            }

            if (failed)
                throw new EmailAccountSynchronisationException("Unhandled exceptions were thrown whilst attempting to synchronise at least one email account.");
        }

        public void SyncEmailAccount(string emailAccountId, SyncType syncType)
        {
            EmailAccount emailAccount = Slx.GetRecord<EmailAccount>(emailAccountId);
            if (emailAccount == null)
            {
                Logger.WarnFormat("Email Account ({0}) could not be found to synchronise it.", emailAccountId);
                return;
            }

            SyncEmailAccount(emailAccount, syncType);
        }

        public void SyncEmailAccount(EmailAccount emailAccount, SyncType syncType)
        {
            Logger.DebugFormat("Synchronising EmailAccount Started ({0})", emailAccount.AccountName);
            if (string.IsNullOrWhiteSpace(emailAccount.ApiKey))
            {
                Logger.ErrorFormat("The login credentials for Email Account ({0}) are empty.", emailAccount.AccountName);
                return;
            }

            string emailAccountDescription = "Email account: " + (emailAccount.AccountName ?? "") + "/" + (emailAccount.ApiKey ?? "");

            bool synchronisingDataLabelsCompleted;
            bool synchronisingEmailCampaignHeaderInfoCompleted;
            bool synchronisingAddressBooksCompleted = false;
            bool synchronisingCampaignActivityCompleted = false;
            bool processingGlobalSuppressionsCompleted;

            try
            {
                DateTime syncTime = DateTime.UtcNow;

                try
                {
                    bool newLabelAdded;
                    SyncDataLabels(emailAccount, syncTime, out newLabelAdded);
                    synchronisingDataLabelsCompleted = true;
                }
                catch (Exception ex)
                {
                    synchronisingDataLabelsCompleted = false;
                    Logger.Error("An exception was thrown whilst synchronising data labels.  " + emailAccountDescription, ex);
                }

                try
                {
                    SyncEmailCampaignHeaderInfo(emailAccount, syncTime);
                    synchronisingEmailCampaignHeaderInfoCompleted = true;
                }
                catch (Exception ex)
                {
                    synchronisingEmailCampaignHeaderInfoCompleted = false;
                    Logger.Error("An exception was thrown whilst synchronising campaign header information.  " + emailAccountDescription, ex);
                }

                if (synchronisingDataLabelsCompleted)
                {
                    try
                    {
                        var addrBookSyncer = new EmailAddressBookSynchroniser(Slx, DotMailer);
                        addrBookSyncer.SyncEmailAddressBookHeaders(emailAccount, syncType, syncTime);

                        bool newMappingsAdded = WereNewMappingsModified(emailAccount, emailAccount.LastSynchronised);
                        addrBookSyncer.SyncEmailAddressBookMembers(emailAccount, syncType, syncTime, newMappingsAdded, true);
                        synchronisingAddressBooksCompleted = true;
                    }
                    catch (Exception ex)
                    {
                        synchronisingAddressBooksCompleted = false;
                        Logger.Error("An exception was thrown whilst synchronising email address books.  " + emailAccountDescription, ex);
                    }
                }

                //We're happy to synchronise email campaign activity even if any of the previous steps have failed.
                try
                {
                    var activitySyncer = new EmailCampaignActivitySynchroniser(Slx, DotMailer);
                    activitySyncer.SyncEachEmailCampaignsActivity(emailAccount, syncType, syncTime);
                    synchronisingCampaignActivityCompleted = true;
                }
                catch (Exception ex)
                {
                    synchronisingCampaignActivityCompleted = false;
                    Logger.Error("An exception was thrown whilst synchronising email campaign activity.  " + emailAccountDescription, ex);
                }

                //We're happy to process global suppressions even if any of the previous steps have failed.
                try
                {
                    ProcessGlobalSuppressions(emailAccount);
                    processingGlobalSuppressionsCompleted = true;
                }
                catch (Exception ex)
                {
                    processingGlobalSuppressionsCompleted = false;
                    Logger.Error("An exception was thrown whilst processing global suppressions.  " + emailAccountDescription, ex);
                }

                bool emailAccountFullySynchronised =
                    (synchronisingDataLabelsCompleted) &&
                    (synchronisingEmailCampaignHeaderInfoCompleted) &&
                    (synchronisingAddressBooksCompleted) &&
                    (synchronisingCampaignActivityCompleted) &&
                    (processingGlobalSuppressionsCompleted);

                if (emailAccountFullySynchronised)
                {
                    emailAccount.LastSynchronised = syncTime;
                    Slx.UpdateEmailAccount(emailAccount);
                }
                else
                    throw new EmailAccountSynchronisationException("Unhandled exceptions prevented email account from being fully synchronised.  " + emailAccountDescription);
            }
            catch (DmException ex)
            {
                if (ex.Code == DMErrorCodes.ERROR_INVALID_LOGIN)
                {
                    string message = "The login credentials for Email Account ({0}) are incorrect. Login failed.{1}{2}";
                    if (Logger.IsDebugEnabled)
                        Logger.ErrorFormat(message, emailAccount.AccountName, Environment.NewLine, ex);
                    else
                        Logger.ErrorFormat(message, emailAccount.AccountName, null, null);

                    return;
                }
                else
                {
                    throw;
                }
            }
            catch (CryptographicException)
            {
                Logger.ErrorFormat("The login credentials for Email Account ({0}) are corrupt.", emailAccount.AccountName);
                return;
            }

            Logger.DebugFormat("Synchronising EmailAccount Completed ({0})", emailAccount.AccountName);
        }

        private static Dictionary<string, List<string>> BuildBaseEmailAddressBookEntitiesAndFields()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Contact", new List<string>() { "email" } },
                { "Lead", new List<string>() { "email" } }
            };
        }

        private bool WereNewMappingsModified(EmailAccount emailAccount, DateTime? sinceDate)
        {
            if (sinceDate == null)
            {
                // Indicates that this is the first ever sync, so don't flag new mappings
                return false;
            }

            ICollection<DataFieldMapping> mappings = Slx.GetDataMappings(emailAccount.Id);
            DataFieldMapping firstMappingModifiedSinceDate = mappings.FirstOrDefault(m => m.ModifyDate >= sinceDate);
            return firstMappingModifiedSinceDate != null;
        }

        private void CreateDefaultMappings(DataLabel slxLabel, string newlabelId)
        {
            // If this is one of the 'system' labels, then create some default mappings
            DataLabel addedLabel = null;
            foreach (MappingDefinition defaultMap in defaultDataMappings.Where(m => string.Equals(m.LabelName, slxLabel.Name, StringComparison.OrdinalIgnoreCase)))
            {
                if (addedLabel == null)
                {
                    // N.B. Need to load the actual 'live' datalabel record to keep sdata happy.
                    addedLabel = Slx.GetRecord<DataLabel>(newlabelId);
                }

                DataFieldMapping newFieldMapping = new DataFieldMapping();
                newFieldMapping.EmailAccountId = slxLabel.EmailAccountId;
                newFieldMapping.EntityType = defaultMap.EntityType;
                newFieldMapping.FieldName = defaultMap.FieldName;
                newFieldMapping.LinkedFieldName = defaultMap.SubFieldName;
                newFieldMapping.MapDirection = DataFieldMapping.InformationFlowsFromCrm;
                newFieldMapping.DataLabel = ((AtomEntry)addedLabel.SourceData).GetSDataPayload();

                Logger.InfoFormat("Creating Default Mapping for Data Label ({0}) - ({1}.{2}.{3})", defaultMap.LabelName, defaultMap.EntityType, defaultMap.FieldName, defaultMap.SubFieldName);
                Slx.CreateRecord(newFieldMapping);
            }
        }

        public static bool SyncDataLabelsOnAddressBooksAccount(string slxEmailAddressBookId)
        {
            try
            {
                Synchroniser syncer = new Synchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());

                EmailAddressBook slxEmailAddressBook = syncer.Slx.GetRecord<EmailAddressBook>(slxEmailAddressBookId);
                if (slxEmailAddressBookId != null)
                {
                    string emailAccountId = slxEmailAddressBook.EmailAccountId;
                    EmailAccount emailAccount = syncer.Slx.GetRecord<EmailAccount>(emailAccountId);
                    if (emailAccount != null)
                    {
                        bool newLabelAdded;
                        DateTime syncTime = DateTime.UtcNow;
                        syncer.SyncDataLabels(emailAccount, syncTime, out newLabelAdded);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool SyncDataLabelsOnEmailAccount(EmailAccount emailAccount)
        {
            try
            {
                Synchroniser syncer = new Synchroniser(ObjectFactory.Instance.GetSlxConnector(), ObjectFactory.Instance.GetDotMailerConnector());

                bool newLabelAdded;
                DateTime syncTime = DateTime.UtcNow;
                syncer.SyncDataLabels(emailAccount, syncTime, out newLabelAdded);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void SyncDataLabels(EmailAccount emailAccount, DateTime syncTime, out bool newLabelAdded)
        {
            Logger.InfoFormat("Synchronising Data Labels started for email account ({0})", emailAccount.AccountName);
            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();
            newLabelAdded = false;

            RemoveDataLabelsThatHaveBeenDeletedInSaleslogixFromDotMailer(emailAccount);
            WhereNewDataLabelsHaveBeenAddedToSaleslogixSinceTheLastEmailAccountSyncCreateThemInDotMailer(emailAccount, syncTime, ref newLabelAdded);
            CreateOrUpdateDataLabelsInSaleslogixWhereDotMailerHasUnknownLabelsOrLabelsWithChangedIsPrivateFlag(emailAccount);

            Logger.InfoFormat("Synchronising Data Labels complete for email account ({0})", emailAccount.AccountName);
        }

        private void CreateOrUpdateDataLabelsInSaleslogixWhereDotMailerHasUnknownLabelsOrLabelsWithChangedIsPrivateFlag(EmailAccount emailAccount)
        {
            ICollection<DataLabel> allLabels = Slx.GetDataLabelsAddedBetween(emailAccount.Id, SlxSdata.MinimumDateTimeValue, DateTime.UtcNow);
            DmDataFieldDefinitionCollection dmDataFields = DotMailer.GetDataFields();
            foreach (var dataField in dmDataFields)
            {
                DataLabel slxLabel = allLabels.FirstOrDefault(slx => string.Equals(slx.Name, dataField.Name, StringComparison.OrdinalIgnoreCase));

                if (slxLabel == null)
                {
                    // Add it
                    slxLabel = new DataLabel();
                    slxLabel.Name = dataField.Name;
                    switch (dataField.Type)
                    {
                        case DmDataFieldTypes.Date:
                            slxLabel.DataType = "Date";
                            break;

                        case DmDataFieldTypes.Boolean:
                            slxLabel.DataType = "Boolean";
                            break;

                        case DmDataFieldTypes.Decimal:
                            slxLabel.DataType = "Numeric";
                            break;

                        case DmDataFieldTypes.String:
                            slxLabel.DataType = "String";
                            break;

                        default:
                            throw new InvalidOperationException(string.Format("Invalid data field data type ({0})", dataField.Type));
                    }

                    // N.B. API does not give access to default value.
                    slxLabel.IsPrivate = dataField.IsPrivate;
                    slxLabel.SyncedWithEmailService = true;
                    slxLabel.EmailAccountId = emailAccount.Id;

                    Logger.DebugFormat("Creating data label ({0}) in SalesLogix", slxLabel.Name);
                    string newlabelId = Slx.CreateRecord(slxLabel);
                    slxLabel.Id = newlabelId;

                    CreateDefaultMappings(slxLabel, newlabelId);
                }
                else
                {
                    if (slxLabel.IsPrivate != dataField.IsPrivate)
                    {
                        slxLabel.IsPrivate = dataField.IsPrivate;
                        Logger.DebugFormat("Updating data label ({0}) in SalesLogix to isprivate = ({0})", slxLabel.Name, slxLabel.IsPrivate);
                        Slx.UpdateRecord(slxLabel);
                    }
                }
            }
        }

        private void WhereNewDataLabelsHaveBeenAddedToSaleslogixSinceTheLastEmailAccountSyncCreateThemInDotMailer(EmailAccount emailAccount, DateTime syncTime, ref bool newLabelAdded)
        {
            ICollection<DataLabel> newLabels = Slx.GetDataLabelsAddedBetween(emailAccount.Id, emailAccount.LastSynchronised, syncTime);
            foreach (DataLabel newLabel in newLabels)
            {
                if (!newLabel.SyncedWithEmailService)
                {
                    Logger.InfoFormat("Creating data label ({0}) for email account ({1})", newLabel.Name, newLabel.EmailAccountId);
                    DotMailer.CreateDataField(newLabel.Name, newLabel.DataType, newLabel.DefaultValue);
                    newLabel.SyncedWithEmailService = true;
                    Slx.UpdateDataLabel(newLabel);
                    newLabelAdded = true;
                }
            }
        }

        private void RemoveDataLabelsThatHaveBeenDeletedInSaleslogixFromDotMailer(EmailAccount emailAccount)
        {
            ICollection<DeletedItem> deletedLabelsForAllAccounts = Slx.GetDeletedItemsUnprocessed("EMDatalabel");
            foreach (var deletedlabel in deletedLabelsForAllAccounts)
            {
                XmlSerializer xs = new XmlSerializer(typeof(DeletedLabelDetails));
                StringReader sr = new StringReader(deletedlabel.Data);
                DeletedLabelDetails deletedLabelDetails = (DeletedLabelDetails)xs.Deserialize(sr);

                bool deletedLabelBelongsToThisAccount = string.Equals(deletedLabelDetails.EmailAccountId, emailAccount.Id, StringComparison.OrdinalIgnoreCase);
                if (deletedLabelBelongsToThisAccount)
                {
                    if (string.IsNullOrWhiteSpace(deletedLabelDetails.Name))
                        Logger.WarnFormat("Couldn't delete data label ({0}) because its name is blank or null", deletedLabelDetails.DataLabelId);
                    else
                    {
                        Logger.InfoFormat("Deleting data label ({0}) from email account ({1})", deletedLabelDetails.Name, deletedLabelDetails.EmailAccountId);
                        DotMailer.DeleteDataField(deletedLabelDetails.Name);
                    }

                    deletedlabel.ProcessedByEMSync = true;
                    Slx.UpdateDeletedItem(deletedlabel);
                }
            }
        }

        private Dictionary<string, List<string>> GetSubEntitiesAndFields(EmailAccount emailAccount)
        {
            Dictionary<string, List<string>> subEntitiesAndFields = BuildBaseEmailAddressBookEntitiesAndFields();
            var dataMappings = Slx.GetDataMappings(emailAccount.Id);
            foreach (var dataMapping in dataMappings)
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

            return subEntitiesAndFields;
        }

        private void SyncEmailCampaignHeaderInfo(EmailAccount emailAccount, DateTime syncTime)
        {
            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();

            if (DotMailerApiCredentialsAreBlank(DotMailer.Username, DotMailer.Password))
            {
                Logger.DebugFormat("The login credentials for Email Account ({0}) are empty.", emailAccount.AccountName);
                return;
            }

            var dotMailerCampaigns = DotMailer.GetEmailCampaigns();
            var slxEmailCampaigns = Slx.GetEmailCampaigns(emailAccount.Id);

            CampaignMapper mapper = new CampaignMapper();

            bool errorOccurredSyncingCampaignHeaders = false;
            foreach (var dmCampaign in dotMailerCampaigns)
            {
                try
                {
                    EmailCampaign slxCampaign = slxEmailCampaigns.FirstOrDefault(c => c.DotMailerId == dmCampaign.Id);

                    bool emailCampaignExistsOnlyInDotMailer = slxCampaign == null;
                    if (emailCampaignExistsOnlyInDotMailer)
                    {
                        // A new email campaign that has not been loaded into SLX yet
                        slxCampaign = mapper.CreateSlxEmailCampaign(dmCampaign, emailAccount.Id);
                        slxCampaign.LastSynchronised = DateTime.UtcNow;
                        Logger.DebugFormat("Creating Email Campaign ({0}) in SalesLogix for EmailAccount ({1})", dmCampaign.Name, emailAccount.Description);
                        Slx.CreateRecord(slxCampaign);
                        Logger.DebugFormat("Created Email Campaign ({0}) in SalesLogix for EmailAccount ({1})", dmCampaign.Name, emailAccount.Description);
                    }
                    else
                    {
                        // An email campaign that is already loaded into SLX
                        bool modified = mapper.UpdateSlxEmailCampaign(dmCampaign, slxCampaign);
                        slxCampaign.CopyStatisticsFrom(dmCampaign);
                        if (modified)
                            Logger.DebugFormat("Updating Email Campaign ({0}) in SalesLogix for EmailAccount ({1})", dmCampaign.Name, emailAccount.Description);
                        else
                            Logger.DebugFormat("Only updating sync date - No changes to Email Campaign ({0}) in SalesLogix for EmailAccount ({1})", dmCampaign.Name, emailAccount.Description);

                        slxCampaign.LastSynchronised = DateTime.UtcNow;
                        Slx.UpdateEmailCampaign(slxCampaign);
                        Logger.DebugFormat("Updated Email Campaign ({0}) in SalesLogix for EmailAccount ({1})", dmCampaign.Name, emailAccount.Description);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("An exception was thrown whilst trying to synchronise headers for this email campaign: " + (dmCampaign.Name ?? ""), ex);
                    errorOccurredSyncingCampaignHeaders = true;
                }
            }
            if (errorOccurredSyncingCampaignHeaders)
                throw new EmailCampaignHeaderSynchronisationException("An exception was thrown during the synchronisation of at least one email campaign's headers");
        }

        private static bool DotMailerApiCredentialsAreBlank(string apiKey, string password)
        {
            if ((apiKey == null) || (password == null))
                return true;
            if ((apiKey.Trim() == "") || (password.Trim() == ""))
                return true;
            return false;
        }

        private void ProcessGlobalSuppressions(EmailAccount emailAccount)
        {
            Logger.InfoFormat("Starting to process suppressions for EmailAccount ({0})", emailAccount.AccountName);
            DotMailer.Username = emailAccount.ApiKey;
            DotMailer.Password = emailAccount.GetDecryptedPassword();

            DateTime lastSyncTime = emailAccount.LastSynchronised.GetValueOrDefault(DateTime.MinValue);
            DmSuppressedContactCollection suppressions = DotMailer.GetSuppressionsSince(lastSyncTime);
            int numSuppressions = 0;

            if (suppressions != null)
            {
                numSuppressions = suppressions.Count;
                HashSet<string> leadsSuppressed = new HashSet<string>();
                HashSet<string> contactsSuppressed = new HashSet<string>();
                List<string> addrBookMembersToRemove = new List<string>();
                foreach (DmSuppressedContact suppression in suppressions)
                {
                    // Is it already on the suppression list?
                    EmailSuppression slxSuppression = Slx.GetEmailSuppressionByEmailAddr(emailAccount.Id, suppression.Email);

                    bool suppressionDoesNotExistYetInSaleslogix = slxSuppression == null;
                    if (suppressionDoesNotExistYetInSaleslogix)
                    {
                        // If not, add it
                        slxSuppression = new EmailSuppression();
                        slxSuppression.EmailAccountId = emailAccount.Id;
                        slxSuppression.EmailServiceContactId = suppression.Id;
                        slxSuppression.EmailAddress = suppression.Email;
                        slxSuppression.DateRemoved = suppression.DateSuppressed;
                        slxSuppression.Reason = suppression.SuppressionCause.ToString();

                        Logger.DebugFormat("Adding suppression to SalesLogix for ({0}) in EmailAccount ({1})", suppression.Email, emailAccount.AccountName);
                        Slx.CreateRecord(slxSuppression);

                        // Get all email address book members for this email address so we can remove and mark as DoNotEmail
                        ICollection<EmailAddressBookMember> addrBookMembers = Slx.GetEmailAddressBookMembersByEmailAddr(QueryEntityType.EmailAccount, emailAccount.Id, suppression.Email);
                        if (addrBookMembers != null)
                        {
                            foreach (EmailAddressBookMember addrBookMember in addrBookMembers)
                            {
                                Logger.DebugFormat("Batching AddressBookMember ({0}) to be removed from AddressBook ({1})", addrBookMember.Id, addrBookMember.EmailAddressBookId);
                                addrBookMembersToRemove.Add(addrBookMember.Id);

                                switch (addrBookMember.SlxMemberType.ToUpperInvariant())
                                {
                                    case "CONTACT":
                                        if (!contactsSuppressed.Contains(addrBookMember.SlxContactId))
                                        {
                                            Logger.DebugFormat("Batching Contact to be set to DoNotEmail ({0})", addrBookMember.SlxContactId);
                                            contactsSuppressed.Add(addrBookMember.SlxContactId);
                                        }

                                        break;

                                    case "LEAD":
                                        if (!leadsSuppressed.Contains(addrBookMember.SlxContactId))
                                        {
                                            Logger.DebugFormat("Batching Lead to be set to DoNotEmail ({0})", addrBookMember.SlxLeadId);
                                            leadsSuppressed.Add(addrBookMember.SlxLeadId);
                                        }

                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("{0} is not a valid address book member type", addrBookMember.SlxMemberType));
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("Suppression already exists in SalesLogix for ({0}) in EmailAccount ({1})", suppression.Email, emailAccount.AccountName);
                    }
                }

                if (addrBookMembersToRemove.Count > 0)
                {
                    // Process AddressBookMember deletions
                    Logger.DebugFormat("Removing ({0}) email address book members", addrBookMembersToRemove.Count);
                    Slx.BatchDeleteRecords<EmailAddressBookMember>(addrBookMembersToRemove);

                    // Set DoNotEmail on Contacts
                    Logger.DebugFormat("({0}) Contacts in list to be set to DoNotEmail", contactsSuppressed.Count);
                    var contacts = Slx.GetRecords<SlxContact>(contactsSuppressed);
                    if (contacts != null && contacts.Count > 0)
                    {
                        Logger.DebugFormat("({0}) Contacts retrieved for setting DoNotEmail", contacts.Count);
                        foreach (var contact in contacts)
                        {
                            contact.DoNotEmail = true;
                        }

                        Slx.BatchUpdateRecords<SlxContact>(contacts);
                        Logger.Debug("Batch of Contacts updated");
                    }

                    // Set DoNotEmail on Leads
                    Logger.DebugFormat("({0}) Leads in list to be set to DoNotEmail", leadsSuppressed.Count);
                    var leads = Slx.GetRecords<SlxLead>(leadsSuppressed);
                    if (leads != null && leads.Count > 0)
                    {
                        Logger.DebugFormat("({0}) Leads retrieved for setting DoNotEmail", leads.Count);
                        foreach (var lead in leads)
                        {
                            lead.DoNotEmail = true;
                        }

                        Slx.BatchUpdateRecords<SlxLead>(leads);
                        Logger.Debug("Batch of Leads updated");
                    }
                }
            }

            Logger.InfoFormat("Finished processing ({1}) suppressions for EmailAccount ({0})", emailAccount.AccountName, numSuppressions);
        }

        private class MappingDefinition
        {
            /// <summary>
            /// Initializes a new instance of the MappingDefinition class.
            /// </summary>
            public MappingDefinition(string entityType, string labelName, string fieldName, string subFieldName)
            {
                EntityType = entityType;
                LabelName = labelName;
                FieldName = fieldName;
                SubFieldName = subFieldName;
            }

            public string EntityType { get; private set; }

            public string LabelName { get; private set; }

            public string FieldName { get; private set; }

            public string SubFieldName { get; private set; }
        }
    }
}