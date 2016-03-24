namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    /// <summary>
    /// Details of a member/contact who has unsubscribed from a specific email address book.
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailAddressBookUnsubscriber : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMAddrBookUnsubscriber";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailaddressbookunsubscribers";

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

        /// <summary>Gets or sets the email address of the unsubscriber.</summary>
        [SchemaName("EmailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>Gets or sets the email service ID of the contact that has unsubscribed.</summary>
        [SchemaName("EmailServiceContactID")]
        public int? EmailServiceContactId { get; set; }

        /// <summary>Gets or sets the date and time that of the the unsubscribe.</summary>
        [SchemaName("DateUnsubscribed")]
        public DateTime? DateUnsubscribed { get; set; }

        /// <summary>Gets or sets the reason for unsubscribing.</summary>
        [SchemaName("ReasonForUnsubscribing")]
        public string ReasonForUnsubscribing { get; set; }

        /// <summary>Gets or sets the ID of the email account that this unsubscriber belongs to.</summary>
        [SchemaName("EMEmailAccountID")]
        public string EmailAccountId { get; set; }

        public override string ToString()
        {
            return EmailAddress;
        }
    }
}