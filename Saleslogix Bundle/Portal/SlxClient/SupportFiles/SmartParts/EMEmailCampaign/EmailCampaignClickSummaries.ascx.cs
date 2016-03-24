using System;
using System.Collections.Generic;
using System.Web;
using Sage.Platform.WebPortal.SmartParts;
using System.Web.UI;
using Sage.Entity.Interfaces;
using Sage.Platform.Repository;
using Sage.Platform;
using Sage.SalesLogix.Web.Controls;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for EmailCampaignClickSummaries
/// </summary>
public partial class EmailCampaignClickSummaries : EntityBoundSmartPartInfoProvider
{
    /// <summary>
    /// Gets or sets the role security service.
    /// </summary>
    /// <value>The role security service.</value>
    [Sage.Platform.Application.ServiceDependency]
    public Sage.Platform.Security.IRoleSecurityService RoleSecurityService { get; set; }

    public override Type EntityType
    {
        get { return typeof(Sage.Entity.Interfaces.IEMEmailCampaign); }
    }
        
    protected override void OnAddEntityBindings()
    {
        
    }
    
    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
        ddlFilter.SelectedIndexChanged += new EventHandler(ddlFilterView_SelectedIndexChanged);
        ddlView.SelectedIndexChanged += new EventHandler(ddlFilterView_SelectedIndexChanged);
    }

    public string ProcessEvalData(string dataTag, object slxID, string entityType)
    {
        if (slxID == null)
        {
            return "";
        }
        else
        {
            if (entityType == "Contact")
            {
                IContact contact = EntityFactory.GetById<IContact>(slxID);
                if (contact == null)
                {
                    if (dataTag == "ContactWorkPhone")
                    {
                        return "";
                    }
                    else
                    {
                        return "Unavailable";
                    }
                }
                else
                {
                    switch (dataTag)
                    {
                        case "ContactFullName":
                            return contact.FullName;
                        case "ContactEmail":
                            return contact.Email;
                        case "ContactAccountId":
                            return contact.Account == null ? "" : contact.Account.Id.ToString();
                        case "ContactAccountName":
                            return contact.AccountName;
                        case "ContactWorkPhone":
                            return contact.WorkPhone;
                        default:
                            return "";
                    }
                }
            }
            else if (entityType == "Lead")
            {
                ILead lead = EntityFactory.GetById<ILead>(slxID);
                if (lead == null)
                {
                    return "Unavailable";
                }
                else
                {
                    switch (dataTag)
                    {
                        case "LeadFullName":
                            return lead.LeadFullName;
                        case "LeadCompanyName":
                            return lead.Company;
                        case "LeadLeadSource":
                            return lead.LeadSource == null ? "" : lead.LeadSource.ToString();
                        case "LeadEmail":
                            return lead.Email;
                        default:
                            return "";
                    }
                }
            }
            else
            {
                return "";
            }
        }
    }

    void ddlFilterView_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedFilter = ddlFilter.SelectedValue;
        string selectedView = ddlView.SelectedValue;

        ShowGrid(selectedFilter, selectedView);
    }

    private bool IsCampaignSendWizardOrHasItAsAChildControl(Control control)
    {
        if ((control.Visible) && (control.GetType().Name.ToLower().Contains("sendemailcampaignwizard"))) //we're expecting "smartparts_ememailcampaign_sendemailcampaignwizard_ascx"
            return true;
        foreach (Control childControl in control.Controls)
        {
            if (IsCampaignSendWizardOrHasItAsAChildControl(childControl))
                return true;
        }
        return false;
    }

    private bool SendCampaignWizardIsVisible()
    {
        return IsCampaignSendWizardOrHasItAsAChildControl(Page);
    }

    protected override void OnFormBound()
    {
            if (SendCampaignWizardIsVisible())
                return; //SLX-5
        
            IEMEmailCampaign emailCampaign = (IEMEmailCampaign)this.BindingSource.Current;
            ICollection<IEMCampaignConClick> campaignClicks = emailCampaign.EMCampaignConClicks;
            if (!ddlFilter.Items.Contains(ddlFilter.Items.FindByValue("All")))
            {
                ddlFilter.Items.Add(new ListItem("All", "All"));
            }
            foreach (IEMCampaignConClick clickSummary in campaignClicks)
            {
                if (ddlFilter.Items.FindByText(clickSummary.Keyword) == null)
                {
                    ddlFilter.Items.Add(new ListItem(clickSummary.Keyword, clickSummary.Keyword));
                }
            }

            List<string> itemsToRemove = new List<string>();
            foreach (ListItem item in ddlFilter.Items)
            {
                if (item.Value != "All")
                {
                    bool shouldRemove = true;
                    foreach (IEMCampaignConClick clickSummary in campaignClicks)
                    {
                        if (clickSummary.Keyword == item.Value)
                        {
                            shouldRemove = false;
                        }
                    }
                    if (shouldRemove)
                    {
                        itemsToRemove.Add(item.Value);
                    }
                }
            }

            foreach (string itemToRemove in itemsToRemove)
            {
                ddlFilter.Items.Remove(ddlFilter.Items.FindByValue(itemToRemove));
            }
        

        

        string selectedFilter = ddlFilter.SelectedValue;
        string selectedView = ddlView.SelectedValue;

        ShowGrid(selectedFilter, selectedView);
    }

    protected void ShowGrid(string filter, string view)
    {

        filter = string.IsNullOrEmpty(filter) ? "All" : filter;
        view = string.IsNullOrEmpty(view) ? "Responses" : view;

        switch (view)
        {
            case "Responses":
                grdCampaignClickSummaries_Responses.Visible = true;
                grdCampaignClickSummaries_Contacts.Visible = false;
                grdCampaignClickSummaries_Leads.Visible = false;
                BindGrid(grdCampaignClickSummaries_Responses, filter);
                break;
            case "Contacts":
                grdCampaignClickSummaries_Responses.Visible = false;
                grdCampaignClickSummaries_Contacts.Visible = true;
                grdCampaignClickSummaries_Leads.Visible = false;
                BindGrid(grdCampaignClickSummaries_Contacts, filter);
                break;
            case "Leads":
                grdCampaignClickSummaries_Responses.Visible = false;
                grdCampaignClickSummaries_Contacts.Visible = false;
                grdCampaignClickSummaries_Leads.Visible = true;
                BindGrid(grdCampaignClickSummaries_Leads, filter);
                break;
        }
    }

    private void BindGrid(SlxGridView grid, string filter)
    {
        IEMEmailCampaign emailCampaign = (IEMEmailCampaign)this.BindingSource.Current;

        IRepository<IEMCampaignConClick> rep = EntityFactory.GetRepository<IEMCampaignConClick>();
        IQueryable qry = (IQueryable)rep;
        IExpressionFactory ef = qry.GetExpressionFactory();

        ICriteria criteria = qry.CreateCriteria();
        criteria.Add(ef.Eq("EMEmailCampaign.Id", emailCampaign.Id));

        switch (grid.ID.ToString())
        {
            case "grdCampaignClickSummaries_Responses":
                break;
            case "grdCampaignClickSummaries_Contacts":
                criteria.Add(ef.Eq("SlxMemberType", "Contact"));
                break;
            case "grdCampaignClickSummaries_Leads":
                criteria.Add(ef.Eq("SlxMemberType", "Lead"));
                break;
        }
        if (filter != "All")
        {
            criteria.Add(ef.Eq("Keyword", filter));
        }
        IList<IEMCampaignConClick> data = criteria.List<IEMCampaignConClick>();
        grid.DataSource = data;
        grid.DataBind();
    }

    protected void grdCampaignClickSummaries_Responses_Changing(object sender, GridViewPageEventArgs e)
    {
        grdCampaignClickSummaries_Responses.PageIndex = e.NewPageIndex;
        grdCampaignClickSummaries_Responses.DataBind();
    }

    protected void grdCampaignClickSummaries_Contacts_Changing(object sender, GridViewPageEventArgs e)
    {
        grdCampaignClickSummaries_Contacts.PageIndex = e.NewPageIndex;
        grdCampaignClickSummaries_Contacts.DataBind();
    }

    protected void grdCampaignClickSummaries_Leads_Changing(object sender, GridViewPageEventArgs e)
    {
        grdCampaignClickSummaries_Leads.PageIndex = e.NewPageIndex;
        grdCampaignClickSummaries_Leads.DataBind();
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