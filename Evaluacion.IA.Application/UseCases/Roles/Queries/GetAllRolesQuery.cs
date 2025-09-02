using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<ApiResponse<List<RoleDto>>>
    {
        public bool IncludeUserCount { get; set; } = true;
    }
}
