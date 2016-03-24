/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="422aa5ae-3cbf-4e76-98ca-9ce96a199491">
 <assembly>Sage.SnippetLibrary.CSharp</assembly>
 <name>OnAfterDeleteStep</name>
 <references>
  <reference>
   <assemblyName>Sage.Entity.Interfaces.dll</assemblyName>
   <hintPath>%BASEBUILDPATH%\interfaces\bin\Sage.Entity.Interfaces.dll</hintPath>
  </reference>
  <reference>
   <assemblyName>Sage.Form.Interfaces.dll</assemblyName>
   <hintPath>%BASEBUILDPATH%\formInterfaces\bin\Sage.Form.Interfaces.dll</hintPath>
  </reference>
  <reference>
   <assemblyName>Sage.Platform.dll</assemblyName>
   <hintPath>%BASEBUILDPATH%\assemblies\Sage.Platform.dll</hintPath>
  </reference>
  <reference>
   <assemblyName>Sage.SalesLogix.API.dll</assemblyName>
  </reference>
 </references>
</snippetHeader>
*/

#region Usings
using System;
using Sage.Entity.Interfaces;
using Sage.Form.Interfaces;
using Sage.SalesLogix.API;
#endregion Usings

namespace Sage.BusinessRules.CodeSnippets
{
    public static partial class EMAddressBookMemberBusinessRules
    {
        public static void OnAfterDeleteStep( IEMAddressBookMember emaddressbookmember)
        {
            // Only perform extra processing if we are not in a cascade delete from the parent address book
            Sage.SalesLogix.Orm.EntityBase addrBookBase = emaddressbookmember.EMAddressBook as Sage.SalesLogix.Orm.EntityBase;
            if (addrBookBase == null
                || (addrBookBase.PersistentState != Platform.Orm.Interfaces.PersistentState.Deleted
                && addrBookBase.PersistentState != Platform.Orm.Interfaces.PersistentState.Deleting))
            {
	            // Record deletion in Deleted Items table
				IEMDeletedItem item = emaddressbookmember.BuildDeletedItemObject();
				item.Save();

				//Delete reference in child collections
				Sage.Platform.Repository.IRepository<IEMCampaignSendSummary> cssRep = Sage.Platform.EntityFactory.GetRepository<IEMCampaignSendSummary>();
				Sage.Platform.Repository.IQueryable cssQry = (Sage.Platform.Repository.IQueryable)cssRep;
	        	Sage.Platform.Repository.IExpressionFactory cssEf = cssQry.GetExpressionFactory();
				Sage.Platform.Repository.ICriteria cssCriteria = cssQry.CreateCriteria();
	        	System.Collections.Generic.IList<IEMCampaignSendSummary> sendSummaries = cssCriteria.Add(cssEf.Eq("EMAddressBookMemberID", emaddressbookmember.Id)).List<IEMCampaignSendSummary>();
				foreach (IEMCampaignSendSummary sendSummary in sendSummaries)
				{
					sendSummary.EMAddressBookMemberID = null;
				}
				
				Sage.Platform.Repository.IRepository<IEMCampaignConClick> cccRep = Sage.Platform.EntityFactory.GetRepository<IEMCampaignConClick>();
				Sage.Platform.Repository.IQueryable cccQry = (Sage.Platform.Repository.IQueryable)cccRep;
				Sage.Platform.Repository.IExpressionFactory cccEf = cccQry.GetExpressionFactory();
				Sage.Platform.Repository.ICriteria cccCriteria = cccQry.CreateCriteria();
				System.Collections.Generic.IList<IEMCampaignConClick> clickSummaries = cccCriteria.Add(cccEf.Eq("EMAddressBookMemberID", emaddressbookmember.Id)).List<IEMCampaignConClick>();
				foreach (IEMCampaignConClick clickSummary in clickSummaries)
				{
					clickSummary.EMAddressBookMemberID = null;
				}
			}
        }
    }
		
	// N.B. This utility class is also used in the BuildDeletedItemOject rule
	public class DeletedMemberDetails
	{
		public string MemberType {get; set;}
		public string MemberId {get; set;}
		public string AddressBookId {get; set;}
        public string EmailAddress { get; set; }
	}
}
