namespace EmailMarketing.SalesLogix.Tasks
{
    public class DeletedLabelDetails
    {
        public string EmailAccountId { get; set; }
        public string DataLabelId { get; set; }
        public string Name { get; set; }
        public bool? SyncWithEmailService { get; set; }
    }
}