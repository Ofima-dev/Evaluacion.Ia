using System;

namespace Evaluacion.IA.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public int RoleId { get; private set; }
        public Role? Role { get; private set; }
        public DateTime CreateAt { get; private set; }

    private User() { Email = string.Empty; PasswordHash = string.Empty; }

        public User(string email, string passwordHash, int roleId)
        {
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId;
            CreateAt = DateTime.UtcNow;
        }

        public void SetRole(Role role)
        {
            Role = role;
        }
    }
}
