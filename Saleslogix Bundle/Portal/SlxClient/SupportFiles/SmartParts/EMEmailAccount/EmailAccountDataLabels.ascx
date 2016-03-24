<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailAccountDataLabels.ascx.cs" Inherits="EmailAccountDataLabels" %>
<%@ Register Assembly="Sage.SalesLogix.Client.GroupBuilder" Namespace="Sage.SalesLogix.Client.GroupBuilder" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.PickList" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.DependencyLookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Lookup" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.Web.Controls" Namespace="Sage.SalesLogix.Web.Controls.Timeline" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.SalesLogix.HighLevelTypes" Namespace="Sage.SalesLogix.HighLevelTypes" TagPrefix="SalesLogix" %>
<%@ Register Assembly="Sage.Platform.WebPortal" Namespace="Sage.Platform.WebPortal.SmartParts" TagPrefix="SalesLogix" %>

  <SalesLogix:SlxGridView runat="server" ID="grdEMDataLabels" GridLines="None"
AutoGenerateColumns="false" CellPadding="4" CssClass="datagrid" PagerStyle-CssClass="gridPager"
AlternatingRowStyle-CssClass="rowdk" RowStyle-CssClass="rowlt" SelectedRowStyle-CssClass="rowSelected" ShowEmptyTable="true" EnableViewState="false"
 AllowPaging="true" PageSize="10" OnPageIndexChanging="grdEMDataLabelspage_changing"  ExpandableRows="True" ResizableColumns="True"  OnRowCommand="grdEMDataLabels_RowCommand" 
 DataKeyNames="InstanceId,Id" 
 AllowSorting="true" OnSorting="grdEMDataLabels_Sorting"  OnRowEditing="grdEMDataLabels_RowEditing"  OnRowDeleting="grdEMDataLabels_RowDeleting" OnRowDataBound="grdEMDataLabels_RowDataBound"  ShowSortIcon="true" >
<Columns>
 <asp:ButtonField CommandName="Edit" 
  Text="<%$ resources: grdEMDataLabels.0af7b0de-a898-4cc3-a941-ff468a01b492.Text %>"               >
  	    </asp:ButtonField>
     <asp:ButtonField CommandName="Delete" 
  Text="<%$ resources: grdEMDataLabels.4303f4e9-1885-4004-bb34-caf0fb76a42b.Text %>"                >
  	    </asp:ButtonField>
      <asp:BoundField DataField="Name" 
      HeaderText="<%$ resources: grdEMDataLabels.51ebbbc1-f985-49e8-9a75-37005839145b.ColumnHeading %>"       SortExpression="Name"    >
      </asp:BoundField>
    <asp:BoundField DataField="DataType" 
      HeaderText="<%$ resources: grdEMDataLabels.06f7f084-d1c4-40fa-bbbb-2bf655e088e5.ColumnHeading %>"       SortExpression="DataType"    >
      </asp:BoundField>
    <asp:CheckBoxField DataField="IsPrivate" ReadOnly="True" 
      HeaderText="<%$ resources: grdEMDataLabels.23d19d73-89f8-4f6e-9474-e7285ecd7878.ColumnHeading %>"        >
  	    </asp:CheckBoxField>
    <asp:CheckBoxField DataField="SyncWithEmailService" ReadOnly="True" 
      HeaderText="<%$ resources: grdEMDataLabels.4ab742dd-0fb0-41c4-a770-c84e268492a8.ColumnHeading %>"        >
  	    </asp:CheckBoxField>
 </Columns>
    <PagerSettings Mode="NumericFirstLast" FirstPageImageUrl="ImageResource.axd?scope=global&type=Global_Images&key=Start_16x16" LastPageImageUrl="ImageResource.axd?scope=global&type=Global_Images&key=End_16x16" />
</SalesLogix:SlxGridView>

<script runat="server" type="text/C#">

      
	</script>
 

 <SalesLogix:SmartPartToolsContainer runat="server" ID="EmailAccountDataLabels_RTools" ToolbarLocation="right">
    <asp:ImageButton runat="server" ID="btnAdd"
 AlternateText="<%$ resources: btnAdd.Caption %>" ToolTip="<%$ resources: btnAdd.ToolTip %>" ImageUrl="~/ImageResource.axd?scope=global&type=Global_Images&key=plus_16x16"  />
 
   </SalesLogix:SmartPartToolsContainer>

<script type="text/javascript">
</script>
