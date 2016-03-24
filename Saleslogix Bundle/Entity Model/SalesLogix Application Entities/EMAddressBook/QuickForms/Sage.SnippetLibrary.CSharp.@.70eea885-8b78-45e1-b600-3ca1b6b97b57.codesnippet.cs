/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="70eea885-8b78-45e1-b600-3ca1b6b97b57">
 <assembly>Sage.SnippetLibrary.CSharp</assembly>
 <name>OnLoadStep</name>
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
    public static partial class EmailAddressBookDetailsEventHandlers
    {
        public static void OnLoadStep( IEmailAddressBookDetails form,  EventArgs args)
        {
            // Once a sync has occured, do not allow change of certain fields
			// (as they are populated by sync service)
			if (form.dntLastSynchronised.DateTimeValue != null)
			{
				form.lueEmailAccount.Enabled = false;
				form.txtEmailServAddrBookID.Enabled = false;
				form.txtEmailServAddrBookName.Enabled = false;
			}
        }
    }
}
