using System;
using System.Data;
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sage.Platform.WebPortal.SmartParts;
using Sage.Entity.Interfaces;
using Sage.Platform.Application.UI;
using System.Collections.Generic;
using Sage.Platform.Repository;
using System.Text;
using System.Text.RegularExpressions;
using Sage.Platform;
using Sage.Platform.Application;
using Sage.Platform.Security;
using System.Collections;
using Sage.SalesLogix.CampaignTarget;
using System.Threading;
using Sage.SalesLogix.Client.GroupBuilder;
using log4net;
using System.Reflection;
using NHibernate;
using Sage.Platform.Data;
using Sage.Platform.Framework;
using Sage.Platform.Application.Services;

/// <summary>
/// Summary description for AddAddressBookMembers
/// </summary>
public partial class AddAddressBookMembers : EntityBoundSmartPartInfoProvider
{
    #region properties

    private IContextService _Context;
    private AddManageFilterStateInfo _State;
    private int _Timeout;
    
    #endregion

    #region Public definitions

    /// <summary>
    /// Gets or sets the role security service.
    /// </summary>
    /// <value>The role security service.</value>
    [Sage.Platform.Application.ServiceDependency]
    public Sage.Platform.Security.IRoleSecurityService RoleSecurityService { get; set; }

    public override Type EntityType
    {
        get { return typeof(Sage.Entity.Interfaces.IEMAddressBookMember); }
    }

    /// <summary>
    /// Gets the smart part info.
    /// </summary>
    /// <param name="smartPartInfoType">Type of the smart part info.</param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    public class AddManageFilterStateInfo
    {
        public DataTable memberList = new DataTable();
        public string memberType = String.Empty;
        public string groupName = String.Empty;
    }

    public enum SearchParameter
    {
        StartingWith,
        Contains,
        EqualTo,
        NotEqualTo,
        EqualOrLessThan,
        EqualOrGreaterThan,
        LessThan,
        GreaterThan
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Registers the client script.
    /// </summary>
    private void RegisterClientScript()
    {
        StringBuilder sb = new StringBuilder(GetLocalResourceObject("ManageMembers_ClientScript").ToString());
        
        sb.Replace("@mt_chkCompanyId", chkCompany.ClientID);
        sb.Replace("@mt_lbxCompanyId", lbxCompany.ClientID);
        sb.Replace("@mt_txtCompanyId", txtCompany.ClientID);
        sb.Replace("@mt_chkIndustryId", chkIndustry.ClientID);
        sb.Replace("@mt_lbxIndustryId", lbxIndustry.ClientID);
        sb.Replace("@mt_pklIndustryId", pklIndustry.ClientID + "_obj");
        sb.Replace("@mt_chkSICId", chkSIC.ClientID);
        sb.Replace("@mt_lbxSICId", lbxSIC.ClientID);
        sb.Replace("@mt_txtSICId", txtSIC.ClientID);
        sb.Replace("@mt_chkTitleId", chkTitle.ClientID);
        sb.Replace("@mt_lbxTitleId", lbxTitle.ClientID);
        sb.Replace("@mt_pklTitleId", pklTitle.ClientID + "_obj");
        sb.Replace("@mt_chkProductsId", chkProducts.ClientID);
        sb.Replace("@mt_lbxProductsId", lbxProducts.ClientID);
        sb.Replace("@mt_lueProductsId", lueProducts.ClientID);
        sb.Replace("@mt_chkStatusId", chkStatus.ClientID);
        sb.Replace("@mt_lbxStatusId", lbxStatus.ClientID);
        sb.Replace("@mt_pklStatusId", pklStatus.ClientID + "_obj");
        sb.Replace("@mt_chkSolicitId", chkSolicit.ClientID);
        sb.Replace("@mt_chkEmailId", chkEmail.ClientID);
        sb.Replace("@mt_chkCallId", chkCall.ClientID);
        sb.Replace("@mt_chkMailId", chkMail.ClientID);
        sb.Replace("@mt_chkFaxId", chkFax.ClientID);
        sb.Replace("@mt_chkCityId", chkCity.ClientID);
        sb.Replace("@mt_lbxCityId", lbxCity.ClientID);
        sb.Replace("@mt_txtCityId", txtCity.ClientID);
        sb.Replace("@mt_chkStateId", chkState.ClientID);
        sb.Replace("@mt_lbxStateId", lbxState.ClientID);
        sb.Replace("@mt_txtStateId", txtState.ClientID);
        sb.Replace("@mt_chkZipId", chkZip.ClientID);
        sb.Replace("@mt_lbxZipId", lbxZip.ClientID);
        sb.Replace("@mt_txtZipId", txtZip.ClientID);
        sb.Replace("@mt_chkLeadSourceId", chkLeadSource.ClientID);
        sb.Replace("@mt_lbxLeadSourceId", lbxLeadSource.ClientID);
        sb.Replace("@mt_lueLeadSourceId", lueLeadSource.ClientID);
        sb.Replace("@mt_chkImportSourceId", chkImportSource.ClientID);
        sb.Replace("@mt_lbxImportSourceId", lbxImportSource.ClientID);
        sb.Replace("@mt_pklImportSourceId", pklImportSource.ClientID + "_obj");
        sb.Replace("@mt_chkCreateDateId", chkCreateDate.ClientID);
        sb.Replace("@mt_dtpFromDateId", dtpCreateFromDate.ClientID);
        sb.Replace("@mt_dtpToDateId", dtpCreateToDate.ClientID);
        sb.Replace("@mt_divLookupMembersId", divLookupMembers.ClientID);
        sb.Replace("@mt_divAddFromGroupId", divAddFromGroup.ClientID);
        sb.Replace("@mt_tabLookupMemberId", tabLookupMembers.ClientID);
        sb.Replace("@mt_tabAddFromGroupId", tabAddFromGroup.ClientID);
        sb.Replace("@mt_txtSelectedTabId", txtSelectedTab.ClientID);
        
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ManageMemberScript", sb.ToString(), false);
         
    }

