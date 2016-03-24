using dotMailer.Sdk.Campaigns;
using dotMailer.Sdk.Enums;

namespace EmailMarketing.SalesLogix.Entities
{
    using System;
    using dotMailer.Sdk;

    /// <summary>
    /// Holds detail of an Email Campaign
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailCampaign : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMEmailCampaign";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailcampaigns";

        public const string StatusCreated = "Created";
        public const string StatusSendInProgress = "Send In Progress";
        public const string StatusSendComplete = "Send Complete";
        public const string StatusSendFailed = "Send Failed";

        public const string CampaignTypeStandard = "Standard";
        public const string CampaignTypeNewMember = "New Member";

        /// <summary>Gets or sets the ID of the email account that this email campaign belongs to.</summary>
        [SchemaName("EMEmailAccountID")]
        public string EmailAccountId { get; set; }

        /// <summary>Gets or sets the name of the email cmapaign. </summary>
        [SchemaName("EmailCampaignName")]
        public string Name { get; set; }

        /// <summary>Gets or sets the last synchronised datetime </summary>
        [SchemaName("LastSynchronised")]
        public DateTime? LastSynchronised { get; set; }

        /// <summary>Gets or sets the date/time the email campaign was last sent. </summary>
        [SchemaName("LastSent")]
        public DateTime? LastSent { get; set; }

        /// <summary>Gets or sets the ID of the email campaign within the email service. </summary>
        [SchemaName("EmailServiceCampaignID")]
        public int DotMailerId { get; set; }

        /// <summary>Gets or sets the email campaign type (Standard/New Member). </summary>
        [SchemaName("CampaignType")]
        public string CampaignType { get; set; }

        /// <summary>Gets or sets the date/time of the last New Member send. </summary>
        [SchemaName("LastNewMemberSend")]
        public DateTime? LastNewMemberSend { get; set; }

        /// <summary>Gets or sets the status of the email campaign. </summary>
        [SchemaName("Status")]
        public string Status { get; set; }

        /// <summary>Gets or sets the ID of an SLX Campaign that this email campaign is linked to. </summary>
        [SchemaName("SLXCampaignID")]
        public string SlxCampaignId { get; set; }

        /// <summary>Gets or sets the date/time this email campaign was last synchrnoised with its linked SLX Campaign. </summary>
        [SchemaName("LastSynchronisedToSLX")]
        public DateTime? LastSynchronisedToSlx { get; set; }

        /// <summary>Gets or sets a value indicating whether this email campaign should be synchronised with the email service. </summary>
        [SchemaName("SyncWithEmailService")]
        public bool SyncWithEmailService { get; set; }

        /// <summary>Gets or sets a value indicating whether this Email Campaign may be sent to the same email address more than once. </summary>
        [SchemaName("AllowRepeatSendsToRecipient")]
        public bool AllowRepeatSendsToRecipient { get; set; }

        /// <summary>Gets or sets the subject of the email sent by this email campaign. </summary>
        [SchemaName("Subject")]
        public string Subject { get; set; }

        /// <summary>Gets or sets the reply to email address </summary>
        [SchemaName("ReplyEmail")]
        public string ReplyEmail { get; set; }

        /// <summary>Gets or sets the Campaign Reply Action e.g. WebMailForward, Webmail or Delete. </summary>
        [SchemaName("ReplyAction")]
        public string ReplyAction { get; set; }

        /// <summary>Gets or sets the send time of the email campaign. </summary>
        [SchemaName("SendTime")]
        public DateTime? SendTime { get; set; }

        /// <summary>Gets or sets a value indicating whether this is a split test campaign. </summary>
        [SchemaName("IsSplitTestCampaign")]
        public bool IsSplitTestCampaign { get; set; }

        /// <summary>Gets or sets the split percentage </summary>
        [SchemaName("SplitTestPercent")]
        public int? SplitTestPercent { get; set; }

        /// <summary>Gets or sets the split test open time </summary>
        [SchemaName("SplitTestOpenTime")]
        public int? SplitTestOpenTime { get; set; }

        /// <summary>Gets or sets the split test metric (Clicks or Opens)</summary>
        [SchemaName("SplitTestMetric")]
        public string SplitTestMetric { get; set; }

        /// <summary>Gets or sets the Friendly From Name for emails from this email campaign. </summary>
        [SchemaName("FriendlyFromName")]
        public string FriendlyFromName { get; set; }

        /// <summary>Gets or sets the last date and time that Email Campaign activity data was updated. Activity data include all interactions with recipients, clicks, etc. </summary>
        [SchemaName("LastActivityUpdate")]
        public DateTime? LastActivityUpdate { get; set; }

