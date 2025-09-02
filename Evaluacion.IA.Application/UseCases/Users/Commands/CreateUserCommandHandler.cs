using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.Services;
using Evaluacion.IA.Application.UseCases.Users.DTOs;
using Evaluacion.IA.Domain.Entities;
using Evaluacion.IA.Domain.ValueObjects;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar que el email no exista
                var email = Email.Create(request.Email);
                var existingUser = await _unitOfWork.Users.AnyAsync(u => u.Email == email);
                if (existingUser)
                {
                    return ApiResponse<UserDto>.Failure("El email ya está registrado");
                }

                // Validar que el rol exista
                var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
                if (role == null)
                {
                    return ApiResponse<UserDto>.Failure("El rol especificado no existe");
                }

                // Crear el usuario
                var passwordHash = _passwordHasher.HashPassword(request.Password);
                var user = new User(email, passwordHash, request.RoleId);
                user.SetRole(role);

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.Description.Value ?? "",
                    CreateAt = user.CreateAt
                };

                return ApiResponse<UserDto>.Success(userDto, "Usuario creado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.Failure($"Error al crear el usuario: {ex.Message}");
            }
        }
    }
}
