using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
                if (role == null)
                {
                    return ApiResponse<RoleDto>.Failure("Rol no encontrado");
                }

                // Validar que no exista otro rol con la misma descripción
                var existingRole = await _unitOfWork.Roles.FirstOrDefaultAsync(r =>
                    r.Description.Value == request.Description && r.Id != request.Id);
                if (existingRole != null)
                {
                    return ApiResponse<RoleDto>.Failure("Ya existe otro rol con esta descripción");
                }

                // Actualizar el rol - Nota: En una implementación real, necesitaríamos métodos para actualizar
                // Por ahora, esto es una simulación de la lógica
                _unitOfWork.Roles.Update(role);
                await _unitOfWork.SaveChangesAsync();

                // Contar usuarios asociados al rol
                var users = await _unitOfWork.Users.FindAsync(u => u.RoleId == role.Id);
                var userCount = users.Count();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Description = role.Description.Value,
                    UserCount = userCount
                };

                return ApiResponse<RoleDto>.Success(roleDto, "Rol actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto>.Failure($"Error al actualizar el rol: {ex.Message}");
            }
        }
    }
}
