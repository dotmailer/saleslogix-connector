using System.Diagnostics;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Entities;
    using Sage.SData.Client.Atom;
    using Sage.SData.Client.Core;
    using Sage.SData.Client.Extensions;

    public class SlxSdata : ISlxSdata
    {
        /// <summary>
        /// The minimum date time that we will use in our queries.
        /// </summary>
        public static readonly DateTime MinimumDateTimeValue = new DateTime(1901, 1, 1);

        /// <summary>
        /// Initializes a new instance of the SlxSdata class.
        /// </summary>
        public SlxSdata(string sdataUrl, string sdataUsername, string sdatapassword)
        {
            SdataUrl = sdataUrl;
            SdataUsername = sdataUsername;
            Sdatapassword = sdatapassword;
        }

        public string Sdatapassword { get; set; }

        public string SdataUrl { get; set; }

        public string SdataUsername { get; set; }

        public void BatchCreateRecords<T>(ICollection<T> recordsToCreate)
            where T : Entity
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            List<SDataSingleResourceRequest> createRequests = new List<SDataSingleResourceRequest>(recordsToCreate.Count);
            var constField = typeof(T).GetField("ResourceKind");
            string resourceKind = (string)constField.GetValue(null);
            foreach (var recordToCreate in recordsToCreate)
            {
                var createRequest = new SDataSingleResourceRequest(service);
                createRequest.ResourceKind = resourceKind;

                var payload = recordToCreate.BuildSDataPayload();
                AtomEntry entry = CreateSalesLogixAtomEntry();
                entry.SetSDataPayload(payload);

                createRequest.Entry = entry;

                createRequests.Add(createRequest);
            }

            using (SDataBatchRequest batch = new SDataBatchRequest(service))
            {
                batch.ResourceKind = resourceKind;
                foreach (var req in createRequests)
                {
                    req.Create();
                }

                batch.Commit();
            }
        }

        public void BatchDeleteRecords<T>(ICollection<string> idsOfRecordsToDelete)
            where T : Entity
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            List<SDataSingleResourceRequest> deleteRequests = new List<SDataSingleResourceRequest>(idsOfRecordsToDelete.Count);
            var constField = typeof(T).GetField("ResourceKind");
            string resourceKind = (string)constField.GetValue(null);
            foreach (var idOfRecordToDelete in idsOfRecordsToDelete)
            {
                var deleteRequest = new SDataSingleResourceRequest(service);
                deleteRequest.ResourceKind = resourceKind;
                deleteRequest.ResourceSelector = "'" + idOfRecordToDelete + "'";

                deleteRequests.Add(deleteRequest);
            }

            using (SDataBatchRequest batch = new SDataBatchRequest(service))
            {
                batch.ResourceKind = resourceKind;
                foreach (var req in deleteRequests)
                {
                    req.Delete();
                }

                batch.Commit();
            }
        }

        public void BatchUpdateRecords<T>(ICollection<T> recordsToUpdate, bool excludeLeadsAndContactsForEmailAddressBookMembers = true)
            where T : Entity
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            List<SDataSingleResourceRequest> updateRequests = new List<SDataSingleResourceRequest>(recordsToUpdate.Count);
            var constField = typeof(T).GetField("ResourceKind");
            string resourceKind = (string)constField.GetValue(null);
            foreach (var recordToUpdate in recordsToUpdate)
            {
                if (recordToUpdate.SourceData == null || string.IsNullOrWhiteSpace(recordToUpdate.Id))
                {
                    throw new ArgumentException("recordToUpdate must have its SourceData and Id populated. I.e. It must have been loaded from the DB before being updated.");
                }

                var updateRequest = new SDataSingleResourceRequest(service);
                updateRequest.ResourceKind = resourceKind;
                updateRequest.ResourceSelector = "'" + recordToUpdate.Id + "'";

                AtomEntry entry = (AtomEntry)recordToUpdate.SourceData;
                SDataPayload payload = recordToUpdate.BuildSDataPayload();

                // Special case for Email Address Book Members
                if ((excludeLeadsAndContactsForEmailAddressBookMembers) && (typeof(T) == typeof(EmailAddressBookMember)))
                {
                    payload.Values.Remove("Contact");
                    payload.Values.Remove("Lead");
                }

                entry.SetSDataPayload(payload);

                updateRequest.Entry = entry;
                updateRequest.ResourceSelector = string.Format("'{0}'", recordToUpdate.Id);

                updateRequests.Add(updateRequest);
            }

            using (SDataBatchRequest batch = new SDataBatchRequest(service))
            {
                batch.ResourceKind = resourceKind;
                foreach (var req in updateRequests)
                {
                    req.Update();
                }

                batch.Commit();
            }
        }

        public int CountEmailAddressBookMembers(string emailAddressBookId)
        {
            int count;
            string query = string.Format("Emaddressbookid eq '{0}'", CleanQueryParameterString(emailAddressBookId));
            count = CountRecords(EmailAddressBookMember.ResourceKind, query);
            return count;
        }

        public int CountRecords(string resourceKind, string whereClause = null)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = resourceKind;

            // Only get back the ID because we don't care about the data, only the count
            req.QueryValues.Add("select", "Id");
            req.Count = 1;  // This is the number of records per page, not a limit on the total number of records
            if (!string.IsNullOrWhiteSpace(whereClause))
            {
                req.QueryValues.Add("where", whereClause);
            }

            AtomFeedReader reader = req.ExecuteReader();

            if (reader == null)
            {
                return 0;
            }
            else
            {
                return reader.Count;
            }
        }

        public string CreateRecord(Entity recordToCreate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var createRequest = new SDataSingleResourceRequest(service);

            var constField = recordToCreate.GetType().GetField("ResourceKind");
            createRequest.ResourceKind = (string)constField.GetValue(null);

            var payload = recordToCreate.BuildSDataPayload();
            AtomEntry entry = CreateSalesLogixAtomEntry();
            entry.SetSDataPayload(payload);

            createRequest.Entry = entry;
            var res = createRequest.Create();
            string idCreated = res.GetSDataPayload().Key;
            return idCreated;
        }

        public void DeleteRecord<T>(string idOfRecordToDelete)
            where T : Entity
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var deleteRequest = new SDataSingleResourceRequest(service);

            var constField = typeof(T).GetField("ResourceKind");
            deleteRequest.ResourceKind = (string)constField.GetValue(null);
            deleteRequest.ResourceSelector = string.Format("'{0}'", idOfRecordToDelete);

            var res = deleteRequest.Delete();
        }

        /// <summary>
        /// Iterate over each Address Book Member so we can handle any amount of records by taking advantage
        /// of the built in paging ability of the AtomFeedReader.
        /// </summary>
        /// <returns>All Address Book Members, one at a time</returns>
        public IEnumerable<AtomEntry> GetAllAddressBookMembers()
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookMember.ResourceKind;

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    yield return entry;
                }
            }
        }

        public IEnumerable<AtomEntry> GetEnumerableAddressBookMembers(string emailAddressBookId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            SDataResourceCollectionRequest req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookMember.ResourceKind;
            req.QueryValues.Add("where", string.Format("Emaddressbookid eq '{0}'", CleanQueryParameterString(emailAddressBookId)));

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                foreach (var entry in reader)
                    yield return entry;
            }
        }

        public IList<AtomEntry> GetNewMemberEmailCampaigns(string emailAccountId, string emailAddressBookId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaign.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "SyncWithEmailService eq true and EMEmailAccountID eq '{0}' and CampaignType eq '{2}' and EMCampaignAddressBooks.EMAddressBookID eq '{1}'",
                    CleanQueryParameterString(emailAccountId),
                    CleanQueryParameterString(emailAddressBookId),
                    EmailCampaign.CampaignTypeNewMember));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetDataLabelsAddedBetween(string emailAccountId, DateTime startDate, DateTime endDate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = DataLabel.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "Ememailaccountid eq '{0}' and CreateDate gt @{1}@ and CreateDate lt @{2}@",
                    CleanQueryParameterString(emailAccountId),
                    startDate.ToString("s"),
                    endDate.ToString("s")));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                    atoms.Add(entry);
            }

            return atoms;
        }

        public IList<AtomEntry> GetDataLabelsModifiedBetween(string emailAccountId, DateTime startDate, DateTime endDate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = DataLabel.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "Ememailaccountid eq '{0}' and SyncWithEmailService = true and ModifyDate gt @{1}@ and ModifyDate lt @{2}@",
                    CleanQueryParameterString(emailAccountId),
                    startDate.ToString("s"),
                    endDate.ToString("s")));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetDataMappings(string emailAccountId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = DataFieldMapping.ResourceKind;
            req.QueryValues.Add("where", string.Format("EMEmailAccountID eq '{0}'", CleanQueryParameterString(emailAccountId)));
            req.QueryValues.Add("select", "*,EMDataLabel/*");

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetDeletedItemsUnprocessed(string exactFilterForEntityTypeField = "", string likeFilterForDataField = "")
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = DeletedItem.ResourceKind;

            string whereClause = "(ProcessedByEMSync eq false or ProcessedByEMSync eq null)";
            if (exactFilterForEntityTypeField != string.Empty)
            {
                whereClause += string.Format(" and EntityType eq '{0}'", CleanQueryParameterString(exactFilterForEntityTypeField));
            }

            if (likeFilterForDataField != string.Empty)
            {
                // Do not 'clean' this because it appears to be being cleaned inside of the request object.
                // This was converting "%abcde%" => "%25abcde%25" => "%2525abcde%2525"
                //whereClause += string.Format(" and Data like '{0}'", CleanQueryParameterString(likeFilterForDataField));
                whereClause += string.Format(" and Data like '{0}'", likeFilterForDataField);
            }

            req.QueryValues.Add("where", whereClause);

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetAllEmailAccounts()
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAccount.ResourceKind;

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailAccountsLastSyncedBefore(DateTime lastSynced)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAccount.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                "LastSynchronised lt @{0}@ or LastSynchronised eq null",
                lastSynced.ToString("s")));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailAddressBookMembers(string emailAddressBookId, Dictionary<string, List<string>> subEntitiesAndFields)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookMember.ResourceKind;
            req.QueryValues.Add("where", string.Format("Emaddressbookid eq '{0}'", CleanQueryParameterString(emailAddressBookId)));

            string selectQuery = GetMappingSelectQuery(subEntitiesAndFields);
            req.QueryValues.Add("select", selectQuery);

            AtomFeedReader reader = req.ExecuteReader();

            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailAddressBookMembersAddedBetween(
            string emailAddressBookId,
            DateTime startDate,
            DateTime endDate,
            Dictionary<string, List<string>> subEntitiesAndFields)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookMember.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "Emaddressbookid eq '{0}' and CreateDate gt @{1}@ and CreateDate lt @{2}@",
                    CleanQueryParameterString(emailAddressBookId),
                    startDate.ToString("s"),
                    endDate.ToString("s")));

            string selectQuery = GetMappingSelectQuery(subEntitiesAndFields);
            req.QueryValues.Add("select", selectQuery);

            AtomFeedReader reader = req.ExecuteReader();

            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                    atoms.Add(entry);
            }

            return atoms;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="typeOfQuery">Either QueryEntityType.EmailAccount or QueryEntityType.EmailAddressBook</param>
        /// <param name="idOfQueryEntity"></param>
        /// <param name="emailAddress"></param>
        /// <param name="subEntitiesAndFields"></param>
        /// <returns></returns>
        public IList<AtomEntry> GetEmailAddressBookMembersByEmailAddr(
            QueryEntityType typeOfQuery,
            string idOfQueryEntity,
            string emailAddress,
            Dictionary<string, List<string>> subEntitiesAndFields = null)
        {
            IList<AtomEntry> result = GetEmailAddressBookMembersByEmailAddresses(typeOfQuery, idOfQueryEntity, new string[] { emailAddress },
                subEntitiesAndFields);
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="typeOfQuery">Either QueryEntityType.EmailAccount or QueryEntityType.EmailAddressBook</param>
        /// <param name="idOfQueryEntity"></param>
        /// <param name="emailAddresses">Email addresses for which to retrieve email address book members.</param>
        /// <param name="subEntitiesAndFields"></param>
        /// <returns></returns>
        public IList<AtomEntry> GetEmailAddressBookMembersByEmailAddresses(
            QueryEntityType typeOfQuery,
            string idOfQueryEntity,
            string[] emailAddresses,
            Dictionary<string, List<string>> subEntitiesAndFields = null)
        {
            if ((emailAddresses == null) || (emailAddresses.Length < 1))
                return new List<AtomEntry>();

            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookMember.ResourceKind;
            string queryEntityName;
            switch (typeOfQuery)
            {
                case QueryEntityType.EmailAccount:
                    queryEntityName = "EMAddressBook.Ememailaccountid";
                    break;

                case QueryEntityType.EmailAddressBook:
                    queryEntityName = "Emaddressbookid";
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid QueryEntity: ({0})", typeOfQuery));
            }

            if (emailAddresses.Length == 1)
            {
                req.QueryValues.Add(
                    "where",
                    string.Format(
                        "{2} eq '{0}' and LastSyncedEmailAddress eq {1}",
                        CleanQueryParameterString(idOfQueryEntity),
                        PrepareQuotedStringLiteral(emailAddresses[0]),
                        queryEntityName));
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < emailAddresses.Length; i++)
                {
                    if (i > 0)
                        sb.Append(",");
                    string quotedEmailAddress = PrepareQuotedStringLiteral(emailAddresses[i]);
                    sb.Append(quotedEmailAddress);
                }

                req.QueryValues.Add(
                    "where",
                    string.Format(
                        "{2} eq '{0}' and LastSyncedEmailAddress in ({1})",
                        CleanQueryParameterString(idOfQueryEntity),
                        sb,
                        queryEntityName));
            }

            if (subEntitiesAndFields != null)
            {
                string selectQuery = GetMappingSelectQuery(subEntitiesAndFields);
                req.QueryValues.Add("select", selectQuery);
            }

            List<AtomEntry> atoms = new List<AtomEntry>();

            const int maxUrlLength = 1024;  //553 bytes was normal before we were batching up multiple email addresses like this.  This site claims that IIS can handle 16K by default (but it is configurable): http://technomanor.wordpress.com/2012/04/03/maximum-url-size/
            string urlToUse = req.ToString();
            bool urlTooLong = urlToUse.Length > maxUrlLength;
            if (urlTooLong && emailAddresses.Length > 1) //If the URL is too long and we're only using a single email address, then we have no better option than just to go ahead and send it anyway.
            {
                //If the URL is in danger of being too long then we split the job into two requests.  This is best avoided as reduces efficiency.
                int halfCount = emailAddresses.Length / 2;
                int remainder = emailAddresses.Length - halfCount;
                Debug.Assert(halfCount > 0);
                string[] emailAddresses1 = new string[halfCount];
                string[] emailAddresses2 = new string[remainder];
                Array.Copy(emailAddresses, 0, emailAddresses1, 0, halfCount);
                Array.Copy(emailAddresses, halfCount, emailAddresses2, 0, remainder);
                IList<AtomEntry> atoms1 = GetEmailAddressBookMembersByEmailAddresses(typeOfQuery, idOfQueryEntity, emailAddresses1, subEntitiesAndFields);
                IList<AtomEntry> atoms2 = GetEmailAddressBookMembersByEmailAddresses(typeOfQuery, idOfQueryEntity, emailAddresses2, subEntitiesAndFields);
                atoms.AddRange(atoms1);
                atoms.AddRange(atoms2);
            }
            else
            {
                AtomFeedReader reader = req.ExecuteReader();

                if (reader != null)
                {
                    foreach (var entry in reader)
                        atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailAddressBookMembersFromImport(string emailAddressBookId, Guid importGuid)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookMember.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "Emaddressbookid eq '{0}' and LastEmailServiceImport eq '{1}'",
                    CleanQueryParameterString(emailAddressBookId),
                    importGuid.ToString("B")));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailAddressBookMembersModifiedBetween(
            string emailAddressBookId,
            DateTime startDate,
            DateTime endDate,
            Dictionary<string, List<string>> subEntitiesAndFields)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            // When running this sdata query, if you specify a criterion on an entity that is null, then
            // no records are returned.  Since our AddressBookMember will always be linked to
            // EITHER a Lead or a Contact, then one of those links will always be null.  The same applies if
            // the data mappings specify a sub-entity.  We will need to run one query per entity.

            List<string> whereClauses = new List<string>();
            string whereClausePrefix = string.Format(
                "Emaddressbookid eq '{0}'",
                CleanQueryParameterString(emailAddressBookId));

            // Build the select clause which specifies which entities & fields to select.
            // Also, add a where clause to the list for every sub-entity (e.g. Contact, Lead, Contact.Address etc.)
            StringBuilder selectQueryBuilder = new StringBuilder("*,"); // Include all fields of AddressBookMember
            foreach (KeyValuePair<string, List<string>> entityWithFields in subEntitiesAndFields)
            {
                foreach (string fieldName in entityWithFields.Value)
                {
                    selectQueryBuilder.AppendFormat("{0}/{1},", entityWithFields.Key, fieldName);
                }

                // In a "select" statement, sub-entity references look like "Contact/Address",
                // but in a "where" statement they look like "Contact.Address"
                string entitySelector = entityWithFields.Key.Replace('/', '.');

                whereClauses.Add(
                    string.Format(
                        "{0} and ({1}.ModifyDate gt @{2}@ and {1}.ModifyDate lt @{3}@)",
                        whereClausePrefix,
                        entitySelector,
                        startDate.ToString("s"),
                        endDate.ToString("s")));
            }

            List<AtomEntry> atoms = new List<AtomEntry>();
            HashSet<string> entityIdsAlreadyRetrieved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var whereClause in whereClauses)
            {
                var req = new SDataResourceCollectionRequest(service);
                req.ResourceKind = EmailAddressBookMember.ResourceKind;
                req.QueryValues.Add("select", selectQueryBuilder.ToString());
                req.QueryValues.Add("where", whereClause);

                AtomFeedReader reader = req.ExecuteReader();

                if (reader != null)
                {
                    foreach (var entry in reader)
                    {
                        // Make sure a particular AddressBookMember only appears once in our return list
                        var payload = entry.GetSDataPayload();
                        string entityId = payload.Key;
                        if (!entityIdsAlreadyRetrieved.Contains(entityId))
                        {
                            atoms.Add(entry);
                            entityIdsAlreadyRetrieved.Add(entityId);
                        }
                    }
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailAddressBooks(string emailAccountId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBook.ResourceKind;
            req.QueryValues.Add("where", string.Format("Ememailaccountid eq '{0}'", CleanQueryParameterString(emailAccountId)));
            req.QueryValues.Add("orderBy", "SyncOrder");

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailAddressBooksForEmailCampaign(string emailCampaignId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBook.ResourceKind;
            req.QueryValues.Add("where", string.Format("EMCampaignAddressBooks.Ememailcampaignid eq '{0}'", CleanQueryParameterString(emailCampaignId)));
            req.QueryValues.Add("orderBy", "SyncOrder");

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public AtomEntry GetEmailAddressBookUnsubscriber(string emailAddressBookId, string emailAddress)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookUnsubscriber.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                "Emaddressbookid eq '{0}' and EmailAddress eq '{1}'",
                CleanQueryParameterString(emailAddressBookId),
                CleanQueryParameterString(emailAddress)));

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count <= 0)
                {
                    return null;
                }
                else if (reader.Count == 1)
                {
                    return reader[0];
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "EmailAddressBookUnsubscriber record should only exist once for emailAddressBookId({0}) and EmailAddress({1}) but actually exists ({2}) times.",
                              emailAddressBookId,
                              emailAddress,
                              reader.Count));
                }
            }

            return null;
        }

        public AtomEntry GetEmailCampaignAddressBookLink(string emailCampaignId, string emailAddressBookId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaignAddressBookLink.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                "Ememailcampaignid eq '{0}' and EMAddressBookID eq '{1}'",
                CleanQueryParameterString(emailCampaignId),
                CleanQueryParameterString(emailAddressBookId)));

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count <= 0)
                    return null;
                if (reader.Count == 1)
                    return reader[0];
                throw new InvalidOperationException(
                    string.Format(
                        "EmailCampaign to AddressBook link record should only exist once for EmailCampaign ({0}) and AdddressBook ({1}) but actually exists ({2}) times.",
                        emailCampaignId,
                        emailAddressBookId,
                        reader.Count));
            }

            return null;
        }

        public AtomEntry GetEmailCampaignByEmailServiceId(int emailServiceCampaignId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaign.ResourceKind;
            req.QueryValues.Add("where", string.Format("EmailServiceCampaignID eq {0}", emailServiceCampaignId));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                if (reader.Count != 1)
                {
                    return null;
                }
                else
                {
                    return reader[0];
                }
            }

            return null;
        }

        public AtomEntry GetEmailCampaignClick(string emailCampaignId, string emailAddress, DateTime dateClicked)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaignClick.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "Ememailcampaignid eq '{0}' and EmailAddress eq '{1}' and DateClicked eq @{2}@",
                    CleanQueryParameterString(emailCampaignId),
                    CleanQueryParameterString(emailAddress),
                    dateClicked.ToString("s")));

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count == 0)
                {
                    return null;
                }
                else
                {
                    return reader[0];
                }
            }

            return null;
        }

        public IList<AtomEntry> GetEmailCampaignClicksForEmailCampaignBetween(
            string slxCampaignId,
            DateTime startDate,
            DateTime endDate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaignClick.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                "Ememailcampaignid eq '{0}' and CreateDate gt @{1}@ and CreateDate lt @{2}@",
                CleanQueryParameterString(slxCampaignId),
                startDate.ToString("s"),
                endDate.ToString("s")));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailCampaignSends(string emailCampaignId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaignSend.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format("Ememailcampaignid eq '{0}'", CleanQueryParameterString(emailCampaignId)));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailCampaignSendSummaries(string emailCampaignId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaignSendSummary.ResourceKind;
            req.QueryValues.Add("where", string.Format("EMEmailCampaignID eq '{0}'", CleanQueryParameterString(emailCampaignId)));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public AtomEntry GetEmailCampaignSendSummary(string emailCampaignId, string emailAddress, DateTime sendDate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaignSendSummary.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "EMEmailCampaignID eq '{0}' and EmailAddress eq '{1}' and DateSent eq @{2}@",
                    CleanQueryParameterString(emailCampaignId),
                    CleanQueryParameterString(emailAddress),
                    sendDate.ToString("s")));

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count != 1)
                {
                    return null;
                }
                else
                {
                    return reader[0];
                }
            }

            return null;
        }

        public IList<AtomEntry> GetEmailCampaigns(string emailAccountId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaign.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "EMEmailAccountID eq '{0}'",
                    CleanQueryParameterString(emailAccountId)));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetEmailCampaignsLinkedToSlxCampaign()
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailCampaign.ResourceKind;
            req.QueryValues.Add("where", "SLXCampaignID ne null");

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public AtomEntry GetEmailSuppressionByEmailAddr(string emailAccountId, string emailAddress)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailSuppression.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "Ememailaccountid eq '{0}' and EmailAddress eq '{1}'",
                    CleanQueryParameterString(emailAccountId),
                    CleanQueryParameterString(emailAddress)));

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count <= 0)
                {
                    return null;
                }
                else if (reader.Count == 1)
                {
                    return reader[0];
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "EmailSuppression record should only exist once for EmailAccount ({0}) and EmailAddress ({1}) but actually exists ({2}) times.",
                              emailAccountId,
                              emailAddress,
                              reader.Count));
                }
            }

            return null;
        }

        public AtomEntry GetFirstEmailAddressBookMemberInCampaign(string emailCampaignId, string emailAddress)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = EmailAddressBookMember.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "EMAddressBook.EMCampaignAddressBooks.Ememailcampaignid eq '{0}' and LastSyncedEmailAddress eq '{1}'",
                    CleanQueryParameterString(emailCampaignId),
                    CleanQueryParameterString(emailAddress)));
            req.QueryValues.Add("orderBy", "CreateDate");

            // Include some fields of sub-entities that we will need
            req.QueryValues.Add("select", "*,Contact/NameLF,Contact/Account/Id,Contact/AccountName,Lead/LeadNameLastFirst");

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return reader[0];
                }
            }

            return null;
        }

        public AtomEntry GetRecord<T>(string slxRecordId)
            where T : Entity, new()
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataSingleResourceRequest(service);

            var constField = typeof(T).GetField("ResourceKind");
            req.ResourceKind = (string)constField.GetValue(null);

            req.ResourceSelector = string.Format("'{0}'", slxRecordId);

            AtomEntry entry = null;
            try
            {
                entry = req.Read();
            }
            catch (SDataClientException ex)
            {
                // N.B. The exception does not contain any data to indicate what went wrong, so the only way is to check the message.
                if (ex.Message == "Entity does not exist")
                {
                    throw new EntityNotFoundException(string.Format("Entity of type ({0}) with id ({1}) was not found.", typeof(T).Name, slxRecordId), typeof(T).Name, slxRecordId, ex);
                }

                throw;
            }

            return entry;
        }

        public List<AtomEntry> GetRecords<T>(ICollection<string> slxRecordIds, string fieldsToSelect = null)
            where T : Entity, new()
        {
            IEnumerable<string> quotedIds = slxRecordIds.Select(id => "'" + CleanQueryParameterString(id) + "'");
            string ids = string.Join(",", quotedIds);

            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            var constField = typeof(T).GetField("ResourceKind");
            req.ResourceKind = (string)constField.GetValue(null);
            req.QueryValues.Add("where", string.Format("Id in ({0})", ids));

            if (!string.IsNullOrWhiteSpace(fieldsToSelect))
            {
                req.QueryValues.Add("select", fieldsToSelect);
            }

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public AtomEntry GetSlxCampaignTarget(string slxCampaignId, string entityId, string targetType)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = SlxCampaignTarget.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format(
                    "Campaign.Id eq '{0}' and EntityId eq '{1}' and TargetType eq '{2}'",
                    CleanQueryParameterString(slxCampaignId),
                    CleanQueryParameterString(entityId),
                    CleanQueryParameterString(targetType)));

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return reader[0];
                }
            }

            return null;
        }

        public IList<AtomEntry> GetSyncTasksBeforeDate(DateTime endDateUtc, IEnumerable<string> statuses = null)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = SyncTask.ResourceKind;
            string whereClause = string.Format("ScheduledStartTime lt @{0}@", endDateUtc.ToString("s"));

            if (statuses != null)
            {
                string statusWhereClause = " and (";
                foreach (string status in statuses)
                {
                    statusWhereClause += string.Format("Status eq '{0}' or ", CleanQueryParameterString(status));
                }

                statusWhereClause = statusWhereClause.Substring(0, statusWhereClause.Length - 4);
                statusWhereClause += ")";
                whereClause += statusWhereClause;
            }

            req.QueryValues.Add("where", whereClause);
            req.QueryValues.Add("orderBy", "ScheduledStartTime");

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public IList<AtomEntry> GetTargetsForSlxCampaign(string slxCampaignId)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = SlxCampaignTarget.ResourceKind;
            req.QueryValues.Add(
                "where",
                string.Format("Campaign.Id eq '{0}'", CleanQueryParameterString(slxCampaignId)));

            AtomFeedReader reader = req.ExecuteReader();
            List<AtomEntry> atoms = new List<AtomEntry>();

            if (reader != null)
            {
                foreach (var entry in reader)
                {
                    atoms.Add(entry);
                }
            }

            return atoms;
        }

        public AtomEntry GetUserByUserName(string username)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);
            var req = new SDataResourceCollectionRequest(service);
            req.ResourceKind = SlxUser.ResourceKind;
            req.QueryValues.Add("where", string.Format("UserName eq '{0}'", CleanQueryParameterString(username)));
            req.QueryValues.Add("select", "UserName");

            AtomFeedReader reader = req.ExecuteReader();

            if (reader != null)
            {
                if (reader.Count != 1)
                {
                    return null;
                }
                else
                {
                    return reader[0];
                }
            }

            return null;
        }

        public void UpdateDataLabel(DataLabel dataLabelToUpdate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);
            updateRequest.ResourceKind = DataLabel.ResourceKind;
            AtomEntry entry = (AtomEntry)dataLabelToUpdate.SourceData;
            entry.SetSDataPayload(dataLabelToUpdate.BuildSDataPayload());

            updateRequest.Entry = entry;
            updateRequest.ResourceSelector = "'" + dataLabelToUpdate.Id + "'";
            updateRequest.Update();
        }

        public void UpdateDeletedItem(DeletedItem deletedItemToUpdate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);
            updateRequest.ResourceKind = DeletedItem.ResourceKind;
            AtomEntry entry = (AtomEntry)deletedItemToUpdate.SourceData;
            entry.SetSDataPayload(deletedItemToUpdate.BuildSDataPayload());

            updateRequest.Entry = entry;
            updateRequest.ResourceSelector = "'" + deletedItemToUpdate.Id + "'";
            updateRequest.Update();
        }

        public void UpdateEmailAccount(EmailAccount emailAccountToUpdate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);
            updateRequest.ResourceKind = EmailAccount.ResourceKind;
            AtomEntry entry = (AtomEntry)emailAccountToUpdate.SourceData;
            entry.SetSDataPayload(emailAccountToUpdate.BuildSDataPayload());

            updateRequest.Entry = entry;
            updateRequest.ResourceSelector = "'" + emailAccountToUpdate.Id + "'";
            updateRequest.Update();
        }

        public void UpdateEmailAddressBook(EmailAddressBook emailAddressBookToUpdate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);
            updateRequest.ResourceKind = EmailAddressBook.ResourceKind;
            AtomEntry entry = (AtomEntry)emailAddressBookToUpdate.SourceData;
            entry.SetSDataPayload(emailAddressBookToUpdate.BuildSDataPayload());

            updateRequest.Entry = entry;
            updateRequest.ResourceSelector = "'" + emailAddressBookToUpdate.Id + "'";
            updateRequest.Update();
        }

        public void UpdateEmailCampaign(EmailCampaign emailCampaignToUpdate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);
            updateRequest.ResourceKind = EmailCampaign.ResourceKind;
            AtomEntry entry = (AtomEntry)emailCampaignToUpdate.SourceData;
            entry.SetSDataPayload(emailCampaignToUpdate.BuildSDataPayload());

            updateRequest.Entry = entry;
            updateRequest.ResourceSelector = "'" + emailCampaignToUpdate.Id + "'";
            updateRequest.Update();
        }

        public void UpdateRecord(Entity recordToUpdate)
        {
            if (recordToUpdate.SourceData == null || string.IsNullOrWhiteSpace(recordToUpdate.Id))
            {
                throw new ArgumentException("recordToUpdate must not be null and must have its SourceData and Id populated. I.e. It must have been loaded from the DB before being updated.");
            }

            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);

            var constField = recordToUpdate.GetType().GetField("ResourceKind");
            updateRequest.ResourceKind = (string)constField.GetValue(null);

            AtomEntry entry = (AtomEntry)recordToUpdate.SourceData;
            entry.SetSDataPayload(recordToUpdate.BuildSDataPayload());

            updateRequest.Entry = entry;
            updateRequest.ResourceSelector = string.Format("'{0}'", recordToUpdate.Id);
            updateRequest.Update();
        }

        public void UpdateRecord(AtomEntry recordToUpdate, string resourceKind)
        {
            if (recordToUpdate == null || recordToUpdate.GetSDataPayload() == null || string.IsNullOrWhiteSpace(recordToUpdate.GetSDataPayload().Key))
            {
                throw new ArgumentException("recordToUpdate must not be null");
            }

            SDataPayload payload = recordToUpdate.GetSDataPayload();
            if (payload == null || string.IsNullOrWhiteSpace(payload.Key))
            {
                throw new ArgumentException("recordToUpdate must contain an SDataPayload with a .Key set.");
            }

            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);

            updateRequest.ResourceKind = resourceKind;
            updateRequest.Entry = recordToUpdate;
            updateRequest.ResourceSelector = string.Format("'{0}'", payload.Key);
            updateRequest.Update();
        }

        public void UpdateSyncTask(SyncTask syncTaskToUpdate)
        {
            ISDataService service = new SDataService(SdataUrl, SdataUsername, Sdatapassword);

            var updateRequest = new SDataSingleResourceRequest(service);
            updateRequest.ResourceKind = SyncTask.ResourceKind;
            AtomEntry entry = (AtomEntry)syncTaskToUpdate.SourceData;
            entry.SetSDataPayload(syncTaskToUpdate.BuildSDataPayload());

            updateRequest.Entry = entry;
            updateRequest.ResourceSelector = "'" + syncTaskToUpdate.Id + "'";
            updateRequest.Update();
        }

        //Calls to this method might be better replaced with calls to the PrepareQuotedStringLiteral method, as this method has certainly been shown to be lacking when handling email addresses with apostrophes in, and may be inappropriate in other circumstances too.
        public static string CleanQueryParameterString(string queryParameterToClean)
        {
            if (queryParameterToClean == null)
            {
                return null;
            }

            string cleaned = queryParameterToClean.Replace("%", "%25");
            cleaned = cleaned.Replace("'", "%27%27");
            return cleaned;
        }

        /// <summary>
        /// Given a string, adds quotes and escapes quotes within it.  This is a suitable format for a string literal in an SData query string.
        /// </summary>
        /// <param name="s">The value to represent as a quoted string.</param>
        /// <returns>A quoted and appropriately escaped string.</returns>
        public static string PrepareQuotedStringLiteral(string s)
        {
            //This code is based in information here:
            //http://interop.sage.com/daisy/sdata/AnatomyOfAnSDataURL/QueryLanguage.html
            //It has been well tested on email addresses including some very quirky ones.  While SData doesn't escape apostrophes in its URLs (and really should), I've not seen it cause any problems.  Characters such as % are escaped.

            if (s == null)
                return null;
            else
            {
                string result = s.Replace("'", "''");
                result = "'" + result + "'";
                return result;
            }
        }

        private static AtomEntry CreateSalesLogixAtomEntry()
        {
            AtomEntry entry = new AtomEntry();
            entry.Authors.Add(new AtomPersonConstruct() { Name = "SalesLogix" });
            return entry;
        }

        private static string GetMappingSelectQuery(Dictionary<string, List<string>> subEntitiesAndFields)
        {
            StringBuilder selectQueryBuilder = new StringBuilder("*,"); // Include all fields of AddressBookMember
            foreach (KeyValuePair<string, List<string>> entityWithFields in subEntitiesAndFields)
            {
                foreach (string fieldName in entityWithFields.Value)
                {
                    selectQueryBuilder.AppendFormat("{0}/{1},", entityWithFields.Key, fieldName);
                }
            }

            string selectQuery = selectQueryBuilder.ToString();
            return selectQuery;
        }
    }
}