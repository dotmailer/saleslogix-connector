namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    /// <summary>
    /// Represents a sent email and a summary of its subsequent activity.
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailCampaignSendSummary : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMCampaignSendSummary";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailcampaignsendsummaries";

        /// <summary>Gets or sets the ID of the email send that this send summary relates to.</summary>
        [SchemaName("Ememailcampaignsendid")]
        public string EmailCampaignSendId { get; set; }

        /// <summary>Gets or sets the ID of the email campaign that this send summary relates to.</summary>
        [SchemaName("EMEmailCampaignID")]
        public string EmailCampaignId { get; set; }

        /// <summary>Gets or sets the ID of the email account that this send summary belongs to.</summary>
        [SchemaName("EMEmailAccountID")]
        public string EmailAccountId { get; set; }

        /// <summary>Gets or sets the number of times the user has clicked on links in the email.</summary>
        [SchemaName("NumClicks")]
        public int? NumberOfClicks { get; set; }

        /// <summary>Gets or sets the Date and time the email was first opened.</summary>
        [SchemaName("DateFirstOpened")]
        public DateTime? DateFirstOpened { get; set; }

        /// <summary>Gets or sets the Date and time the email was last opened.</summary>
        [SchemaName("DateLastOpened")]
        public DateTime? DateLastOpened { get; set; }

        /// <summary>Gets or sets the Date and time the email was sent.</summary>
        [SchemaName("DateSent")]
        public DateTime? DateSent { get; set; }

        /// <summary>Gets or sets the email address that was sent to.</summary>
        [SchemaName("EmailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>Gets or sets the estimmated number of forwards.</summary>
        [SchemaName("NumEstimatedForwards")]
        public int? NumberOfEstimatedForwards { get; set; }

        /// <summary>Gets or sets the IP address that first opened the email.</summary>
        [SchemaName("FirstOpenIp")]
        public string FirstOpenIPAddress { get; set; }

        /// <summary>Gets or sets the number of forwards to friends.</summary>
        [SchemaName("NumForwardToFriend")]
        public int? NumberOfForwardsToFriends { get; set; }

        /// <summary>Gets or sets a value indicating whether the email hard bounced from this email address</summary>
        [SchemaName("HardBounced")]
        public bool? HardBounced { get; set; }

        /// <summary>Gets or sets the number of opens.</summary>
        [SchemaName("NumOpens")]
        public int? NumberOfOpens { get; set; }

        /// <summary>Gets or sets the number of replies.</summary>
        [SchemaName("NumReplies")]
        public int? NumberOfReplies { get; set; }

        /// <summary>Gets or sets the ID of the contact within the email service.</summary>
        [SchemaName("EmailServiceContactID")]
        public int? EmailServiceContactId { get; set; }

        /// <summary>Gets or sets a value indicating whether the email soft bounced from this email address</summary>
        [SchemaName("SoftBounced")]
        public bool? SoftBounced { get; set; }

        /// <summary>Gets or sets a value indicating whether the address book member has unsubscribed from the address book?</summary>
        [SchemaName("Unsubscribed")]
        public bool? Unsubscribed { get; set; }

        /// <summary>Gets or sets the number of views.</summary>
        [SchemaName("NumViews")]
        public int? NumberOfViews { get; set; }

        /// <summary>Gets or sets the type of entity that the address book member is linked to.</summary>
        [SchemaName("SlxMemberType")]
        public string SlxMemberType { get; set; }

        /// <summary>Gets or sets the ID of the Contact linked to the address book member (If it is linked to a Contact).</summary>
        [SchemaName("SlxContactID")]
        public string SlxContactId { get; set; }

        /// <summary>Gets or sets the ID of the Lead linked to the address book member (If it is linked to a Lead).</summary>
        [SchemaName("SlxLeadID")]
        public string SlxLeadId { get; set; }

        /// <summary>Gets or sets the ID of the email address book member that the email was sent to.</summary>
        [SchemaName("EMAddressBookMemberID")]
        public string AddressBookMemberId { get; set; }

        /// <summary>Gets or sets the ID of the email address book that contains the member that the email was sent to.</summary>
        [SchemaName("EMAddressBookID")]
        public string AddressBookId { get; set; }

        public override string ToString()
        {
            return EmailAddress;
        }
    }
}