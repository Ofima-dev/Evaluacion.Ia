using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using Evaluacion.IA.Domain.Entities;
using Evaluacion.IA.Domain.ValueObjects;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Commands
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar que no exista un rol con la misma descripción
                var description = Description.Create(request.Description);
                var existingRole = await _unitOfWork.Roles.AnyAsync(r => r.Description == description);
                if (existingRole)
                {
                    return ApiResponse<RoleDto>.Failure("Ya existe un rol con esta descripción");
                }

                // Crear el rol
                var role = new Role(description);

                await _unitOfWork.Roles.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Description = role.Description.Value,
                    UserCount = 0
                };

                return ApiResponse<RoleDto>.Success(roleDto, "Rol creado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto>.Failure($"Error al crear el rol: {ex.Message}");
            }
        }
    }
}