        /// <summary>Gets or sets the Number of Unique Opens (html) </summary>
        [SchemaName("NumUniqueOpens")]
        public int? NumUniqueOpens { get; set; }

        /// <summary>Gets or sets the Number of Unique Opens (Text) </summary>
        [SchemaName("NumUniqueTextOpens")]
        public int? NumUniqueTextOpens { get; set; }

        /// <summary>Gets or sets the Total Number of Unique Opens </summary>
        [SchemaName("NumTotalUniqueOpens")]
        public int? NumTotalUniqueOpens { get; set; }

        /// <summary>Gets or sets the Number of Opens (html) </summary>
        [SchemaName("NumOpens")]
        public int? NumOpens { get; set; }

        /// <summary>Gets or sets the Number of Opens (Text) </summary>
        [SchemaName("NumTextOpens")]
        public int? NumTextOpens { get; set; }

        /// <summary>Gets or sets the Total Number of Opens </summary>
        [SchemaName("NumTotalOpens")]
        public int? NumTotalOpens { get; set; }

        /// <summary>Gets or sets the Number of Clicks (html) </summary>
        [SchemaName("NumClicks")]
        public int? NumClicks { get; set; }

        /// <summary>Gets or sets the Number of Clicks (Text) </summary>
        [SchemaName("NumTextClicks")]
        public int? NumTextClicks { get; set; }

        /// <summary>Gets or sets the Total Number of Clicks </summary>
        [SchemaName("NumTotalClicks")]
        public int? NumTotalClicks { get; set; }

        /// <summary>Gets or sets the Number of Page Views (html) </summary>
        [SchemaName("NumPageViews")]
        public int? NumPageViews { get; set; }

        /// <summary>Gets or sets the Total Number of Page Views </summary>
        [SchemaName("NumTotalPageViews")]
        public int? NumTotalPageViews { get; set; }

        /// <summary>Gets or sets the Number of Page Views (Text) </summary>
        [SchemaName("NumTextPageViews")]
        public int? NumTextPageViews { get; set; }

        /// <summary>Gets or sets the Number of Forwards (html) </summary>
        [SchemaName("NumForwards")]
        public int? NumForwards { get; set; }

        /// <summary>Gets or sets the Number of Forwards (Text) </summary>
        [SchemaName("NumTextForwards")]
        public int? NumTextForwards { get; set; }

        /// <summary>Gets or sets the Estimated Number of Forwards (html) </summary>
        [SchemaName("NumEstimatedForwards")]
        public int? NumEstimatedForwards { get; set; }

        /// <summary>Gets or sets the Estimated Number of Forwards (Text) </summary>
        [SchemaName("NumTextEstimatedForwards")]
        public int? NumTextEstimatedForwards { get; set; }

        /// <summary>Gets or sets the Total Estimated Number of Forwards </summary>
        [SchemaName("NumTotalEstimatedForwards")]
        public int? NumTotalEstimatedForwards { get; set; }

        /// <summary>Gets or sets the Number of Replies (html) </summary>
        [SchemaName("NumReplies")]
        public int? NumReplies { get; set; }

        /// <summary>Gets or sets the Number of Replies (Text) </summary>
        [SchemaName("NumTextReplies")]
        public int? NumTextReplies { get; set; }

        /// <summary>Gets or sets the Total Number of Replies </summary>
        [SchemaName("NumTotalReplies")]
        public int? NumTotalReplies { get; set; }

        /// <summary>Gets or sets the Number of Hard Bounces (html) </summary>
        [SchemaName("NumHardBounces")]
        public int? NumHardBounces { get; set; }

        /// <summary>Gets or sets the Number of Hard Bounces (Text) </summary>
        [SchemaName("NumTextHardBounces")]
        public int? NumTextHardBounces { get; set; }

        /// <summary>Gets or sets the Total Number of Hard Bounces </summary>
        [SchemaName("NumTotalHardBounces")]
        public int? NumTotalHardBounces { get; set; }

        /// <summary>Gets or sets the Number of Soft Bounces (html) </summary>
        [SchemaName("NumSoftBounces")]
        public int? NumSoftBounces { get; set; }

        /// <summary>Gets or sets the Number of Soft Bounces (Text) </summary>
        [SchemaName("NumTextSoftBounces")]
        public int? NumTextSoftBounces { get; set; }

        /// <summary>Gets or sets the Total Number of Soft Bounces </summary>
        [SchemaName("NumTotalSoftBounces")]
        public int? NumTotalSoftBounces { get; set; }

