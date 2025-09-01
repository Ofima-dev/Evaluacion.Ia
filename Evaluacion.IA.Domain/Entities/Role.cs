using System.Collections.Generic;

namespace Evaluacion.IA.Domain.Entities
{
    public class Role
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Role() { Description = string.Empty; }

        public Role(string description)
        {
            Description = description;
        }

        public void AddUser(User user)
        {
            if (!_users.Contains(user))
                _users.Add(user);
        }
    }
}
