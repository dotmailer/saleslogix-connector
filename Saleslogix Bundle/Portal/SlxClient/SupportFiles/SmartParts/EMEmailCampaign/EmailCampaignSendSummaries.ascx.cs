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
/// Summary description for EmailCampaignSendSummaries
/// </summary>
public partial class EmailCampaignSendSummaries : EntityBoundSmartPartInfoProvider
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
                        case "ContactAccountId":
                            return contact.Account.Id.ToString();
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
                            return lead.LeadSource.ToString();
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

    protected override void OnFormBound()
    {
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
                grdCampaignSendSummaries_Responses.Visible = true;
                grdCampaignSendSummaries_Contacts.Visible = false;
                grdCampaignSendSummaries_Leads.Visible = false;
                BindGrid(grdCampaignSendSummaries_Responses, filter);
                break;
            case "Contacts":
                grdCampaignSendSummaries_Responses.Visible = false;
                grdCampaignSendSummaries_Contacts.Visible = true;
                grdCampaignSendSummaries_Leads.Visible = false;
                BindGrid(grdCampaignSendSummaries_Contacts, filter);
                break;
            case "Leads":
                grdCampaignSendSummaries_Responses.Visible = false;
                grdCampaignSendSummaries_Contacts.Visible = false;
                grdCampaignSendSummaries_Leads.Visible = true;
                BindGrid(grdCampaignSendSummaries_Leads, filter);
                break;
        }
    }

    private void BindGrid(SlxGridView grid, string filter)
    {
        IEMEmailCampaign emailCampaign = (IEMEmailCampaign)this.BindingSource.Current;

        IRepository<IEMCampaignSendSummary> rep = EntityFactory.GetRepository<IEMCampaignSendSummary>();
        IQueryable qry = (IQueryable)rep;
        IExpressionFactory ef = qry.GetExpressionFactory();

        ICriteria criteria = qry.CreateCriteria();
        criteria.Add(ef.Eq("EMEmailCampaign.Id", emailCampaign.Id));

        switch (grid.ID.ToString())
        {
            case "grdCampaignSendSummaries_Responses":
                break;
            case "grdCampaignSendSummaries_Contacts":
                criteria.Add(ef.Eq("SlxMemberType", "Contact"));
                break;
            case "grdCampaignSendSummaries_Leads":
                criteria.Add(ef.Eq("SlxMemberType", "Lead"));
                break;
        }

        switch (filter)
        {
            case "All":
                break;
            case "Opened":
                criteria.Add(ef.Gt("NumOpens", 0));
                break;
            case "Clicked":
                criteria.Add(ef.Gt("NumClicks", 0));
                break;
            case "Viewed":
                criteria.Add(ef.Gt("NumViews", 0));
                break;
            case "Replied":
                criteria.Add(ef.Gt("NumReplies", 0));
                break;
            case "Forwarded":
                criteria.Add(ef.Gt("NumForwardToFriend", 0));
                break;
            case "HardBounce":
                criteria.Add(ef.Eq("HardBounced", true));
                break;
            case "SoftBounce":
                criteria.Add(ef.Eq("SoftBounced", true));
                break;
            case "Unsubscribed":
                criteria.Add(ef.Eq("Unsubscribed", true));
                break;
        }
        IList<IEMCampaignSendSummary> data = criteria.List<IEMCampaignSendSummary>();
        grid.DataSource = data;
        grid.DataBind();
    }

    protected void grdCampaignSendSummaries_Responses_Changing(object sender, GridViewPageEventArgs e)
    {
        grdCampaignSendSummaries_Responses.PageIndex = e.NewPageIndex;
        grdCampaignSendSummaries_Responses.DataBind();
    }

    protected void grdCampaignSendSummaries_Contacts_Changing(object sender, GridViewPageEventArgs e)
    {
        grdCampaignSendSummaries_Contacts.PageIndex = e.NewPageIndex;
        grdCampaignSendSummaries_Contacts.DataBind();
    }

    protected void grdCampaignSendSummaries_Leads_Changing(object sender, GridViewPageEventArgs e)
    {
        grdCampaignSendSummaries_Leads.PageIndex = e.NewPageIndex;
        grdCampaignSendSummaries_Leads.DataBind();
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