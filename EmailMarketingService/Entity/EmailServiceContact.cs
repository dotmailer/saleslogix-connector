using dotMailer.Sdk.Collections;
using dotMailer.Sdk.Contacts.DataFields;
using dotMailer.Sdk.Enums;

namespace EmailMarketing.SalesLogix.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using dotMailer.Sdk.Interfaces;

    public class EmailServiceContact : IDmContact
    {
        private DmDataFieldValueCollection dataFields;

        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Postcode { get; set; }
        public Dictionary<string, object> DataFieldValues { get; set; }

        public void AssignDmDataFields(DmDataFieldValueCollection defaultDataFields)
        {
            dataFields = defaultDataFields;
            foreach (var kvp in DataFieldValues)
            {
                DmDataFieldValue field = dataFields.FirstOrDefault(
                    f => string.Equals(f.Definition.Name, kvp.Key, StringComparison.OrdinalIgnoreCase));
                if (field == null)
                {
                    throw new InvalidOperationException(string.Format("Data field ({0}) was expected in email service GetDefaultEmptyDataFieldValues collection but does not exist.", kvp.Key));
                }

                field.Value = kvp.Value;
            }
        }

        void IDmContact.SetContactId(int newId)
        {
#if DEBUG
            //During testing this'll give us an exception, in the release version it will fail silently.
            throw new NotImplementedException("SetContactId method is not supported by EmailServiceContact class.");
#endif
        }

        ContactAudience IDmContact.Audience
        {
            get { return ContactAudience.Unknown; }
        }

        DmDataFieldValueCollection IDmContact.DataFields
        {
            get { return dataFields; }
        }

        string IDmContact.Email
        {
            get { return EmailAddress; }
        }

        EmailContentTypes IDmContact.EmailType
        {
            get { return EmailContentTypes.Html; }
        }

        bool IDmContact.HasDataFields
        {
            get { return true; }
        }

        int IDmContact.Id
        {
            get { return 0; }
        }

        string IDmContact.Notes
        {
            get { return string.Empty; }
        }

        EmailOptIn IDmContact.OptIn
        {
            get { return EmailOptIn.Single; }
        }

        int IDmObject.AccountId
        {
            get { return 1; }
        }

        string IDmObject.ExternalId
        {
            get { return "0"; }
        }

        bool IDmObject.IsDirty
        {
            get { return false; }
        }

        string IDmObject.ObjectType
        {
            get
            {
                return (GetType().FullName);
            }
            set
            {
                // Do nothing
            }
        }
    }
}