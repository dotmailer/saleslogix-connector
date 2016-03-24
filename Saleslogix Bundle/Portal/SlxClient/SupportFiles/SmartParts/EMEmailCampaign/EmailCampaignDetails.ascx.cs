using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sage.Platform.WebPortal.SmartParts;
using System.Web.UI;
using Sage.Entity.Interfaces;
using Sage.Platform;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for EmailCampaignDetails
/// </summary>
public partial class EmailCampaignDetails : EntityBoundSmartPartInfoProvider
{
    private Sage.Platform.Security.IRoleSecurityService _roleSecurityService;
    /// <summary>
    /// Gets or sets the role security service.
    /// </summary>
    /// <value>The role security service.</value>
    [Sage.Platform.Application.ServiceDependency]
    public Sage.Platform.Security.IRoleSecurityService RoleSecurityService
    {
        set
        {
            _roleSecurityService = Sage.Platform.Application.ApplicationContext.Current.Services.Get<Sage.Platform.Security.IRoleSecurityService>(true);
        }
        get
        {
            return _roleSecurityService;
        }
    }

    public override Type EntityType
    {
        get { return typeof(Sage.Entity.Interfaces.IEMEmailCampaign); }
    }

    protected override void OnAddEntityBindings()
    {
        // txtCampaignName.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtCampaignNameTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("EmailCampaignName", txtCampaignName, "Text");
        BindingSource.Bindings.Add(txtCampaignNameTextBinding);
        // txtSubject.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtSubjectTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("Subject", txtSubject, "Text");
        BindingSource.Bindings.Add(txtSubjectTextBinding);
        // pklStatus.PickListValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding pklStatusPickListValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("Status", pklStatus, "PickListValue");
        BindingSource.Bindings.Add(pklStatusPickListValueBinding);
        // txtReplyEmail.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtReplyEmailTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("ReplyEmail", txtReplyEmail, "Text");
        BindingSource.Bindings.Add(txtReplyEmailTextBinding);
        // txtFriendlyFromName.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtFriendlyFromNameTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("FriendlyFromName", txtFriendlyFromName, "Text");
        BindingSource.Bindings.Add(txtFriendlyFromNameTextBinding);
        // pklReplyAction.PickListValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding pklReplyActionPickListValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("ReplyAction", pklReplyAction, "PickListValue");
        BindingSource.Bindings.Add(pklReplyActionPickListValueBinding);
        // lueEmailAccount.LookupResultValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding lueEmailAccountLookupResultValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("EMEmailAccount", lueEmailAccount, "LookupResultValue");
        BindingSource.Bindings.Add(lueEmailAccountLookupResultValueBinding);
        // chkSyncWithEmailService.Checked Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding chkSyncWithEmailServiceCheckedBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("SyncWithEmailService", chkSyncWithEmailService, "Checked");
        BindingSource.Bindings.Add(chkSyncWithEmailServiceCheckedBinding);
        // lueSLXCampaign.LookupResultValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding lueSLXCampaignLookupResultValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("SLXCampaignID", lueSLXCampaign, "LookupResultValue");
        BindingSource.Bindings.Add(lueSLXCampaignLookupResultValueBinding);
        // radCampaignType.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding radCampaignTypeTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("CampaignType", radCampaignType, "Text");
        BindingSource.Bindings.Add(radCampaignTypeTextBinding);
        // chkAllowRepeatSends.Checked Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding chkAllowRepeatSendsCheckedBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("AllowRepeatSendsToRecipient", chkAllowRepeatSends, "Checked");
        BindingSource.Bindings.Add(chkAllowRepeatSendsCheckedBinding);
        // dntLastSent.DateTimeValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding dntLastSentDateTimeValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("LastSent", dntLastSent, "DateTimeValue");
        BindingSource.Bindings.Add(dntLastSentDateTimeValueBinding);
        // dntSendTime.DateTimeValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding dntSendTimeDateTimeValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("SendTime", dntSendTime, "DateTimeValue");
        BindingSource.Bindings.Add(dntSendTimeDateTimeValueBinding);
        // chkIsSplitTest.Checked Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding chkIsSplitTestCheckedBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("IsSplitTestCampaign", chkIsSplitTest, "Checked");
        BindingSource.Bindings.Add(chkIsSplitTestCheckedBinding);
        // txtMetric.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtMetricTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("SplitTestMetric", txtMetric, "Text");
        BindingSource.Bindings.Add(txtMetricTextBinding);
        // txtSplitTestPercent.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtSplitTestPercentTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("SplitTestPercent", txtSplitTestPercent, "Text");
        BindingSource.Bindings.Add(txtSplitTestPercentTextBinding);
        // txtSplitTestOpenTime.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtSplitTestOpenTimeTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("SplitTestOpenTime", txtSplitTestOpenTime, "Text");
        BindingSource.Bindings.Add(txtSplitTestOpenTimeTextBinding);


    }

