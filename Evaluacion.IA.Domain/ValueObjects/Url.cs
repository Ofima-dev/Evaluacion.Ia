using Evaluacion.IA.Domain.Primitives;

namespace Evaluacion.IA.Domain.ValueObjects
{
    public sealed class Url : ValueObject
    {
        public string Value { get; private set; }

        private Url(string value)
        {
            Value = value;
        }

        public static Url Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("URL cannot be empty", nameof(value));

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
                throw new ArgumentException("Invalid URL format", nameof(value));

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("URL must be HTTP or HTTPS", nameof(value));

            return new Url(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Url url) => url.Value;
    }
}
