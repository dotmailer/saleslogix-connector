namespace EmailMarketing.SalesLogix.Entities
{
    using System;
    using Sage.SData.Client.Extensions;

    [SchemaName(EntityName)]
    public class SlxCampaignTarget : Entity
    {
        public const string EntityName = "CampaignTarget";
        public const string ResourceKind = "campaigntargets";

        public SlxCampaignTarget()
        {
            SDataPayload payload = new SDataPayload();
            payload.Namespace = "http://schemas.sage.com/dynamic/2007";
            payload.ResourceName = "Campaign";

            Campaign = payload;
        }

        [SchemaName("Campaign")]
        public SDataPayload Campaign { get; set; }

        [SchemaName("EntityId")]
        public String EntityId { get; set; }

        [SchemaName("GroupName")]
        public String GroupName { get; set; }

        [SchemaName("InitialTarget")]
        public Boolean? InitialTarget { get; set; }

        [SchemaName("Stage")]
        public String Stage { get; set; }

        [SchemaName("Status")]
        public String Status { get; set; }

        [SchemaName("TargetType")]
        public String TargetType { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}