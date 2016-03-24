namespace EmailMarketing.SalesLogix.Entities
{
    /// <summary>
    /// M:M link table between Email Campaign and Email Address Book.
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailCampaignAddressBookLink : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMCampaignAddressBook";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailcampaignaddressbooks";

        /// <summary>Gets or sets the ID of the Email Campaign that this record is linked to.</summary>
        [SchemaName("Ememailcampaignid")]
        public string EmailCampaignId { get; set; }

        /// <summary>Gets or sets the ID of the Email Address Book that this record is linked to.</summary>
        [SchemaName("EmAddressBookID")]
        public string EmailAddressBookId { get; set; }

        /// <summary>Gets or sets a value indicating whether this Email Campaign been sent to this Email Address Book.</summary>
        [SchemaName("Sent")]
        public bool? Sent { get; set; }

        public override string ToString()
        {
            return this.Id;
        }
    }
}