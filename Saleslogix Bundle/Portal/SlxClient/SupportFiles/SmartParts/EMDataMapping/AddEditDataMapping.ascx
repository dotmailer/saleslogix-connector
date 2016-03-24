<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddEditDataMapping.ascx.cs"
    Inherits="AddEditDataMapping" %>
<%@ Register Assembly="Sage.SalesLogix.Client.GroupBuilder" Namespace="Sage.SalesLogix.Client.GroupBuilder"
    TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.PickList"
    TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls"
    TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.DependencyLookup"
    TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Lookup"
    TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Timeline"
    TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.HighLevelTypes" Namespace="Sage.SalesLogix.HighLevelTypes"
    TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.Platform.WebPortal" Namespace="Sage.Platform.WebPortal.SmartParts"
    TagPrefix="SalesLogix" %>
<table border="0" cellpadding="1" cellspacing="0" class="formtable">
    <col width="50%" />
    <col width="50%" />
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="lueLabel_lbl" AssociatedControlID="lueLabel" runat="server" Text="<%$ resources: lueLabel.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol lookup">
                <SalesLogix:LookupControl runat="server" ID="lueLabel" LookupEntityName="EMDataLabel"
                    LookupEntityTypeName="Sage.Entity.Interfaces.IEMDataLabel, Sage.Entity.Interfaces, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
                    AutoPostBack="true" SeedProperty="EMEmailAccount.Id">
                    <LookupProperties>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueLabel.LookupProperties.Name.PropertyHeader %>"
                            PropertyName="Name" PropertyType="System.String" PropertyFormat="None" PropertyFormatString=""
                            UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueLabel.LookupProperties.DataType.PropertyHeader %>"
                            PropertyName="DataType" PropertyType="System.String" PropertyFormat="None" PropertyFormatString=""
                            UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueLabel.LookupProperties.Description.PropertyHeader %>"
                            PropertyName="Description" PropertyType="System.String" PropertyFormat="None"
                            PropertyFormatString="" UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                    </LookupProperties>
                    <LookupPreFilters>
                    </LookupPreFilters>
                </SalesLogix:LookupControl>
            </div>
        </td>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="pklMapDirection_lbl" AssociatedControlID="pklMapDirection" runat="server"
                    Text="<%$ resources: pklMapDirection.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol picklist">
                <SalesLogix:PickListControl runat="server" ID="pklMapDirection" PickListName="EMMapDirection"
                    MustExistInList="false" Enabled="false" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="pklEntity_lbl" AssociatedControlID="pklEntity" runat="server" Text="<%$ resources: pklEntity.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol picklist">
                <SalesLogix:PickListControl runat="server" ID="pklEntity" PickListName="EMMappedEntity"
                    MustExistInList="false" Enabled="false" AutoPostBack="true" />
            </div>
        </td>
        <td>
            <div class="slxlabel">
                <asp:Label runat="server" ID="lblWarning" Text="<%$ resources: lblWarning.Text %>"
                    Visible="false" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="ddlFieldName_lbl" AssociatedControlID="ddlFieldName" runat="server"
                    Text="<%$ resources: pklFieldName.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol picklist">
                <asp:DropDownList runat="server" ID="ddlFieldName" Enabled="false" AutoPostBack="true" />
            </div>
        </td>
        <td rowspan="2">
            <asp:Literal ID="litFiltered" runat="server" Text="<%$ resources: litFiltered.Caption %>"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="ddlLinkedFieldName_lbl" AssociatedControlID="ddlLinkedFieldName" runat="server"
                    Text="<%$ resources: pklLinkedFieldName.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol picklist">
                <asp:DropDownList runat="server" ID="ddlLinkedFieldName" Enabled="false" />
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label ID="txtDescription_lbl" AssociatedControlID="txtDescription" runat="server"
                    Text="<%$ resources: txtDescription.Caption %>"></asp:Label>
            </div>
            <div class="twocoltextcontrol">
                <asp:TextBox runat="server" ID="txtDescription" Rows="3" TextMode="MultiLine" Columns="40"
                    Enabled="false" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="lueEmailAccount_lbl" AssociatedControlID="lueEmailAccount" runat="server"
                    Text="<%$ resources: lueEmailAccount.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol lookup">
                <SalesLogix:LookupControl runat="server" ID="lueEmailAccount" ReadOnly="true" EnableLookup="false"
                    LookupEntityName="EMEmailAccount" LookupEntityTypeName="Sage.Entity.Interfaces.IEMEmailAccount, Sage.Entity.Interfaces, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null">
                    <LookupProperties>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueEmailAccount.LookupProperties.AccountName.PropertyHeader %>"
                            PropertyName="AccountName" PropertyType="System.String" PropertyFormat="None"
                            PropertyFormatString="" UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueEmailAccount.LookupProperties.Description.PropertyHeader %>"
                            PropertyName="Description" PropertyType="System.String" PropertyFormat="None"
                            PropertyFormatString="" UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                    </LookupProperties>
                    <LookupPreFilters>
                    </LookupPreFilters>
                </SalesLogix:LookupControl>
            </div>
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <asp:Panel runat="server" ID="conButtons" CssClass="controlslist ">
                <asp:Button runat="server" ID="btnOK" Text="<%$ resources: btnOK.Caption %>" CssClass="slxbutton" />
                <asp:Button runat="server" ID="btnCancel" Text="<%$ resources: btnCancel.Caption %>"
                    CssClass="slxbutton" />
            </asp:Panel>
        </td>
    </tr>
</table>
<SalesLogix:SmartPartToolsContainer runat="server" ID="AddEditDataMapping_RTools"
    ToolbarLocation="right">
    <SalesLogix:PageLink ID="lnkAddEditDataMappingHelp" runat="server" LinkType="HelpFileName"
        ToolTip="<%$ resources: Portal, Help_ToolTip %>" Target="MCWebHelp" NavigateUrl="AddEditDataMapping"
        ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Help_16x16"></SalesLogix:PageLink>
</SalesLogix:SmartPartToolsContainer>

<script type="text/javascript">
</script>