    /// <summary>
    /// Called when [add entity bindings].
    /// </summary>
    protected override void OnAddEntityBindings()
    {
    }

    /// <summary>
    /// Called when [wire event handlers].
    /// </summary>
    protected override void OnWireEventHandlers()
    {
        cmdCancel.Click += new EventHandler(DialogService.CloseEventHappened);
        cmdClearAll.Attributes.Add("onClick", "return false;");
        base.OnWireEventHandlers();
    }

    /// <summary>
    /// Called when [activating].
    /// </summary>
    protected override void OnActivating()
    {
        lblHowMany.Text = String.Empty;
        FilterOptions options = new FilterOptions();
        SetFilterControls(options);
        AddDistinctGroupItemsToList(lbxContactGroups, "Contact");
        AddDistinctGroupItemsToList(lbxLeadGroups, "Lead");
    }

    /// <summary>
    /// Called when the dialog is closing.
    /// </summary>
    protected override void OnClosing()
    {
        _Context.RemoveContext("AddManageEMFilterStateInfo");
        FilterOptions options = new FilterOptions();
        SetFilterOptions(options);
        options.Save();
        base.OnClosing();
        Refresh();
    }

    /// <summary>
    /// Adds the distinct group items to list.
    /// </summary>
    private static void AddDistinctGroupItemsToList(ListBox listBox, String entityName)
    {
        listBox.Items.Clear();
        listBox.Items.Add(String.Empty);

        IList groups = GroupInfo.GetGroupList(entityName);
        if (groups != null)
        {
            ListItem item;
            foreach (GroupInfo group in groups)
            {
                if (!String.IsNullOrEmpty(group.DisplayName))
                {
                    item = new ListItem();
                    item.Text = group.DisplayName;
                    item.Value = group.GroupID;
                    listBox.Items.Add(item);
                }
            }
        }
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

        if (Visible)
        {
            _Context = ApplicationContext.Current.Services.Get<IContextService>();
            if (_Context.HasContext("AddManageEMFilterStateInfo"))
            {
                _State = (AddManageFilterStateInfo)_Context.GetContext("AddManageEMFilterStateInfo");
            }
            if (_State == null)
            {
                _State = new AddManageFilterStateInfo();
                _State.memberList = GetDataGridLayout();
            }

            rdgIncludeType.Items[0].Attributes.Add("onclick", "javascript:onSearchTypeChange(0);");
            rdgIncludeType.Items[1].Attributes.Add("onclick", "javascript:onSearchTypeChange(1);");
            rdgIncludeType.Items[2].Attributes.Add("onclick", "javascript:onSearchTypeChange(2);");
            rdgIncludeType.Items[3].Attributes.Add("onclick", "javascript:onSearchTypeChange(3);");
            tabLookupMembers.Attributes.Add("onclick", "javascript:OnTabLookupMemberClick()");
            tabAddFromGroup.Attributes.Add("onclick", "javascript:OnTabAddFromGroupClick()");
        }
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        Server.ScriptTimeout = _Timeout;
    }

    /// <summary>
    /// Called when the smartpart has been bound.  Derived components should override this method to run code that depends on entity 
    /// context being set and it not changing.
    /// </summary>
    protected override void OnFormBound()
    {
        ClientBindingMgr.RegisterDialogCancelButton(cmdCancel);
        if (_State != null)
        {
            if (!String.IsNullOrEmpty(txtSelectedTab.Value))
            {
                if (Convert.ToInt16(txtSelectedTab.Value) == 1)
                {
                    divLookupMembers.Style.Add(HtmlTextWriterStyle.Display, "none");
                    divAddFromGroup.Style.Add(HtmlTextWriterStyle.Display, "inline");
                    tabLookupMembers.CssClass = "inactiveTab tab";
                    tabAddFromGroup.CssClass = "activeTab tab";
                }
                else
                {
                    divAddFromGroup.Style.Add(HtmlTextWriterStyle.Display, "none");
                    divLookupMembers.Style.Add(HtmlTextWriterStyle.Display, "inline");
                    tabLookupMembers.CssClass = "activeTab tab";
                    tabAddFromGroup.CssClass = "inactiveTab tab";
                }
            }
            grdMembers.DataSource = _State.memberList;
        }
        grdMembers.DataBind();
        cmdAddMembers.Enabled = grdMembers.Rows.Count > 0;

        // Inject javascript for showing dialogs.  N.B. This is slightly different from other examples of this function
        string js = @"
function showDialog(title, msg, v8width, v8icon, callback){
    if (Sage && Sage.UI && Sage.UI.Dialogs) {
        // Use 'V8' dialog
        var opts = {
            title: title,
            query: msg,
            callbackFn: callback,
            yesText: 'OK',
            noText: false,
            scope: window,
            icon: v8icon,
            style: { width: v8width }
        };
        Sage.UI.Dialogs.raiseQueryDialogExt(opts);
    } else {
        // Use 'V7' dialog
        Sage.Services.getService('WebClientMessageService').showClientMessage(title, msg, callback);
    }
}";
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "showDialogs", js, true);
    }

    /// <summary>
    /// Called when [register client scripts].
    /// </summary>
    protected override void OnRegisterClientScripts()
    {
        base.OnRegisterClientScripts();
        RegisterClientScript();
    }

