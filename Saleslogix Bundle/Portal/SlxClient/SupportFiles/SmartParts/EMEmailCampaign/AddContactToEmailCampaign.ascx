<%@ Control Language="C#" CodeFile="AddContactToEmailCampaign.ascx.cs" Inherits="AddContactToEmailCampaign" %>
<%@ Register Assembly="Sage.SalesLogix.Client.GroupBuilder" Namespace="Sage.SalesLogix.Client.GroupBuilder" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.PickList" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.DependencyLookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Lookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Timeline" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.HighLevelTypes" Namespace="Sage.SalesLogix.HighLevelTypes" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.Platform.WebPortal" Namespace="Sage.Platform.WebPortal.SmartParts" TagPrefix="SalesLogix" %>

<div style="padding:15px;" >
<p style="width:100%;"><asp:Literal runat="server" ID="litAddInstructions" Text="<%$ resources: litAddInstructions.Caption %>" ></asp:Literal></p>
<div  class="textcontrol lookup" >
<br />
    <SalesLogix:LookupControl runat="server" ID="lueEmailCampaign" LookupEntityName="EMEmailCampaign" LookupEntityTypeName="Sage.Entity.Interfaces.IEMEmailCampaign, Sage.Entity.Interfaces, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" LookupBindingMode="Object" AutoPostBack="true"  >
        <LookupProperties>
            <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueEmailCampaign.LookupProperties.EmailCampaignName.PropertyHeader %>" PropertyName="EmailCampaignName" PropertyType="System.String" PropertyFormat="None" PropertyFormatString="" UseAsResult="True" ExcludeFromFilters="False"></SalesLogix:LookupProperty>
        </LookupProperties>
        <LookupPreFilters>
        </LookupPreFilters>
    </SalesLogix:LookupControl>
</div>
</div>
<div style="clear:both;"></div>

<p style="padding:5px 15px 0px 15px;"><asp:Literal runat="server" ID="litAddressBooks" Text="<%$ resources: litAddressBooks.Caption %>" /></p>
<div style="padding:15px;">
<asp:Literal runat="server" ID="litNoData" Visible="false" Text="<%$ resources:litNoData.Caption %>"></asp:Literal>
    <SalesLogix:SlxGridView runat="server" ID="grdTargetAddressBooks" GridLines="None"
    AutoGenerateColumns="false" CellPadding="4" CssClass="datagrid tableWithId" PagerStyle-CssClass="gridPager"
    AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt" SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="false" EnableViewState="true"
    ExpandableRows="True" ResizableColumns="True" AllowPaging="false" DataKeyNames="Id" Height="165px" >
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ resources: grdTargetAddressBooks.AddressBooks.Caption %>">
                <ItemTemplate>
                    <asp:Label ID="lblAddressBookName"  runat="server" Text='<%# Eval("EMAddressBook.Name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ resources: grdTargetsAddressBooks.CampaignAddressBooksCount.Caption %>">
                <ItemTemplate>
                    <asp:Label ID="lblCount" runat="server" Text='<%# Eval("EMAddressBook.EMCampaignAddressBooks.Count") %>' ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("Id") %>' ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </SalesLogix:SlxGridView>
</div>
      
<div style="float:right;padding:15px;">
    <asp:Panel runat="server" ID="contButtons" CssClass="controlslist " >
        <asp:Button runat="server" ID="btnAdd" Text="<%$ resources: btnAdd.Caption %>" CssClass="slxbutton addButton"  />
        <asp:Button runat="server" ID="btnCancel" Text="<%$ resources: btnCancel.Caption %>" CssClass="slxbutton"  />
    </asp:Panel>
</div>
<div style="clear:both;"></div>
<div style="font-size:0.85em;font-style:italic;color:Red;padding:0px 10px 0px 10px;"><asp:Literal runat="server" ID="litMultiCampaignWarning" Text="<%$ resources: litMultiCampaignWarning.Caption %>" ></asp:Literal></div>


<SalesLogix:SmartPartToolsContainer runat="server" ID="AddContactToEmailCampaign_RTools" ToolbarLocation="right">
    <SalesLogix:PageLink ID="lnkAddContactToEmailCampaignHelp" runat="server" LinkType="HelpFileName" ToolTip="<%$ resources: Portal, Help_ToolTip %>" Target="MCWebHelp" NavigateUrl="AddContactToEmailCampaign" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Help_16x16"></SalesLogix:PageLink>
</SalesLogix:SmartPartToolsContainer>

<style type="text/css">
.tableWithId .slx-grid-cell-last 
 {
     display: none;
 }
 .addButton
 {
     display: none;
 }
</style>

<script type="text/javascript">
</script>
