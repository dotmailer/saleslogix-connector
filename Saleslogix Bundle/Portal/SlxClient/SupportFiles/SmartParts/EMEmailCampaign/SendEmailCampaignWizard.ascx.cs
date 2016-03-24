using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sage.Platform.WebPortal.SmartParts;
using System.Web.UI;
using Sage.Entity.Interfaces;
using System.Text;
using System.Web.UI.WebControls;
using Sage.Platform;
using Sage.SalesLogix.Entities;
using System.Xml.Serialization;
using System.IO;
using Sage.Platform.Repository;

/// <summary>
/// Summary description for SendEmailCampaignWizard
/// </summary>
public partial class SendEmailCampaignWizard : EntityBoundSmartPartInfoProvider
{
    private const string SessionKeyTotalUniqueTargets = "TotalUniqueTargets";
    private const string SessionKeyEmailAddressBookIds = "EmailAddressBookIds";
    private const string SessionKeySendTime = "SendTime";
    private const string SessionKeySendYear = "SendYear";
    private const string SessionKeySendMonth = "SendMonth";
    private const string SessionKeySendDay = "SendDay";
    private const string SessionKeySendHour = "SendHour";
    private const string SessionKeySendMinute = "SendMinute";
    private const string SessionKeySplitPercentage = "SplitPercentage";
    private const string SessionKeySplitPeriodHours = "SplitPeriodHours";
    private const string SessionKeySplitMetric = "SplitMetric";
    
    private int _Timeout;

    /// <summary>
    /// Gets or sets the role security service.
    /// </summary>
    /// <value>The role security service.</value>
    [Sage.Platform.Application.ServiceDependency]
    public Sage.Platform.Security.IRoleSecurityService RoleSecurityService { get; set; }

    public override Type EntityType
    {
        get { return typeof(Sage.Entity.Interfaces.IEMSyncTask); }
    }

