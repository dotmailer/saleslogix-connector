namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    /// <summary>
    /// An email address that has been globally suppressed from an email account.
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailSuppression : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMEmailSuppression";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailsuppressions";

        /// <summary>Gets or sets the ID of the email account that this suppression belongs to.</summary>
        [SchemaName("Ememailaccountid")]
        public string EmailAccountId { get; set; }

        /// <summary>Gets or sets the email address that was suppressed.</summary>
        [SchemaName("EmailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>Gets or sets the reason for the suppression.</summary>
        [SchemaName("Reason")]
        public string Reason { get; set; }

        /// <summary>Gets or sets the date the suppression happened.</summary>
        [SchemaName("DateRemoved")]
        public DateTime? DateRemoved { get; set; }

        /// <summary>Gets or sets the ID of the contact within the email service.</summary>
        [SchemaName("EmailServiceContactID")]
        public int? EmailServiceContactId { get; set; }

        public override string ToString()
        {
            return EmailAddress;
        }
    }
}