/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="96faf73b-6b62-4714-9ce1-f50d3d6e8a3e">
 <assembly>Sage.SnippetLibrary.CSharp</assembly>
 <name>OnBeforeInsertStep</name>
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
using NHibernate;
using Sage.SalesLogix.API;
#endregion Usings

namespace Sage.BusinessRules.CodeSnippets
{
    public static partial class EMCampaignAddressBookBusinessRules
    {
        public static void OnBeforeInsertStep( IEMCampaignAddressBook emcampaignaddressbook,  ISession session)
        {
			if (emcampaignaddressbook != null 
				&& emcampaignaddressbook.EMEmailCampaign != null
				&& emcampaignaddressbook.EMEmailCampaign.EMCampaignAddressBooks != null)
			{
				IEMEmailCampaign ememailcampaign = emcampaignaddressbook.EMEmailCampaign;
				foreach (IEMCampaignAddressBook addrBook in ememailcampaign.EMCampaignAddressBooks)
				{
					if (addrBook.EMAddressBook.Id == emcampaignaddressbook.EMAddressBook.Id)
					{
						throw new Sage.Platform.Application.ValidationException("This Address Book already exists in this Campaign");
					}
				}
			}
        }
    }
}
