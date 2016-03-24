/*
 * This metadata is used by the Sage platform.  Do not remove.
<snippetHeader xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="c7350c1f-91ea-4c66-b341-af92703eea57">
 <assembly>Sage.SnippetLibrary.CSharp</assembly>
 <name>CountMembersStep</name>
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
using Sage.Platform;
using Sage.Platform.Repository;
#endregion Usings

namespace Sage.BusinessRules.CodeSnippets
{
    public static partial class EMAddressBookBusinessRules
    {
        public static void CountMembersStep( IEMAddressBook emaddressbook, out Int32 result)
        {
			var query = (IQueryable)EntityFactory.GetRepository<IEMAddressBookMember>();
            IExpressionFactory expressions = query.GetExpressionFactory();
            IProjections projections = query.GetProjectionsFactory();
			
            ICriteria criteria = query.CreateCriteria()
                    .SetProjection(projections.RowCount());
			criteria.Add(expressions.Eq("Emaddressbookid", emaddressbook.Id));
            
			int count = criteria.UniqueResult<int>();
			
			result = count;
        }
    }
}