using System.Runtime.Serialization;

namespace EmailMarketing.SalesLogix
{
    using System;

    [Serializable]
    public class KeyEntityNotFoundException : Exception
    {
        public KeyEntityNotFoundException()
        {
        }

        public KeyEntityNotFoundException(string message) : base(message)
        {
        }

        public KeyEntityNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        public KeyEntityNotFoundException(string message, string entityName, string entityId) : base(message)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public KeyEntityNotFoundException(string message, string entityName, string entityId, Exception inner)
            : base(message, inner)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public string EntityName { get; set; }
        public string EntityId { get; set; }

        protected KeyEntityNotFoundException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        { }
    }
}