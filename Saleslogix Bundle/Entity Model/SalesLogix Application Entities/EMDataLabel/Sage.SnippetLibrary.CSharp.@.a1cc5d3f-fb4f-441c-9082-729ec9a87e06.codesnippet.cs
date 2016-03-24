/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="a1cc5d3f-fb4f-441c-9082-729ec9a87e06">
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
    public static partial class EMDataLabelBusinessRules
    {
        public static void OnAfterDeleteStep( IEMDataLabel emdatalabel)
        {
            // Record deletion in Deleted Items table
			IEMDeletedItem item = Sage.Platform.EntityFactory.Create<IEMDeletedItem>();
			item.EntityType = "EMDataLabel";
			item.EntityID = emdatalabel.Id.ToString();
			item.ProcessedByCampaignSync = false;
			item.ProcessedByEMSync = false;
			
			var delLabel = new DeletedLabelDetails();
			delLabel.EmailAccountId = emdatalabel.Ememailaccountid;
			delLabel.DataLabelId = emdatalabel.Id.ToString();
			delLabel.Name = emdatalabel.Name;
			delLabel.SyncWithEmailService = emdatalabel.SyncWithEmailService;
			
			XmlSerializer xs = new XmlSerializer(typeof(DeletedLabelDetails));
			StringWriter sw = new StringWriter();
			xs.Serialize(sw, delLabel);
			string xml = sw.ToString();
			
			item.Data = xml;
			
			item.Save();
        }
    }
		
	public class DeletedLabelDetails
    {
        public string EmailAccountId { get; set; }
        public string DataLabelId { get; set; }
        public string Name { get; set; }
        public bool? SyncWithEmailService { get; set; }
    }
}
