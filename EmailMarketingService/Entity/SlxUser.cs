namespace EmailMarketing.SalesLogix.Entities
{
    [SchemaName(EntityName)]
    public class SlxUser : Entity
    {
        public const string EntityName = "User";
        public const string ResourceKind = "Users";

        [SchemaName("UserName")]
        public string UserName { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}