        /// <summary>Gets or sets the Number of Unsubscribes (html) </summary>
        [SchemaName("NumUnsubscribes")]
        public int? NumUnsubscribes { get; set; }

        /// <summary>Gets or sets the Number of Unsubscribes (Text) </summary>
        [SchemaName("NumTextUnsubscribes")]
        public int? NumTextUnsubscribes { get; set; }

        /// <summary>Gets or sets the Total Number of Unsubscribes </summary>
        [SchemaName("NumTotalUnsubscribes")]
        public int? NumTotalUnsubscribes { get; set; }

        /// <summary>Gets or sets tNumber of ISP Complaints (html) </summary>
        [SchemaName("NumISPComplaints")]
        public int? NumISPComplaints { get; set; }

        /// <summary>Gets or sets tNumber of ISP Complaints (Text) </summary>
        [SchemaName("NumTextISPComplaints")]
        public int? NumTextISPComplaints { get; set; }

        /// <summary>Gets or sets the Total Number of ISP Complaints </summary>
        [SchemaName("NumTotalISPComplaints")]
        public int? NumTotalISPComplaints { get; set; }

        /// <summary>Gets or sets the Number of Mail Blocks (html) </summary>
        [SchemaName("NumMailBlocks")]
        public int? NumMailBlocks { get; set; }

        /// <summary>Gets or sets the Number of Mail Blocks (Text) </summary>
        [SchemaName("NumTextMailBlocks")]
        public int? NumTextMailBlocks { get; set; }

        /// <summary>Gets or sets the Total Number of Mail Blocks </summary>
        [SchemaName("NumTotalMailBlocks")]
        public int? NumTotalMailBlocks { get; set; }

        /// <summary>Gets or sets the Number Sent (html) </summary>
        [SchemaName("NumSent")]
        public int? NumSent { get; set; }

        /// <summary>Gets or sets the Number Sent (Text) </summary>
        [SchemaName("NumTextSent")]
        public int? NumTextSent { get; set; }

        /// <summary>Gets or sets the Total Number Sent </summary>
        [SchemaName("NumTotalSent")]
        public int? NumTotalSent { get; set; }

        /// <summary>Gets or sets the Number of Recipients Clicked (html) </summary>
        [SchemaName("NumRecipientsClicked")]
        public int? NumRecipientsClicked { get; set; }

        /// <summary>Gets or sets the Number Delivered (html) </summary>
        [SchemaName("NumDelivered")]
        public int? NumDelivered { get; set; }

        /// <summary>Gets or sets the Number Delivered (Text) </summary>
        [SchemaName("NumTextDelivered")]
        public int? NumTextDelivered { get; set; }

        /// <summary>Gets or sets the Total Number Delivered </summary>
        [SchemaName("NumTotalDelivered")]
        public int? NumTotalDelivered { get; set; }

        /// <summary>Gets or sets the Delivered Rate (%) </summary>
        [SchemaName("PercentageDelivered")]
        public double? PercentageDelivered { get; set; }

        /// <summary>Gets or sets the Unique Open Rate (%) </summary>
        [SchemaName("PercentageUniqueOpens")]
        public double? PercentageUniqueOpens { get; set; }

        /// <summary>Gets or sets the Open Rate (%) </summary>
        [SchemaName("PercentageOpens")]
        public double? PercentageOpens { get; set; }

        /// <summary>Gets or sets the Unsubscribe Rate (%) </summary>
        [SchemaName("PercentageUnsubscribes")]
        public double? PercentageUnsubscribes { get; set; }

        /// <summary>Gets or sets the Reply Rate (%) </summary>
        [SchemaName("PercentageReplies")]
        public double? PercentageReplies { get; set; }

        /// <summary>Gets or sets the Hard Bounce Rate (%) </summary>
        [SchemaName("PercentageHardBounces")]
        public double? PercentageHardBounces { get; set; }

        /// <summary>Gets or sets the Soft Bounce Rate (%) </summary>
        [SchemaName("PercentageSoftBounces")]
        public double? PercentageSoftBounces { get; set; }

        /// <summary>Gets or sets the User Click Rate (%) </summary>
        [SchemaName("PercentageUsersClicked")]
        public double? PercentageUsersClicked { get; set; }

        /// <summary>Gets or sets the Click rate (%) </summary>
        [SchemaName("PercentageClicksToOpens")]
        public double? PercentageClicksToOpens { get; set; }