    protected override void OnAddEntityBindings()
    {
		// Set the default time to UtcNow otherwise the 'default' when
		// you open up the picker is not shown in local time.
        if (dntSendTime.DateTimeValue == null)
        {
            dntSendTime.DateTimeValue = DateTime.UtcNow;
        }
    }

    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
        btnNext.Click += new EventHandler(btnNext_Click);
        btnBack.Click += new EventHandler(btnBack_Click);
        btnSendCampaign.Click += new EventHandler(btnSendCampaign_Click);
        btnCancel.Click += btnCancel_Click;
    }

    void btnCancel_Click(object sender, EventArgs e)
    {
		// We cannot simply close the dialog because this does not work properly after you
		// have gone "Back" to the first page, then try to cancel.
        DialogService.CloseEventHappened(sender, e);
        IEMEmailCampaign EmailCampaign = (IEMEmailCampaign)GetParentEntity();
        Response.Redirect("EMEmailCampaign.aspx?entityid=" + EmailCampaign.Id.ToString());
    }

    void btnSendCampaign_Click(object sender, EventArgs e)
    {
        IEMEmailCampaign EmailCampaign = (IEMEmailCampaign)GetParentEntity();
        IEMSyncTask thisSyncTask = (IEMSyncTask)this.BindingSource.Current;

        CampaignSendData taskData = new CampaignSendData();
        taskData.CampaignId = EmailCampaign.Id.ToString();
        taskData.AddressBookIds = (List<string>)Session[SessionKeyEmailAddressBookIds];
        if (Session[SessionKeySendTime].ToString() == "Immediately")
        {
            DateTime currentTimeUtc = DateTime.UtcNow;
            taskData.SendDate = currentTimeUtc;
            thisSyncTask.ScheduledStartTime = currentTimeUtc;
        }
        else
        {
            taskData.SendDate = (DateTime)Session[SessionKeySendTime];
            thisSyncTask.ScheduledStartTime = (DateTime)Session[SessionKeySendTime];
        }

        if (Session[SessionKeySplitPercentage] != null && Session[SessionKeySplitPeriodHours] != null && Session[SessionKeySplitMetric] != null)
        {
            taskData.TestPercentage = int.Parse(Session[SessionKeySplitPercentage].ToString());
            taskData.TestPeriodHours = int.Parse(Session[SessionKeySplitPeriodHours].ToString());
            taskData.TestMetric = Session[SessionKeySplitMetric].ToString();
            thisSyncTask.TaskType = "SendSplitEmailCampaign";
        }
        else
        {
            thisSyncTask.TaskType = "SendEmailCampaign";
        }

        XmlSerializer xs = new XmlSerializer(typeof(CampaignSendData));
        StringWriter sw = new StringWriter();
        xs.Serialize(sw, taskData);

        thisSyncTask.TaskData = sw.ToString();
        thisSyncTask.Status = "Pending";
        thisSyncTask.Save();

        page4.Visible = false;
        page5.Visible = true;
        btnCancel.Text = "Close";
    }

    void btnBack_Click(object sender, EventArgs e)
    {
        if (page4.Visible)
        {
            page4.Visible = false;
            page3.Visible = true;
            if (divSplitTest.Visible)
            {
                if (!string.IsNullOrEmpty(Session[SessionKeySplitPercentage].ToString()))
                {
                    txtTargetPercentage.Text = Session[SessionKeySplitPercentage].ToString();
                }
            }
        }
        else if (page3.Visible)
        {
            page3.Visible = false;
            page2.Visible = true;
        }
        else if (page2.Visible)
        {
            page2.Visible = false;
            page1.Visible = true;
            IEMEmailCampaign EmailCampaign = (IEMEmailCampaign)GetParentEntity();
            Response.Redirect("EMEmailCampaign.aspx?entityid=" + EmailCampaign.Id.ToString() + "&activedialog=SendEmailCampaignWizard");
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        if (page1.Visible)
        {
            // Clear out any values from previous runs
            this.ClearSession();

            List<IEMCampaignAddressBook> selectedCampAddrBooks = new List<IEMCampaignAddressBook>();
            List<string> selectedCampAddrBookIds = new List<string>();

            foreach (GridViewRow row in grdTargetAddressBooks.Rows)
            {
                CheckBox rowCheck = (CheckBox)row.FindControl("chkSelect");
                if (rowCheck.Checked == true)
                {
                    string campAddrBookId = grdTargetAddressBooks.DataKeys[row.RowIndex].Value.ToString();
                    IEMCampaignAddressBook selectedCampAddrBook = EntityFactory.GetById<IEMCampaignAddressBook>(campAddrBookId);
                    selectedCampAddrBooks.Add(selectedCampAddrBook);
                    selectedCampAddrBookIds.Add(selectedCampAddrBook.EMAddressBook.Id.ToString());
                } 
            }

            if (selectedCampAddrBooks.Count != 0)
            {
                page1.Visible = false;
                page2.Visible = true;

                int totPotentialTargets = 0;
                int duplicateTargets = 0;
                int doNotSolicitTargets = 0;
                int noEmailTargets = 0;
                int numTargetsAlreadySentTo = 0;

                IEMEmailCampaign emailCampaign = (IEMEmailCampaign)GetParentEntity();
                HashSet<string> memberEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                
                HashSet<string> sentContacts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                HashSet<string> sentLeads = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (emailCampaign.AllowRepeatSendsToRecipient != true)
                {
                    foreach (IEMCampaignSendSummary sendSummary in emailCampaign.EMCampaignSendSummaries)
                    {
                        if (string.Equals(sendSummary.SlxMemberType, "CONTACT", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!sentContacts.Contains(sendSummary.SlxContactID))
                            {
                                sentContacts.Add(sendSummary.SlxContactID);
                            }
                        }
                        else if (string.Equals(sendSummary.SlxMemberType, "LEAD", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!sentLeads.Contains(sendSummary.SlxLeadID))
                            {
                                sentLeads.Add(sendSummary.SlxLeadID);
                            }
                        }
                    }
                }

                foreach (IEMCampaignAddressBook campAddrBook in selectedCampAddrBooks)
                {
                    NHibernate.ISession session = Sage.Platform.Framework.SessionFactoryHolder.HolderInstance.CreateSession();
                    int pageSize = 500;
                    int pageNumber = 0;
                    string addrBookId = campAddrBook.EMAddressBook.Id.ToString();

                    IList<EMAddressBookMember> memberPage = null;
                    while (memberPage == null || memberPage.Count > 0)
                    {
                        // Clear session cache to avoid running out of memory on large data sets
                        session.Clear();

                        // Page through member data
                        NHibernate.ICriteria criteria = session.CreateCriteria<EMAddressBookMember>();
                        criteria.Add(NHibernate.Criterion.Expression.Eq("Emaddressbookid", addrBookId));
                        criteria.SetFirstResult(pageNumber * pageSize).SetMaxResults(pageSize);
                        memberPage = criteria.List<EMAddressBookMember>();
                        pageNumber++;

                        totPotentialTargets += memberPage.Count;
                        foreach (IEMAddressBookMember member in memberPage)
                        {
                            if (member.Contact != null)
                            {
                                if (emailCampaign.AllowRepeatSendsToRecipient != true && sentContacts.Contains(member.Contact.Id.ToString()))
                                {
                                    numTargetsAlreadySentTo++;
                                }
                                else if (string.IsNullOrEmpty(member.Contact.Email))
                                {
                                    noEmailTargets++;
                                }
                                else if (memberEmails.Contains(member.Contact.Email))
                                {
                                    duplicateTargets++;
                                }
                                else if (member.Contact.DoNotSolicit == true || member.Contact.DoNotEmail == true)
                                {
                                    doNotSolicitTargets++;
                                }
                                else
                                {
                                    memberEmails.Add(member.Contact.Email);
                                }
                            }

                            if (member.Lead != null)
                            {
                                if (emailCampaign.AllowRepeatSendsToRecipient != true && sentLeads.Contains(member.Lead.Id.ToString()))
                                {
                                    numTargetsAlreadySentTo++;
                                }
                                else if (string.IsNullOrEmpty(member.Lead.Email))
                                {
                                    noEmailTargets++;
                                }
                                else if (memberEmails.Contains(member.Lead.Email))
                                {
                                    duplicateTargets++;
                                }
                                else if (member.Lead.DoNotSolicit == true || member.Lead.DoNotEmail == true)
                                {
                                    doNotSolicitTargets++;
                                }
                                else
                                {
                                    memberEmails.Add(member.Lead.Email);
                                }
                            }
                        }
                    }
                }

                TotalPotentialTargets.Text = totPotentialTargets.ToString();

                if (emailCampaign.AllowRepeatSendsToRecipient != true)
                {
                    TargetsAlreadySent.Text = numTargetsAlreadySentTo.ToString();
                }
                else
                {
                    TargetsAlreadySent.Text = "Not Applicable";
                }

                LessDuplicateTargets.Text = duplicateTargets.ToString();
                LessTargetsDoNotSolicit.Text = doNotSolicitTargets.ToString();
                LessTargetsNoEmail.Text = noEmailTargets.ToString();
                TotalUniqueTargets.Text = memberEmails.Count.ToString();

                Session.Add(SessionKeyTotalUniqueTargets, memberEmails.Count);
                Session.Add(SessionKeyEmailAddressBookIds, selectedCampAddrBookIds);
            }
        }
        else if (page2.Visible)
        {
            page2.Visible = false;
            page3.Visible = true;
            
            IEMEmailCampaign EmailCampaign = (IEMEmailCampaign)GetParentEntity();
            if (EmailCampaign.IsSplitTestCampaign == true)
            {
                divSplitTest.Visible = true;
            }
            else
            {
                divSplitTest.Visible = false;
            }

            litTotalUniqueTargets.Text = Session[SessionKeyTotalUniqueTargets].ToString();
        }
        else if (page3.Visible)
        {
            if (radgrpSendTime.SelectedValue == "Immediately")
            {
                Session.Add(SessionKeySendTime, "Immediately");
            }
            else
            {
                //DateTime sendTime = dntSendTime.DateTimeValue.Value;
                Session.Add(SessionKeySendTime, dntSendTime.DateTimeValue);

                Session.Add(SessionKeySendYear, dntSendTime.DateTimeValue.Value.Year);
                if (dntSendTime.DateTimeValue.Value.Month < 10)
                {
                    Session.Add(SessionKeySendMonth, "0" + dntSendTime.DateTimeValue.Value.Month.ToString());
                }
                else
                {
                    Session.Add(SessionKeySendMonth, dntSendTime.DateTimeValue.Value.Month.ToString());
                }
                if (dntSendTime.DateTimeValue.Value.Day < 10)
                {
                    Session.Add(SessionKeySendDay, "0" + dntSendTime.DateTimeValue.Value.Day.ToString());
                }
                else
                {
                    Session.Add(SessionKeySendDay, dntSendTime.DateTimeValue.Value.Day.ToString());
                }
                if (dntSendTime.DateTimeValue.Value.Hour < 10)
                {
                    Session.Add(SessionKeySendHour, "0" + dntSendTime.DateTimeValue.Value.Hour.ToString());
                }
                else
                {
                    Session.Add(SessionKeySendHour, dntSendTime.DateTimeValue.Value.Hour.ToString());
                }
                if (dntSendTime.DateTimeValue.Value.Minute < 10)
                {
                    Session.Add(SessionKeySendMinute, "0" + dntSendTime.DateTimeValue.Value.Minute.ToString());
                }
                else
                {
                    Session.Add(SessionKeySendMinute, dntSendTime.DateTimeValue.Value.Minute.ToString());
                }
            }

            if (divSplitTest.Visible)
            {
                Session.Add(SessionKeySplitPercentage, txtTargetPercentage.Text);
                Session.Add(SessionKeySplitPeriodHours, txtWaitHours.Text);
                Session.Add(SessionKeySplitMetric, ddlMetric.SelectedValue);
            }

            page3.Visible = false;
            page4.Visible = true;

            IEMEmailCampaign EmailCampaign = (IEMEmailCampaign)GetParentEntity();
            SummaryCampaignName.Text = EmailCampaign.EmailCampaignName;
            SummarySubject.Text = EmailCampaign.Subject;
            SummaryFriendlyFromAddress.Text = EmailCampaign.FriendlyFromName;
            SummaryReplyAction.Text = EmailCampaign.ReplyAction;
            SummaryCampaignType.Text = EmailCampaign.CampaignType;
            SummaryReplyEmail.Text = EmailCampaign.ReplyEmail;

            if (Session[SessionKeySendTime].ToString() == "Immediately")
            {
                SummarySendTime.Text = Session[SessionKeySendTime].ToString();
            }
            else
            {
                StringBuilder dateString = new StringBuilder();
                dateString.Append(Session[SessionKeySendYear].ToString());
                dateString.Append("-");
                dateString.Append(Session[SessionKeySendMonth].ToString());
                dateString.Append("-");
                dateString.Append(Session[SessionKeySendDay].ToString());
                dateString.Append("T");
                dateString.Append(Session[SessionKeySendHour].ToString());
                dateString.Append(":");
                dateString.Append(Session[SessionKeySendMinute].ToString());
                SummarySendTime.Text = dateString.ToString();
            }
        }
    }

    private void ClearSession()
    {
        Session.Remove(SessionKeyTotalUniqueTargets);
        Session.Remove(SessionKeyEmailAddressBookIds);
        Session.Remove(SessionKeySendTime);
        Session.Remove(SessionKeySendYear);
        Session.Remove(SessionKeySendMonth);
        Session.Remove(SessionKeySendDay);
        Session.Remove(SessionKeySendHour);
        Session.Remove(SessionKeySendMinute);
        Session.Remove(SessionKeySplitPercentage);
        Session.Remove(SessionKeySplitPeriodHours);
        Session.Remove(SessionKeySplitMetric);
    }

    /// <summary>
    /// Registers the client script.
    /// </summary>
    private void RegisterClientScript()
    {
        StringBuilder sb = new StringBuilder(GetLocalResourceObject("SendCampaignWizard_ClientScript").ToString());
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "SendCampaignWizardScript", sb.ToString(), false);
    }

    protected override void OnFormBound()
    {
        IEMEmailCampaign EmailCampaign = (IEMEmailCampaign)GetParentEntity();
        
        CampaignName.Text = EmailCampaign.EmailCampaignName;
        Subject.Text = EmailCampaign.Subject;
        FriendlyFromAddress.Text = EmailCampaign.FriendlyFromName;
        ReplyAction.Text = EmailCampaign.ReplyAction;
        CampaignType.Text = EmailCampaign.CampaignType;
        ReplyEmail.Text = EmailCampaign.ReplyEmail;

        if (page1.Visible)
        {
            BindTargetAddressBooksGrid(EmailCampaign);
        }
    }
    
    /// <summary>
    /// Called when [register client scripts].
    /// </summary>
    protected override void OnRegisterClientScripts()
    {
        base.OnRegisterClientScripts();
        RegisterClientScript();
    }

    protected void BindTargetAddressBooksGrid(IEMEmailCampaign thisEmailCampaign)
    {
            ICollection<IEMCampaignAddressBook> campaignAddressBooks = thisEmailCampaign.EMCampaignAddressBooks;
            grdTargetAddressBooks.DataSource = campaignAddressBooks;
            grdTargetAddressBooks.DataBind();
    }

    /// <summary>
    /// Handles the Init event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Init(object sender, EventArgs e)
    {
        _Timeout = Server.ScriptTimeout;
        Server.ScriptTimeout = 3600;

        ScriptManager scriptMgr = ScriptManager.GetCurrent(Page);
        scriptMgr.AsyncPostBackTimeout = 3600;
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        Server.ScriptTimeout = _Timeout;
    }

    public override Sage.Platform.Application.UI.ISmartPartInfo GetSmartPartInfo(Type smartPartInfoType)
    {
        ToolsSmartPartInfo tinfo = new ToolsSmartPartInfo();
        if (BindingSource != null)
        {
            if (BindingSource.Current != null)
            {
                tinfo.Description = BindingSource.Current.ToString();
                tinfo.Title = BindingSource.Current.ToString();
            }
        }

        foreach (Control c in Controls)
        {
            SmartPartToolsContainer cont = c as SmartPartToolsContainer;
            if (cont != null)
            {
                switch (cont.ToolbarLocation)
                {
                    case SmartPartToolsLocation.Right:
                        foreach (Control tool in cont.Controls)
                        {
                            tinfo.RightTools.Add(tool);
                        }
                        break;
                    case SmartPartToolsLocation.Center:
                        foreach (Control tool in cont.Controls)
                        {
                            tinfo.CenterTools.Add(tool);
                        }
                        break;
                    case SmartPartToolsLocation.Left:
                        foreach (Control tool in cont.Controls)
                        {
                            tinfo.LeftTools.Add(tool);
                        }
                        break;
                }
            }
        }

        return tinfo;
    }

    public int GetMemberCount(object addrBook)
    {
        IEMAddressBook castedAddrBook = (IEMAddressBook)addrBook;
        return castedAddrBook.CountMembers();
    }

    public class CampaignSendData
    {
        public string CampaignId { get; set; }
        public List<string> AddressBookIds { get; set; }
        public DateTime SendDate { get; set; }
        public int? TestPercentage { get; set; }
        public int? TestPeriodHours { get; set; }
        public string TestMetric { get; set; }
    }
}