/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="142cb956-5709-4b9a-8c0e-bdbae8fa70db">
 <assembly>Sage.SnippetLibrary.CSharp</assembly>
 <name>OnBeforeDeleteStep</name>
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
    public static partial class EMAddressBookBusinessRules
    {
        public static void OnBeforeDeleteStep( IEMAddressBook emaddressbook,  ISession session)
        {
			// For performance reasons we are handling the child deletion manually instead of using a cascade
			// This needs to happen OnBeforeDelete because otherwise the members have an invalid AddressBook
            session.CreateQuery("delete from EMAddressBookMember m where m.Emaddressbookid = (:bookId)")
                .SetParameter("bookId", emaddressbook.Id)
                .ExecuteUpdate();
        }
    }
}