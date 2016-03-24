namespace EmailMarketing.SalesLogix.Entities
{
    /// <summary>
    /// Details describing a custom data field within the email service.
    /// </summary>
    [SchemaName(EntityName)]
    public class DataLabel : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMDatalabel";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emaildatalabels";

        /// <summary>Gets or sets the ID of the email account that this label belongs to.</summary>
        [SchemaName("Ememailaccountid")]
        public string EmailAccountId { get; set; }

        /// <summary>Gets or sets the name of the data label.</summary>
        [SchemaName("Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the data type of the label.</summary>
        [SchemaName("DataType")]
        public string DataType { get; set; }

        /// <summary>Gets or sets the default value of the label.</summary>
        [SchemaName("DefaultValue")]
        public string DefaultValue { get; set; }

        /// <summary>Gets or sets a description of the label.</summary>
        [SchemaName("Description")]
        public string Description { get; set; }

        /// <summary>Gets or sets a value indicating whether this label has been synchronised with the email service?</summary>
        [SchemaName("SyncWithEmailService")]
        public bool SyncedWithEmailService { get; set; }

        /// <summary>Gets or sets a value indicating whether this label is private</summary>
        [SchemaName("IsPrivate")]
        public bool IsPrivate { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}