using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Sage.Platform.WebPortal.SmartParts;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Summary description for EmailAccountDataLabels
/// </summary>
public partial class EmailAccountDataLabels : EntityBoundSmartPartInfoProvider
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
        get { return typeof(Sage.Entity.Interfaces.IEMEmailAccount); }
    }

    private Sage.Platform.WebPortal.Binding.WebEntityListBindingSource _dsEMDataLabels;
    public Sage.Platform.WebPortal.Binding.WebEntityListBindingSource dsEMDataLabels
    {
        get
        {
            if (_dsEMDataLabels == null)
            {
                _dsEMDataLabels = new Sage.Platform.WebPortal.Binding.WebEntityListBindingSource(typeof(Sage.Entity.Interfaces.IEMDataLabel),
           EntityType, "EMDataLabels", System.Reflection.MemberTypes.Property);
                _dsEMDataLabels.UseSmartQuery = true;
            }
            return _dsEMDataLabels;
        }
    }

    void dsEMDataLabels_OnCurrentEntitySet(object sender, EventArgs e)
    {
        if (Visible)
        {
            if (BindingSource.Current != null)
            {


                dsEMDataLabels.SourceObject = BindingSource.Current;
            }
        }
        if (Visible)
        {
            RegisterBindingsWithClient(dsEMDataLabels);
        }
    }







    protected override void OnAddEntityBindings()
    {
        dsEMDataLabels.Bindings.Add(new Sage.Platform.WebPortal.Binding.WebEntityListBinding("EMDataLabels", grdEMDataLabels));
        dsEMDataLabels.BindFieldNames = new String[] { "Id", "Name", "DataType", "IsPrivate", "SyncWithEmailService" };

        BindingSource.OnCurrentEntitySet += new EventHandler(dsEMDataLabels_OnCurrentEntitySet);

    }

    protected void btnAdd_ClickAction(object sender, EventArgs e)
    {
        if (DialogService != null)
        {
            // InsertChildDialogActionItem
            DialogService.SetSpecs(300, 450, "AddEditDataLabel", GetLocalResourceObject("a674cce8-97d7-4cf9-8b20-6baff790929f.DialogTitleOverride").ToString());
            DialogService.EntityType = typeof(Sage.Entity.Interfaces.IEMDataLabel);
            DialogService.SetChildIsertInfo(
              typeof(Sage.Entity.Interfaces.IEMDataLabel),
              typeof(Sage.Entity.Interfaces.IEMEmailAccount),
              typeof(Sage.Entity.Interfaces.IEMDataLabel).GetProperty("EMEmailAccount"),
              typeof(Sage.Entity.Interfaces.IEMEmailAccount).GetProperty("EMDataLabels"));
            DialogService.ShowDialog();
        }
    }

    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
        btnAdd.Click += new ImageClickEventHandler(btnAdd_ClickAction);


    }

    protected override void OnFormBound()
    {
        grdEMDataLabels.Columns[0].Visible = (_roleSecurityService.HasAccess("Entities/EMEmailAccount/Edit"));
        grdEMDataLabels.Columns[1].Visible = (_roleSecurityService.HasAccess("Entities/EMEmailAccount/Delete"));
        if (dsEMDataLabels.SourceObject == null) { dsEMDataLabels.SourceObject = BindingSource.Current; }
        if (dsEMDataLabels.SourceObject == null) { RegisterBindingsWithClient(dsEMDataLabels); }
        dsEMDataLabels.Bind();

        if (!_roleSecurityService.HasAccess("Entities/EMEmailAccount/Add"))
        {
            btnAdd.Visible = false;
        }

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


    private int _grdEMDataLabelsdeleteColumnIndex = -2;
    protected int grdEMDataLabelsDeleteColumnIndex
    {
        get
        {
            if (_grdEMDataLabelsdeleteColumnIndex == -2)
            {
                int bias = (grdEMDataLabels.ExpandableRows) ? 1 : 0;
                _grdEMDataLabelsdeleteColumnIndex = -1;
                int colcount = 0;
                foreach (DataControlField col in grdEMDataLabels.Columns)
                {
                    ButtonField btn = col as ButtonField;
                    if (btn != null)
                    {
                        if (btn.CommandName == "Delete")
                        {
                            _grdEMDataLabelsdeleteColumnIndex = colcount + bias;
                            break;
                        }
                    }
                    colcount++;
                }
            }
            return _grdEMDataLabelsdeleteColumnIndex;
        }
    }

    protected void grdEMDataLabels_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((grdEMDataLabelsDeleteColumnIndex >= 0) && (grdEMDataLabelsDeleteColumnIndex < e.Row.Cells.Count))
            {
                TableCell cell = e.Row.Cells[grdEMDataLabelsDeleteColumnIndex];
                foreach (Control c in cell.Controls)
                {
                    LinkButton btn = c as LinkButton;
                    if (btn != null)
                    {
                        btn.Attributes.Add("onclick", "javascript: return confirm('" + Sage.Platform.WebPortal.PortalUtil.JavaScriptEncode(GetLocalResourceObject("grdEMDataLabels.4303f4e9-1885-4004-bb34-caf0fb76a42b.ConfirmationMessage").ToString()) + "');");
                        return;
                    }
                }
            }
        }
    }

    protected void grdEMDataLabelspage_changing(object sender, GridViewPageEventArgs e)
    {
        grdEMDataLabels.PageIndex = e.NewPageIndex;
        grdEMDataLabels.DataBind();
    }
    protected void grdEMDataLabels_Sorting(object sender, GridViewSortEventArgs e) { }
    protected void grdEMDataLabels_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Page")
            return;
        int rowIndex;
        if (Int32.TryParse(e.CommandArgument.ToString(), out rowIndex))
        {
            dsEMDataLabels.SelectedIndex = rowIndex;
            object currentEntity = dsEMDataLabels.Current;
            if ((currentEntity is Sage.Platform.ComponentModel.ComponentView) && !((Sage.Platform.ComponentModel.ComponentView)currentEntity).IsVirtualComponent)
                currentEntity = ((Sage.Platform.ComponentModel.ComponentView)currentEntity).Component;
            string id = String.Empty;
            //Check if this is an unpersisted entity and use its InstanceId
            if (Sage.Platform.WebPortal.PortalUtil.ObjectIsNewEntity(currentEntity))
            {
                if (grdEMDataLabels.DataKeys[0].Values.Count > 1)
                {
                    foreach (DictionaryEntry val in grdEMDataLabels.DataKeys[rowIndex].Values)
                    {
                        if (val.Key.ToString() == "InstanceId")
                        {
                            Guid instanceId = (Guid)val.Value;
                            dsEMDataLabels.SetCurrentEntityByInstanceId(instanceId);
                            id = instanceId.ToString();
                            currentEntity = dsEMDataLabels.Current;
                            if ((currentEntity is Sage.Platform.ComponentModel.ComponentView) && !((Sage.Platform.ComponentModel.ComponentView)currentEntity).IsVirtualComponent)
                                currentEntity = ((Sage.Platform.ComponentModel.ComponentView)currentEntity).Component;
                        }
                    }
                }
            }
            else
            {
                foreach (DictionaryEntry val in grdEMDataLabels.DataKeys[rowIndex].Values)
                {
                    if (val.Key.ToString() != "InstanceId")
                    {
                        id = val.Value.ToString();
                    }
                }
            }
            if (e.CommandName.Equals("Edit"))
            {
                if (DialogService != null)
                {
                    // QFDataGrid
                    DialogService.SetSpecs(300, 450, "AddEditDataLabel", GetLocalResourceObject("11874dae-f8ee-44f3-a3e2-9f4e78ce494a.DialogTitleOverride").ToString());
                    DialogService.EntityType = typeof(Sage.Entity.Interfaces.IEMDataLabel);
                    DialogService.EntityID = id;
                    DialogService.ShowDialog();
                }
            }
            if (e.CommandName.Equals("Delete"))
            {
                Sage.Entity.Interfaces.IEMEmailAccount mainentity = this.BindingSource.Current as Sage.Entity.Interfaces.IEMEmailAccount;
                if (mainentity != null)
                {
                    Sage.Entity.Interfaces.IEMDataLabel childEntity = null;
                    if ((currentEntity != null) && (currentEntity is Sage.Entity.Interfaces.IEMDataLabel))
                    {
                        childEntity = (Sage.Entity.Interfaces.IEMDataLabel)currentEntity;
                    }
                    else if (id != null)
                    {
                        childEntity = Sage.Platform.EntityFactory.GetById<Sage.Entity.Interfaces.IEMDataLabel>(id);
                    }
                    if (childEntity != null)
                    {
                        List<string> reservedNames = new List<string>() { "FIRSTNAME", "FULLNAME", "GENDER", "LASTNAME", "POSTCODE" };

                        if (reservedNames.Contains(childEntity.Name))
                        {
                            throw new Sage.Platform.Application.ValidationException(string.Format("{0} is a system Data Label and cannot be deleted.", childEntity.Name));
                        }
                        else
                        {
                            mainentity.EMDataLabels.Remove(childEntity);
                            if ((childEntity.PersistentState & Sage.Platform.Orm.Interfaces.PersistentState.New) <= 0)
                            {
                                childEntity.Delete();
                            }
                        }
                        dsEMDataLabels.SelectedIndex = -1;
                    }
                }
            }
        }
        grdEMDataLabels_refresh();
    }

    protected void grdEMDataLabels_refresh()
    {
        if (PageWorkItem != null)
        {
            Sage.Platform.WebPortal.Services.IPanelRefreshService refresher = PageWorkItem.Services.Get<Sage.Platform.WebPortal.Services.IPanelRefreshService>();
            if (refresher != null)
            {
                refresher.RefreshAll();
            }
            else
            {
                Response.Redirect(Request.Url.ToString());
            }
        }
    }

    protected void grdEMDataLabels_RowEditing(object sender, GridViewEditEventArgs e)
    {
        grdEMDataLabels.SelectedIndex = e.NewEditIndex;
        e.Cancel = true;
    }
    protected void grdEMDataLabels_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        grdEMDataLabels.SelectedIndex = -1;
    }
}
