/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="d62c6ff9-37d3-4f2c-a31c-4a853c7d04bb">
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
using Sage.Platform.Repository;
using Sage.Platform;
#endregion Usings

namespace Sage.BusinessRules.CodeSnippets
{
    public static partial class EMAddressBookBusinessRules
    {
        public static void OnBeforeInsertStep( IEMAddressBook emaddressbook,  ISession session)
        {
			if (string.IsNullOrEmpty(emaddressbook.Name) || string.IsNullOrEmpty(emaddressbook.Name.Trim()))
			{
				throw new Sage.Platform.Application.ValidationException("Email Address Book Name is required");
			}
			
			if (emaddressbook.EMEmailAccount == null)
			{
				throw new Sage.Platform.Application.ValidationException("Email Account is required");
			}
			
            if (emaddressbook.Name.ToUpper() == "TEST")
			{
				throw new Sage.Platform.Application.ValidationException("Invalid Address Book Name 'Test'.");
			}
			
			IRepository<IEMAddressBook> rep = EntityFactory.GetRepository<IEMAddressBook>();
			IQueryable qry = (IQueryable)rep;
			IExpressionFactory ef = qry.GetExpressionFactory();
			Sage.Platform.Repository.ICriteria criteria = qry.CreateCriteria();
			System.Collections.Generic.IList<IEMAddressBook> existingAddrBooks = criteria.Add(ef.Eq("Name", emaddressbook.Name)).Add(ef.Eq("EMEmailAccount.Id", emaddressbook.EMEmailAccount.Id)).List<IEMAddressBook>();
			
			if (existingAddrBooks.Count > 0)
			{
				throw new Sage.Platform.Application.ValidationException("An Address Book with this Name already exists!");
			}
        }
    }
}
