namespace EmailMarketing.SalesLogix
{
    using System;
    using dotMailer.Sdk.Campaigns;
    using Entities;

    public class CampaignMapper
    {
        public CampaignMapper()
        {
        }

        public EmailCampaign CreateSlxEmailCampaign(DmCampaign dotMailerCampaign, string slxEmailAccountId)
        {
            if (dotMailerCampaign == null)
            {
                throw new ArgumentNullException("dotMailerCampaign");
            }

            EmailCampaign output = new EmailCampaign();

            output.EmailAccountId = slxEmailAccountId;
            output.Name = dotMailerCampaign.Name;
            output.LastSynchronised = null;
            output.LastSent = null;
            output.DotMailerId = dotMailerCampaign.Id;
            output.CampaignType = EmailCampaign.CampaignTypeStandard;
            output.LastNewMemberSend = null;
            output.Status = EmailCampaign.StatusCreated;
            output.SlxCampaignId = null;
            output.LastSynchronisedToSlx = null;
            output.SyncWithEmailService = false;
            output.AllowRepeatSendsToRecipient = false;
            output.Subject = dotMailerCampaign.Subject;
            output.ReplyEmail = dotMailerCampaign.ReplyToEmailAddress;
            output.ReplyAction = dotMailerCampaign.ReplyAction.ToString();
            output.SendTime = null;
            output.IsSplitTestCampaign = dotMailerCampaign.IsSplitTest;
            output.SplitTestPercent = 0;
            output.SplitTestOpenTime = 0;
            output.FriendlyFromName = dotMailerCampaign.FromName;
            output.LastActivityUpdate = null;

            return output;
        }

        public bool UpdateSlxEmailCampaign(DmCampaign dotMailerCampaign, EmailCampaign slxCampaignToUpdate)
        {
            if (dotMailerCampaign == null)
            {
                throw new ArgumentNullException("dotMailerCampaign");
            }

            bool modified = false;

            if (slxCampaignToUpdate.Name != dotMailerCampaign.Name)
            {
                slxCampaignToUpdate.Name = dotMailerCampaign.Name;
                modified = true;
            }

            if (slxCampaignToUpdate.DotMailerId != dotMailerCampaign.Id)
            {
                slxCampaignToUpdate.DotMailerId = dotMailerCampaign.Id;
                modified = true;
            }

            if (slxCampaignToUpdate.Subject != dotMailerCampaign.Subject)
            {
                slxCampaignToUpdate.Subject = dotMailerCampaign.Subject;
                modified = true;
            }

            if (slxCampaignToUpdate.ReplyEmail != dotMailerCampaign.ReplyToEmailAddress)
            {
                slxCampaignToUpdate.ReplyEmail = dotMailerCampaign.ReplyToEmailAddress;
                modified = true;
            }

            if (slxCampaignToUpdate.ReplyAction != dotMailerCampaign.ReplyAction.ToString())
            {
                slxCampaignToUpdate.ReplyAction = dotMailerCampaign.ReplyAction.ToString();
                modified = true;
            }

            if (slxCampaignToUpdate.IsSplitTestCampaign != dotMailerCampaign.IsSplitTest)
            {
                slxCampaignToUpdate.IsSplitTestCampaign = dotMailerCampaign.IsSplitTest;
                modified = true;
            }

            if (slxCampaignToUpdate.FriendlyFromName != dotMailerCampaign.FromName)
            {
                slxCampaignToUpdate.FriendlyFromName = dotMailerCampaign.FromName;
                modified = true;
            }

            return modified;
        }
    }
}