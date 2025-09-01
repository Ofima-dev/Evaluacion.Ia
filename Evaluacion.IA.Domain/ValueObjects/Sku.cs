using Evaluacion.IA.Domain.Primitives;

namespace Evaluacion.IA.Domain.ValueObjects
{
    public sealed class Sku : ValueObject
    {
        public string Value { get; private set; }

        private Sku(string value)
        {
            Value = value;
        }

        public static Sku Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SKU cannot be empty", nameof(value));

            if (value.Length < 3 || value.Length > 20)
                throw new ArgumentException("SKU must be between 3 and 20 characters", nameof(value));

            if (!value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
                throw new ArgumentException("SKU can only contain letters, digits, hyphens, and underscores", nameof(value));

            return new Sku(value.ToUpperInvariant());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Sku sku) => sku.Value;
    }
}
