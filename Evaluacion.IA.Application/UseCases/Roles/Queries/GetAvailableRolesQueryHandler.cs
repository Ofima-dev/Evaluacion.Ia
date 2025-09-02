using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetAvailableRolesQueryHandler : IRequestHandler<GetAvailableRolesQuery, ApiResponse<List<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<RoleDto>>> Handle(GetAvailableRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _unitOfWork.Roles.GetAllAsync();

                var roleDtos = roles.Select(role => new RoleDto
                {
                    Id = role.Id,
                    Description = role.Description.Value,
                    UserCount = 0 // No necesitamos el conteo para selección
                }).ToList();

                return ApiResponse<List<RoleDto>>.Success(roleDtos, "Roles disponibles obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<RoleDto>>.Failure($"Error al obtener los roles disponibles: {ex.Message}");
            }
        }
    }
}
