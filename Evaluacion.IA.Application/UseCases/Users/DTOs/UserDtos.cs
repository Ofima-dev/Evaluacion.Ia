namespace Evaluacion.IA.Application.UseCases.Users.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }

    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    public class ChangePasswordDto
    {
        public int Id { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