    /// <summary>
    /// Gets the list of members based on the filter options.
    /// </summary>
    /// <returns>DataTable </returns>
    private DataTable GetMembers()
    {
        DataTable dt = null;
        IList members = null;
        if (EntityContext != null)
        {
            dt = GetDataGridLayout();
            switch (rdgIncludeType.SelectedIndex)
            {
                case 0:
                    members = GetLeadMembers();
                    break;
                case 1:
                    members = GetAccountMembers();
                    break;
                case 2:
                    members = GetAccountMembers();
                    break;
                case 3:
                    members = GetContactMembers();
                    break;
            }
            if (members != null)
            {
                foreach (object[] data in members)
                {
                    dt.Rows.Add(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8]);
                }
                if (_State != null)
                {
                    _State.memberType = rdgIncludeType.SelectedIndex == 0 ? "Lead" : "Contact";
                    _State.memberList = dt;
                    _Context.SetContext("AddManageEMFilterStateInfo", _State);
                }
            }
        }
        return dt;
    }

    /// <summary>
    /// Builds the column collection for the data grid layout.
    /// </summary>
    /// <returns></returns>
    private static DataTable GetDataGridLayout()
    {
        DataTable dt = new DataTable();
        try
        {
            dt.Columns.Add("EntityId");
            dt.Columns.Add("FirstName");
            dt.Columns.Add("LastName");
            dt.Columns.Add("Company");
            dt.Columns.Add("Email");
            dt.Columns.Add("City");
            dt.Columns.Add("State");
            dt.Columns.Add("Zip");
            dt.Columns.Add("WorkPhone");
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
            throw;
        }
        return dt;
    }

    /// <summary>
    /// Gets a list of the Lead members.
    /// </summary>
    /// <returns></returns>
    private IList GetLeadMembers()
    {
        IList leadList;
        try
        {
            IQueryable query = (IQueryable)EntityFactory.GetRepository<ILead>();
            IExpressionFactory expressions = query.GetExpressionFactory();
            IProjections projections = query.GetProjectionsFactory();
            Sage.Platform.Repository.ICriteria criteria = query.CreateCriteria("a1")
                .CreateCriteria("Addresses", "address")
                    .SetProjection(projections.ProjectionList()
                        .Add(projections.Distinct(projections.Property("Id")))
                        .Add(projections.Property("FirstName"))
                        .Add(projections.Property("LastName"))
                        .Add(projections.Property("Company"))
                        .Add(projections.Property("Email"))
                        .Add(projections.Property("address.City"))
                        .Add(projections.Property("address.State"))
                        .Add(projections.Property("address.PostalCode"))
                        .Add(projections.Property("WorkPhone"))
                        );
            AddExpressionsCriteria(criteria, expressions);
            leadList = criteria.List();
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
            throw;
        }
        return leadList;
    }

    /// <summary>
    /// Gets the contact members via a join through Account.
    /// </summary>
    /// <returns></returns>
    private IList GetAccountMembers()
    {
        IList contactList;
        try
        {
            IQueryable query = (IQueryable)EntityFactory.GetRepository<IContact>();
            IExpressionFactory expressions = query.GetExpressionFactory();
            IProjections projections = query.GetProjectionsFactory();
            Sage.Platform.Repository.ICriteria criteria = query.CreateCriteria("a1")
                .CreateCriteria("Account", "account")
                .CreateCriteria("Addresses", "address")
                    .SetProjection(projections.ProjectionList()
                        .Add(projections.Distinct(projections.Property("Id")))
                        .Add(projections.Property("FirstName"))
                        .Add(projections.Property("LastName"))
                        .Add(projections.Property("Account"))
                        .Add(projections.Property("Email"))
                        .Add(projections.Property("address.City"))
                        .Add(projections.Property("address.State"))
                        .Add(projections.Property("address.PostalCode"))
                        .Add(projections.Property("WorkPhone"))
                        );
            AddExpressionsCriteria(criteria, expressions);
            contactList = criteria.List();
        }
        catch (NHibernate.QueryException nex)
        {
            log.Error(nex.Message);
            string message = GetLocalResourceObject("QueryError").ToString();
            if (nex.Message.Contains("could not resolve property"))
                message += "  " + GetLocalResourceObject("QueryErrorInvalidParameter");

            throw new ValidationException(message);
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
            throw;
        }
        return contactList;
    }

    /// <summary>
    /// Gets the contact members.
    /// </summary>
    /// <returns></returns>
    private IList GetContactMembers()
    {
        IList contactList;
        try
        {
            IQueryable query = (IQueryable)EntityFactory.GetRepository<IContact>();
            IExpressionFactory expressions = query.GetExpressionFactory();
            IProjections projections = query.GetProjectionsFactory();
            Sage.Platform.Repository.ICriteria criteria = query.CreateCriteria("a1")
                .CreateCriteria("Account", "account")
                .CreateCriteria("a1.Addresses", "address")
                    .SetProjection(projections.ProjectionList()
                        .Add(projections.Distinct(projections.Property("Id")))
                        .Add(projections.Property("FirstName"))
                        .Add(projections.Property("LastName"))
                        .Add(projections.Property("Account"))
                        .Add(projections.Property("Email"))
                        .Add(projections.Property("address.City"))
                        .Add(projections.Property("address.State"))
                        .Add(projections.Property("address.PostalCode"))
                        .Add(projections.Property("WorkPhone"))
                        );
            AddExpressionsCriteria(criteria, expressions);
            contactList = criteria.List();
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
            throw;
        }
        return contactList;
    }

    /// <summary>
    /// Adds the expressions criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="expressions">The expressions.</param>
    /// <returns></returns>
    private void AddExpressionsCriteria(Sage.Platform.Repository.ICriteria criteria, IExpressionFactory expressions)
    {
        if (criteria != null)
        {
            SearchParameter clause;
            Boolean isLeads = (rdgIncludeType.SelectedIndex == 0);
            Boolean isPrimaryContact = (rdgIncludeType.SelectedIndex == 1);
            Boolean isIndividual = (rdgIncludeType.SelectedIndex == 3);

            criteria.Add(expressions.Eq("address.IsPrimary", true));
            if (isPrimaryContact)
            {
                criteria.Add(expressions.Eq("a1.IsPrimary", true));
            }
            if (chkCompany.Checked)
            {
                clause = (SearchParameter)lbxCompany.SelectedIndex;
                if (isLeads)
                    criteria.Add(GetExpression(expressions, clause, "a1.Company", txtCompany.Text));
                else
                    criteria.Add(GetExpression(expressions, clause, "account.AccountName", txtCompany.Text));
            }
            if (chkIndustry.Checked)
            {
                clause = (SearchParameter)lbxIndustry.SelectedIndex;
                if (isLeads)
                    criteria.Add(GetExpression(expressions, clause, "a1.Industry", pklIndustry.PickListValue));
                else
                    criteria.Add(GetExpression(expressions, clause, "account.Industry", pklIndustry.PickListValue));
            }
            if (chkSIC.Checked)
            {
                clause = (SearchParameter)lbxSIC.SelectedIndex;
                if (isLeads)
                    criteria.Add(GetExpression(expressions, clause, "a1.SICCode", txtSIC.Text));
                else
                    criteria.Add(GetExpression(expressions, clause, "account.SicCode", txtSIC.Text));
            }
            if (chkTitle.Checked)
            {
                clause = (SearchParameter)lbxTitle.SelectedIndex;
                criteria.Add(GetExpression(expressions, clause, "a1.Title", pklTitle.PickListValue));
            }
            if (chkProducts.Checked && !isLeads)
            {
                criteria.CreateCriteria("account.AccountProducts", "product");
                clause = (SearchParameter)lbxProducts.SelectedIndex;
                criteria.Add(GetExpression(expressions, clause, "product.ProductName", lueProducts.Text));
            }
            if (chkStatus.Checked)
            {
                clause = (SearchParameter)lbxStatus.SelectedIndex;
                if (isLeads || isIndividual)
                    criteria.Add(GetExpression(expressions, clause, "a1.Status", pklStatus.PickListValue));
                else
                    criteria.Add(GetExpression(expressions, clause, "account.Status", pklStatus.PickListValue));
            }
            if (!chkSolicit.Checked)
                criteria.Add(expressions.Or(expressions.Eq("a1.DoNotSolicit", false), expressions.IsNull("a1.DoNotSolicit")));
            if (!chkEmail.Checked)
                criteria.Add(expressions.Or(expressions.Eq("a1.DoNotEmail", false), expressions.IsNull("a1.DoNotEmail")));
            if (!chkCall.Checked)
                criteria.Add(expressions.Or(expressions.Eq("a1.DoNotPhone", false), expressions.IsNull("a1.DoNotPhone")));
            if (!chkMail.Checked)
                criteria.Add(expressions.Or(expressions.Eq("a1.DoNotMail", false), expressions.IsNull("a1.DoNotMail")));
            if (!chkFax.Checked)
            {
                if (isLeads)
                    criteria.Add(expressions.Or(expressions.Eq("a1.DoNotFAX", false), expressions.IsNull("a1.DoNotFAX")));
                else
                    criteria.Add(expressions.Or(expressions.Eq("a1.DoNotFax", false), expressions.IsNull("a1.DoNotFax")));
            }
            if (chkCity.Checked)
            {
                clause = (SearchParameter)lbxCity.SelectedIndex;
                AddCommaDelimitedStringsToExpression(criteria, expressions, txtCity.Text, "address.City", clause);
            }
            if (chkState.Checked)
            {
                clause = (SearchParameter)lbxState.SelectedIndex;
                AddCommaDelimitedStringsToExpression(criteria, expressions, txtState.Text, "address.State", clause);
            }
            if (chkZip.Checked)
            {
                clause = (SearchParameter)lbxZip.SelectedIndex;
                AddCommaDelimitedStringsToExpression(criteria, expressions, txtZip.Text, "address.PostalCode", clause);
            }
            if (chkLeadSource.Checked)
            {
                switch (rdgIncludeType.SelectedIndex)
                {
                    case 0:
                        criteria.CreateCriteria("a1.LeadSource", "leadsource");
                        break;
                    case 3:
                        criteria.CreateCriteria("a1.LeadSources", "leadsource");
                        break;
                    default:
                        criteria.CreateCriteria("account.LeadSource", "leadsource");
                        break;
                }
                clause = (SearchParameter)lbxLeadSource.SelectedIndex;
                criteria.Add(GetExpression(expressions, clause, "leadsource.Description", lueLeadSource.Text));
            }
            if (chkImportSource.Checked)
            {
                clause = (SearchParameter)lbxImportSource.SelectedIndex;
                if (isLeads || isIndividual)
                    criteria.Add(GetExpression(expressions, clause, "a1.ImportSource", pklImportSource.PickListValue));
                else
                    criteria.Add(GetExpression(expressions, clause, "account.ImportSource", pklImportSource.PickListValue));
            }
            if (!string.IsNullOrEmpty(dtpCreateFromDate.Text))
            {
                if (chkCreateDate.Checked && (isLeads || isIndividual))
                    criteria.Add(expressions.Between("a1.CreateDate", CheckForNullDate(dtpCreateFromDate.DateTimeValue), CheckForNullDate(dtpCreateToDate.DateTimeValue)));
                else if (chkCreateDate.Checked)
                    criteria.Add(expressions.Between("account.CreateDate", CheckForNullDate(dtpCreateFromDate.DateTimeValue), CheckForNullDate(dtpCreateToDate.DateTimeValue)));
            }
        }
        return;
    }

    /// <summary>
    /// Gets the expression.
    /// </summary>
    /// <param name="ef">The ef.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="propName">Name of the prop.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    private static IExpression GetExpression(IExpressionFactory ef, SearchParameter expression, string propName, string value)
    {
        switch (expression)
        {
            case SearchParameter.StartingWith:
                return ef.InsensitiveLike(propName, value, LikeMatchMode.BeginsWith);
            case SearchParameter.Contains:
                return ef.InsensitiveLike(propName, value, LikeMatchMode.Contains);
            case SearchParameter.EqualOrGreaterThan:
                return ef.Ge(propName, value);
            case SearchParameter.EqualOrLessThan:
                return ef.Le(propName, value);
            case SearchParameter.EqualTo:
                return ef.Eq(propName, value);
            case SearchParameter.GreaterThan:
                return ef.Gt(propName, value);
            case SearchParameter.LessThan:
                return ef.Lt(propName, value);
            case SearchParameter.NotEqualTo:
                return ef.InsensitiveNe(propName, value);
        }
        return null;
    }

    /// <summary>
    /// Adds the comma delimited strings to the expression factory.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="expressions">The expressions.</param>
    /// <param name="text">The text.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="clause">The clause.</param>
    private static void AddCommaDelimitedStringsToExpression(Sage.Platform.Repository.ICriteria criteria, IExpressionFactory expressions, String text,
        String propertyName, SearchParameter clause)
    {
        if (!string.IsNullOrEmpty(text))
        {
            IList<IExpression> expression = new List<IExpression>();
            IJunction junction;
            string[] values = text.Split(',');
            foreach (String value in values)
            {
                expression.Add(GetExpression(expressions, clause, propertyName, value));
            }
            junction = expressions.Disjunction();
            foreach (IExpression e in expression)
            {
                junction.Add(e);
            }
            criteria.Add(junction);
        }
        return;
    }

    /// <summary>
    /// Handles the OnClick event of the HowMany control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void HowMany_OnClick(object sender, EventArgs e)
    {
        IList members = null;
        int count = 0;
        switch (rdgIncludeType.SelectedIndex)
        {
            case 0:
                members = GetLeadMembers();
                break;
            case 1:
                members = GetAccountMembers();
                break;
            case 2:
                members = GetAccountMembers();
                break;
            case 3:
                members = GetContactMembers();
                break;
        }
        if (members != null)
            count = members.Count;
        lblHowMany.Text = String.Format(GetLocalResourceObject("HowManyMembers_Msg").ToString(), count);
    }

    /// <summary>
    /// Handles the OnClick event of the Search control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Search_OnClick(object sender, EventArgs e)
    {
        lblHowMany.Text = String.Empty;
        grdMembers.DataSource = GetMembers();
    }

    /// <summary>
    /// Handles the OnClick event of the AddFromGroup control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void AddFromGroup_OnClick(object sender, EventArgs e)
    {
        string groupId = String.Empty;
        string entityName = String.Empty;
        if (rdgAddFromGroup.SelectedIndex == 0)
        {
            entityName = "Lead";
            groupId = lbxLeadGroups.SelectedValue;
        }
        else
        {
            entityName = "Contact";
            groupId = lbxContactGroups.SelectedValue;
        }

        if (!String.IsNullOrEmpty(groupId))
        {
			DataTable targetList = Helpers.GetEntityGroupList(entityName, groupId);
            if (_State != null)
            {
                _State.memberList = targetList;
                _State.memberType = entityName;
                _State.groupName = lbxContactGroups.SelectedItem.ToString();
                _Context.SetContext("AddManageEMFilterStateInfo", _State);
            }
        }
        else
        {
            throw new ValidationException(GetLocalResourceObject("error_NoGroupSelected").ToString());
        }
    }

    /// <summary>
    /// Handles the changing event of the grdMemberspage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="GridViewPageEventArgs"/> instance containing the event data.</param>
    protected void grdMemberspage_changing(object sender, GridViewPageEventArgs e)
    {
    }

    /// <summary>
    /// Handles the Sorting event of the grdMembers control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="GridViewSortEventArgs"/> instance containing the event data.</param>
    protected void grdMembers_Sorting(object sender, GridViewSortEventArgs e)
    {
    }

    /// <summary>
    /// Checks for null date and returns current date/time if null.
    /// </summary>
    /// <param name="dateTime">The date time value.</param>
    /// <returns></returns>
    private DateTime? CheckForNullDate(DateTime? dateTime)
    {
        if (dateTime == null)
            dateTime = DateTime.UtcNow;
        return dateTime;
    }

    /// <summary>
    /// Sets the filter controls.
    /// </summary>
    /// <param name="options">The options.</param>
    private void SetFilterControls(FilterOptions options)
    {
        chkCompany.Checked = options.CompanyEnabled;
        SetListBox(lbxCompany, options.CompanyOperator);
        txtCompany.Text = options.CompanyValue;

        chkTitle.Checked = options.TitleEnabled;
        SetListBox(lbxTitle, options.TitleOperator);
        pklTitle.PickListValue = options.TitleValue;

        chkIndustry.Checked = options.IndustryEnabled;
        SetListBox(lbxIndustry, options.IndustryOperator);
        pklIndustry.PickListValue = options.IndustryValue;

        chkSIC.Checked = options.SICEnabled;
        SetListBox(lbxSIC, options.SICOperator);
        txtSIC.Text = options.SICValue;

        chkProducts.Checked = options.ProdOwnedEnabled;
        SetListBox(lbxProducts, options.ProdOwnedOperator);
        lueProducts.Text = options.ProdOwnedValue;

        chkLeadSource.Checked = options.LeadSourceEnabled;
        SetListBox(lbxLeadSource, options.LeadSourceOperator);
        lueLeadSource.Text = options.LeadSourceValue;

        chkStatus.Checked = options.StatusEnabled;
        SetListBox(lbxStatus, options.StatusOperator);
        pklStatus.PickListValue = options.StatusValue;

        chkState.Checked = options.StateEnabled;
        SetListBox(lbxState, options.StateOperator);
        txtState.Text = options.StateValue;

        chkZip.Checked = options.PostalCodeEnabled;
        SetListBox(lbxZip, options.PostalCodeOperator);
        txtZip.Text = options.PostalCodeValue;

        chkCity.Checked = options.CityEnabled;
        SetListBox(lbxCity, options.CityOperator);
        txtCity.Text = options.CityValue;

        chkImportSource.Checked = options.ImportSourceEnabled;
        SetListBox(lbxImportSource, options.ImportSourceOperator);
        pklImportSource.PickListValue = options.ImportSourceValue;

        chkMail.Checked = options.IncludeDoNotMail;
        chkEmail.Checked = options.IncludeDoNotEmail;
        chkCall.Checked = options.IncludeDoNotPhone;
        chkFax.Checked = options.IncludeDoNotFax;
        chkSolicit.Checked = options.IncludeDoNotSolicit;

        rdgIncludeType.SelectedIndex = (int)options.IncludeType;

        chkCreateDate.Checked = options.CreateDateEnabled = chkCreateDate.Checked;
        dtpCreateFromDate.DateTimeValue = options.CreateDateFromValue;
        dtpCreateToDate.DateTimeValue = options.CreateDateToValue;
    }

    /// <summary>
    /// Sets the filter options.
    /// </summary>
    /// <param name="options">The options.</param>
    private void SetFilterOptions(FilterOptions options)
    {
        options.CompanyEnabled = chkCompany.Checked;
        options.CompanyOperator = (FilterOperator)lbxCompany.SelectedIndex;
        options.CompanyValue = txtCompany.Text;

        options.TitleEnabled = chkTitle.Checked;
        options.TitleOperator = (FilterOperator)lbxTitle.SelectedIndex;
        options.TitleValue = pklTitle.PickListValue;

        options.IndustryEnabled = chkIndustry.Checked;
        options.IndustryOperator = (FilterOperator)lbxIndustry.SelectedIndex;
        options.IndustryValue = pklIndustry.PickListValue;

        options.SICEnabled = chkSIC.Checked;
        options.SICOperator = (FilterOperator)lbxSIC.SelectedIndex;
        options.SICValue = txtSIC.Text;

        options.ProdOwnedEnabled = chkProducts.Checked;
        options.ProdOwnedOperator = (FilterOperator)lbxProducts.SelectedIndex;
        string prodOwnerID = lueProducts.ClientID + "_LookupText";
        string prodOwner = Request.Form[prodOwnerID.Replace("_", "$")];
        options.ProdOwnedValue = prodOwner;

        options.LeadSourceEnabled = chkLeadSource.Checked;
        options.LeadSourceOperator = (FilterOperator)lbxLeadSource.SelectedIndex;
        string leadSourceID = lueLeadSource.ClientID + "_LookupText";
        string leadSource = Request.Form[leadSourceID.Replace("_", "$")];
        options.LeadSourceValue = leadSource;

        options.StatusEnabled = chkStatus.Checked;
        options.StatusOperator = (FilterOperator)lbxStatus.SelectedIndex;
        options.StatusValue = pklStatus.PickListValue;

        options.StateEnabled = chkState.Checked;
        options.StateOperator = (FilterOperator)lbxState.SelectedIndex;
        options.StateValue = txtState.Text;

        options.PostalCodeEnabled = chkZip.Checked;
        options.PostalCodeOperator = (FilterOperator)lbxZip.SelectedIndex;
        options.PostalCodeValue = txtZip.Text;

        options.CityEnabled = chkCity.Checked;
        options.CityOperator = (FilterOperator)lbxCity.SelectedIndex;
        options.CityValue = txtCity.Text;

        options.ImportSourceEnabled = chkImportSource.Checked;
        options.ImportSourceOperator = (FilterOperator)lbxImportSource.SelectedIndex;
        options.ImportSourceValue = pklImportSource.PickListValue;

        options.IncludeDoNotMail = chkMail.Checked;
        options.IncludeDoNotEmail = chkEmail.Checked;
        options.IncludeDoNotPhone = chkCall.Checked;
        options.IncludeDoNotFax = chkFax.Checked;
        options.IncludeDoNotSolicit = chkSolicit.Checked;
        options.IncludeType = (FilterIncludeType)rdgIncludeType.SelectedIndex;
        options.CreateDateEnabled = chkCreateDate.Checked;
        options.CreateDateFromValue = dtpCreateFromDate.DateTimeValue;
        options.CreateDateToValue = dtpCreateToDate.DateTimeValue;
    }

    /// <summary>
    /// Sets the list box.
    /// </summary>
    /// <param name="lbx">The LBX.</param>
    /// <param name="filterOperator">The filter operator.</param>
    private void SetListBox(ListBox lbx, FilterOperator filterOperator)
    {
        lbx.SelectedIndex = (int)filterOperator;
    }

    /// <summary>
    /// Handles the OnClick event of the AddMembers control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
    protected void AddMembers_OnClick(object sender, EventArgs e)
    {
        if (_State != null)
        {
            if (_State.memberList.Rows.Count > 0)
            {
                InsertMemberManager insertManager = new InsertMemberManager();
                insertManager.AddressBookId = EntityContext.EntityID.ToString();
                insertManager.MemberList = _State.memberList;
                insertManager.MemberType = _State.memberType;
                insertManager.GroupName = _State.groupName;

                int numDuplicateEmailAddresses;
                int numBlankEmailAddresses;
                insertManager.StartMemberInsertProcess(out numDuplicateEmailAddresses, out numBlankEmailAddresses);
                DialogService.CloseEventHappened(sender, e);
                Refresh();
                IEMAddressBook addrBook = (IEMAddressBook)GetParentEntity();

                // Reload the page so that the grid check boxes get re-initialised
                string url = "EMAddressBook.aspx?entityid=" + addrBook.Id.ToString();
                if (numDuplicateEmailAddresses == 0)
                {
                    Response.Redirect(url);
                }
                else
                {
                    string message = string.Format("The Email Address Book Members you were creating contained the following errors:<br><ul style=\"margin-left: 15px;\"><li>{0} with blank/null email address</li><li>{1} with the same email address as another Member of this Address Book</li></ul><br>Those Contacts/Leads have not been added to the address book.", numBlankEmailAddresses, numDuplicateEmailAddresses);
                    string js = @"
function reloadAfterDialog() {
  window.location.href = '" + url + @"';
}
showDialog('Email Address Errors', '" + message + "', '600px', 'warningIcon', reloadAfterDialog);";
                    ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, js, true);
                }
            }
            else
            {
                DialogService.ShowMessage(GetLocalResourceObject("error_NoMembersSelected").ToString());
            }
        }
        else
        {
            DialogService.ShowMessage(GetLocalResourceObject("error_NoMembersSelected").ToString());
        }
    }    
	
    #endregion
}

