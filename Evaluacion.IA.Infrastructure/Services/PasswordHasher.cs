using Konscious.Security.Cryptography;
using System.Text;
using Evaluacion.IA.Application.Services;

namespace Evaluacion.IA.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 4;
        private const int MemorySize = 65536; // 64 MB
        private const int DegreeOfParallelism = 2;

        public string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var hash = HashWithArgon2id(password, salt);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string hash)
        {
            var parts = hash.Split('.');
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var hashToCompare = HashWithArgon2id(password, salt);
            return parts[1] == Convert.ToBase64String(hashToCompare);
        }

        private static byte[] HashWithArgon2id(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                Iterations = Iterations,
                MemorySize = MemorySize,
            };
            return argon2.GetBytes(HashSize);
        }
    }
}
