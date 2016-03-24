using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sage.Platform.WebPortal.SmartParts;
using Sage.Entity.Interfaces;
using Sage.Platform.Application.UI;
using System.Collections.Generic;
using Sage.Platform.Repository;
using System.Text;
using Sage.Platform;
using Sage.Platform.Application;
using System.Collections;
using Sage.SalesLogix.CampaignTarget;
using System.Threading;
using Sage.SalesLogix.Client.GroupBuilder;
using log4net;
using System.Reflection;
using NHibernate;
using Sage.Platform.Framework;

/// <summary>
/// Summary description for AddEditDataMapping
/// </summary>
public partial class AddEditDataMapping : EntityBoundSmartPartInfoProvider
{
    /// <summary>
    /// Gets or sets the role security service.
    /// </summary>
    /// <value>The role security service.</value>
    [Sage.Platform.Application.ServiceDependency]
    public Sage.Platform.Security.IRoleSecurityService RoleSecurityService { get; set; }

    public override Type EntityType
    {
        get { return typeof(Sage.Entity.Interfaces.IEMDataMapping); }
    }



    protected override void OnAddEntityBindings()
    {
        // lueLabel.LookupResultValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding lueLabelLookupResultValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("EMDataLabel", lueLabel, "LookupResultValue");
        BindingSource.Bindings.Add(lueLabelLookupResultValueBinding);
        // pklMapDirection.PickListValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding pklMapDirectionPickListValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("MapDirection", pklMapDirection, "PickListValue");
        BindingSource.Bindings.Add(pklMapDirectionPickListValueBinding);
        // pklEntity.PickListValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding pklEntityPickListValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("EntityType", pklEntity, "PickListValue");
        BindingSource.Bindings.Add(pklEntityPickListValueBinding);
        // ddlFieldName.PickListValue Binding
        //Sage.Platform.WebPortal.Binding.WebEntityBinding ddlFieldNamePickListValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("FieldName", ddlFieldName, "SelectedValue");
        //BindingSource.Bindings.Add(ddlFieldNamePickListValueBinding);
        // pklLinkedFieldName.PickListValue Binding
        //Sage.Platform.WebPortal.Binding.WebEntityBinding ddlLinkedFieldNamePickListValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("LinkedFieldName", ddlLinkedFieldName, "SelectedValue");
        //BindingSource.Bindings.Add(ddlLinkedFieldNamePickListValueBinding);
        // txtDescription.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtDescriptionTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("Description", txtDescription, "Text");
        BindingSource.Bindings.Add(txtDescriptionTextBinding);
        // lueEmailAccount.LookupResultValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding lueEmailAccountLookupResultValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("EMEmailAccount", lueEmailAccount, "LookupResultValue");
        BindingSource.Bindings.Add(lueEmailAccountLookupResultValueBinding);


    }

    protected void btnOK_ClickAction(object sender, EventArgs e)
    {
        Sage.Entity.Interfaces.IEMDataMapping _entity = BindingSource.Current as Sage.Entity.Interfaces.IEMDataMapping;
        if (_entity != null)
        {
            object _parent = GetParentEntity();
            if (DialogService.ChildInsertInfo != null)
            {
                //if (_parent != null)
                //{
                //    if (DialogService.ChildInsertInfo.ParentReferenceProperty != null)
                //    {
                //        DialogService.ChildInsertInfo.ParentReferenceProperty.SetValue(_entity, _parent, null);
                //    }
                //}
            }
            bool shouldSave = true;



            // Additional validation
            if (this.lueLabel.LookupResultValue == null)
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException("You must choose a Label for this Data Mapping");
            }
			
            string mappingDirection = this.pklMapDirection.PickListValue;
            if (string.IsNullOrEmpty(mappingDirection))
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException("You must choose a Map Direction for this Data Mapping.");
            }

