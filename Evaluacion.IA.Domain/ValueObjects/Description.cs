using Evaluacion.IA.Domain.Primitives;

namespace Evaluacion.IA.Domain.ValueObjects
{
    public sealed class Description : ValueObject
    {
        public string Value { get; private set; }

        private Description(string value)
        {
            Value = value;
        }

        public static Description Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new Description(string.Empty);

            if (value.Length > 1000)
                throw new ArgumentException("Description cannot exceed 1000 characters", nameof(value));

            var trimmedValue = value.Trim();
            
            return new Description(trimmedValue.ToLower());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Description description) => description.Value;
    }
}
