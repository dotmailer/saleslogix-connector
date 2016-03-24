namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    /// <summary>
    /// Header information for a list of Contacts/Leads who can be added to an Email Marketing Campaign.
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailAddressBook : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMAddressBook";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailaddressbooks";

        public const string SyncStatusSyncedToEmailService = "Synced To Email Service";
        public const string SyncStatusSyncedFromEmailService = "Synced From Email Service";
        public const string SyncStatusLinkedWithEmailService = "Linked with Email Service";
        public const string SyncStatusDeletedFromService = "Error - Not Found in Email Service";
        public const string SyncStatusSyncError = "Sync Error";
        public const string SyncStatusAddressBookNameReservedByEmailService = "Error - Address book name is reserved by email service";

        /// <summary>Gets a default sync order to use when none has been specified.</summary>
        public static int DefaultSyncOrder
        {
            get
            {
                return 10;
            }
        }

        /// <summary>Gets or sets the ID of the email account that this address book belongs to.</summary>
        [SchemaName("Ememailaccountid")]
        public string EmailAccountId { get; set; }

        /// <summary>Gets or sets the status of synchronisation for this address book.</summary>
        [SchemaName("SyncStatus")]
        public string SyncStatus { get; set; }

        /// <summary>Gets or sets a number that specifies what order address books will be synchronised in (RESERVED FOR FUTURE USE).</summary>
        [SchemaName("SyncOrder")]
        public int SyncOrder { get; set; }

        /// <summary>Gets or sets the status of member synchronisation for this address book.</summary>
        [SchemaName("MemberSyncStatus")]
        public string MemberSyncStatus { get; set; }

        /// <summary>Gets or sets the date and time that the last member synchronisation was processed.</summary>
        [SchemaName("LastMemberChangeSynced")]
        public DateTime? LastMemberChangeSynced { get; set; }

        /// <summary>Gets or sets a data block used to track the progress of member synchronisation.</summary>
        [SchemaName("MemberSyncProgressObject")]
        public string MemberSyncProgressObject { get; set; }

        /// <summary>Gets or sets the name of the adress book.</summary>
        [SchemaName("Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets a text description of the address book.</summary>
        [SchemaName("Description")]
        public string Description { get; set; }

        /// <summary>Gets or sets a value indicating whether the address book should be excluded from automatic synchronisation.</summary>
        [SchemaName("ManualSyncOnly")]
        public bool ManualSyncOnly { get; set; }

        /// <summary>Gets or sets the ID of the address book within the email service.</summary>
        [SchemaName("EmailServiceAddressBookID")]
        public int EmailServiceAddressBookId { get; set; }

        /// <summary>Gets or sets the name of the address book within the email service.</summary>
        [SchemaName("EmailServiceAddressBookName")]
        public string EmailServiceAddressBookName { get; set; }

        /// <summary>Gets or sets the number of members within this address book.</summary>
        [SchemaName("EmailServiceAddressBookCount")]
        public int EmailServiceAddressBookContactCount { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}