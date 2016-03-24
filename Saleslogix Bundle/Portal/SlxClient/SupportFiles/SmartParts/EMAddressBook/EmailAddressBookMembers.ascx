<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailAddressBookMembers.ascx.cs" Inherits="EmailAddressBookMembers" %>
<%@ Register Assembly="Sage.SalesLogix.Client.GroupBuilder" Namespace="Sage.SalesLogix.Client.GroupBuilder" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.PickList" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.DependencyLookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Lookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Timeline" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.HighLevelTypes" Namespace="Sage.SalesLogix.HighLevelTypes" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.Platform.WebPortal" Namespace="Sage.Platform.WebPortal.SmartParts" TagPrefix="SalesLogix" %>


<div id="filterDiv" style="display:none;">
        <table border="0" cellpadding="1" cellspacing="0" class="formtable">
            <col width="33%" />
            <col width="33%" />
            <col width="33%" />
            <tr>
                <td id="row1col1">
                    <div class="slxlabel alignleft">
                        <asp:CheckBox runat="server" ID="chkContacts" Checked="true"
                            CssClass="checkbox" Text="<%$ resources: chkContacts.Caption %>" />
                        <asp:CheckBox runat="server" ID="chkLeads" Checked="true"
                            CssClass="checkbox" Text="<%$ resources: chkLeads.Caption %>" />
                    </div>
                </td>
                <td id="row1col2">
                    <div class=" lbl alignleft">
                        <asp:Label ID="lblDoNotSolicit" AssociatedControlID="ddlDoNotSolicit" runat="server" Text="Do Not Solicit:" ></asp:Label>
                    </div>   
                    <div  class="textcontrol select"  > 
                        <asp:ListBox runat="server" ID="ddlDoNotSolicit"  SelectionMode="Single" Rows="1"  >
                            <asp:ListItem  Text="<%$ resources: ddlDoNotSolicit_item0.Text %>" Value="Either" />
                            <asp:ListItem  Text="<%$ resources: ddlDoNotSolicit_item1.Text %>" Value="Yes" />
                            <asp:ListItem  Text="<%$ resources: ddlDoNotSolicit_item2.Text %>" Value="No" />
                        </asp:ListBox>
                    </div>
                </td>
                <td id="row1col3">
                    <div class=" lbl alignleft">
                        <asp:Label ID="lblDoNotEmail" AssociatedControlID="ddlDoNotEmail" runat="server" Text="Do Not Email:" ></asp:Label>
                    </div>   
                    <div  class="textcontrol select"  > 
                        <asp:ListBox runat="server" ID="ddlDoNotEmail"  SelectionMode="Single" Rows="1"  >
                            <asp:ListItem  Text="<%$ resources: ddlDoNotEmail_item0.Text %>" Value="Either" />
                            <asp:ListItem  Text="<%$ resources: ddlDoNotEmail_item1.Text %>" Value="Yes" />
                            <asp:ListItem  Text="<%$ resources: ddlDoNotEmail_item2.Text %>" Value="No" />
                        </asp:ListBox>
                    </div>
                </td>
            </tr>
        </table>
    </div>

  <SalesLogix:SlxGridView runat="server" ID="grdEMAddressBookMembers" GridLines="None"
AutoGenerateColumns="false" CellPadding="4" CssClass="datagrid tableWithId" PagerStyle-CssClass="gridPager"
AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt" SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="true"
 AllowPaging="true" PageSize="10" OnPageIndexChanging="grdEMAddressBookMemberspage_changing"  ExpandableRows="True" ResizableColumns="True" DataKeyNames="Id" >
<Columns>
    <asp:TemplateField>
        <ItemTemplate>
            <asp:CheckBox ID="chkSelect" runat="server" CssClass="chkSelect" />
            <asp:Label ID="lblEntityId" runat="server" CssClass="lblEntityId" Text='<%# Eval("Id") %>' ></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="<%$ resources: grdEMAddressBookMembers.FullName.ColumnHeading %>" SortExpression="Name">
        <ItemTemplate>
            <asp:Label ID="lblFullName" runat="server" Text='<%# Eval("SlxMemberType").ToString() == "Contact" ? Eval("ContactLastName") != null ? Eval("ContactFirstName") + " " + Eval("ContactLastName") : "Unavailable" : Eval("LeadLastName") != null ? Eval("LeadFirstName") + " " + Eval("LeadLastName") : "Unavailable" %>'></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="<%$ resources: grdEMAddressBookMembers.Company.ColumnHeading %>" SortExpression="Company">
        <ItemTemplate>
            <asp:Label ID="lblAccountName" runat="server" Text='<%# Eval("SlxMemberType").ToString() == "Contact" ? Eval("ContactAccountName") != null ? Eval("ContactAccountName") : "Unavailable" : Eval("LeadCompany") != null ? Eval("LeadCompany") : "Unavailable"  %>'></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="SlxMemberType" HeaderText="<%$ resources: grdEMAddressBookMembers.Type.ColumnHeading %>" SortExpression="SlxMemberType" />
    <asp:TemplateField HeaderText="Email Address" SortExpression="EmailAddress">
        <ItemTemplate>
            <asp:Label ID="lblEmailAddress" runat="server" Text='<%# Eval("SlxMemberType").ToString() == "Contact" ? Eval("ContactLastName") != null ? Eval("ContactEmail") : "Unavailable" : Eval("LeadLastName") != null ? Eval("LeadEmail") : "Unavailable" %>'></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="<%$ resources: grdEMAddressBookMembers.DoNotSolicit.ColumnHeading %>" SortExpression="DoNotSolicit">
        <ItemTemplate>
            <asp:Label ID="lblContactDoNotSolicit" runat="server" Text='<%# Eval("SlxMemberType").ToString() == "Contact" ? Convert.ToBoolean(Eval("ContactDoNotSolicit")) ? "Yes" : "No" : ""  %>' ></asp:Label>
            <asp:Label ID="lblLeadDoNotSolicit" runat="server" Text='<%# Eval("SlxMemberType").ToString() == "Lead" ? Convert.ToBoolean(Eval("LeadDoNotSolicit")) ? "Yes" : "No" : ""  %>' ></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="<%$ resources: grdEMAddressBookMembers.DoNotEmail.ColumnHeading %>" SortExpression="DoNotEmail">
        <ItemTemplate>
            <asp:Label ID="lblContactDoNotEmail" runat="server" Text='<%# Eval("SlxMemberType").ToString() == "Contact" ? Convert.ToBoolean(Eval("ContactDoNotEmail")) ? "Yes" : "No" : ""  %>' ></asp:Label>
            <asp:Label ID="lblLeadDoNotEmail" runat="server" Text='<%# Eval("SlxMemberType").ToString() == "Lead" ? Convert.ToBoolean(Eval("LeadDoNotEmail")) ? "Yes" : "No" : ""  %>' ></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField>
        <ItemTemplate>
            <asp:Label ID="lblId" runat="server" Text='<%# Eval("Id") %>' ></asp:Label>
        </ItemTemplate>
    </asp:TemplateField>
 </Columns>
    <PagerSettings Mode="NumericFirstLast" FirstPageImageUrl="ImageResource.axd?scope=global&type=Global_Images&key=Start_16x16" LastPageImageUrl="ImageResource.axd?scope=global&type=Global_Images&key=End_16x16" />
