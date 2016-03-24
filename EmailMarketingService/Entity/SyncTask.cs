namespace EmailMarketing.SalesLogix.Entities
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using Tasks;

    /// <summary>
    /// A task to be processed by the synchronisation service
    /// </summary>
    [SchemaName(EntityName)]
    public class SyncTask : Entity
    {
        /// <summary>The name of the entity within SalesLogix</summary>
        public const string EntityName = "EMSyncTask";

        /// <summary>The name of the SalesLogix sdata feed</summary>
        public const string ResourceKind = "emailsynctasks";

        public const string StatusPending = "Pending";
        public const string StatusInProgress = "InProgress";
        public const string StatusComplete = "Complete";
        public const string StatusFailed = "Failed";

        public const string TaskTypeSendNewMemberCampaign = "SendNewMemberCampaign";
        public const string TaskTypeSendNewMemberCampaignUpper = "SENDNEWMEMBERCAMPAIGN";
        public const string TaskTypeSyncEmailAccount = "SynchroniseEmailAccount";
        public const string TaskTypeSyncEmailAccountUpper = "SYNCHRONISEEMAILACCOUNT";
        public const string TaskTypeSyncAllEmailCampaignHeaders = "SynchroniseAllEmailCampaignHeaders";
        public const string TaskTypeSyncAllEmailCampaignHeadersUpper = "SYNCHRONISEALLEMAILCAMPAIGNHEADERS";

        /// <summary>Gets or sets the type of the task. E.g. SendEmailCampaign</summary>
        [SchemaName("TaskType")]
        public string TaskType { get; set; }

        /// <summary>Gets or sets the Data that specifies the task.</summary>
        [SchemaName("TaskData")]
        public string TaskData { get; set; }

        /// <summary>Gets or sets the date and time that the task should start.</summary>
        [SchemaName("ScheduledStartTime")]
        public DateTime? ScheduledStartTime { get; set; }

        /// <summary>Gets or sets the status of the task.</summary>
        [SchemaName("Status")]
        public string Status { get; set; }

        /// <summary>Gets or sets the date and time that the task actually started.</summary>
        [SchemaName("ActualStartTime")]
        public DateTime? ActualStartTime { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", TaskType, ScheduledStartTime);
        }

        /// <summary>
        /// Serialise the sendData and assign it to the TaskData property
        /// </summary>
        /// <param name="sendData">Campaign send data to put on the task</param>
        public void SetTaskData(CampaignSendData sendData)
        {
            // sendData has been updated, so write it back into the SyncTask
            XmlSerializer xs = new XmlSerializer(sendData.GetType());
            StringWriter sw = new StringWriter();
            xs.Serialize(sw, sendData);
            TaskData = sw.ToString();
        }
    }
}