using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.Services;
using Evaluacion.IA.Application.UseCases.Auth.DTOs;
using Evaluacion.IA.Domain.ValueObjects;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWT _jwt;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(IUnitOfWork unitOfWork, IJWT jwt, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _jwt = jwt;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar el usuario por email
                var email = Email.Create(request.Email);
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return ApiResponse<LoginResponseDto>.Failure("Credenciales inválidas");
                }

                // Por simplicidad, no verificamos la contraseña aquí (se haría con IPasswordHasher)
                // En una implementación real se verificaría: _passwordHasher.VerifyPassword(request.Password, user.PasswordHash)
                if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return ApiResponse<LoginResponseDto>.Failure("Credenciales inválidas");
                }

                // Obtener información del rol
                var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);
                if (role == null)
                {
                    return ApiResponse<LoginResponseDto>.Failure("Error en la configuración del usuario");
                }

                // Generar respuesta básica (sin JWT real)
                var response = new LoginResponseDto
                {
                    Token = _jwt.GenerateToken(user.Id.ToString(), user.Email, user.Role?.Description!, DateTime.Now.AddDays(1)),
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
