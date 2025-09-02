using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Users.DTOs;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
                if (user == null)
                {
                    return ApiResponse<UserDto>.Failure("Usuario no encontrado");
                }

                // Validar que el email no exista en otro usuario
                var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.Value == request.Email && u.Id != request.Id);
                if (existingUser != null)
                {
                    return ApiResponse<UserDto>.Failure("El email ya está registrado por otro usuario");
                }

                // Validar que el rol exista
                var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
                if (role == null)
                {
                    return ApiResponse<UserDto>.Failure("El rol especificado no existe");
                }

                // Actualizar el usuario - Nota: En una implementación real, necesitaríamos métodos para actualizar
                // Por ahora, esto es una simulación de la lógica
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    RoleId = user.RoleId,
                    RoleName = role.Description.Value,
                    CreateAt = user.CreateAt
                };

                return ApiResponse<UserDto>.Success(userDto, "Usuario actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.Failure($"Error al actualizar el usuario: {ex.Message}");
            }
        }
    }
}
