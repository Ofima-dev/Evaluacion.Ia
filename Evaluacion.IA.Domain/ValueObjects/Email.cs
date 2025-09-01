using Evaluacion.IA.Domain.Primitives;
using System.Text.RegularExpressions;

namespace Evaluacion.IA.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        private const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        
        public string Value { get; private set; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty", nameof(value));

            if (!Regex.IsMatch(value, EmailPattern))
                throw new ArgumentException("Invalid email format", nameof(value));

            return new Email(value.ToLowerInvariant());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
    }
}
