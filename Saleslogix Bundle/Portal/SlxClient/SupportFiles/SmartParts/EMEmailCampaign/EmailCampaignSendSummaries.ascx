<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailCampaignSendSummaries.ascx.cs" Inherits="EmailCampaignSendSummaries" %>
<%@ Register Assembly="Sage.SalesLogix.Client.GroupBuilder" Namespace="Sage.SalesLogix.Client.GroupBuilder" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.PickList" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.DependencyLookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Lookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Timeline" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.HighLevelTypes" Namespace="Sage.SalesLogix.HighLevelTypes" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.Platform.WebPortal" Namespace="Sage.Platform.WebPortal.SmartParts" TagPrefix="SalesLogix" %>

<div id="filterDiv">
        <table border="0" cellpadding="1" cellspacing="0" class="formtable">
            <col width="50%" />
            <col width="50%" />
            <tr>
                <td id="row1col1">
                    <div class=" lbl alignleft">
                        <asp:Label ID="lblFilter" AssociatedControlID="ddlFilter" runat="server" Text="<%$ resources: lblFilter.Caption %>" ></asp:Label>
                    </div>   
                    <div  class="textcontrol select"  > 
                        <asp:ListBox runat="server" ID="ddlFilter"  SelectionMode="Single" Rows="1" AutoPostBack="true"  >
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item0.Text %>" Value="All" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item1.Text %>" Value="Opened" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item2.Text %>" Value="Clicked" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item3.Text %>" Value="Viewed" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item4.Text %>" Value="Replied" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item5.Text %>" Value="Forwarded" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item6.Text %>" Value="HardBounce" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item7.Text %>" Value="SoftBounce" />
                            <asp:ListItem  Text="<%$ resources: ddlFilter_item8.Text %>" Value="Unsubscribed" />
                        </asp:ListBox>
                    </div>
                </td>
                <td id="row1col2">
                    <div class=" lbl alignleft">
                        <asp:Label ID="lblView" AssociatedControlID="ddlView" runat="server" Text="<%$ resources: lblView.Caption %>" ></asp:Label>
                    </div>   
                    <div  class="textcontrol select"  > 
                        <asp:ListBox runat="server" ID="ddlView"  SelectionMode="Single" Rows="1" AutoPostBack="true" >
                            <asp:ListItem  Text="<%$ resources: ddlView_item0.Text %>" Value="Responses" />
                            <asp:ListItem  Text="<%$ resources: ddlView_item1.Text %>" Value="Contacts" />
                            <asp:ListItem  Text="<%$ resources: ddlView_item2.Text %>" Value="Leads" />
                        </asp:ListBox>
                    </div>
                </td>
            </tr>
        </table>
    </div>

<SalesLogix:SlxGridView runat="server" ID="grdCampaignSendSummaries_Responses" GridLines="None" AutoGenerateColumns="false"
 CellPadding="4" CssClass="datagrid" PagerStyle-CssClass="gridPager" AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt"
 SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="false" ExpandableRows="true" ResizableColumns="true"
 Visible="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="grdCampaignSendSummaries_Responses_Changing" >
    <Columns>
        <asp:TemplateField HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.EmailAddress.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Email runat="server" ID="grdCampaignSendSummaries_Responses_EmailAddress" DisplayMode="AsHyperlink" Text='<%# Eval("EmailAddress") %>' CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.DateSent.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:DateTimePicker runat="server" ID="grdCampaignSendSummaries_Responses_DateSent"  DisplayMode="AsText" DateTimeValue='<%# Eval("DateSent") %>'  CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="NumOpens" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.Opens.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="NumViews" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.Views.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="NumClicks" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.Clicks.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="NumReplies" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.Replies.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="NumEstimatedForwards" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.EstimatedForwards.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="NumForwardToFriend" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.ForwardToFriend.ColumnHeading %>" ></asp:BoundField>
        <asp:CheckBoxField DataField="HardBounced" ReadOnly="True" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.HardBounced.ColumnHeading %>" ></asp:CheckBoxField>
        <asp:CheckBoxField DataField="SoftBounced" ReadOnly="True" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.SoftBounced.ColumnHeading %>" ></asp:CheckBoxField>
        <asp:CheckBoxField DataField="Unsubscribed" ReadOnly="True" HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.Unsubscribed.ColumnHeading %>" ></asp:CheckBoxField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.DateFirstOpened.ColumnHeading %>" >
        <itemtemplate>
            <SalesLogix:DateTimePicker runat="server" ID="grdCampaignSendSummaries_Responses_DateFirstOpened"  DisplayMode="AsText" DateTimeValue='<%# Eval("DateFirstOpened") %>'  CssClass=""  />
        </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.DateLastOpened.ColumnHeading %>" >
        <itemtemplate>
            <SalesLogix:DateTimePicker runat="server" ID="grdCampaignSendSummaries_Responses_DateLastOpened"  DisplayMode="AsText" DateTimeValue='<%# Eval("DateLastOpened") %>'  CssClass=""  />
        </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.LastSynchronised.ColumnHeading %>" >
        <itemtemplate>
            <SalesLogix:DateTimePicker runat="server" ID="grdCampaignSendSummaries_Responses_LastSynchronised"  DisplayMode="AsText" DateTimeValue='<%# Eval("EMEmailCampaign.LastSynchronised") %>'  CssClass=""  />
        </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.AddressBook.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignSendSummaries_Responses_AddressBook" runat="server" NavigateUrl="EMAddressBook" EntityId='<%# Eval("EMAddressBook.Id") %>' Text='<%# Eval("EMAddressBook.Name") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.Contact.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignSendSummaries_Responses_Contact" runat="server" NavigateUrl="Contact" EntityId='<%# Eval("SlxContactId") %>' Text='<%# ProcessEvalData("ContactFullName", Eval("SlxContactId"), "Contact") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.Lead.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignSendSummaries_Responses_Lead" runat="server" NavigateUrl="Lead" EntityId='<%# Eval("SlxLeadId") %>' Text='<%# ProcessEvalData("LeadFullName", Eval("SlxLeadId"), "Lead") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
    </Columns>
