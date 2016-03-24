using System.Reflection;
using EmailMarketing.SalesLogix.Entities;
using EmailMarketing.SalesLogix.Exceptions;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using log4net;

    public class SlxCampaignSynchroniser
    {
        /// <summary>log4net logger object</summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ISlxConnector Slx { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Synchroniser class.
        /// </summary>
        public SlxCampaignSynchroniser(ISlxConnector slx)
        {
            Slx = slx;
        }

        public void SynchroniseAllEmailCampaigns()
        {
            var emailCampaigns = Slx.GetEmailCampaignsLinkedToSlxCampaign();

            if (emailCampaigns != null)
            {
                foreach (EmailCampaign emailCampaign in emailCampaigns)
                {
                    try
                    {
                        SynchroniseSingleEmailCampaign(emailCampaign);
                    }
                    catch (Exception ex)
                    {
                        string emailCampaignName = emailCampaign.Name ?? "";
                        Logger.Error("Exception thrown whilst synchronising campaign (" + emailCampaignName + ")", ex);
                    }
                }
            }
        }

        private void SynchroniseSingleEmailCampaign(EmailCampaign emailCampaign)
        {
            DateTime syncDate = DateTime.UtcNow;
            bool errorOccurred = false;
            string emailCampaignName = emailCampaign.Name ?? "";

            var addrBooks = Slx.GetEmailAddressBooksForEmailCampaign(emailCampaign.Id);
            if (addrBooks != null)
            {
                foreach (var addrBook in addrBooks)
                {
                    try
                    {
                        SynchroniseSingleAddressBookInEmailCampaign(emailCampaign, syncDate, addrBook);
                    }
                    catch (Exception ex)
                    {
                        string addressBookName = addrBook.Name ?? "";
                        Logger.Error("Exception thrown whilst synchronising campaign/address book (" + emailCampaignName + "/" + addressBookName + ")", ex);
                        errorOccurred = true;
                    }
                }
            }

            if (errorOccurred)
                throw new SlxCampaignSynchroniserException("Not all address books for campaign synchronised successfully (" + emailCampaignName + ")");

            emailCampaign.LastSynchronisedToSlx = syncDate;
            Slx.UpdateRecord(emailCampaign);
        }

        private void SynchroniseSingleAddressBookInEmailCampaign(EmailCampaign emailCampaign, DateTime syncDate, EmailAddressBook addrBook)
        {
            ICollection<EmailAddressBookMember> newMembers = Slx.GetEmailAddressBookMembersAddedBetween(addrBook.Id, emailCampaign.LastSynchronisedToSlx, syncDate, new Dictionary<string, List<string>>());

            List<SlxCampaignTarget> targetsToAdd = new List<SlxCampaignTarget>();
            if (newMembers != null)
            {
                foreach (var newMember in newMembers)
                {
                    string targetType;
                    string entityId;

                    if (string.Equals(newMember.SlxMemberType, "contact", StringComparison.OrdinalIgnoreCase))
                    {
                        targetType = "Contact";
                        entityId = newMember.SlxContactId;
                    }
                    else if (string.Equals(newMember.SlxMemberType, "lead", StringComparison.OrdinalIgnoreCase))
                    {
                        targetType = "Lead";
                        entityId = newMember.SlxLeadId;
                    }
                    else
                    {
                        Logger.DebugFormat("Sync to SalesLogix Campaign ignoring address book member of type ({0}) for email campaign ({1})", newMember.SlxMemberType, emailCampaign.Name);
                        continue;
                    }

                    var target = Slx.GetSlxCampaignTarget(emailCampaign.SlxCampaignId, entityId, targetType);
                    if (target == null)
                    {
                        target = new SlxCampaignTarget();
                        target.EntityId = entityId;
                        target.TargetType = targetType;
                        target.Campaign.Key = emailCampaign.SlxCampaignId;
                        target.GroupName = "";
                        target.InitialTarget = false;
                        target.Stage = null;
                        target.Status = "Added";
                        Logger.DebugFormat("Batching SalesLogix Campaign Target to be created for ({0}) in Campaign ({1})", target.EntityId, emailCampaign.SlxCampaignId);
                        //this.Slx.CreateRecord(target);
                        targetsToAdd.Add(target);
                    }
                    else
                        Logger.DebugFormat("SalesLogix Campaign Target already exists for ({0}) in Campaign ({1})", target.EntityId, emailCampaign.SlxCampaignId);
                }
            }

            if (targetsToAdd.Count > 0)
            {
                Logger.DebugFormat("Submitting batch create for ({0}) campaign targets", targetsToAdd.Count);
                Slx.BatchCreateRecords<SlxCampaignTarget>(targetsToAdd);
                Logger.Debug("Batch create complete");
            }
        }
    }
}