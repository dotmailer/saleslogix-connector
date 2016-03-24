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
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for AddEditDataLabel
/// </summary>
public partial class AddEditDataLabel : EntityBoundSmartPartInfoProvider
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
        get { return typeof(Sage.Entity.Interfaces.IEMDataLabel); }
    }



    protected override void OnAddEntityBindings()
    {
        // txtName.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtNameTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("Name", txtName, "Text");
        BindingSource.Bindings.Add(txtNameTextBinding);

        // pklDataType.PickListValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding pklDataTypePickListValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("DataType", pklDataType, "PickListValue");
        BindingSource.Bindings.Add(pklDataTypePickListValueBinding);

        // radIsPrivate.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding radIsPrivateTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("IsPrivate", radIsPrivate, "Text");
        BindingSource.Bindings.Add(radIsPrivateTextBinding);
        
        // txtDefaultValue.Text Binding        
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtDefaultValueTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("DefaultValue", txtDefaultValue, "Text");
        BindingSource.Bindings.Add(txtDefaultValueTextBinding);            
        
        // txtDescription.Text Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding txtDescriptionTextBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("Description", txtDescription, "Text");
        BindingSource.Bindings.Add(txtDescriptionTextBinding);

        // lueEmailAccount.LookupResultValue Binding
        Sage.Platform.WebPortal.Binding.WebEntityBinding lueEmailAccountLookupResultValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("EMEmailAccount", lueEmailAccount, "LookupResultValue");
        BindingSource.Bindings.Add(lueEmailAccountLookupResultValueBinding);
    }

    protected void btnOK_ClickAction(object sender, EventArgs e)
    {
        Sage.Entity.Interfaces.IEMDataLabel _entity = BindingSource.Current as Sage.Entity.Interfaces.IEMDataLabel;
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
            
            // ---------------------------
            // Name validation
            //
            if (string.IsNullOrEmpty(txtName.Text))
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException("You must provide a name for this data label");
            }
            if (txtName.Text.ToUpper() == "EMAIL" 
                || txtName.Text.ToUpper() == "FIRSTNAME"
                || txtName.Text.ToUpper() == "FULLNAME"
                || txtName.Text.ToUpper() == "GENDER"
                || txtName.Text.ToUpper() == "LASTNAME"
                || txtName.Text.ToUpper() == "POSTCODE")
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException(string.Format("{0} is a reserved name, please rename your Data Label", txtName.Text)); 
            }

            // For now, hyphens are not allowed in a label name due to an SDK problem.  To be re-enabled when the problem is fixed.
            //_entity.Name = Regex.Replace(_entity.Name, "[^a-zA-Z0-9_/-]", "").ToUpperInvariant();
            _entity.Name = Regex.Replace(_entity.Name, "[^a-zA-Z0-9_]", "").ToUpperInvariant();

            IEMEmailAccount emailAccount = (IEMEmailAccount)_parent;
            foreach (IEMDataLabel dataLabel in emailAccount.EMDataLabels)
            {
                if (dataLabel.Id != _entity.Id)
                {
                    if (dataLabel.Name.ToLower() == _entity.Name.ToLower())
                    {
                        shouldSave = false;
                        throw new Sage.Platform.Application.ValidationException("A data label with that name already exists!");
                    }
                }
            }
            
            // DataType validation
            string pickListValue = this.pklDataType.PickListValue;
            if (string.IsNullOrEmpty(pickListValue))
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException("You must choose a Data Type for this data label.");
            }

            bool knownDataType =
                (pickListValue == "String") ||
                (pickListValue == "Date") ||
                (pickListValue == "Boolean") ||
                (pickListValue == "Numeric");
            if (!knownDataType)
            {
                shouldSave = false;
                throw new Sage.Platform.Application.ValidationException("You must choose a valid Data Type for this data label.");
            }

            Sage.Platform.WebPortal.EntityPage page = Page as Sage.Platform.WebPortal.EntityPage;
            if (page != null)
            {
                if (IsInDialog() && page.ModeId.ToUpper() == "INSERT")
                {
                    shouldSave = false;
                }
            }

            // ---------------------------------------------------------------------
            // Update the entity with the "unbound" properties (default value)
            // (ONLY allowed in "New" mode, not "Edit"
            //
            if (_entity.Id == null) //we're in new mode
            {
                // Depending on the type selected, show appropriate control
                switch (this.pklDataType.PickListValue.ToLowerInvariant())
                {
                    case "string":
                        _entity.DefaultValue = this.txtDefaultString.Text;
                        break;

                    case "date":
                        // Has the user put something in the date picker?
                        // N.B. calDefaultDate.DataTimeValue has a value set if the user opened the picker but clicked cancel.
                        // Must check the .Text property.
                        if (!string.IsNullOrEmpty(this.calDefaultDate.Text) && this.calDefaultDate.DateTimeValue.HasValue)
                        {
                            _entity.DefaultValue = this.calDefaultDate.DateTimeValue.Value.ToString("d");
                        }

                        break;

                    case "boolean":
                        _entity.DefaultValue = (this.radDefaultBoolean.SelectedIndex != 0).ToString();
                        break;

                    case "numeric":
                        // Number validation
                        int numValue;
                        if (!string.IsNullOrEmpty(this.numDefaultNumeric.Text))
                        {
                            if (int.TryParse(this.numDefaultNumeric.Text, out numValue) == false)
                            {
                                shouldSave = false;
                                throw new Sage.Platform.Application.ValidationException("You can only include digits for a numeric default value");
                            }
                            _entity.DefaultValue = numValue.ToString();
                        }

                        break;
                }
            }
            // ---------------------------------------------------------------------

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


    void pklDataType_PickListValueChanged(object sender, EventArgs e)
    {
        // Change/Show/Hide correct control based on chosen datatype
        if (string.IsNullOrEmpty(this.pklDataType.PickListValue))
        {
            // Nothing picked yet (possible?)
            return;
        }

        // Hide all controls by default
        this.txtDefaultString.Visible = false;
        this.calDefaultDate.Visible = false;
        this.radDefaultBoolean.Visible = false;
        this.numDefaultNumeric.Visible = false;

        // Depending on the type selected, show appropriate control
        switch (this.pklDataType.PickListValue.ToLowerInvariant())
        {
            case "string":
                this.txtDefaultString.Visible = true;
                break;
                
            case "date":
                this.calDefaultDate.Visible = true;
                break;

            case "boolean":
                this.radDefaultBoolean.Visible = true;
                break;

            case "numeric":
                this.numDefaultNumeric.Visible = true;
                break;
        }
    }


    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
        btnOK.Click += new EventHandler(btnOK_ClickAction);
        btnOK.Click += new EventHandler(DialogService.CloseEventHappened);
        btnOK.Click += new EventHandler(Refresh);
        btnCancel.Click += new EventHandler(DialogService.CloseEventHappened);

        pklDataType.PickListValueChanged += new EventHandler(pklDataType_PickListValueChanged);
    }


    protected override void OnFormBound()
    {
        ClientBindingMgr.RegisterSaveButton(btnOK);

        ScriptManager.RegisterStartupScript(Page, GetType(), "cleanupcontainer", "jQuery(\".controlslist > div:empty\").remove();", true);

        // For now, hyphens are not allowed in a label name due to an SDK problem.  To be re-enabled when the problem is fixed.
        //ScriptManager.RegisterStartupScript(Page, GetType(), "filterCharacters", @"jQuery(""[id$='txtName']"").keyup(function(){this.value=this.value.replace(/[^a-zA-Z0-9_/-]/g,'').toUpperCase();});", true);
        ScriptManager.RegisterStartupScript(Page, GetType(), "filterCharacters", @"jQuery(""[id$='txtName']"").keyup(function(){this.value=this.value.replace(/[^a-zA-Z0-9_]/g,'').toUpperCase();});", true);
        ClientBindingMgr.RegisterDialogCancelButton(btnCancel);

        if (radIsPrivate.SelectedIndex < 0)
        {
            radIsPrivate.SelectedIndex = 1;
        }

        if (radDefaultBoolean.SelectedIndex < 0)
        {
            radDefaultBoolean.SelectedIndex = 0;
        }

        Sage.Entity.Interfaces.IEMDataLabel _entity = BindingSource.Current as Sage.Entity.Interfaces.IEMDataLabel;
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
            // ------------

            if (_entity.Id != null) //we're in edit mode
            {
                txtName.Enabled = false;
                pklDataType.Enabled = false;
                radIsPrivate.Enabled = false;
                txtDefaultValue.Visible = true;
                txtDefaultValue.Enabled = false;
                txtDescription.Enabled = true; // Only editable field

                // Hide all "default value" controls in edit mode
                this.txtDefaultString.Visible = false;
                this.calDefaultDate.Visible = false;
                this.radDefaultBoolean.Visible = false;
                this.numDefaultNumeric.Visible = false;
            }
            else
            {
                txtName.Enabled = true;
                pklDataType.Enabled = true;
                radIsPrivate.Enabled = true;
                txtDefaultValue.Visible = false;
                txtDefaultValue.Enabled = false;
                txtDescription.Enabled = true;

                // No data type picked yet?
                if (string.IsNullOrEmpty(this.pklDataType.PickListValue))
                {
                    // SHow "string" box by default (so there isn't a gap!)
                    this.txtDefaultString.Visible = true;

                    // Hide/Reset all controls by default
                    this.calDefaultDate.Visible = false;
                    this.radDefaultBoolean.Visible = false;
                    this.numDefaultNumeric.Visible = false;
                    this.txtDefaultString.Text = string.Empty;
                    this.calDefaultDate.DateTimeValue = null;
                    this.radDefaultBoolean.SelectedIndex = 0;
                    this.numDefaultNumeric.Text = string.Empty;
                }
            }
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