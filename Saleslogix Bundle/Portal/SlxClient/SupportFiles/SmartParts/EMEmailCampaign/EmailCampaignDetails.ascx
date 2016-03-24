<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailCampaignDetails.ascx.cs"
    Inherits="EmailCampaignDetails" %>
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
                <asp:Label ID="txtCampaignName_lbl" AssociatedControlID="txtCampaignName" runat="server"
                    Text="<%$ resources: txtCampaignName.Caption %>"></asp:Label>
            </div>
            <div class="twocoltextcontrol">
                <asp:TextBox runat="server" ID="txtCampaignName" ReadOnly="true" Rows="1" MaxLength="255" />
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label ID="txtSubject_lbl" AssociatedControlID="txtSubject" runat="server" Text="<%$ resources: txtSubject.Caption %>"></asp:Label>
            </div>
            <div class="twocoltextcontrol">
                <asp:TextBox runat="server" ID="txtSubject" ReadOnly="true" Rows="1" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="pklStatus_lbl" AssociatedControlID="pklStatus" runat="server" Text="<%$ resources: pklStatus.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol picklist">
                <SalesLogix:PickListControl runat="server" ID="pklStatus" ReadOnly="true" MustExistInList="false" />
            </div>
        </td>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="txtReplyEmail_lbl" AssociatedControlID="txtReplyEmail" runat="server"
                    Text="<%$ resources: txtReplyEmail.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol">
                <asp:TextBox runat="server" ID="txtReplyEmail" ReadOnly="true" Rows="1" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="txtFriendlyFromName_lbl" AssociatedControlID="txtFriendlyFromName"
                    runat="server" Text="<%$ resources: txtFriendlyFromName.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol">
                <asp:TextBox runat="server" ID="txtFriendlyFromName" ReadOnly="true" Rows="1" />
            </div>
        </td>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="pklReplyAction_lbl" AssociatedControlID="pklReplyAction" runat="server"
                    Text="<%$ resources: pklReplyAction.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol picklist">
                <SalesLogix:PickListControl runat="server" ID="pklReplyAction" ReadOnly="true" MustExistInList="false" />
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
                <SalesLogix:LookupControl runat="server" ID="lueEmailAccount" Enabled="false" LookupEntityName="EMEmailAccount"
                    LookupEntityTypeName="Sage.Entity.Interfaces.IEMEmailAccount, Sage.Entity.Interfaces, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null">
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
    </tr>
    <tr>
        <td>
            <div class="slxlabel  alignleft checkbox">
                <SalesLogix:SLXCheckBox runat="server" ID="chkSyncWithEmailService" CssClass="checkbox "
                    Text="<%$ resources: chkSyncWithEmailService.Caption %>" />
            </div>
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="twocollbl alignleft">
                <asp:Label ID="lueSLXCampaign_lbl" AssociatedControlID="lueSLXCampaign" runat="server"
                    Text="<%$ resources: lueSLXCampaign.Caption %>"></asp:Label>
            </div>
            <div class="twocoltextcontrol lookup">
                <SalesLogix:LookupControl runat="server" ID="lueSLXCampaign" LookupEntityName="Campaign"
                    LookupEntityTypeName="Sage.Entity.Interfaces.ICampaign, Sage.Entity.Interfaces, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
                    LookupBindingMode="String" AllowClearingResult="true" AutoPostBack="true" EnableHyperLinking="true">
                    <LookupProperties>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueSLXCampaign.LookupProperties.CampaignName.PropertyHeader %>"
                            PropertyName="CampaignName" PropertyType="System.String" PropertyFormat="None"
                            PropertyFormatString="" UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                        <SalesLogix:LookupProperty PropertyHeader="<%$ resources: lueSLXCampaign.LookupProperties.CampaignType.PropertyHeader %>"
                            PropertyName="CampaignType" PropertyType="System.String" PropertyFormat="None"
                            PropertyFormatString="" UseAsResult="True" ExcludeFromFilters="False">
                        </SalesLogix:LookupProperty>
                    </LookupProperties>
                    <LookupPreFilters>
                    </LookupPreFilters>
                </SalesLogix:LookupControl>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="mainContentHeader2" style="width: 99.5%;"> <%-- Work-around For Firefox rendering bug that causes an spurious scroll bar To show --%>
                <span id="hrSendInfo">
                    <asp:Localize runat="server" Text="<%$ resources: hrSendInfo.Caption %>">Send Information</asp:Localize></span></div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="slxlabel">
                <asp:Label runat="server" ID="lblCampaignType" Text="<%$ resources: lblCampaignType.Text %>" />
            </div>
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <fieldset class="slxlabel radio" style="width: 75%;">
                <asp:RadioButtonList runat="server" ID="radCampaignType" RepeatDirection="Vertical">
                    <asp:ListItem Text="<%$ resources: radCampaignType_item0.Text %>" Value="Standard" />
                    <asp:ListItem Text="<%$ resources: radCampaignType_item1.Text %>" Value="New Member" />
                </asp:RadioButtonList>
            </fieldset>
        </td>
    </tr>
    <tr>
        <td>
            <div class="slxlabel  alignleft checkbox">
                <SalesLogix:SLXCheckBox runat="server" ID="chkAllowRepeatSends" CssClass="checkbox "
                    Text="<%$ resources: chkAllowRepeatSends.Caption %>" />
            </div>
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="dntLastSent_lbl" AssociatedControlID="dntLastSent" runat="server"
                    Text="<%$ resources: dntLastSent.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol datepicker">
                <SalesLogix:DateTimePicker runat="server" ID="dntLastSent" ReadOnly="true" />
            </div>
        </td>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="dntSendTime_lbl" AssociatedControlID="dntSendTime" runat="server"
                    Text="<%$ resources: dntSendTime.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol datepicker">
                <SalesLogix:DateTimePicker runat="server" ID="dntSendTime" ReadOnly="true" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="slxlabel  alignleft checkbox">
                <SalesLogix:SLXCheckBox runat="server" ID="chkIsSplitTest" CssClass="checkbox " Text="<%$ resources: chkIsSplitTest.Caption %>"
                    Enabled="false" />
            </div>
        </td>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="txtMetric_lbl" AssociatedControlID="txtMetric" runat="server"
                    Text="<%$ resources: txtMetric.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol">
                <asp:TextBox runat="server" ID="txtMetric" ReadOnly="true" Rows="1" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="txtSplitTestPercent_lbl" AssociatedControlID="txtSplitTestPercent"
                    runat="server" Text="<%$ resources: txtSplitTestPercent.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol">
                <asp:TextBox runat="server" ID="txtSplitTestPercent" ReadOnly="true" Rows="1" />
            </div>
        </td>
        <td>
            <div class=" lbl alignleft">
                <asp:Label ID="txtSplitTestOpenTime_lbl" AssociatedControlID="txtSplitTestOpenTime"
                    runat="server" Text="<%$ resources: txtSplitTestOpenTime.Caption %>"></asp:Label>
            </div>
            <div class="textcontrol">
                <asp:TextBox runat="server" ID="txtSplitTestOpenTime" ReadOnly="true" Rows="1" />
            </div>
        </td>
    </tr>