</SalesLogix:SlxGridView>
<asp:TextBox runat="server" ID="lblDeleteIds" CssClass="lblDeleteIds" EnableViewState="true"></asp:TextBox>


<SalesLogix:SmartPartToolsContainer runat="server" ID="EmailAddressBookMembers_LTools" ToolbarLocation="left">   
    <a href="#" id="lnkFilters" onclick="javascript:toggleFilters()">Show Filters</a>
    <asp:Image runat="server" ImageUrl="~/images/icons/Filter_16x16.png" ID="imgFilter" Visible="false" ToolTip="<%$ resources: imgFilter.Caption %>" />
    <asp:Button runat="server" ID="btnClearSelection" Text="<%$ resources: btnClearSelection.Caption %>" ToolTip="<%$ resources: btnClearSelection.Caption %>" CssClass="slxbutton" OnClientClick="clearSelection(); return false;" />
    <asp:Button runat="server" ID="btnRefresh" Text="<%$ resources: btnRefresh.Caption %>" ToolTip="<%$ resources: btnRefresh.Caption %>" CssClass="slxbutton" OnClick="btnRefresh_OnClick" />
</SalesLogix:SmartPartToolsContainer>

 <SalesLogix:SmartPartToolsContainer runat="server" ID="EmailAddresBookMembersGrid_RTools" ToolbarLocation="right">
     <asp:Button runat="server" ID="btnDeleteAll" Text="<%$ resources: btnDeleteAll.Caption %>" ToolTip="<%$ resources: btnDeleteAll.Caption %>" CssClass="slxbutton" OnClick="btnDeleteAll_OnClick" />
     <asp:Button runat="server" ID="btnDeleteSelected" Text="<%$ resources: btnDeleteSelected.Caption %>" ToolTip="<%$ resources: btnDeleteSelected.Caption %>" CssClass="slxbutton" OnClick="btnDeleteSelected_OnClick" />
     <asp:ImageButton runat="server" ID="cmdManageList" OnClick="ManageMembers_OnClick" AlternateText="<%$ resources: cmdManageList.Caption %>"
            ToolTip="<%$ resources: cmdManageList.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=plus_16x16" />
     <SalesLogix:PageLink ID="lnkEmailAddresBookMembersGridHelp" runat="server" LinkType="HelpFileName" ToolTip="<%$ resources: Portal, Help_ToolTip %>" Target="MCWebHelp" NavigateUrl="EmailAddresBookMembersGrid" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Help_16x16"></SalesLogix:PageLink>
 </SalesLogix:SmartPartToolsContainer>


<style type="text/css">
#lnkFilters:hover 
{
    cursor:pointer;
}
.tableWithId .slx-grid-cell-last, .lblDeleteIds, .lblEntityId
 {
     display: none;
 }
</style>

<script type="text/javascript">
    function toggleFilters() {
        if ($('#filterDiv').css('display') == 'none') {
            $('#filterDiv').css({ 'display': 'block' });
            $('#lnkFilters').html('Hide Filters');
        }
        else {
            $('#filterDiv').css({ 'display': 'none' });
            $('#lnkFilters').html('Show Filters');
        }
    };

    function reload() {
        window.location.reload();
    }
</script>

<script type="text/javascript">
    function flagSelected() {
        $(".chkSelect > input").each(function () {
            $(this).click(function () {
                
                if ($(this).attr("checked") == true) {
                    $(".lblDeleteIds").val($(".lblDeleteIds").val() + $(this).parent().siblings(".lblEntityId").html() + ",");
                } else {
                    $(".lblDeleteIds").val($(".lblDeleteIds").val().replace($(this).parent().siblings(".lblEntityId").html() + ",", ""));
                }
                
            });
        });
    }
    
    function clearSelection() {
        $(".lblDeleteIds").val('');
        $(".chkSelect > input").attr("checked") = false;
    }
    
</script>