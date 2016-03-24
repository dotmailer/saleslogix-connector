using System.Runtime.Serialization;

namespace EmailMarketing.SalesLogix
{
    using System;

    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        public EntityNotFoundException(string message, string entityName, string entityId) : base(message)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public EntityNotFoundException(string message, string entityName, string entityId, Exception inner)
            : base(message, inner)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public string EntityName { get; set; }
        public string EntityId { get; set; }

        protected EntityNotFoundException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        { }
    }
}