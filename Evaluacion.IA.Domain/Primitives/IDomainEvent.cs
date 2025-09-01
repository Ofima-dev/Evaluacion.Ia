namespace Evaluacion.IA.Domain.Primitives
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