        /// <summary>
        /// Copy the statistics from a dotMailer campaign object into this EmailCampaign object
        /// </summary>
        /// <param name="dmCamp">The dotMailer campaign to copy from.  dmCampaign.Summary will be used.</param>
        public void CopyStatisticsFrom(DmCampaign dmCamp)
        {
            try
            {
                NumClicks = dmCamp.Summary.NumClicks;
                NumDelivered = dmCamp.Summary.NumDelivered;
                NumEstimatedForwards = dmCamp.Summary.NumEstimatedForwards;
                NumForwards = dmCamp.Summary.NumForwards;
                NumHardBounces = dmCamp.Summary.NumHardBounces;
                NumISPComplaints = dmCamp.Summary.NumIspComplaints;
                NumMailBlocks = dmCamp.Summary.NumMailBlocks;
                NumOpens = dmCamp.Summary.NumOpens;
                NumPageViews = dmCamp.Summary.NumPageViews;
                NumRecipientsClicked = dmCamp.Summary.NumRecipientsClicked;
                NumReplies = dmCamp.Summary.NumReplies;
                NumSent = dmCamp.Summary.NumSent;
                NumSoftBounces = dmCamp.Summary.NumSoftBounces;
                NumTextClicks = dmCamp.Summary.NumTextClicks;
                NumTextDelivered = dmCamp.Summary.NumTextDelivered;
                NumTextEstimatedForwards = dmCamp.Summary.NumTextEstimatedForwards;
                NumTextForwards = dmCamp.Summary.NumTextForwards;
                NumTextHardBounces = dmCamp.Summary.NumTextHardBounces;
                NumTextISPComplaints = dmCamp.Summary.NumTextIspComplaints;
                NumTextMailBlocks = dmCamp.Summary.NumTextMailBlocks;
                NumTextOpens = dmCamp.Summary.NumTextOpens;
                NumTextPageViews = dmCamp.Summary.NumTextPageViews;
                NumTextReplies = dmCamp.Summary.NumTextReplies;
                NumTextSent = dmCamp.Summary.NumTextSent;
                NumTextSoftBounces = dmCamp.Summary.NumTextSoftBounces;
                NumTextUnsubscribes = dmCamp.Summary.NumTextUnsubscribes;
                NumTotalClicks = dmCamp.Summary.NumTotalClicks;
                NumTotalDelivered = dmCamp.Summary.NumTotalDelivered;
                NumTotalEstimatedForwards = dmCamp.Summary.NumTotalEstimatedForwards;
                NumTotalHardBounces = dmCamp.Summary.NumTotalHardBounces;
                NumTotalISPComplaints = dmCamp.Summary.NumTotalIspComplaints;
                NumTotalMailBlocks = dmCamp.Summary.NumTotalMailBlocks;
                NumTotalOpens = dmCamp.Summary.NumTotalOpens;
                NumTotalPageViews = dmCamp.Summary.NumTotalPageViews;
                NumTotalReplies = dmCamp.Summary.NumTotalReplies;
                NumTotalSent = dmCamp.Summary.NumTotalSent;
                NumTotalSoftBounces = dmCamp.Summary.NumTotalSoftBounces;
                NumTotalUniqueOpens = dmCamp.Summary.NumTotalUniqueOpens;
                NumTotalUnsubscribes = dmCamp.Summary.NumTotalUnsubscribes;
                NumUniqueOpens = dmCamp.Summary.NumUniqueOpens;
                NumUniqueTextOpens = dmCamp.Summary.NumUniqueTextOpens;
                NumUnsubscribes = dmCamp.Summary.NumUnsubscribes;
                PercentageClicksToOpens = dmCamp.Summary.PercentageClicksToOpens * 100;
                PercentageDelivered = dmCamp.Summary.PercentageDelivered * 100;
                PercentageHardBounces = dmCamp.Summary.PercentageHardBounces * 100;
                PercentageOpens = dmCamp.Summary.PercentageOpens * 100;
                PercentageReplies = dmCamp.Summary.PercentageReplies * 100;
                PercentageSoftBounces = dmCamp.Summary.PercentageSoftBounces * 100;
                PercentageUniqueOpens = dmCamp.Summary.PercentageUniqueOpens * 100;
                PercentageUnsubscribes = dmCamp.Summary.PercentageUnsubscribes * 100;
                PercentageUsersClicked = dmCamp.Summary.PercentageUsersClicked * 100;
            }
            catch (DmException ex)
            {
                if (ex.Code == DMErrorCodes.ERROR_CAMPAIGN_INVALID)
                {
                    // The campaign has never been sent, so no statistics are available.
                    // Ignore the exception
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}