using System.Runtime.Serialization;

namespace EventsAndAssignments.Services.Exceptions
{
    class NotEnoughPermissionsException : Exception
    {
        const string _defaultMessage = "Not enough permissions to edit the entity.";
        public Type? EntityType { get; }
        public long EntityId { get; }

        public NotEnoughPermissionsException() : base(_defaultMessage)
        {
        }

        public NotEnoughPermissionsException(string message) : base(message)
        {
        }

        public NotEnoughPermissionsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public NotEnoughPermissionsException(Type? entityType, long entityId)
            : this($"Not enough permissions to edit the entity with type: '{entityType}' and Id: '{entityId.ToString()}'.")
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        protected NotEnoughPermissionsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            EntityType = (Type)info.GetValue(nameof(EntityId), typeof(Type))!;
            EntityId = (long)info.GetValue(nameof(EntityId), typeof(long))!;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(EntityType), EntityType, typeof(Type));
            info.AddValue(nameof(EntityId), EntityId, typeof(long));
        }
    }
}