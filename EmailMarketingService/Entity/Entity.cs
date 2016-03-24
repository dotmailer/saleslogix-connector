namespace EmailMarketing.SalesLogix.Entities
{
    using System;
    using System.Reflection;
    using Sage.SData.Client.Extensions;

    public class Entity
    {
        public Entity()
        {
        }

        /// <summary>Gets or sets the data retrieved by the underlying data layer.  To be used when performing an update</summary>
        public object SourceData { get; set; }

        /// <summary>Gets or sets the unique ID of this record.  Do not edit this value.  It will be set automatically through the sData service.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets the UserId that created this record.  Do not edit this value.  It will be set automatically through the sData service.</summary></summary>
        [SchemaName("CreateUser")]
        public string CreateUserId { get; set; }

        /// <summary>Gets or sets the date and time that this record was created.  Do not edit this value.  It will be set automatically through the sData service.</summary></summary>
        [SchemaName("CreateDate")]
        public DateTime? CreateDate { get; set; }

        /// <summary>Gets or sets the UserId that last modified this record.  Do not edit this value.  It will be set automatically through the sData service.</summary></summary>
        [SchemaName("ModifyUser")]
        public string ModifyUserId { get; set; }

        /// <summary>Gets or sets the date and time that this record was last modified.  Do not edit this value.  It will be set automatically through the sData service.</summary></summary>
        [SchemaName("ModifyDate")]
        public DateTime? ModifyDate { get; set; }

        public SDataPayload BuildSDataPayload()
        {
            // Get the schema name of the entity
            Type t = GetType();
            object[] classAttrs = t.GetCustomAttributes(typeof(SchemaNameAttribute), false);

            if (classAttrs.Length == 1)
            {
                // Our custom attribute exists on this class
                SchemaNameAttribute classSchemaAttr = (SchemaNameAttribute)classAttrs[0];
                SDataPayload payload = new SDataPayload();
                payload.Namespace = "http://schemas.sage.com/dynamic/2007";
                payload.ResourceName = classSchemaAttr.SchemaName;

                PropertyInfo[] props = t.GetProperties();
                foreach (var prop in props)
                {
                    object[] propAttrs = prop.GetCustomAttributes(typeof(SchemaNameAttribute), false);

                    if (propAttrs.Length == 1)
                    {
                        // Our custom attribute exists on this property
                        SchemaNameAttribute propAttr = (SchemaNameAttribute)propAttrs[0];

                        object temp = prop.GetValue(this, null);
                        if (prop.PropertyType == typeof(DateTime?))
                        {
                            DateTime? tempNullableDateTime = (DateTime?)temp;

                            // N.B. All times are in UTC, but sdata does not understand a 'Z' suffix so use +00:00
                            payload.Values[propAttr.SchemaName] = tempNullableDateTime == null ? null : tempNullableDateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss+00:00");
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                        {
                            DateTime? tempDateTime = (DateTime)temp;

                            // N.B. All times are in UTC, but sdata does not understand a 'Z' suffix so use +00:00
                            payload.Values[propAttr.SchemaName] = tempDateTime.Value.ToString("yyyy-MM-ddThh:mm:ss+00:00");
                        }
                        else if (prop.PropertyType == typeof(SDataPayload))
                        {
                            payload.Values[propAttr.SchemaName] = temp;
                        }
                        else
                        {
                            payload.Values[propAttr.SchemaName] = temp == null ? temp : temp.ToString();
                        }
                    }
                }

                return payload;
            }
            else
            {
                throw new InvalidOperationException("SchameName attribute is missing from class");
            }
        }
    }
}