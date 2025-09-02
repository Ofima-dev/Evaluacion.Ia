using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
                if (role == null)
                {
                    return ApiResponse<RoleDto>.Failure("Rol no encontrado");
                }

                // Contar usuarios asociados al rol
                var users = await _unitOfWork.Users.FindAsync(u => u.RoleId == role.Id);
                var userCount = users.Count();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Description = role.Description.Value,
                    UserCount = userCount
                };

                return ApiResponse<RoleDto>.Success(roleDto, "Rol encontrado");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto>.Failure($"Error al obtener el rol: {ex.Message}");
            }
        }
    }
}
