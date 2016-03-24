using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sage.Platform.WebPortal.SmartParts;
using System.Web.UI;
using Sage.Platform;
using Sage.Entity.Interfaces;
using System.Text;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for AddContactToEmailCampaign
/// </summary>
public partial class AddContactToEmailCampaign : EntityBoundSmartPartInfoProvider
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
        // lueEmailCampaign.LookupResultValue Binding
        //Sage.Platform.WebPortal.Binding.WebEntityBinding lueEmailCampaignLookupResultValueBinding = new Sage.Platform.WebPortal.Binding.WebEntityBinding("EmailCampaignName", lueEmailCampaign, "LookupResultValue");
        //BindingSource.Bindings.Add(lueEmailCampaignLookupResultValueBinding);


    }


    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
        btnCancel.Click += new EventHandler(btnCancel_Click);
        btnAdd.Click += new EventHandler(btnAdd_Click);
        lueEmailCampaign.LookupResultValueChanged += new EventHandler(lueEmailCampaign_LookupResultValueChanged);
        DialogService.onDialogClosing += new Sage.Platform.WebPortal.Services.dlgClosing(DialogService_onDialogClosing);

    }

    void DialogService_onDialogClosing(object from, Sage.Platform.WebPortal.Services.WebDialogClosingEventArgs e)
    {
        lueEmailCampaign.LookupResultValue = null;
        grdTargetAddressBooks.DataSource = null;
        grdTargetAddressBooks.DataBind();
    }

    void btnAdd_Click(object sender, EventArgs e)
    {
        IContact currentContact = (IContact)GetParentEntity();

        foreach (GridViewRow row in grdTargetAddressBooks.Rows)
        {
            CheckBox rowCheck = (CheckBox)row.FindControl("chkSelect");
            if (rowCheck.Checked == true)
            {
                string campAddrBookId = grdTargetAddressBooks.DataKeys[row.RowIndex].Value.ToString();
                IEMCampaignAddressBook selectedCampAddrBook = EntityFactory.GetById<IEMCampaignAddressBook>(campAddrBookId);
                bool addToAddrBook = true;

                foreach (IEMAddressBookMember addrBookMember in currentContact.EMAddressBookMembers)
                {
                    if (selectedCampAddrBook.EMAddressBook.Id == addrBookMember.EMAddressBook.Id)
                    {
                        addToAddrBook = false;
                    }
                }

                if (addToAddrBook)
                {
                    IEMAddressBookMember newMember = EntityFactory.Create<IEMAddressBookMember>();
                    newMember.Contact = currentContact;
                    newMember.SlxMemberType = "Contact";
                    newMember.EMAddressBook = selectedCampAddrBook.EMAddressBook;
                    newMember.Save();
                }
            }
        }
        
        
        DialogService.CloseEventHappened(sender, e); 
    }

    void btnCancel_Click(object sender, EventArgs e)
    {
        
        DialogService.CloseEventHappened(sender, e);
    }

    protected void lueEmailCampaign_LookupResultValueChanged(object sender, EventArgs e)
    {
        IEMEmailCampaign selectedCampaign = (IEMEmailCampaign)lueEmailCampaign.LookupResultValue;

        
        //lueEmailCampaign.Text = selectedCampaign.ToString();

        grdTargetAddressBooks.DataSource = selectedCampaign.EMCampaignAddressBooks;
        grdTargetAddressBooks.DataBind();

        foreach (GridViewRow row in grdTargetAddressBooks.Rows)
        {
            CheckBox rowCheck = (CheckBox)row.FindControl("chkSelect");
            rowCheck.Attributes.Add("onclick", "checkChanged()");
        }

        if (selectedCampaign.EMCampaignAddressBooks.Count == 0)
        {
            litNoData.Visible = true;
        }
        else
        {
            litNoData.Visible = false;
        }
    }

    protected override void OnFormBound()
    {
        ScriptManager.RegisterStartupScript(Page, GetType(), "cleanupcontainer", "jQuery(\".controlslist > div:empty\").remove();", true);
        ScriptManager.RegisterStartupScript(Page, GetType(), ClientID, "function checkChanged () { if ($('input[id*=\"grdTargetAddressBooks\"]:checked').length > 0) { $('.addButton').css({'display':'inline'}) } else { $('.addButton').css({'display':'none'}) } }", true);
        ClientBindingMgr.RegisterDialogCancelButton(btnCancel);

        IContact currContact = (IContact)GetParentEntity();

        litAddInstructions.Text = string.Format(litAddInstructions.Text, currContact.FirstName + " " + currContact.LastName, currContact.AccountName);
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