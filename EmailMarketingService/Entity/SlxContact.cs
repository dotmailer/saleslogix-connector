namespace EmailMarketing.SalesLogix.Entities
{
    [SchemaName(EntityName)]
    public class SlxContact : Entity
    {
        public const string EntityName = "Contact";
        public const string ResourceKind = "contacts";

        [SchemaName("DoNotEmail")]
        public bool? DoNotEmail { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}