internal class InsertMemberManager
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private string _addressBookId;
    private string _memberType;
    private string _groupName;
    private DataTable _memberList;
    public string AddressBookId
    {
        get
        {
            return this._addressBookId;
        }
        set
        {
            this._addressBookId = value;
        }
    }
    public string MemberType
    {
        get
        {
            return this._memberType;
        }
        set
        {
            this._memberType = value;
        }
    }
    public DataTable MemberList
    {
        get
        {
            return this._memberList;
        }
        set
        {
            this._memberList = value;
        }
    }
    public string GroupName
    {
        get
        {
            return this._groupName;
        }
        set
        {
            this._groupName = value;
        }
    }

    public void StartMemberInsertProcess(out int numDuplicateEmailAddresses, out int numBlankEmailAddresses)
    {
        IEMAddressBook addrBook = EntityFactory.GetById<IEMAddressBook>(this.AddressBookId);
        numDuplicateEmailAddresses = 0;
        numBlankEmailAddresses = 0;
        if (addrBook != null)
        {
            HashSet<string> memberIds;
            HashSet<string> memberEmailAddresses;
            InsertMemberManager.GetAddressBookMemberList(addrBook.Id.ToString(), this.MemberType, out memberIds, out memberEmailAddresses);
            IEnumerator enumerator = this.MemberList.Rows.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    DataRow dataRow = (DataRow)enumerator.Current;
                    try
                    {
                        string entityId = dataRow["EntityId"].ToString();
                        if (!string.IsNullOrEmpty(entityId))
                        {
                            if (!memberIds.Contains(entityId))
                            {
                                string emailAddress;
								string emailColumnKey = "Email";
                                if (dataRow[emailColumnKey] == DBNull.Value)
                                {
                                    emailAddress = null;
                                }
                                else
                                {
                                    emailAddress = (string)dataRow[emailColumnKey];
                                }

                                if (string.IsNullOrEmpty(emailAddress))
                                {
                                    numBlankEmailAddresses++;
                                }
                                else if (memberEmailAddresses.Contains(emailAddress))
                                {
                                    numDuplicateEmailAddresses++;
                                }
                                else
                                {
                                    this.AddAddressBookMember(addrBook, entityId);
                                    memberIds.Add(entityId);
                                    memberEmailAddresses.Add(emailAddress);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        InsertMemberManager.log.Error(exception.Message);
                        throw new ValidationException(exception.Message);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            addrBook.Save();
        }
    }

    private void AddAddressBookMember(IEMAddressBook addrBook, string entityId)
    {
        IEMAddressBookMember addrBookMember = EntityFactory.Create<IEMAddressBookMember>();
        if (this.MemberType == "Lead")
        {
            addrBookMember.SlxLeadID = entityId;
        }
        else if (this.MemberType == "Contact")
        {
            addrBookMember.SlxContactID = entityId;
        }
        addrBookMember.SlxMemberType = this.MemberType;
        addrBookMember.EMAddressBook = addrBook;
        addrBook.EMAddressBookMembers.Add(addrBookMember);
    }
    //private static IList GetAddressBookMemberList(string addrBookId, string memberType)
    private static void GetAddressBookMemberList(string addrBookId, string memberType, out HashSet<string> memberIds, out HashSet<string> memberEmailAddresses)
    {
        IList result;
        string select = string.Empty;
        string join = string.Empty;
        try
        {
            ISession session = SessionFactoryHolder.HolderInstance.CreateSession();
            try
            {
                if (memberType == "Lead")
                {
                    select = "Select abm.SlxLeadID, abm.Lead.Email ";
                }
                else if (memberType == "Contact")
                {
                    select = "Select abm.SlxContactID, abm.Contact.Email ";
                }
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(select).Append("From EMAddressBookMember abm ").Append("Where abm.EMAddressBook.Id = :addrBookId");
                IQuery query = session.CreateQuery(stringBuilder.ToString());
                query.SetAnsiString("addrBookId", addrBookId);
                result = query.List();
                
                memberIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                memberEmailAddresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (object[] row in result)
                {
                    memberIds.Add((string)row[0]);
                    memberEmailAddresses.Add((string)row[1]);
                }

                return;
            }
            finally
            {
                SessionFactoryHolder.HolderInstance.ReleaseSession(session);
            }
        }
        catch (Exception exception)
        {
            InsertMemberManager.log.Error(exception.Message);
            memberIds = null;
            memberEmailAddresses = null;
            return;
        }
    }
}

// Selectively taken from v753 Sage.SalesLogix.BusinessRules.dll
internal class Helpers
{
	private static string BuildSelectSQL(string entityName, GroupInfo groupInfo)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("SELECT ");
		if (entityName.ToUpper().Equals("CONTACT"))
		{
			if (groupInfo.UseAliases)
			{
				Helpers.BuildContactSelectSQLWithAliases(stringBuilder, groupInfo);
			}
			else
			{
				Helpers.BuildContactSelectSQLWithoutAliases(stringBuilder, groupInfo);
			}
			stringBuilder.Append(", ");
			if (groupInfo.UseAliases)
			{
				Helpers.BuildAddressSelectSQLWithAliases(stringBuilder, groupInfo, "ADDRESS (..)");
			}
			else
			{
				Helpers.BuildAddressSelectSQLWithoutAliases(stringBuilder, groupInfo);
			}
		}
		else
		{
			if (!entityName.ToUpper().Equals("LEAD"))
			{
				throw new ValidationException(string.Format("Invalid Entity ({0})", entityName.ToUpper()));
			}
			if (groupInfo.UseAliases)
			{
				Helpers.BuildLeadSelectSQLWithAliases(stringBuilder, groupInfo);
			}
			else
			{
				Helpers.BuildLeadSelectSQLWithoutAliases(stringBuilder, groupInfo);
			}
			stringBuilder.Append(", ");
			if (groupInfo.UseAliases)
			{
				Helpers.BuildAddressSelectSQLWithAliases(stringBuilder, groupInfo, "LEAD_ADDRESS (..)");
			}
			else
			{
				Helpers.BuildAddressSelectSQLWithoutAliases(stringBuilder, groupInfo);
			}
		}
		return stringBuilder.ToString();
	}
	private static void BuildLeadSelectSQLWithoutAliases(StringBuilder selectSQL, GroupInfo groupInfo)
	{
		if (Regex.IsMatch(groupInfo.FromSQL, "[ ]*LEAD[\\.]?[ ]*"))
		{
			selectSQL.Append("FIRSTNAME, LASTNAME, COMPANY As Company, EMAIL, WORKPHONE, LEADID As EntityId");
			return;
		}
		selectSQL.Append("null as FIRSTNAME, null as LASTNAME, null as Company, null as EMAIL, null as WORKPHONE, null as EntityId");
	}
	private static void BuildLeadSelectSQLWithAliases(StringBuilder selectSQL, GroupInfo groupInfo)
	{
		string value = Regex.Match(groupInfo.FromSQL, "LEAD (..)").Groups[1].Value;
		if (value.Length > 0)
		{
			selectSQL.AppendFormat("{0}.FIRSTNAME, {0}.LASTNAME, {0}.COMPANY As Company, {0}.EMAIL, {0}.WORKPHONE, {0}.LEADID As EntityId", value);
			return;
		}
		selectSQL.Append("null as FIRSTNAME, null as LASTNAME, null as Company, null as EMAIL, null as WORKPHONE, null as EntityId");
	}
	private static void BuildAddressSelectSQLWithoutAliases(StringBuilder selectSQL, GroupInfo groupInfo)
	{
		if (Regex.IsMatch(groupInfo.FromSQL, "[ ]*ADDRESS[\\.]?[ ]*"))
		{
			selectSQL.Append("CITY, STATE, POSTALCODE As Zip");
			return;
		}
		selectSQL.Append("null as CITY, null as STATE, null as Zip");
	}
	private static void BuildAddressSelectSQLWithAliases(StringBuilder selectSQL, GroupInfo groupInfo, string aliasRegExpression)
	{
		string value = Regex.Match(groupInfo.FromSQL, aliasRegExpression).Groups[1].Value;
		if (value.Length > 0)
		{
			selectSQL.AppendFormat("{0}.CITY, {0}.STATE, {0}.POSTALCODE As Zip", value);
			return;
		}
		selectSQL.Append("null as CITY, null as STATE, null as Zip");
	}
	private static void BuildContactSelectSQLWithoutAliases(StringBuilder selectSQL, GroupInfo groupInfo)
	{
		if (Regex.IsMatch(groupInfo.FromSQL, "[ ]*CONTACT[\\.]?[ ]*"))
		{
			selectSQL.Append("FIRSTNAME, LASTNAME, ACCOUNT As Company, EMAIL, WORKPHONE, CONTACTID As EntityId");
			return;
		}
		selectSQL.Append("null as FIRSTNAME, null as LASTNAME, null as Company, null as EMAIL, null as WORKPHONE, null as EntityId");
	}
	private static void BuildContactSelectSQLWithAliases(StringBuilder selectSQL, GroupInfo groupInfo)
	{
		string value = Regex.Match(groupInfo.FromSQL, "CONTACT (..)").Groups[1].Value;
		if (value.Length > 0)
		{
			selectSQL.AppendFormat("{0}.FIRSTNAME, {0}.LASTNAME, {0}.ACCOUNT As Company, {0}.EMAIL, {0}.WORKPHONE, {0}.CONTACTID As EntityId", value);
			return;
		}
		selectSQL.Append("null as FIRSTNAME, null as LASTNAME, null as Company, null as EMAIL, null as WORKPHONE, null as EntityId");
	}
	public static DataTable GetEntityGroupList(string entityName, string groupId)
	{
		DataTable result = new DataTable();
		if (!string.IsNullOrEmpty(entityName) && !string.IsNullOrEmpty(groupId))
		{
			GroupInfo groupInfo = new GroupInfo
			{
				UseAliases = false,
				GroupID = groupId
			};
			string text = groupInfo.WhereSQL;
			if (!string.IsNullOrEmpty(text))
			{
				text = "WHERE " + text;
			}
			string arg = Helpers.BuildSelectSQL(entityName, groupInfo);
			string commandText = string.Format("{0} FROM {1} {2}", arg, groupInfo.FromSQL, text);
			IDataService dataService = ApplicationContext.Current.Services.Get<IDataService>();
			using (IDbConnection openConnection = dataService.GetOpenConnection())
			{
				IDbDataAdapter dbDataAdapter = dataService.GetDbProviderFactory().CreateDataAdapter(commandText, openConnection);
				foreach (DbParameter current in groupInfo.Parameters)
				{
					dbDataAdapter.SelectCommand.Parameters.Add(current);
				}
				DataSet dataSet = new DataSet();
				try
				{
					using (new SparseQueryScope())
					{
						dbDataAdapter.Fill(dataSet);
					}
					if (dataSet.Tables.Count > 0)
					{
						result = dataSet.Tables[0];
					}
				}
				catch (Exception ex)
				{
					throw new ValidationException(ex.Message);
				}
			}
		}
		return result;
	}
	private static DateTime? ConvertDate(object dateTime)
	{
		if (dateTime == null)
		{
			return new DateTime?(DateTime.MinValue);
		}
		return new DateTime?(Convert.ToDateTime(dateTime));
	}
}