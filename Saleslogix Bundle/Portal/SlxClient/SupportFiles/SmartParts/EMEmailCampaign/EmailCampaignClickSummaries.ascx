<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailCampaignClickSummaries.ascx.cs" Inherits="EmailCampaignClickSummaries" %>
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
                        <asp:ListBox runat="server" ID="ddlFilter"  SelectionMode="Single" Rows="1" AutoPostBack="true" ></asp:ListBox>
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

<SalesLogix:SlxGridView runat="server" ID="grdCampaignClickSummaries_Responses" GridLines="None" AutoGenerateColumns="false"
 CellPadding="4" CssClass="datagrid" PagerStyle-CssClass="gridPager" AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt"
 SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="false" ExpandableRows="true" ResizableColumns="true"
 Visible="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="grdCampaignClickSummaries_Responses_Changing" >
    <Columns>
        <asp:TemplateField HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.EmailAddress.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Email runat="server" ID="grdCampaignClickSummaries_Responses_EmailAddress" DisplayMode="AsHyperlink" Text='<%# Eval("EmailAddress") %>' CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.DateClicked.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:DateTimePicker runat="server" ID="grdCampaignClickSummaries_Responses_DateClicked"  DisplayMode="AsText" DateTimeValue='<%#  Eval("DateClicked")  %>'  CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Keyword" HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.Keyword.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="URL" HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.URL.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="IPAddress" HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.IPAddress.ColumnHeading %>" ></asp:BoundField>
        <asp:BoundField DataField="UserAgent" HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.UserAgent.ColumnHeading %>" ></asp:BoundField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.AddressBook.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="PageLink1" runat="server" NavigateUrl="EMAddressBook" EntityId='<%# Eval("EMAddressBook.Id") %>'     Text='<%# Eval("EMAddressBook.Name") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.Contact.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignClickSummaries_Responses_Contact" runat="server" NavigateUrl="Contact" EntityId='<%# Eval("SlxContactId") %>' Text='<%# ProcessEvalData("ContactFullName", Eval("SlxContactId"), "Contact") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.Lead.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignSendSummaries_Responses_Lead" runat="server" NavigateUrl="Lead" EntityId='<%# Eval("SlxLeadId") %>' Text='<%# ProcessEvalData("LeadFullName", Eval("SlxLeadId"), "Lead") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
    </Columns>
</SalesLogix:SlxGridView>

<SalesLogix:SlxGridView runat="server" ID="grdCampaignClickSummaries_Contacts" GridLines="None" AutoGenerateColumns="false"
 CellPadding="4" CssClass="datagrid" PagerStyle-CssClass="gridPager" AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt"
 SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="false" ExpandableRows="true" ResizableColumns="true"
 Visible="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="grdCampaignClickSummaries_Contacts_Changing" >
    <Columns>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Contacts.Contact.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignClickSummaries_Contacts_Contact" runat="server" NavigateUrl="Contact" EntityId='<%# Eval("SlxContactId") %>' Text='<%# ProcessEvalData("ContactFullName", Eval("SlxContactId"), "Contact") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<%$ resources: grdCampaignClickSummaries_Contacts.EmailAddress.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Email runat="server" ID="grdCampaignClickSummaries_Contacts_EmailAddress" DisplayMode="AsHyperlink" Text='<%# ProcessEvalData("ContactEmail", Eval("SlxContactId"), "Contact") %>' CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Contacts.Account.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignClickSummaries_Contacts_Account" runat="server" NavigateUrl="Account" EntityId='<%# ProcessEvalData("ContactAccountId", Eval("SlxContactId"), "Contact") %>' Text='<%# ProcessEvalData("ContactAccountName", Eval("SlxContactId"), "Contact") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Contacts.WorkPhone.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Phone ID="phnContactWorkPhone" runat="server" Text='<%# ProcessEvalData("ContactWorkPhone", Eval("SlxContactId"), "Contact") %>' DisplayAsLabel="true" ></SalesLogix:Phone>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.DateClicked.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:DateTimePicker runat="server" ID="grdCampaignClickSummaries_Responses_DateClicked"  DisplayMode="AsText" DateTimeValue='<%#  Eval("DateClicked")  %>'  CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
    </Columns>
</SalesLogix:SlxGridView>

<SalesLogix:SlxGridView runat="server" ID="grdCampaignClickSummaries_Leads" GridLines="None" AutoGenerateColumns="false"
 CellPadding="4" CssClass="datagrid" PagerStyle-CssClass="gridPager" AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt"
 SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="false" ExpandableRows="true" ResizableColumns="true"
 Visible="false" AllowPaging="true" PageSize="10" OnPageIndexChanging="grdCampaignClickSummaries_Leads_Changing" >
    <Columns>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Leads.Lead.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:PageLink ID="grdCampaignClickSummaries_Leads_Lead" runat="server" NavigateUrl="Lead" EntityId='<%# Eval("SlxLeadId") %>' Text='<%# ProcessEvalData("LeadFullName", Eval("SlxLeadId"), "Lead") %>' LinkType="EntityAlias"></SalesLogix:PageLink>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Leads.Company.ColumnHeading %>" >
            <itemtemplate>
                <asp:Literal ID="Literal1" runat="server" Text='<%# ProcessEvalData("LeadCompanyName", Eval("SlxLeadId"), "Lead") %>' ></asp:Literal>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Leads.LeadSource.ColumnHeading %>" >
            <itemtemplate>
                <asp:Literal ID="Literal1" runat="server" Text='<%# ProcessEvalData("LeadLeadSource", Eval("SlxLeadId"), "Lead") %>' ></asp:Literal>
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<%$ resources: grdCampaignClickSummaries_Leads.EmailAddress.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:Email runat="server" ID="grdCampaignClickSummaries_Leads_EmailAddress" DisplayMode="AsHyperlink" Text='<%# ProcessEvalData("LeadEmail", Eval("SlxLeadId"), "Lead") %>' CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
        <asp:TemplateField   HeaderText="<%$ resources: grdCampaignClickSummaries_Responses.DateClicked.ColumnHeading %>" >
            <itemtemplate>
                <SalesLogix:DateTimePicker runat="server" ID="grdCampaignClickSummaries_Responses_DateClicked"  DisplayMode="AsText" DateTimeValue='<%#  Eval("DateClicked")  %>'  CssClass=""  />
            </itemtemplate>
        </asp:TemplateField>
    </Columns>
</SalesLogix:SlxGridView>

 <SalesLogix:SmartPartToolsContainer runat="server" ID="EmailCampaignClickSummaries_RTools" ToolbarLocation="right">
  <SalesLogix:PageLink ID="lnkEmailCampaignClickSummariesHelp" runat="server" LinkType="HelpFileName" ToolTip="<%$ resources: Portal, Help_ToolTip %>" Target="MCWebHelp" NavigateUrl="EmailCampaignClickSummaries" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=Help_16x16"></SalesLogix:PageLink>
 </SalesLogix:SmartPartToolsContainer>

<script type="text/javascript">
</script>
