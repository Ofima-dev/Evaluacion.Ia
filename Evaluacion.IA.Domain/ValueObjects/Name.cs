using Evaluacion.IA.Domain.Primitives;

namespace Evaluacion.IA.Domain.ValueObjects
{
    public sealed class Name : ValueObject
    {
        public string Value { get; private set; }

        private Name(string value)
        {
            Value = value;
        }

        public static Name Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty", nameof(value));

            if (value.Length < 2 || value.Length > 100)
                throw new ArgumentException("Name must be between 2 and 100 characters", nameof(value));

            var trimmedValue = value.Trim();
            
            return new Name(trimmedValue);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Name name) => name.Value;
    }
}
