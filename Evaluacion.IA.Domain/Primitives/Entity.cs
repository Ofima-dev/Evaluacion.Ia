namespace Evaluacion.IA.Domain.Primitives
{
    public abstract class Entity
    {
        public int Id { get; protected set; }

        protected Entity(int id)
        {
            Id = id;
        }

        protected Entity() { }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            var entity = (Entity)obj;
            return Id == entity.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity? left, Entity? right)
        {
            return left is not null && right is not null && left.Equals(right);
        }

        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right);
        }
    }
}
