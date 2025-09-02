namespace Evaluacion.IA.Application.Services
{
    public interface IJWT
    {
        string GenerateToken(string userId, string email, string role, DateTime? expires = null);
    }
}
