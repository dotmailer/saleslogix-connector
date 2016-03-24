namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    /// <summary>
    /// Represents a click by a recipient of an email campaign
    /// </summary>
    [SchemaName(EntityName)]
    public class EmailCampaignClick : Entity
    {
        private string _URL;
        private string _UserAgent;

        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMCampaignConClick";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailcampaigncontactclicks";

        /// <summary>Gets or sets the ID of the Email Campaign that this click belongs to.</summary>
        [SchemaName("Ememailcampaignid")]
        public string Ememailcampaignid { get; set; }

        /// <summary>Gets or sets the date and time of the click.</summary>
        [SchemaName("DateClicked")]
        public DateTime? DateClicked { get; set; }

        /// <summary>Gets or sets the IP Address of the clicker.</summary>
        [SchemaName("IPAddress")]
        public string IPAddress { get; set; }

        /// <summary>Gets or sets the keyword of the click.</summary>
        [SchemaName("Keyword")]
        public string Keyword { get; set; }

        /// <summary>Gets or sets the ID of the clicker within the email service.</summary>
        [SchemaName("EmailServiceContactID")]
        public int? EmailServiceContactID { get; set; }

        /// <summary>Gets or sets the URL clicked on.</summary>
        [SchemaName("URL")]
        public string URL
        {
            get
            {
                return _URL;
            }
            set
            {
                if (value != null && value.Length > 300)
                {
                    _URL = value.Substring(0, 300);
                }
                else
                {
                    _URL = value;
                }
            }
        }

        /// <summary>Gets or sets the User Agent of the clicker.</summary>
        [SchemaName("UserAgent")]
        public string UserAgent
        {
            get
            {
                return _UserAgent;
            }
            set
            {
                if (value != null && value.Length > 300)
                {
                    _UserAgent = value.Substring(0, 300);
                }
                else
                {
                    _UserAgent = value;
                }
            }
        }

        /// <summary>Gets or sets the ID of the Address Book Member linked to this click.</summary>
        [SchemaName("EMAddressBookMemberID")]
        public string AddressBookMemberId { get; set; }

        /// <summary>Gets or sets the ID of the Email Account that the click belongs to.</summary>
        [SchemaName("EMEmailAccountID")]
        public string EmailAccountID { get; set; }

        /// <summary>Gets or sets the type of entity that this member represents (Lead or Contact).</summary>
        [SchemaName("SlxMemberType")]
        public string SlxMemberType { get; set; }

        /// <summary>Gets or sets the ID of the Contact that this member represents (If it represents a Contact).</summary>
        [SchemaName("SlxContactID")]
        public string SlxContactId { get; set; }

        /// <summary>Gets or sets the ID of the Lead that this member represents (If it represents a Lead).</summary>
        [SchemaName("SlxLeadID")]
        public string SlxLeadId { get; set; }

        /// <summary>Gets or sets the ID of the Address Book Member linked to this click.</summary>
        [SchemaName("EMAddressBookID")]
        public string AddressBookId { get; set; }

        /// <summary>Gets or sets the email address of the clicker.</summary>
        [SchemaName("EmailAddress")]
        public string EmailAddress { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}