</table>
<SalesLogix:SmartPartToolsContainer runat="server" ID="EmailAccountDetails_CTools"
    ToolbarLocation="center">
    
</SalesLogix:SmartPartToolsContainer>
<SalesLogix:SmartPartToolsContainer runat="server" ID="EmailCampaignDetails_RTools"
    ToolbarLocation="right">
    <SalesLogix:GroupNavigator runat="server" ID="QFSLXGroupNavigator"></SalesLogix:GroupNavigator>
    <asp:ImageButton runat="server" ID="btnOpenDotMailer" AlternateText="<%$ resources: btnOpenDotMailer.Caption %>"
        ToolTip="<%$ resources: btnOpenDotMailer.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Email_Campaign_Settings22x16" />
    <asp:ImageButton runat="server" ID="btnOpenReports" AlternateText="<%$ resources: btnOpenReports.Caption %>"
        ToolTip="<%$ resources: btnOpenReports.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Email_Campaign_Report22x16" />
    <asp:ImageButton runat="server" ID="btnSynchronise" AlternateText="<%$ resources: btnSynchronise.Caption %>"
        ToolTip="<%$ resources: btnSynchronise.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=SyncBtn16" />
    <asp:ImageButton runat="server" ID="btnSendEmailCampaign" AlternateText="<%$ resources: btnSendEmailCampaign.Caption %>"
        ToolTip="<%$ resources: btnSendEmailCampaign.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=email_campaign16" />
    <asp:ImageButton runat="server" ID="btnSave" AlternateText="<%$ resources: btnSave.Caption %>"
        ToolTip="<%$ resources: btnSave.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Save_16x16" />
    <asp:ImageButton runat="server" ID="btnDelete" AlternateText="<%$ resources: btnDelete.Caption %>"  
        ToolTip="<%$ resources: btnDelete.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Delete_16x16" />
    <SalesLogix:PageLink ID="lnkEmailCampaignDetailsHelp" runat="server" LinkType="HelpFileName"
        ToolTip="<%$ resources: Portal, Help_ToolTip %>" Target="MCWebHelp" NavigateUrl="EmailCampaignDetails"
        ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Help_16x16"></SalesLogix:PageLink>
</SalesLogix:SmartPartToolsContainer>

<script type="text/javascript">
</script>