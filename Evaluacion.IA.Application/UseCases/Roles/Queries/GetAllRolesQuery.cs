using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<ApiResponse<List<RoleDto>>>
    {
        public bool IncludeUserCount { get; set; } = true;
    }
}
