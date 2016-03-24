namespace EmailMarketing.SalesLogix.Entities
{
    using Sage.SData.Client.Extensions;

    /// <summary>
    /// A 'person' (Contact or Lead) who forms part of a list of people who can be added to an Email Marketing Campaign.
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailAddressBookMember : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMAddressBookMember";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailaddressbookmembers";

        /// <summary>Gets or sets the ID of the email address book that this member belongs to.</summary>
        [SchemaName("Emaddressbookid")]
        public string EmailAddressBookId { get; set; }

        /// <summary>Gets or sets the type of entity that this member represents (Lead or Contact)</summary>
        [SchemaName("SlxMemberType")]
        public string SlxMemberType { get; set; }

        /// <summary>Gets or sets the ID of the Contact that this member represents (If it represents a Contact).</summary>
        [SchemaName("SlxContactID")]
        public string SlxContactId { get; set; }

        /// <summary>Gets or sets the ID of the Lead that this member represents (If it represents a Lead).</summary>
        [SchemaName("SlxLeadID")]
        public string SlxLeadId { get; set; }

        /// <summary>Gets or sets the hash code of the combined data that was last synchronised.</summary>
        [SchemaName("LastSyncHashCode")]
        public int? LastSyncHashCode { get; set; }

        /// <summary>Gets or sets the hash code of the combined data that is currently waiting to be imported into the email service.</summary>
        [SchemaName("PendingImportHashCode")]
        public int? PendingImporthashCode { get; set; }

        /// <summary>Gets or sets the ID of the member within the email service.</summary>
        [SchemaName("EmailServiceContactId")]
        public bool EmailServiceContactId { get; set; }

        /// <summary>Gets or sets an SData payload object that represents the linked SalesLogix Contact</summary>
        [SchemaName("Contact")]
        public SDataPayload Contact { get; set; }

        /// <summary>Gets or sets an SData payload object that represents the linked SalesLogix Lead</summary>
        [SchemaName("Lead")]
        public SDataPayload Lead { get; set; }

        /// <summary>Gets or sets the email address of the member that was last sent to the email service.</summary>
        [SchemaName("LastSyncedEmailAddress")]
        public string LastSyncedEmailAddress { get; set; }

        /// <summary>Gets or sets some text (currently GUID) that identifies the email service import which last loaded this member.</summary>
        [SchemaName("LastEmailServiceImport")]
        public string LastEmailServiceImport { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}