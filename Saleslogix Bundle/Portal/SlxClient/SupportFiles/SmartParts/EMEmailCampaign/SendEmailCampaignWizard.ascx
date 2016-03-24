<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SendEmailCampaignWizard.ascx.cs" Inherits="SendEmailCampaignWizard" %>
<%@ Register Assembly="Sage.SalesLogix.Client.GroupBuilder" Namespace="Sage.SalesLogix.Client.GroupBuilder" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.PickList" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.DependencyLookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Lookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Timeline" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.HighLevelTypes" Namespace="Sage.SalesLogix.HighLevelTypes" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.Platform.WebPortal" Namespace="Sage.Platform.WebPortal.SmartParts" TagPrefix="SalesLogix" %>


<div runat="server" class="pageContainer" id="page1" visible="true">
    <div class="titleContainer">
        <p class="mainTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignTargets.MainTitle %>" /></p>
        <p class="subTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignTargets.SubTitle %>" /></p>
    </div>
    <hr />
    <div class="contentContainer">
        <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignDetails.Title %>" /></p>
        <table class="twoColTable">
            <col width="40%" />
            <col width="60%" />
            <tr>
                <td><asp:Label runat="server" ID="lblCampaignName" Text="<%$ resources: lblCampaignName.Caption %>" /></td>
                <td><asp:Label runat="server" ID="CampaignName" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblSubject" Text="<%$ resources: lblSubject.Caption %>" /></td>
                <td><asp:Label runat="server" ID="Subject" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblFriendlyFromAddress" Text="<%$ resources: lblFriendlyFromAddress.Caption %>" /></td>
                <td><asp:Label runat="server" ID="FriendlyFromAddress" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblReplyAction" Text="<%$ resources: lblReplyAction.Caption %>" /></td>
                <td><asp:Label runat="server" ID="ReplyAction" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblCampaignType" Text="<%$ resources: lblCampaignType.Caption %>" /></td>
                <td><asp:Label runat="server" ID="CampaignType" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblReplyEmail" Text="<%$ resources: lblReplyEmail.Caption %>" /></td>
                <td><asp:Label runat="server" ID="ReplyEmail" CssClass="dataLabel" /></td>
            </tr>
        </table>
        <br />
        <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litTargetAddressBooks.Title %>" /></p>
        <SalesLogix:SlxGridView runat="server" ID="grdTargetAddressBooks" GridLines="None"
    AutoGenerateColumns="false" CellPadding="4" CssClass="datagrid tableWithId" PagerStyle-CssClass="gridPager"
    AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt" SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="true"
     ExpandableRows="True" ResizableColumns="True" AllowPaging="false" DataKeyNames="Id" Height="165px" >
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="List Name" SortExpression="List Name">
                <ItemTemplate>
                    <asp:Label ID="lblAddressBooName"  runat="server" Text='<%# Eval("EMAddressBook.Name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Total Members" SortExpression="New Count">
                <ItemTemplate>
                    <asp:Label ID="lblAddressBookTotalMembers" runat="server" Text='<%# GetMemberCount(Eval("EMAddressBook")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Available Members" SortExpression="Member Count">
                <ItemTemplate>
                    <asp:Label ID="lblAddressBookCount" runat="server" Text='<%# Eval("EMAddressBook.EmailServiceAddressBookCount") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Already Mailed" SortExpression="Already Mailed">
                <ItemTemplate>
                    <asp:CheckBox ID="chkAlreadyMailed" runat="server" Checked='<%# Convert.ToBoolean(Eval("Sent")) %>' Enabled="false" />
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
</div>
<div runat="server" class="pageContainer" id="page2" visible="false" >
    <div class="titleContainer">
        <p class="mainTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignTargetsCount.MainTitle %>" /></p>
        <p class="subTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignTargetsCount.SubTitle %>" /></p>
    </div>
    <hr />
    <div class="contentContainer">
        <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litTargetSummary.Title %>" /></p>
        <table class="twoColTable">
            <col width="60%" />
            <col width="40%" />
            <tr>
                <td><asp:Label runat="server" ID="lblTotalPotentialTargets" Text="<%$ resources: lblTotalPotentialTargets.Caption %>" /></td>
                <td><asp:Label runat="server" ID="TotalPotentialTargets" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblTargetsALreadySent" Text="<%$ resources: lblTargetsAlreadySent.Caption %>" /></td>
                <td><asp:Label runat="server" ID="TargetsAlreadySent" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblLessDuplicateTargets" Text="<%$ resources: lblLessDuplicateTargets.Caption %>" /></td>
                <td><asp:Label runat="server" ID="LessDuplicateTargets" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblLessTargetsDoNotSolicit" Text="<%$ resources: lblLessTargetsDoNotSolicit.Caption %>" /></td>
                <td><asp:Label runat="server" ID="LessTargetsDoNotSolicit" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblLessTargetsNoEmail" Text="<%$ resources: lblLessTargetsNoEmail.Caption %>" /></td>
                <td><asp:Label runat="server" ID="LessTargetsNoEmail" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblTotalUniqueTargets" Text="<%$ resources: lblTotalUniqueTargets.Caption %>" /></td>
                <td><asp:Label runat="server" ID="TotalUniqueTargets" CssClass="dataLabel" /></td>
            </tr>
        </table>
        <br />
        <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litSuppressionAnalysis.Title %>" /></p>
        <div id="suppressionChart" style="width: 100%; height: 200px;"></div>
    </div>
