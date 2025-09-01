using Evaluacion.IA.Domain.Primitives;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Domain.Entities
{
    public class Role : Entity
    {
        public Description Description { get; private set; }
        private readonly List<User> _users = [];
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private Role() { Description = Description.Create("Temp"); }

        public Role(Description description)
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
