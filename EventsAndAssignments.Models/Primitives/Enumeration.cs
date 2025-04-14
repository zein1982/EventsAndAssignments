using System.Reflection;

namespace EventsAndAssignments.Models.Primitives
{
    public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
    {
        private static readonly Dictionary<long, TEnum> _enumertions = CreateEnumerations();

        public static IEnumerable<TEnum> GetAll()
        {
            return typeof(TEnum).GetFields(BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.DeclaredOnly)
                 .Select(f => f.GetValue(null))
                 .Cast<TEnum>();
        }

        protected Enumeration(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; protected init; }
        public string Name { get; protected init; }

        public static TEnum? FromValue(long value)
        {
            return _enumertions.TryGetValue(
                value,
                out TEnum? enumeration) ? enumeration : default;
        }

        public static TEnum? FromName(string name)
        {
            return _enumertions.Values.SingleOrDefault(x => x.Name == name);
        }

        public bool Equals(Enumeration<TEnum>? other)
        {
            if (other is null)
            {
                return false;
            }

            return GetType() == other.GetType()
                && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is Enumeration<TEnum> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        private static Dictionary<long, TEnum> CreateEnumerations()
        {
            Type enumerationType = typeof(TEnum);

            IEnumerable<TEnum> fieldsForType = enumerationType
                .GetFields(
                    BindingFlags.Public
                        | BindingFlags.Static
                        | BindingFlags.FlattenHierarchy)
                .Where(fieldInfo =>
                    enumerationType.IsAssignableFrom(fieldInfo.FieldType))
                .Select(fieldInfo =>
                    (TEnum)fieldInfo.GetValue(default)!);

            return fieldsForType.ToDictionary(x => x.Id);
        }
    }
}