</div>
<div runat="server" class="pageContainer" id="page3" visible="false" >
    <div class="titleContainer">
        <p class="mainTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignOptions.MainTitle %>" /></p>
        <p class="subTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignOptions.SubTitle %>" /></p>
    </div>
    <hr />
    <div class="contentContainer">
        <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litSendTime.Title %>" /></p>
        <p><asp:Literal runat="server" Text="<%$ resources: litSendTime.Caption %>" /></p>
        <div id="sendTimeRadioGroup">
            <asp:RadioButtonList runat="server" ID="radgrpSendTime" RepeatDirection="Vertical" >
                <asp:ListItem Selected="True" Text="<%$ resources: radImmediately.Caption %>" Value="Immediately" />
                <asp:ListItem Text="<%$ resources: radScheduled.Caption %>" Value="Scheduled" />
            </asp:RadioButtonList>
        </div>
        <div id="sendTimeDatePicker">
        <SalesLogix:DateTimePicker runat="server" ID="dntSendTime" />
        </div>
    </div>
    <hr />
    <div runat="server" id="divSplitTest" visible="false">
        <div class="contentContainer">
            <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litSplitTesting.Title %>" /></p>
            <p><asp:Literal runat="server" Text="<%$ resources: litSplitTesting.Caption %>" /></p>
            <p><asp:Literal runat="server" Text="<%$ resources: litSplitTestingTargetPercentage.Caption %>" /></p>
            <div id="slider" class="claro"></div>
            <div id="sliderValue">
            <asp:TextBox runat="server" ID="txtTargetPercentage" Text="1" Width="25" ReadOnly="false" />%
            </div>
            <p style="clear:both;"><asp:Literal runat="server" Text="<%$ resources: litSplitTestingWaitHours.Caption %>" /></p>
            <asp:TextBox runat="server" ID="txtWaitHours" Width="25" Text="1" onpaste="return false" oncut="return false" />
            <p><asp:Literal runat="server" Text="<%$ resources: litSplitTestingMetric.Caption %>" /></p>
            <asp:DropDownList runat="server" ID="ddlMetric" >
                <asp:ListItem Text="<%$ resources: ddlMetricOpens.Caption %>" Value="Opens" />
                <asp:ListItem Text="<%$ resources: ddlMetricClicks.Caption %>" Value="Clicks" />
            </asp:DropDownList>
        </div>
        <hr />
    </div>
    <div class="contentContainer">
        <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litSummary.Title %>" /></p>
        <span id="summarySendTime" style="margin-right:3px;"></span>
        <span id="summarySplit"></span>
        
        
        
        
        <span id="immediately" style="display:none;"><asp:Literal ID="litSendSummaryImmediately" runat="server" Text="<%$ resources: litSendSummaryImmediately.Caption %>" /></span>
        <span id="scheduled" style="display:none;"><asp:Literal ID="litSendSummaryScheduled" runat="server" Text="<%$ resources: litSendSummaryScheduled.Caption %>" /></span>
        <span id="notSplit" style="display:none;"><asp:Literal ID="litSendSummaryNoSplit" runat="server" Text="<%$ resources: litSendSummaryNoSplit.Caption %>" /></span>
        <span id="split" style="display:none;"><asp:Literal ID="litSendSummarySplit" runat="server" Text="<%$ resources: litSendSummarySplit.Caption %>" /></span>
        <span id="totalUniqueTargets" style="display:none;"><asp:Literal ID="litTotalUniqueTargets" runat="server" /></span>
    </div>
</div>

