namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    /// <summary>
    /// Represents a send of an Email Campaign
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailCampaignSend : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMEmailCampaignSend";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailsends";

        public const string StatusComplete = "Complete";

        /// <summary>Gets or sets the ID of the Email Campaign that was sent.</summary>
        [SchemaName("Ememailcampaignid")]
        public string EmailCampaignId { get; set; }

        /// <summary>Gets or sets the date and time the send occurred.</summary>
        [SchemaName("DateSent")]
        public DateTime? DateSent { get; set; }

        /// <summary>Gets or sets a description of the send.</summary>
        [SchemaName("Description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the ID of the email account that this campaign send belongs to.</summary>
        [SchemaName("EMEmailAccountID")]
        public string EmailAccountId { get; set; }

        /// <summary>Gets or sets the date and time this email send should be executed.</summary>
        [SchemaName("ScheduledSendDate")]
        public DateTime? ScheduledSendDate { get; set; }

        /// <summary>Gets or sets tmail Campaign Status.</summary>
        [SchemaName("SendStatus")]
        public string SendStatus { get; set; }

        /// <summary>Gets or sets a value indicating whether this is a split test send. </summary>
        [SchemaName("IsSplitTestSend")]
        public bool? IsSplitTestSend { get; set; }

        /// <summary>Gets or sets the split percentage </summary>
        [SchemaName("SplitTestPercent")]
        public int? SplitTestPercent { get; set; }

        /// <summary>Gets or sets the split test open time </summary>
        [SchemaName("SplitTestOpenTime")]
        public int? SplitTestOpenTime { get; set; }

        /// <summary>Gets or sets the split test metric (Clicks or Opens)</summary>
        [SchemaName("SplitTestMetric")]
        public string SplitTestMetric { get; set; }

        /// <summary>Gets or sets the send type</summary>
        [SchemaName("SendType")]
        public string SendType { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}