using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Sage.Entity.Interfaces;
using Sage.Platform;
using Sage.Platform.Application;
using Sage.Platform.Application.UI;
using Sage.Platform.Repository;
using Sage.Platform.WebPortal;
using Sage.Platform.WebPortal.Binding;
using Sage.Platform.WebPortal.Services;
using Sage.Platform.WebPortal.SmartParts;
using Sage.SalesLogix.CampaignTarget;
using Sage.SalesLogix.PickLists;
using System.Collections;
using System.Data;

/// <summary>
/// Summary description for EmailAddressBookMembers
/// </summary>
public partial class EmailAddressBookMembers : EntityBoundSmartPartInfoProvider
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
        get { return typeof(Sage.Entity.Interfaces.IEMAddressBook); }
    }

    private Sage.Platform.WebPortal.Binding.WebHqlListBindingSource _hqlBindingSource;
    public Sage.Platform.WebPortal.Binding.WebHqlListBindingSource HqlBindingSource
    {
        get
        {
            if (_hqlBindingSource == null)
            {
                System.Collections.Generic.List<Sage.Platform.WebPortal.Binding.HqlSelectField> sel = new System.Collections.Generic.List<Sage.Platform.WebPortal.Binding.HqlSelectField>();
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("h.id", "Id"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("c.AccountName", "ContactAccountName"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("c.DoNotEmail", "ContactDoNotEmail"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("c.DoNotSolicit", "ContactDoNotSolicit"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("c.Email", "ContactEmail"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("c.LastName", "ContactLastName"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("c.FirstName", "ContactFirstName"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("l.Company", "LeadCompany"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("l.DoNotEmail", "LeadDoNotEmail"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("l.DoNotSolicit", "LeadDoNotSolicit"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("l.Email", "LeadEmail"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("l.LastName", "LeadLastName"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("l.FirstName", "LeadFirstName"));
                sel.Add(new Sage.Platform.WebPortal.Binding.HqlSelectField("h.SlxMemberType", "SlxMemberType"));
                _hqlBindingSource = new Sage.Platform.WebPortal.Binding.WebHqlListBindingSource(sel, "EMAddressBookMember h");
            }
            return _hqlBindingSource;
        }
    }

    protected override void OnAddEntityBindings()
    {
    }


    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
    }

    protected override void OnFormBound()
    {
        btnDeleteAll.Attributes.Add("onclick", "javascript: return confirm('" + GetLocalResourceObject("btnDeleteAll.ConfirmDelete_Msg").ToString() + "');");
        ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, "$(document).ready(function () { flagSelected(); });", true);

        IEMAddressBook addrBook = (IEMAddressBook)this.BindingSource.Current;
        if (addrBook != null)
        {
            string entityId = string.Empty;
            if (Request.QueryString["entityid"] != null)
            {
                entityId = Request.QueryString["entityid"].ToString();
            }
            string addrId = addrBook.Id.ToString();

            if (entityId != addrId)
            {
                string groupId = string.Empty;
                if (Request.QueryString["gid"] != null)
                {
                    groupId = Request.QueryString["gid"].ToString();
                }
                if (!string.IsNullOrEmpty(groupId))
                {
                    Response.Redirect("EMAddressBook.aspx?entityid=" + addrId + "&gid=" + groupId);
                }
                else
                {
                    Response.Redirect("EMAddressBook.aspx?entityid=" + addrId);
                }
            }

            if (Session["ShowContacts"] != null)
            {
                chkContacts.Checked = (bool)Session["ShowContacts"];
            }
            if (Session["ShowLeads"] != null)
            {
                chkLeads.Checked = (bool)Session["ShowLeads"];
            }
            if (Session["DoNotSolicit"] != null)
            {
                ddlDoNotSolicit.SelectedValue = (string)Session["DoNotSolicit"];
            }
            if (Session["DoNotEmail"] != null)
            {
                ddlDoNotEmail.SelectedValue = (string)Session["DoNotEmail"];
            }
        }

        BindMembersGrid(addrBook);
        ToggleFilterImage();

        
        if (_roleSecurityService != null)
        {
            if (!_roleSecurityService.HasAccess("ENTITIES/EMADDRESSBOOKMEMBER/DELETE"))
            {
                btnDeleteAll.Visible = false;
                btnDeleteSelected.Visible = false;
                grdEMAddressBookMembers.Columns[0].Visible = false;
                btnClearSelection.Visible = false;
            }

            if (!_roleSecurityService.HasAccess("ENTITIES/EMADDRESSBOOKMEMBER/ADD"))
            {
                cmdManageList.Visible = false;
            }
        }
         
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

    protected void BindMembersGrid(IEMAddressBook addrBook)
    {
        HqlBindingSource.From += " left join h.Lead as l left join h.Contact as c";
        HqlBindingSource.Where = string.Format("h.Emaddressbookid = '{0}'", addrBook.Id.ToString());

        HqlBindingSource.Where += FilteredWhereClause();

        HqlBindingSource.BoundGrid = grdEMAddressBookMembers;
        grdEMAddressBookMembers.DataBind();
    }

    private string FilteredWhereClause()
    {
        string whereClause = string.Empty;

        if (chkContacts.Checked && !(chkLeads.Checked))         //Contacts Only
        {
            whereClause += " and h.SlxMemberType = 'Contact'";
        }
        else if (!(chkContacts.Checked) && chkLeads.Checked)    //Leads Only
        {
            whereClause += " and h.SlxMemberType = 'Lead'";
        }
        else if (!(chkContacts.Checked) && !(chkLeads.Checked)) //Neither Leads nor Contacts
        {
            whereClause += " and h.SlxMemberType = ''";
        }

        if (ddlDoNotSolicit.SelectedValue == "Yes")
        {
            whereClause += " and (h.Contact.DoNotSolicit = true or h.Lead.DoNotSolicit = true)";
        }
        else if (ddlDoNotSolicit.SelectedValue == "No")
        {
            whereClause += " and (h.Contact.DoNotSolicit != true or h.Lead.DoNotSolicit != true)";
        }

        if (ddlDoNotEmail.SelectedValue == "Yes")
        {
            whereClause += " and (h.Contact.DoNotEmail = true or h.Lead.DoNotEmail = true)";
        }
        else if (ddlDoNotEmail.SelectedValue == "No")
        {
            whereClause += " and (h.Contact.DoNotEmail != true or h.Lead.DoNotEmail != true)";
        }

        return whereClause;
    }

    protected void ToggleFilterImage()
    {
        if (chkContacts.Checked && chkLeads.Checked && ddlDoNotSolicit.SelectedIndex <= 0 && ddlDoNotEmail.SelectedIndex <= 0)
        {
            imgFilter.Visible = false;
        }
        else
        {
            imgFilter.Visible = true;
        }
    }

    protected void grdEMAddressBookMemberspage_changing(object sender, GridViewPageEventArgs e)
    {
        grdEMAddressBookMembers.PageIndex = e.NewPageIndex;
        grdEMAddressBookMembers.DataBind();
        lblDeleteIds.Text = "";
    }

    protected void btnRefresh_OnClick(object sender, EventArgs e)
    {
        Session.Add("ShowContacts", chkContacts.Checked);
        Session.Add("ShowLeads", chkLeads.Checked);
        Session.Add("DoNotSolicit", ddlDoNotSolicit.SelectedValue);
        Session.Add("DoNotEmail", ddlDoNotEmail.SelectedValue);

        IEMAddressBook addrBook = (IEMAddressBook)this.BindingSource.Current;
        if (addrBook != null)
        {
            string addrId = addrBook.Id.ToString();
            string groupId = string.Empty;
            if (Request.QueryString["gid"] != null)
            {
                groupId = Request.QueryString["gid"].ToString();
            }

            if (!string.IsNullOrEmpty(groupId))
            {
                Response.Redirect("EMAddressBook.aspx?entityid=" + addrId + "&gid=" + groupId);
            }
            else
            {
                Response.Redirect("EMAddressBook.aspx?entityid=" + addrId);
            }
        }
    }

    protected void btnDeleteAll_OnClick(object sender, EventArgs e)
    {
        IEMAddressBook thisAddrBook = (IEMAddressBook)this.BindingSource.Current;
        IList<IEMAddressBookMember> membersToDelete = GetFilteredMembersList();

        List<List<string>> idLists = new List<List<string>>();
        List<string> currentList = new List<string>();
        idLists.Add(currentList);
		
		// Generate a suitable batch of IDs to use
		// (otherwise they get generated on the fly, 10 at a time)
		var idGenerator = new Sage.SalesLogix.SalesLogixEntityKeyGenerator();
        var ids = idGenerator.GenerateIds(typeof(IEMDeletedItem), membersToDelete.Count);
        Queue<string> idQueue = new Queue<string>(ids);

		using (NHibernate.ISession parentSession = new Sage.Platform.Orm.SessionScopeWrapper())
        using (var session = parentSession.SessionFactory.OpenStatelessSession())
        {
            NHibernate.ITransaction tx = null;
            try
            {
                tx = session.BeginTransaction();
                foreach (IEMAddressBookMember member in membersToDelete)
                {
                    if (currentList.Count >= 2000)
                    {
                        currentList = new List<string>();
                        idLists.Add(currentList);

                        // Avoid building an excessively large transaction by flushing/commiting periodically
                        tx.Commit();
                        tx.Dispose();
                        tx = session.BeginTransaction();
                    }

                    currentList.Add(member.Id.ToString());

                    // Create a 'deleted item' record (using a pre-generated ID)
                    IEMDeletedItem deletedItem = member.BuildDeletedItemObject();
                    var itemForId = (Sage.Platform.ComponentModel.IAssignableId)deletedItem;
                    itemForId.Id = idQueue.Dequeue();
                    session.Insert(deletedItem);
                }

                foreach (List<string> idList in idLists)
                {
                    session.CreateQuery("delete from EMAddressBookMember m where m.Id IN (:idList)")
                        .SetParameterList("idList", idList)
                        .ExecuteUpdate();
                }

                tx.Commit();
            }
            finally
            {
                if (tx != null)
                {
                    tx.Dispose();
                }
            }
        }
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
        foreach (IEMAddressBookMember member in membersToDelete)
        {
            if (currentList.Count >= 2000)
            {
                currentList = new List<string>();
                idLists.Add(currentList);
            }

            currentList.Add(member.Id.ToString());
        }

        foreach (List<string> idList in idLists)
        {
            using (NHibernate.ISession session = new Sage.Platform.Orm.SessionScopeWrapper())
            {
                session.CreateQuery("delete from EMAddressBookMember m where m.Id IN (:idList)")
                    .SetParameterList("idList", idList)
                    .ExecuteUpdate();
            }
        }
    }

    protected void btnDeleteSelected_OnClick(object sender, EventArgs e)
    {
        if (lblDeleteIds.Text.Length == 0)
        {
			string js = "showDialog('No  Address Book Members Selected', 'You must select at least one Address Book Member', '400px', 'infoIcon');";
            ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, js, true);
        }
        else
        {
            string[] ids = lblDeleteIds.Text.Split(',');
            foreach (string id in ids)
            {
                if (id.Length > 0)
                {
                    IEMAddressBookMember memberToDelete = EntityFactory.GetById<IEMAddressBookMember>(id);
                    memberToDelete.Delete();
                }
            }

            string groupId = string.Empty;
            if (Request.QueryString["gid"] != null)
            {
                groupId = Request.QueryString["gid"].ToString();
            }
            IEMAddressBook addrBook = (IEMAddressBook)this.BindingSource.Current;
            string addrId = addrBook.Id.ToString();
            if (!string.IsNullOrEmpty(groupId))
            {
                Response.Redirect("EMAddressBook.aspx?entityid=" + addrId + "&gid=" + groupId);
            }
            else
            {
                Response.Redirect("EMAddressBook.aspx?entityid=" + addrId);
            }
        }
    }

    protected void ManageMembers_OnClick(object sender, EventArgs e)
    {
        IEMAddressBook addrBook = (IEMAddressBook)this.BindingSource.Current;
        addrBook.Save();
        if (DialogService != null)
        {
            DialogService.SetSpecs(200, 200, 650, 950, "AddAddressBookMembers", "", true);
            DialogService.ShowDialog();
        }
    }

    private IList<IEMAddressBookMember> GetFilteredMembersList()
    {
        IRepository<IEMAddressBookMember> rep = EntityFactory.GetRepository<IEMAddressBookMember>();
        IQueryable qry = (IQueryable)rep;
        IExpressionFactory ef = qry.GetExpressionFactory();

        ICriteria criteria = AddContactLeadFilterCriteria(qry, ef);

        IList<IEMAddressBookMember> members = criteria.List<IEMAddressBookMember>();
        members = ApplyDoNotFilterCriteria(members);
        return members;
    }

    protected ICriteria AddContactLeadFilterCriteria(IQueryable qry, IExpressionFactory ef)
    {
        IEMAddressBook addrBook = (IEMAddressBook)this.BindingSource.Current;
        ICriteria criteria = qry.CreateCriteria();

        IJunction andJunction = ef.Conjunction();
        andJunction.Add(ef.Eq("EMAddressBook.Id", addrBook.Id));

        if (chkContacts.Checked && !(chkLeads.Checked))         //Contacts Only
        {
            andJunction.Add(ef.Eq("SlxMemberType", "Contact"));
        }
        else if (!(chkContacts.Checked) && chkLeads.Checked)    //Leads Only
        {
            andJunction.Add(ef.Eq("SlxMemberType", "Lead"));
        }
        else if (!(chkContacts.Checked) && !(chkLeads.Checked)) //Neither Leads nor Contacts
        {
            andJunction.Add(ef.Eq("SlxMemberType", ""));
        }

        criteria.Add(andJunction);

        return criteria;
    }

    protected IList<IEMAddressBookMember> ApplyDoNotFilterCriteria(IList<IEMAddressBookMember> members)
    {
        List<IEMAddressBookMember> removeList = new List<IEMAddressBookMember>();
        foreach (IEMAddressBookMember member in members)
        {
            //Filter DoNotSolicits
            if (ddlDoNotSolicit.SelectedValue == "Yes")
            {
                if (member.Contact != null)
                {
                    if (member.Contact.DoNotSolicit.ToString() != "True")
                    {
                        removeList.Add(member);
                    }
                }

                if (member.Lead != null)
                {
                    if (member.Lead.DoNotSolicit.ToString() != "True")
                    {
                        removeList.Add(member);
                    }
                }
            }
            else if (ddlDoNotSolicit.SelectedValue == "No")
            {
                if (member.Contact != null)
                {
                    if (member.Contact.DoNotSolicit.ToString() == "True")
                    {
                        removeList.Add(member);
                    }
                }

                if (member.Lead != null)
                {
                    if (member.Lead.DoNotSolicit.ToString() == "True")
                    {
                        removeList.Add(member);
                    }
                }
            }

            //Filter DoNotEmails
            if (ddlDoNotEmail.SelectedValue == "Yes")
            {
                if (member.Contact != null)
                {
                    if (member.Contact.DoNotEmail.ToString() != "True")
                    {
                        removeList.Add(member);
                    }
                }

                if (member.Lead != null)
                {
                    if (member.Lead.DoNotEmail.ToString() != "True")
                    {
                        removeList.Add(member);
                    }
                }
            }
            else if (ddlDoNotEmail.SelectedValue == "No")
            {
                if (member.Contact != null)
                {
                    if (member.Contact.DoNotEmail.ToString() == "True")
                    {
                        removeList.Add(member);
                    }
                }

                if (member.Lead != null)
                {
                    if (member.Lead.DoNotEmail.ToString() == "True")
                    {
                        removeList.Add(member);
                    }
                }
            }
        }
        if (removeList.Count > 0)
        {
            foreach (IEMAddressBookMember remMember in removeList)
            {
                members.Remove(remMember);
            }
        }
        return members;
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