namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    [SchemaName(EntityName)]
    public class History : Entity
    {
        public const string EntityName = "History";
        public const string ResourceKind = "history";

        [SchemaName("AccountId")]
        public String AccountId { get; set; }

        [SchemaName("AccountName")]
        public String AccountName { get; set; }

        [SchemaName("ActivityId")]
        public String ActivityId { get; set; }

        [SchemaName("Attachment")]
        public Boolean Attachment { get; set; }

        [SchemaName("Category")]
        public String Category { get; set; }

        [SchemaName("CompletedDate")]
        public DateTime CompletedDate { get; set; }

        [SchemaName("CompletedUser")]
        public String CompletedUser { get; set; }

        [SchemaName("ContactId")]
        public String ContactId { get; set; }

        [SchemaName("ContactName")]
        public String ContactName { get; set; }

        [SchemaName("Description")]
        public String Description { get; set; }

        [SchemaName("Duration")]
        public Int32 Duration { get; set; }

        [SchemaName("LeadId")]
        public String LeadId { get; set; }

        [SchemaName("LeadName")]
        public String LeadName { get; set; }

        [SchemaName("LongNotes")]
        public String LongNotes { get; set; }

        [SchemaName("Notes")]
        public String Notes { get; set; }

        [SchemaName("OpportunityId")]
        public String OpportunityId { get; set; }

        [SchemaName("OpportunityName")]
        public String OpportunityName { get; set; }

        [SchemaName("OriginalDate")]
        public DateTime OriginalDate { get; set; }

        [SchemaName("Priority")]
        public String Priority { get; set; }

        [SchemaName("ProcessId")]
        public String ProcessId { get; set; }

        [SchemaName("ProcessNode")]
        public String ProcessNode { get; set; }

        [SchemaName("Result")]
        public String Result { get; set; }

        [SchemaName("ResultCode")]
        public String ResultCode { get; set; }

        [SchemaName("StartDate")]
        public DateTime StartDate { get; set; }

        [SchemaName("TicketId")]
        public String TicketId { get; set; }

        [SchemaName("TicketNumber")]
        public String TicketNumber { get; set; }

        [SchemaName("Timeless")]
        public Boolean Timeless { get; set; }

        [SchemaName("Type")]
        public HistoryType Type { get; set; }

        [SchemaName("UserDef1")]
        public String UserDef1 { get; set; }

        [SchemaName("UserDef2")]
        public String UserDef2 { get; set; }

        [SchemaName("UserDef3")]
        public String UserDef3 { get; set; }

        [SchemaName("UserId")]
        public String UserId { get; set; }

        [SchemaName("UserName")]
        public String UserName { get; set; }

        [SchemaName("AttachmentCount")]
        public Int32? AttachmentCount { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}