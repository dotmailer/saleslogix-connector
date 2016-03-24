namespace EmailMarketing.SalesLogix.Entities
{
    using Sage.SData.Client.Extensions;

    /// <summary>
    /// Details of a mapping between a database field in SalesLogix and a custom data field/label in the email service.
    /// </summary>
    [SchemaName(EntityName)]
    public class DataFieldMapping : Entity
    {
        public const string InformationFlowsFromCrmPartialUpper = "INFORMATION FLOWS FROM CRM";
        public const string InformationFlowsFromCrm = "<--- Information flows from CRM";
        public const string InformationFlowsToCrmPartialUpper = "INFORMATION FLOWS TO CRM";

        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMDataMapping";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emaildatamappings";

        /// <summary>Gets or sets the ID of the data label that this mapping applies to.</summary>
        [SchemaName("Emdatalabelid")]
        public string DataLabelId { get; set; }

        /// <summary>Gets or sets the ID of the email account that this mapping belongs to.</summary>
        [SchemaName("EMEmailAccountID")]
        public string EmailAccountId { get; set; }

        /// <summary>Gets or sets the type of entity that this mapping applies to (Contact or Lead).</summary>
        [SchemaName("EntityType")]
        public string EntityType { get; set; }

        /// <summary>Gets or sets a value indicating which direction data flows between SalesLogix and the email service.</summary>
        [SchemaName("MapDirection")]
        public string MapDirection { get; set; }

        /// <summary>Gets or sets the name of the SalesLogix field that the mapping applies to (or the table name of a sub-entity)</summary>
        [SchemaName("FieldName")]
        public string FieldName { get; set; }

        /// <summary>Gets or sets, if applicable, the name of the mapped SalesLogix field within the sub-entity.</summary>
        [SchemaName("LinkedFieldName")]
        public string LinkedFieldName { get; set; }

        /// <summary>Gets or sets a description of the mapping.</summary>
        [SchemaName("Description")]
        public string Description { get; set; }

        /// <summary>Gets or sets an SDataPayload object describing the Datalabel</summary>
        [SchemaName("EMDataLabel")]
        public SDataPayload DataLabel { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", EntityType, FieldName, LinkedFieldName);
        }
    }
}