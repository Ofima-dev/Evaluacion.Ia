using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Evaluacion.IA.Application.Services;
using Microsoft.IdentityModel.Tokens;

namespace Evaluacion.IA.Infrastructure.Services
{
    public class JWT : IJWT
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public JWT(string key, string issuer, string audience)
        {
            _key = key;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(string userId, string email, string role, DateTime? expires = null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires ?? DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
