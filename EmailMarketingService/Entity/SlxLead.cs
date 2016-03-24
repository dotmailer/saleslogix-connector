namespace EmailMarketing.SalesLogix.Entities
{
    [SchemaName(EntityName)]
    public class SlxLead : Entity
    {
        public const string EntityName = "Lead";
        public const string ResourceKind = "leads";

        [SchemaName("DoNotEmail")]
        public bool? DoNotEmail { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}