</SalesLogix:SlxGridView>

<SalesLogix:SlxGridView runat="server" ID="grdCampaignSendSummaries_Contacts" GridLines="None" AutoGenerateColumns="false"
 CellPadding="4" CssClass="datagrid" PagerStyle-CssClass="gridPager" AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt"
 SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="false" ExpandableRows="true" ResizableColumns="true"
 Visible="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="grdCampaignSendSummaries_Contacts_Changing" >
    <Columns>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Contacts.Contact.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignSendSummaries_Contacts_Contact" runat="server" NavigateUrl="Contact" EntityId='<%# Eval("SlxContactId") %>' Text='<%# ProcessEvalData("ContactFullName", Eval("SlxContactId"), "Contact") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<%$ resources: grdCampaignSendSummaries_Contacts.EmailAddress.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Email runat="server" ID="grdCampaignSendSummaries_Contacts_EmailAddress" DisplayMode="AsHyperlink" Text='<%# Eval("EmailAddress") %>' CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Contacts.Account.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignSendSummaries_Contacts_Account" runat="server" NavigateUrl="Account" EntityId='<%# ProcessEvalData("ContactAccountId", Eval("SlxContactId"), "Contact") %>' Text='<%# ProcessEvalData("ContactAccountName", Eval("SlxContactId"), "Contact") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Contacts.WorkPhone.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Phone ID="Phone1" runat="server" Text='<%# ProcessEvalData("ContactWorkPhone", Eval("SlxContactId"), "Contact") %>' DisplayAsLabel="true" ></SalesLogix:Phone>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.DateSent.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:DateTimePicker runat="server" ID="grdCampaignSendSummaries_Responses_DateSent"  DisplayMode="AsText" DateTimeValue='<%# Eval("DateSent") %>'  CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
    </Columns>
</SalesLogix:SlxGridView>

<SalesLogix:SlxGridView runat="server" ID="grdCampaignSendSummaries_Leads" GridLines="None" AutoGenerateColumns="false"
 CellPadding="4" CssClass="datagrid" PagerStyle-CssClass="gridPager" AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt"
 SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="false" ExpandableRows="true" ResizableColumns="true"
 Visible="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="grdCampaignSendSummaries_Leads_Changing" >
    <Columns>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Leads.Lead.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignSendSummaries_Leads_Lead" runat="server" NavigateUrl="Lead" EntityId='<%# Eval("SlxLeadId") %>' Text='<%# ProcessEvalData("LeadFullName", Eval("SlxLeadId"), "Lead") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Leads.Company.ColumnHeading %>" >
            <itemtemplate>
                <asp:Literal ID="Literal1" runat="server" Text='<%# ProcessEvalData("LeadCompanyName", Eval("SlxLeadId"), "Lead") %>' ></asp:Literal>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Leads.LeadSource.ColumnHeading %>" >
            <itemtemplate>
                <asp:Literal ID="Literal1" runat="server" Text='<%# ProcessEvalData("LeadLeadSource", Eval("SlxLeadId"), "Lead") %>' ></asp:Literal>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<%$ resources: grdCampaignSendSummaries_Leads.EmailAddress.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Email runat="server" ID="grdCampaignSendSummaries_Leads_EmailAddress" DisplayMode="AsHyperlink" Text='<%# Eval("EmailAddress") %>' CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignSendSummaries_Responses.DateSent.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:DateTimePicker runat="server" ID="grdCampaignSendSummaries_Responses_DateSent"  DisplayMode="AsText" DateTimeValue='<%# Eval("DateSent") %>'  CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
    </Columns>
</SalesLogix:SlxGridView>

 <SalesLogix:SmartPartToolsContainer runat="server" ID="EmailCampaignSendSummaries_RTools" ToolbarLocation="right">
  <SalesLogix:PageLink ID="lnkEmailCampaignSendSummariesHelp" runat="server" LinkType="HelpFileName" ToolTip="<%$ resources: Portal, Help_ToolTip %>" Target="MCWebHelp" NavigateUrl="EmailCampaignSendSummaries" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Help_16x16"></SalesLogix:PageLink>
 </SalesLogix:SmartPartToolsContainer>

<script type="text/javascript">
</script>
