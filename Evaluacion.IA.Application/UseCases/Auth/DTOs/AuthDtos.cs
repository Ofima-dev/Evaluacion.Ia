namespace Evaluacion.IA.Application.UseCases.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserInfoDto User { get; set; } = new();
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
