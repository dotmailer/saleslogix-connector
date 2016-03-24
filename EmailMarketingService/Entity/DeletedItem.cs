namespace EmailMarketing.SalesLogix.Entities
{
    /// <summary>
    /// Data related to an entity that has been deleted out of SaleLogix.
    /// </summary>
    [SchemaName(EntityName)]
    public class DeletedItem : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMDeletedItem";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "deleteditems";

        /// <summary>Gets or sets the type of entity that was deleted.</summary>
        [SchemaName("EntityType")]
        public string EntityType { get; set; }

        /// <summary>Gets or sets the ID of the entity that was deleted.</summary>
        [SchemaName("EntityID")]
        public string EntityId { get; set; }

        /// <summary>Gets or sets a value indicating whether the deletion has been processed by the sync with email service</summary>
        [SchemaName("ProcessedByEMSync")]
        public bool ProcessedByEMSync { get; set; }

        /// <summary>Gets or sets a value indicating whether the deletion has been processed by the sync with SalesLogix Campaigns</summary>
        [SchemaName("ProcessedByCampaignSync")]
        public bool ProcessedByCampaignSync { get; set; }

        /// <summary>Gets or sets Data related to the item that was deleted.</summary>
        [SchemaName("Data")]
        public string Data { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", EntityType, EntityId);
        }
    }
}