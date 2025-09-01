using Evaluacion.IA.Domain.Primitives;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Domain.Entities
{
    public class User : Entity
    {
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public int RoleId { get; private set; }
        public Role? Role { get; private set; }
        public DateTime CreateAt { get; private set; }

        private User() { Email = Email.Create("temp@temp.com"); PasswordHash = string.Empty; }

        public User(Email email, string passwordHash, int roleId)
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