<div runat="server" class="pageContainer" id="page4" visible="false" >
    <div class="titleContainer">
        <p class="mainTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignSummary.MainTitle %>" /></p>
        <p class="subTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignSummary.SubTitle %>" /></p>
    </div>
    <hr />
    <div class="contentContainer">
        <p class="contentTitle"><asp:Literal runat="server" Text="<%$ resources: litCheckOptionsSummary.Title %>" /></p>
        <table class="twoColTable">
            <col width="40%" />
            <col width="60%" />
            <tr>
                <td><asp:Label runat="server" ID="lblSummaryCampaignName" Text="<%$ resources: lblSummaryCampaignName.Caption %>" /></td>
                <td><asp:Label runat="server" ID="SummaryCampaignName" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblSummarySubject" Text="<%$ resources: lblSummarySubject.Caption %>" /></td>
                <td><asp:Label runat="server" ID="SummarySubject" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblSummaryFriendlyFromAddress" Text="<%$ resources: lblSummaryFriendlyFromAddress.Caption %>" /></td>
                <td><asp:Label runat="server" ID="SummaryFriendlyFromAddress" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblSummaryReplyAction" Text="<%$ resources: lblSummaryReplyAction.Caption %>" /></td>
                <td><asp:Label runat="server" ID="SummaryReplyAction" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblSummaryCampaignType" Text="<%$ resources: lblSummaryCampaignType.Caption %>" /></td>
                <td><asp:Label runat="server" ID="SummaryCampaignType" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblSummaryReplyEmail" Text="<%$ resources: lblSummaryReplyEmail.Caption %>" /></td>
                <td><asp:Label runat="server" ID="SummaryReplyEmail" CssClass="dataLabel" /></td>
            </tr>
            <tr>
                <td><asp:Label runat="server" ID="lblSummarySendTime" Text="<%$ resources: lblSummarySendTime.Caption %>" /></td>
                <td><asp:Label runat="server" ID="SummarySendTime" CssClass="dataLabel" /></td>
            </tr>
        </table>
    </div>
</div>

<div runat="server" class="pageContainer" id="page5" visible="false" >
    <div class="titleContainer">
        <p class="mainTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignCompletion.MainTitle %>" /></p>
        <p class="subTitle"><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignCompletion.SubTitle %>" /></p>
    </div>
    <hr />
    <div class="contentContainer">
        <p><asp:Literal runat="server" Text="<%$ resources: litEmailCampaignCompletion.Caption %>" /></p>
    </div>
</div>
 
 

<div class="buttonContainer">

    <asp:Button runat="server" CssClass="slxbutton" ID="btnBack" Text="Back" />
    <asp:Button runat="server" CssClass="slxbutton" ID="btnNext" Text="Next" />
    <asp:Button runat="server" CssClass="slxbutton" ID="btnSendCampaign" Text="Send Campaign" />
    <asp:Button runat="server" CssClass="slxbutton" ID="btnCancel" Text="Cancel"/>
</div>

 <SalesLogix:SmartPartToolsContainer runat="server" ID="SendEmailCampaignWizard_RTools" ToolbarLocation="right">
  
 </SalesLogix:SmartPartToolsContainer>


 <style type="text/css">
 /* Added for v8 */
 .dialog-workspace-content {
     height: 520px;
 }
 
 p {
     margin: 0 0 0 0;
 }
 /* ------------ */
 
 .mainTitle
 {
     font-size: 1.33em;
     font-weight:bold;
     padding: 10px 0px 0px 10px;
     margin: 0 0 0 0; /* Added for v8 */
 }
 .subTitle 
 {
     padding-left: 10px;
     margin: 0 0 0 0; /* Added for v8 */
 }
 .contentContainer
 {
     padding:0px 10px 0px 10px;
     margin: 0 0 0 0; /* Added for v8 */
 }
 .contentTitle
 {
     font-weight:bold;
     padding-bottom:5px;
     margin: 0 0 0 0; /* Added for v8 */
 }
 .twoColTable
 {
     width: 100%;
     border: 1px solid #000;
     margin: 0 0 0 0; /* Added for v8 */
 }
 .twoColTable td
 {
     padding: 3px 10px 3px 10px;
 }
 .dataLabel
 {
     font-weight:bold;
 }
 .buttonContainer
 {
     position:absolute;
     top: 500px;
     right: 10px;
 }
 .tableWithId .slx-grid-cell-last 
 {
     display: none;
 }
 #sendTimeRadioGroup
 {
     float: left;
 }
#sendTimeDatePicker
{
    float: left;
    padding: 17px 0px 7px 20px;
}
#sendTimeDatePicker input
{
    width:250px;
}
#slider, #sliderValue 
{
    float:left;
}
#grdTargetAddressBooksContainer
{
    height:20px;
}
 </style>
