using System.Runtime.Serialization;

namespace EventsAndAssignments.Services.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        const string _defaultMessage = "Entity does not exist.";
        public long EntityId { get; }
        public Guid EntityGuid { get; }

        public EntityNotFoundException() : base(_defaultMessage)
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public EntityNotFoundException(long entityId) : this($"Entity does not exist with Id: '{entityId}'.")
        {
            EntityId = entityId;
        }

        public EntityNotFoundException(Guid entityGuid) : this($"Entity does not exist with Id: '{entityGuid}'.")
        {
            EntityGuid = entityGuid;
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            EntityId = (long)info.GetValue(nameof(EntityId), typeof(long))!;
            EntityGuid = (Guid)info.GetValue(nameof(EntityGuid), typeof(Guid))!;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(EntityId), EntityId, typeof(long));
            info.AddValue(nameof(EntityGuid), EntityGuid, typeof(Guid));
        }
    }
}