    protected void btnSendEmailCampaign_ClickAction(object sender, EventArgs e)
    {
        ShowSendWizardDialog();
    }

    private void ShowSendWizardDialog()
    {
        if (DialogService != null)
        {
            // DialogActionItem
            DialogService.SetSpecs(570, 500, "SendEmailCampaignWizard", GetLocalResourceObject("2c03aac9-a7b8-4c38-99c5-5e4d9b689a80.DialogTitleOverride").ToString());
            DialogService.EntityType = typeof(Sage.Entity.Interfaces.IEMSyncTask);
            DialogService.ShowDialog();
        }
    }

    void btnSynchronise_Click(object sender, ImageClickEventArgs e)
    {
        IEMEmailCampaign EmailCampaign = (IEMEmailCampaign)this.BindingSource.Current;

        // Add a sync task if a pending one doesn't already exist for this campaign
        RepositoryHelper<IEMSyncTask> rep = EntityFactory.GetRepositoryHelper<IEMSyncTask>();
        Sage.Platform.Repository.ICriteria criteria = rep.CreateCriteria();
        criteria.Add(rep.EF.Eq("TaskType", "SynchroniseEmailCampaign"));
        criteria.Add(rep.EF.Eq("Status", "Pending"));
        criteria.Add(rep.EF.Like("TaskData", EmailCampaign.Id.ToString()));
        var result = criteria.List<IEMSyncTask>();

        if (result == null || result.Count == 0)
        {
            IEMSyncTask syncTask = EntityFactory.Create<IEMSyncTask>();
            syncTask.TaskType = "SynchroniseEmailCampaign";
            syncTask.ScheduledStartTime = DateTime.UtcNow;
            syncTask.Status = "Pending";
            syncTask.TaskData = EmailCampaign.Id.ToString();
            syncTask.Save();
        }

        string js = string.Format(
            "showDialog('{0}', '{1}', '700px', 'infoIcon');",
            GetLocalResourceObject("msgEmailCampaignSynchronisation.Title"),
            GetLocalResourceObject("msgEmailCampaignSynchronisation.Caption"));
        ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, js, true);
    }

    /// <summary>
    /// Returns true if the passed URL is not empty.  If it is empty, then client-side script is
    /// prepared to inform the user.
    /// </summary>
    /// <param name="emailApplicationUrl">The Email Application URL to test</param>
    /// <returns>False to indicate an empty URL, or true to indicate a URL that is not empty.</returns>
    private bool CheckEmailApplicationUrlIsNotEmpty(string emailApplicationUrl)
    {
        bool emailApplicationUrlIsEmpty = (emailApplicationUrl ?? "").Trim() == "";
        if (emailApplicationUrlIsEmpty)
        {
            string script = @"alert(""This feature is only available after an Email Application URL has been specified in your Email Account's settings."");";
            ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, script, true);
        }
        return !emailApplicationUrlIsEmpty;
    }

    protected void btnOpenDotMailer_Click(object sender, EventArgs e)
    {
        IEMEmailCampaign thisCampaign = (IEMEmailCampaign)this.BindingSource.Current;
        string emailServiceCampaignId = thisCampaign.EmailServiceCampaignID.ToString();

        string emailApplicationUrl = thisCampaign.EMEmailAccount.EmailApplicationUrl;
        if (CheckEmailApplicationUrlIsNotEmpty(emailApplicationUrl))
        {
            if (!emailApplicationUrl.EndsWith("/"))
                emailApplicationUrl = emailApplicationUrl + "/";

            if (!emailApplicationUrl.StartsWith("http"))
                emailApplicationUrl = "http://" + emailApplicationUrl;

            ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, "window.open('" + emailApplicationUrl + "campaigns/Step/Summary.aspx?id=" + emailServiceCampaignId + "');", true);
        }
    }

    protected void btnOpenReports_Click(object sender, EventArgs e)
    {
        IEMEmailCampaign thisCampaign = (IEMEmailCampaign)this.BindingSource.Current;
        string emailServiceCampaignId = thisCampaign.EmailServiceCampaignID.ToString();

        string emailApplicationUrl = thisCampaign.EMEmailAccount.EmailApplicationUrl;
        if (CheckEmailApplicationUrlIsNotEmpty(emailApplicationUrl))
        {
            if (!emailApplicationUrl.EndsWith("/"))
                emailApplicationUrl = emailApplicationUrl + "/";

            if (!emailApplicationUrl.StartsWith("http"))
                emailApplicationUrl = "http://" + emailApplicationUrl;

            ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, "window.open('" + emailApplicationUrl + "reporting/CampaignOverview.aspx?i=" + emailServiceCampaignId + "');", true);
        }
    }

    protected void btnSave_ClickAction(object sender, EventArgs e)
    {
        Sage.Entity.Interfaces.IEMEmailCampaign _entity = BindingSource.Current as Sage.Entity.Interfaces.IEMEmailCampaign;
        if (_entity != null)
        {
            object _parent = GetParentEntity();
            if (DialogService.ChildInsertInfo != null)
            {
                if (_parent != null)
                {
                    if (DialogService.ChildInsertInfo.ParentReferenceProperty != null)
                    {
                        DialogService.ChildInsertInfo.ParentReferenceProperty.SetValue(_entity, _parent, null);
                    }
                }
            }
            bool shouldSave = true;
            Sage.Platform.WebPortal.EntityPage page = Page as Sage.Platform.WebPortal.EntityPage;
            if (page != null)
            {
                if (IsInDialog() && page.ModeId.ToUpper() == "INSERT")
                {
                    shouldSave = false;
                }
            }

            if (shouldSave)
            {
                _entity.Save();
            }

            if (_parent != null)
            {
                if (DialogService.ChildInsertInfo != null)
                {
                    if (DialogService.ChildInsertInfo.ParentsCollectionProperty != null)
                    {
                        System.Reflection.MethodInfo _add = DialogService.ChildInsertInfo.ParentsCollectionProperty.PropertyType.GetMethod("Add");
                        _add.Invoke(DialogService.ChildInsertInfo.ParentsCollectionProperty.GetValue(_parent, null), new object[] { _entity });
                    }
                }
            }
        }
    }

    protected void btnDelete_ClickAction(object sender, EventArgs e)
    {
        Sage.Platform.Orm.Interfaces.IPersistentEntity persistentEntity = this.BindingSource.Current as Sage.Platform.Orm.Interfaces.IPersistentEntity;
        if (persistentEntity != null)
        {
            persistentEntity.Delete();
        }

        btnDelete_ClickActionBRC(sender, e);
    }

    protected void btnDelete_ClickActionBRC(object sender, EventArgs e)
    {
        Response.Redirect("EMEmailCampaign.aspx");
    }

    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
		if (this.RoleSecurityService.HasAccess("ENTITIES/EMEMAILCAMPAIGN/EDIT"))
        {
			btnOpenDotMailer.Click += new ImageClickEventHandler(btnOpenDotMailer_Click);
			btnOpenReports.Click += new ImageClickEventHandler(btnOpenReports_Click);
			btnSynchronise.Click += new ImageClickEventHandler(btnSynchronise_Click);
		}
		
        lueSLXCampaign.LookupResultValueChanged += new EventHandler(lueSLXCampaign_LookupResultValueChanged);

        if (this.RoleSecurityService.HasAccess("EMAILMARKETING/SENDCAMPAIGN"))
        {
            btnSendEmailCampaign.Click += new ImageClickEventHandler(btnSendEmailCampaign_ClickAction);
        }

        if (this.RoleSecurityService.HasAccess("ENTITIES/EMEMAILCAMPAIGN/EDIT"))
        {
            btnSave.Click += new ImageClickEventHandler(btnSave_ClickAction);
        }

        if (this.RoleSecurityService.HasAccess("ENTITIES/EMEMAILCAMPAIGN/DELETE"))
        {
            btnDelete.Click += new ImageClickEventHandler(btnDelete_ClickAction);
        }


    }

    void lueSLXCampaign_LookupResultValueChanged(object sender, EventArgs e)
    {
        string campaignId = lueSLXCampaign.LookupResultValue.ToString();
        if (!string.IsNullOrEmpty(campaignId))
        {
            ICampaign campaign = EntityFactory.GetById<ICampaign>(campaignId);
            if (campaign.CampaignTargets.Count > 0)
            {
                string js = @"
function processResult(btnOrResult) {
  if (btnOrResult === 'no' || btnOrResult === false) {
    document.getElementById('" + lueSLXCampaign.ClientID + @"_LookupText').value = ''; 
    document.getElementById('" + lueSLXCampaign.ClientID + @"_LookupResult').value = ''; 
  }
}

var title = 'Existing Campaign Targets',
  msg = 'Are you sure you wish to link this Email Campaign to a SalesLogix Campaign that already has some targets?<br /><br /><i>The synchronisation process is one-way. Email Address Book Members will be created as targets in the SalesLogix Campaign, but the " + campaign.CampaignTargets.Count.ToString() + @" existing SalesLogix Campaign Targets will not be added to the Email Address Book(s).</i>';

if (Sage && Sage.UI && Sage.UI.Dialogs) {
        // Use 'V8' dialog
        var opts = {
            title: title,
            query: msg,
            callbackFn: processResult,
            yesText: 'Yes',
            noText: 'No',
            scope: window,
            icon: 'warningIcon',
            style: { width: '720px' }
        };
        Sage.UI.Dialogs.raiseQueryDialogExt(opts);
} else {
    // Use 'V7' dialog
    Ext.Msg.show({title: title, msg: msg, buttons: Ext.MessageBox.YESNO, fn:processResult}); 
}";                 
                ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, js, true);
            }
        }
    }

    protected override void OnFormBound()
    {
        ClientBindingMgr.RegisterSaveButton(btnSave);
        if (Request.QueryString["activedialog"] == "SendEmailCampaignWizard")
        {
            ShowSendWizardDialog();
        }

        btnDelete.OnClientClick = string.Format("return confirm('{0}');", Sage.Platform.WebPortal.PortalUtil.JavaScriptEncode(GetLocalResourceObject("btnDelete.ActionConfirmationMessage").ToString()));
		
		if (!this.RoleSecurityService.HasAccess("ENTITIES/EMEMAILCAMPAIGN/EDIT"))
        {
			btnOpenDotMailer.Visible = false;
			btnOpenReports.Visible = false;
			btnSynchronise.Visible = false;
		}

        if (!this.RoleSecurityService.HasAccess("EMAILMARKETING/SENDCAMPAIGN"))
        {
            btnSendEmailCampaign.Visible = false;
        }

        if (!this.RoleSecurityService.HasAccess("ENTITIES/EMEMAILCAMPAIGN/EDIT"))
        {
            btnSave.Visible = false;
        }

        if (!this.RoleSecurityService.HasAccess("ENTITIES/EMEMAILCAMPAIGN/DELETE"))
        {
            btnDelete.Visible = false;
        }

        lueSLXCampaign.LookupPreFilters.Clear();
        Sage.SalesLogix.HighLevelTypes.LookupPreFilter filter = new Sage.SalesLogix.HighLevelTypes.LookupPreFilter("Status", "'Inactive'");
        filter.CondOperator = "Not Equal to";
        lueSLXCampaign.LookupPreFilters.Add(filter);
		
		// Inject javascript for showing dialogs
        string js = @"
function showDialog(title, msg, v8width, v8icon){
    if (Sage && Sage.UI && Sage.UI.Dialogs) {
        // Use 'V8' dialog
        var opts = {
            title: title,
            query: msg,
            callbackFn: false,
            yesText: 'OK',
            noText: false,
            scope: window,
            icon: v8icon,
            style: { width: v8width }
        };
        Sage.UI.Dialogs.raiseQueryDialogExt(opts);
    } else {
        // Use 'V7' dialog
        Sage.Services.getService('WebClientMessageService').showClientMessage(title, msg);
    }
}";
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "showDialogs", js, true);
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
}