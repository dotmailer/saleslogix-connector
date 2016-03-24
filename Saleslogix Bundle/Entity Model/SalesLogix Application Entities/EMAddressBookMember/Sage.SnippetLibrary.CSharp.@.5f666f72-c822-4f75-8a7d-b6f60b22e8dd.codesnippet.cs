/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="5f666f72-c822-4f75-8a7d-b6f60b22e8dd">
 <assembly>Sage.SnippetLibrary.CSharp</assembly>
 <name>BuildDeletedItemObjectStep</name>
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
    public static partial class EMAddressBookMemberBusinessRules
    {
        public static void BuildDeletedItemObjectStep( IEMAddressBookMember emaddressbookmember, out IEMDeletedItem result)
        {
            IEMDeletedItem item = Sage.Platform.EntityFactory.Create<IEMDeletedItem>();
			item.EntityType = "EMAddressBookMember";
			item.EntityID = emaddressbookmember.Id.ToString();
			item.ProcessedByCampaignSync = false;
			item.ProcessedByEMSync = false;
			string id = string.Empty;
			if (string.Equals(emaddressbookmember.SlxMemberType, "LEAD", StringComparison.OrdinalIgnoreCase))
			{
				id = emaddressbookmember.SlxLeadID;
			}
			else if (string.Equals(emaddressbookmember.SlxMemberType, "CONTACT", StringComparison.OrdinalIgnoreCase))
			{
				id = emaddressbookmember.SlxContactID;
			}
			
			// Note: The DeletedMemberDetails class is defined in the EmAddressBookMember OnAfterDelete event
			var member = new DeletedMemberDetails();
			member.MemberType = emaddressbookmember.SlxMemberType;
			member.MemberId = id;
			member.AddressBookId = emaddressbookmember.Emaddressbookid;
			member.EmailAddress = emaddressbookmember.LastSyncedEmailAddress;
			
			XmlSerializer xs = new XmlSerializer(typeof(DeletedMemberDetails));
			StringWriter sw = new StringWriter();
			xs.Serialize(sw, member);
			string xml = sw.ToString();
			item.Data = xml;
			
			result = item;
        }
    }
}