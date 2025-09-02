using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetAvailableRolesQuery : IRequest<ApiResponse<List<RoleDto>>>
    {
        // Query para obtener roles que pueden ser asignados (útil para dropdowns)
    }
}
