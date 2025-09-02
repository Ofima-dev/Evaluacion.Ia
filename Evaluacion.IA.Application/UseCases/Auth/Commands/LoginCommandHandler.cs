using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Auth.DTOs;

namespace Evaluacion.IA.Application.UseCases.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar el usuario por email
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.Value == request.Email);
                if (user == null)
                {
                    return ApiResponse<LoginResponseDto>.Failure("Credenciales inválidas");
                }

                // Por simplicidad, no verificamos la contraseña aquí (se haría con IPasswordHasher)
                // En una implementación real se verificaría: _passwordHasher.VerifyPassword(request.Password, user.PasswordHash)

                // Obtener información del rol
                var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);
                if (role == null)
                {
                    return ApiResponse<LoginResponseDto>.Failure("Error en la configuración del usuario");
                }

                // Generar respuesta básica (sin JWT real)
                var response = new LoginResponseDto
                {
                    Token = $"bearer-token-{user.Id}-{DateTime.UtcNow.Ticks}",
                    RefreshToken = Guid.NewGuid().ToString(),
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Email = user.Email.Value,
                        RoleName = role.Description.Value
                    }
                };

                return ApiResponse<LoginResponseDto>.Success(response, "Login exitoso");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.Failure($"Error en el login: {ex.Message}");
            }
        }
    }
}
