namespace Evaluacion.IA.Application.UseCases.Roles.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }

    public class CreateRoleDto
    {
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateRoleDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