            bool mappingDirectionIsValid =
                (mappingDirection == "<--- Information flows from CRM") ||
                (mappingDirection == "Information flows to CRM --->");
            if (!mappingDirectionIsValid)
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException("You must choose a valid Map Direction for this Data Mapping.");
            }
			
            if (string.IsNullOrEmpty(this.pklEntity.PickListValue))
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException("You must choose an Entity for this Data Mapping");
            }
            // Field name/linked
            if (ddlFieldName.Items.Count > 0
                && (ddlFieldName.SelectedItem == null
                        || string.IsNullOrEmpty(ddlFieldName.SelectedValue)))
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException(
                    "You must choose a Field Name for this Data Mapping");
            }
            if (this.ddlLinkedFieldName.Items.Count > 0
                && (this.ddlLinkedFieldName.SelectedItem == null 
                        || string.IsNullOrEmpty(this.ddlLinkedFieldName.SelectedValue)))
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException(
                    "You must choose a Linked Field Name if you selected an 'Entity' Field Name for this Data Mapping");
            }


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
                _entity.FieldName = ddlFieldName.SelectedValue;
                _entity.LinkedFieldName = ddlLinkedFieldName.SelectedValue;
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

    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
        btnOK.Click += new EventHandler(btnOK_ClickAction);
        btnOK.Click += new EventHandler(DialogService.CloseEventHappened);
        btnOK.Click += new EventHandler(Refresh);
        btnCancel.Click += new EventHandler(DialogService.CloseEventHappened);
        lueLabel.LookupResultValueChanged += new EventHandler(lueLabel_LookupResultValueChanged);
        pklEntity.PickListValueChanged += new EventHandler(pklEntity_PickListValueChanged);
        ddlFieldName.SelectedIndexChanged += new EventHandler(ddlFieldName_SelectedIndexChanged);
    }

    protected void ddlFieldName_SelectedIndexChanged(object sender, EventArgs e)
    {
        IEMDataMapping thisMapping = (IEMDataMapping)this.BindingSource.Current;
        thisMapping.FieldName = ddlFieldName.SelectedValue;
        PopulateLinkedFieldNameList(thisMapping.EntityType, thisMapping.EMDataLabel.DataType, thisMapping.FieldName);
    }

    protected void pklEntity_PickListValueChanged(object sender, EventArgs e)
    {
        ValidateLabelAndEntity();
        IEMDataMapping thisMapping = (IEMDataMapping)this.BindingSource.Current;
        thisMapping.FieldName = "";
        thisMapping.LinkedFieldName = "";
        PopulateFieldNameList(thisMapping.EntityType, thisMapping.EMDataLabel.DataType);
        PopulateLinkedFieldNameList(thisMapping.EntityType, thisMapping.EMDataLabel.DataType, null);
    }

    protected void lueLabel_LookupResultValueChanged(object sender, EventArgs e)
    {
        Sage.Platform.WebPortal.Services.IPanelRefreshService refresher = PageWorkItem.Services.Get<Sage.Platform.WebPortal.Services.IPanelRefreshService>();
        if (refresher != null)
        {
            refresher.RefreshAll();
        }
        ValidateLabelAndEntity();
        IEMDataMapping thisMapping = (IEMDataMapping)this.BindingSource.Current;
        thisMapping.FieldName = "";
        thisMapping.LinkedFieldName = "";
        PopulateFieldNameList(thisMapping.EntityType, thisMapping.EMDataLabel.DataType);
        PopulateLinkedFieldNameList(thisMapping.EntityType, thisMapping.EMDataLabel.DataType, null);
    }

    protected void ValidateLabelAndEntity()
    {
        IEMDataLabel selectedLabel = (IEMDataLabel)lueLabel.LookupResultValue;
        string selectedEntity = pklEntity.PickListValue;
        IEMDataMapping thisMapping = (IEMDataMapping)this.BindingSource.Current;
        IEMEmailAccount emailAccount = (IEMEmailAccount)GetParentEntity();

        if (selectedLabel != null && !string.IsNullOrEmpty(selectedEntity) && emailAccount != null)
        {
            IRepository<IEMDataMapping> rep = EntityFactory.GetRepository<IEMDataMapping>();
            IQueryable qry = (IQueryable)rep;
            IExpressionFactory ef = qry.GetExpressionFactory();
            Sage.Platform.Repository.ICriteria criteria = qry.CreateCriteria();

            IList<IEMDataMapping> mappings = criteria.Add(ef.Conjunction().Add(ef.Eq("EntityType", selectedEntity)).Add(ef.Eq("EMDataLabel", selectedLabel)).Add(ef.Eq("EMEmailAccount", emailAccount))).List<IEMDataMapping>();
            mappings.Remove(thisMapping);

            if (mappings.Count > 0)
            {
                lblWarning.ForeColor = System.Drawing.Color.Red;
                lblWarning.Visible = true;
            }
            else
            {
                lblWarning.Visible = false;
            }
        }
        
    }

    protected void PopulateFieldNameList(string entityType, string dataType)
    {
        ddlFieldName.Enabled = true;
        ddlFieldName.Items.Clear();
        if (entityType == "Contact")
        {
            PropertyInfo[] propertyArray = typeof(IContact).GetProperties();
            if (propertyArray.Length > 0)
            {
                ddlFieldName.Items.Add(new ListItem());
                ddlFieldName.Items.Add(new ListItem("Fields", ""));
                foreach (PropertyInfo property in propertyArray)
                {

                    if (property.PropertyType.ToString().Contains(ToSLXType(dataType)) && !property.PropertyType.ToString().Contains("Collection"))
                    {
                        if (property.Name != "InstanceId" && property.Name != "Creating")
                        {
                            ddlFieldName.Items.Add(new ListItem(" - " + property.Name, property.Name));
                        }
                    }
                }

                ddlFieldName.Items.Add(new ListItem());
                ddlFieldName.Items.Add(new ListItem("Entities", ""));
                foreach (PropertyInfo property in propertyArray)
                {

                    if (property.PropertyType.ToString().StartsWith("Sage.Entity") && !property.PropertyType.ToString().Contains("Collection"))
                    {
                        ddlFieldName.Items.Add(new ListItem(" - " + property.Name, property.Name));
                    }
                }
            }
        }
        else if (entityType == "Lead")
        {
            PropertyInfo[] propertyArray = typeof(ILead).GetProperties();
            if (propertyArray.Length > 0)
            {
                ddlFieldName.Items.Add(new ListItem());
                ddlFieldName.Items.Add(new ListItem("Fields", ""));
                foreach (PropertyInfo property in propertyArray)
                {

                    if (property.PropertyType.ToString().Contains(ToSLXType(dataType)) && !property.PropertyType.ToString().Contains("Collection"))
                    {
                        if (property.Name != "InstanceId" && property.Name != "Creating")
                        {
                            ddlFieldName.Items.Add(new ListItem(" - " + property.Name, property.Name));
                        }
                    }
                }

                ddlFieldName.Items.Add(new ListItem());
                ddlFieldName.Items.Add(new ListItem("Entities", ""));
                foreach (PropertyInfo property in propertyArray)
                {

                    if (property.PropertyType.ToString().StartsWith("Sage.Entity") && !property.PropertyType.ToString().Contains("Collection"))
                    {
                        ddlFieldName.Items.Add(new ListItem(" - " + property.Name, property.Name));
                    }
                }
            }
        }
    }

    protected void PopulateLinkedFieldNameList(string entityType, string dataType, string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
        {
            ddlLinkedFieldName.SelectedValue = "";
            ddlLinkedFieldName.Enabled = false;
            ddlLinkedFieldName.Items.Clear();
        }
        else
        {
            if (entityType == "Contact")
            {
                PropertyInfo property = typeof(IContact).GetProperty(fieldName);
                if (property.PropertyType.ToString().StartsWith("Sage.Entity"))
                {
                    ddlLinkedFieldName.Enabled = true;
                    ddlLinkedFieldName.Items.Clear();

                    Type selectedType = property.PropertyType;

                    PropertyInfo[] propertyArray = selectedType.GetProperties();

                    if (propertyArray.Length > 0)
                    {
                        ddlLinkedFieldName.Items.Add(new ListItem());
                        ddlLinkedFieldName.Items.Add(new ListItem("Fields", ""));
                        foreach (PropertyInfo propInfo in propertyArray)
                        {
                            if (propInfo.PropertyType.ToString().Contains(ToSLXType(dataType)))
                            {
                                ddlLinkedFieldName.Items.Add(new ListItem(" - " + propInfo.Name, propInfo.Name));
                            }
                        }
                    }
                }
                else
                {
                    ddlLinkedFieldName.Enabled = false;
                    ddlLinkedFieldName.Items.Clear();
                    IEMDataMapping thisMapping = (IEMDataMapping)this.BindingSource.Current;
                    thisMapping.LinkedFieldName = "";
                }
            }
            else if (entityType == "Lead")
            {
                PropertyInfo property = typeof(ILead).GetProperty(fieldName);
                if (property.PropertyType.ToString().StartsWith("Sage.Entity"))
                {
                    ddlLinkedFieldName.Enabled = true;
                    ddlLinkedFieldName.Items.Clear();

                    Type selectedType = property.PropertyType;

                    PropertyInfo[] propertyArray = selectedType.GetProperties();

                    if (propertyArray.Length > 0)
                    {
                        ddlLinkedFieldName.Items.Add(new ListItem());
                        ddlLinkedFieldName.Items.Add(new ListItem("Fields", ""));
                        foreach (PropertyInfo propInfo in propertyArray)
                        {
                            if (propInfo.PropertyType.ToString().Contains(ToSLXType(dataType)))
                            {
                                ddlLinkedFieldName.Items.Add(new ListItem(" - " + propInfo.Name, propInfo.Name));
                            }
                        }
                    }
                }
                else
                {
                    ddlLinkedFieldName.Enabled = false;
                    ddlLinkedFieldName.Items.Clear();
                    IEMDataMapping thisMapping = (IEMDataMapping)this.BindingSource.Current;
                    thisMapping.LinkedFieldName = "";
                }
            }
        }
    }

    protected static string ToSLXType(string dataType)
    {
        if (dataType == null)
        {
            return null;
        }

        switch (dataType.ToLowerInvariant())
        {
            case "date":
                return "System.DateTime";
            case "boolean":
                return "System.Boolean";
            case "numeric":
                return "System.Int32";    //Check this one
            case "string":
                return "System";
            default:
                return dataType;
        }
    }

    protected override void OnFormBound()
    {
        IEMEmailAccount emailAccount = (IEMEmailAccount)GetParentEntity();
        lueLabel.SeedValue = emailAccount.Id.ToString();

        ClientBindingMgr.RegisterSaveButton(btnOK);

        ScriptManager.RegisterStartupScript(Page, GetType(), "cleanupcontainer", "jQuery(\".controlslist > div:empty\").remove();", true);
        ClientBindingMgr.RegisterDialogCancelButton(btnCancel);

        IEMDataMapping thisMapping = (IEMDataMapping)this.BindingSource.Current;

        // ------------
        Sage.Entity.Interfaces.IEMDataMapping _entity = BindingSource.Current as Sage.Entity.Interfaces.IEMDataMapping;
        if (_entity != null)
        {
            // Populate the parent (Email Account) on open
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
        }
        // ------------



        if (lueLabel.LookupResultValue == null)
        {
            pklMapDirection.Enabled = false;
            pklEntity.Enabled = false;
            ddlFieldName.Enabled = false;
            ddlLinkedFieldName.Enabled = false;
            txtDescription.Enabled = false;
        }
        else
        {
            pklMapDirection.Enabled = true;
            pklEntity.Enabled = true;
            txtDescription.Enabled = true;
        }

        if (!string.IsNullOrEmpty(thisMapping.EntityType) && !string.IsNullOrEmpty(thisMapping.EMDataLabel.DataType))
        {
            PopulateFieldNameList(thisMapping.EntityType, thisMapping.EMDataLabel.DataType);
            if (!string.IsNullOrEmpty(thisMapping.FieldName))
            {
                ddlFieldName.SelectedValue = thisMapping.FieldName;
                PopulateLinkedFieldNameList(thisMapping.EntityType, thisMapping.EMDataLabel.DataType, thisMapping.FieldName);
                if (!string.IsNullOrEmpty(thisMapping.LinkedFieldName))
                {
                    ddlLinkedFieldName.SelectedValue = thisMapping.LinkedFieldName;
                }
            }
        }
        else
        {
            ddlFieldName.Enabled = false;
            ddlFieldName.SelectedValue = "";
            ddlLinkedFieldName.Enabled = false;
            ddlLinkedFieldName.SelectedValue = "";
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
}