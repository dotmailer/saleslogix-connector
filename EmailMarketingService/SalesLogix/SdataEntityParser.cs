namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Entities;
    using Sage.SData.Client.Atom;
    using Sage.SData.Client.Extensions;

    public static class SdataEntityParser
    {
        public static T Parse<T>(AtomEntry sdataEntryToParse) where T : Entity, new()
        {
            if (sdataEntryToParse == null)
            {
                throw new ArgumentNullException("sdataEntryToParse");
            }

            var payload = sdataEntryToParse.GetSDataPayload();

            T output = ParsePayload<T>(payload);

            output.SourceData = sdataEntryToParse;
            return output;
        }

        public static List<T> ParseList<T>(IList<AtomEntry> atomEntries) where T : Entity, new()
        {
            var output = new List<T>();
            if (atomEntries != null)
            {
                foreach (var entry in atomEntries)
                {
                    output.Add(Parse<T>(entry));
                }
            }

            return output;
        }

        private static T ParsePayload<T>(SDataPayload payload)
                where T : Entity, new()
        {
            Type dataType = typeof(T);
            PropertyInfo[] props = dataType.GetProperties();

            T output = new T();
            output.Id = payload.Key;
            foreach (var prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(SchemaNameAttribute), false);

                if (attrs.Length == 1)
                {
                    // Our custom attribute exists on this property
                    SchemaNameAttribute attr = (SchemaNameAttribute)attrs[0];
                    if (payload.Values.ContainsKey(attr.SchemaName))
                    {
                        object objVal = payload.Values[attr.SchemaName];
                        if (objVal == null)
                        {
                            prop.SetValue(output, null, null);
                        }
                        else if (prop.PropertyType == typeof(SDataPayload))
                        {
                            // Nested object - Don't try to convert to an early bound object
                            prop.SetValue(output, objVal, null);
                        }
                        else
                        {
                            // Our property exists in the payload
                            string value = (string)payload.Values[attr.SchemaName];
                            if (prop.PropertyType == typeof(DateTime?))
                            {
                                prop.SetValue(output, DateTime.Parse(value).ToUniversalTime(), null);
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                prop.SetValue(output, DateTime.Parse(value).ToUniversalTime(), null);
                            }
                            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                            {
                                prop.SetValue(output, int.Parse(value), null);
                            }
                            else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                            {
                                prop.SetValue(output, double.Parse(value), null);
                            }
                            else if (prop.PropertyType == typeof(bool))
                            {
                                prop.SetValue(output, bool.Parse(value), null);
                            }
                            else if (prop.PropertyType == typeof(bool?))
                            {
                                prop.SetValue(output, bool.Parse(value), null);
                            }
                            else if (prop.PropertyType.IsEnum)
                            {
                                prop.SetValue(output, Enum.Parse(prop.PropertyType, value), null);
                            }
                            else
                            {
                                prop.SetValue(output, value, null);
                            }
                        }
                    }
                }
            }

            return output;
        }
    }
}