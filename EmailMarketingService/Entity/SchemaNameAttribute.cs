namespace EmailMarketing.SalesLogix.Entities
{
    using System;

    public class SchemaNameAttribute : Attribute
    {
        public string SchemaName { get; set; }

        /// <summary>
        /// Initializes a new instance of the SchemaNameAttribute class.
        /// </summary>
        public SchemaNameAttribute(string schemaName)
        {
            SchemaName = schemaName;
        }
    }
}