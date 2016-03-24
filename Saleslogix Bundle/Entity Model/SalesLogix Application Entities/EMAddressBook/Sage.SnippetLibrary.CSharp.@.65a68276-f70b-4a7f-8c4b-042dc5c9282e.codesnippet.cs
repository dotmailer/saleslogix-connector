/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="65a68276-f70b-4a7f-8c4b-042dc5c9282e">
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
using System.IO;
using System.Xml.Serialization;
using Sage.Entity.Interfaces;
using Sage.Form.Interfaces;
using Sage.SalesLogix.API;
#endregion Usings

namespace Sage.BusinessRules.CodeSnippets
{
    public static partial class EMAddressBookBusinessRules
    {
        public static void OnAfterDeleteStep( IEMAddressBook emaddressbook)
        {
            // Record deletion in Deleted Items table
			IEMDeletedItem item = Sage.Platform.EntityFactory.Create<IEMDeletedItem>();
			item.EntityType = "EMAddressBook";
			item.EntityID = emaddressbook.Id.ToString();
			item.ProcessedByCampaignSync = false;
			item.ProcessedByEMSync = false;
			
			var book = new DeletedAddressBookDetails();
			book.EmailAccountId = emaddressbook.Ememailaccountid;
			book.EmailAddressBookId = emaddressbook.Id.ToString();
			book.EmailAddressBookName = emaddressbook.Name;
			
			XmlSerializer xs = new XmlSerializer(typeof(DeletedAddressBookDetails));
			StringWriter sw = new StringWriter();
			xs.Serialize(sw, book);
			string xml = sw.ToString();
			
			item.Data = xml;
			
			item.Save();
			
			//Delete reference in child collections
			Sage.Platform.Repository.IRepository<IEMCampaignSendSummary> cssRep = Sage.Platform.EntityFactory.GetRepository<IEMCampaignSendSummary>();
			Sage.Platform.Repository.IQueryable cssQry = (Sage.Platform.Repository.IQueryable)cssRep;
        	Sage.Platform.Repository.IExpressionFactory cssEf = cssQry.GetExpressionFactory();
			
			Sage.Platform.Repository.ICriteria cssCriteria = cssQry.CreateCriteria();
        	System.Collections.Generic.IList<IEMCampaignSendSummary> sendSummaries = cssCriteria.Add(cssEf.Eq("EMAddressBookID", emaddressbook.Id)).List<IEMCampaignSendSummary>();
			
			foreach (IEMCampaignSendSummary sendSummary in sendSummaries)
			{
				sendSummary.EMAddressBookID = null;
				sendSummary.EMAddressBookMemberID = null;
			}
			
			Sage.Platform.Repository.IRepository<IEMCampaignConClick> cccRep = Sage.Platform.EntityFactory.GetRepository<IEMCampaignConClick>();
			Sage.Platform.Repository.IQueryable cccQry = (Sage.Platform.Repository.IQueryable)cccRep;
			Sage.Platform.Repository.IExpressionFactory cccEf = cccQry.GetExpressionFactory();
			
			Sage.Platform.Repository.ICriteria cccCriteria = cccQry.CreateCriteria();
			System.Collections.Generic.IList<IEMCampaignConClick> clickSummaries = cccCriteria.Add(cccEf.Eq("EMAddressBookID", emaddressbook.Id)).List<IEMCampaignConClick>();
			
			foreach (IEMCampaignConClick clickSummary in clickSummaries)
			{
				clickSummary.EMAddressBookID = null;
				clickSummary.EMAddressBookMemberID = null;
			}
        }
    }
		
	public class DeletedAddressBookDetails
	{
		public string EmailAccountId { get; set; }
		public string EmailAddressBookId { get; set; }
		public string EmailAddressBookName { get; set; }
	}
}
