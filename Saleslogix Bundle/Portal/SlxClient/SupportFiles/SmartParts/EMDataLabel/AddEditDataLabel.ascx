<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddEditDataLabel.ascx.cs"
    Inherits="AddEditDataLabel" %>
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
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label ID="txtName_lbl" AssociatedControlID="txtName" runat="server" Text="<%$ resources: txtName.Caption %>"></asp:Label>
            </div>
            <div class="twocoltextcontrol">
                <asp:TextBox runat="server" ID="txtName" Rows="1" MaxLength="20" />
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label ID="pklDataType_lbl" AssociatedControlID="pklDataType" runat="server"
                    Text="<%$ resources: pklDataType.Caption %>"></asp:Label>
            </div>
            <div class="twocoltextcontrol picklist">
                <SalesLogix:PickListControl runat="server" ID="pklDataType" PickListName="EMDataTypes"
                    MustExistInList="false" AutoPostBack="true" />
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label runat="server" ID="lblIsPrivate" Text="<%$ resources: lblIsPrivate.Text %>" />
            </div>
            <div class="slxtwocollinkcontrol">
                <fieldset class="slxlabel radio" style="width: 53%;">
                    <asp:RadioButtonList runat="server" ID="radIsPrivate" RepeatDirection="Horizontal" Enabled="false">
                        <asp:ListItem Text="<%$ resources: radIsPrivate_item0.Text %>" Value="False" enabled="false"/>
                        <asp:ListItem Text="<%$ resources: radIsPrivate_item1.Text %>" Value="True" enabled="false"/>
                    </asp:RadioButtonList>
                </fieldset>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label ID="txtDefaultValue_lbl" runat="server" Text="<%$ resources: txtDefaultValue.Caption %>"></asp:Label>
                <%-- <asp:Label ID="txtDefaultValue_lbl" AssociatedControlID="txtDefaultValue" runat="server" Text="<%$ resources: txtDefaultValue.Caption %>" ></asp:Label> --%>
            </div>
            <div class="slxtwocollinkcontrol">
                <%-- Possible data type fields --%>
                <asp:TextBox runat="server" ID="txtDefaultString" Visible="false" />
                <SalesLogix:DateTimePicker runat="server" ID="calDefaultDate" Visible="false" DisplayTime="false" Timeless="true" />
                
                <asp:TextBox ID="numDefaultNumeric" runat="server" Visible="false" Width="100" />
                <%-- <SalesLogix:NumericControl runat="server" ID="numDefaultNumeric" Visible="false" Width="100" /> --%>
                
                <%-- Actual field that will be used --%>
                <asp:TextBox runat="server" ID="txtDefaultValue" Visible="false" />
                <%--   <SalesLogix:SLXCheckBox runat="server" ID="chkDefaultBoolean" Visible="false" Width="20" /> --%>
                <fieldset class="slxlabel radio">
                    <asp:RadioButtonList runat="server" ID="radDefaultBoolean" RepeatDirection="Horizontal">
                        <%--   TODO: Use Resource strings! --%>
                        <asp:ListItem Text="False" Value="False" />
                        <asp:ListItem Text="True" Value="True" />
                    </asp:RadioButtonList>
                </fieldset>
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
                <asp:TextBox runat="server" ID="txtDescription" Rows="3" TextMode="MultiLine" Columns="40" />
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label ID="lueEmailAccount_lbl" AssociatedControlID="lueEmailAccount" runat="server"
                    Text="<%$ resources: lueEmailAccount.Caption %>"></asp:Label>
            </div>
            <div class="twocoltextcontrol lookup">
                <SalesLogix:LookupControl runat="server" ID="lueEmailAccount"  Enabled="false" EnableLookup="false"
                    LookupEntityName="EMEmailAccount" LookupEntityTypeName="Sage.Entity.Interfaces.IEMEmailAccount, Sage.Entity.Interfaces, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null">
                    <LookupProperties>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueEmailAccount.LookupProperties.AccountName.PropertyHeader %>"
                            PropertyName="AccountName" PropertyType="System.String" PropertyFormat="None"
                            UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueEmailAccount.LookupProperties.Description.PropertyHeader %>"
                            PropertyName="Description" PropertyType="System.String" PropertyFormat="None"
                            UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                    </LookupProperties>
                    <LookupPreFilters>
                    </LookupPreFilters>
                </SalesLogix:LookupControl>
            </div>
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