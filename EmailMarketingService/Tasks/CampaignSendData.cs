namespace EmailMarketing.SalesLogix.Tasks
{
    using System;
    using System.Collections.Generic;

    public class CampaignSendData
    {
        public string CampaignId { get; set; }
        public List<string> AddressBookIds { get; set; }
        public DateTime SendDate { get; set; }
        public int? TestPercentage { get; set; }
        public int? TestPeriodHours { get; set; }
        public string TestMetric { get; set; }
        public string TempAddressBookName { get; set; }
        public string ImportContactResult { get; set; }
        public string CampaignSendResult { get; set; }
        public DateTime? ActualDateSent { get; set; }
        public DateTime? LastCheckForSendComplete { get; set; }
    }
}