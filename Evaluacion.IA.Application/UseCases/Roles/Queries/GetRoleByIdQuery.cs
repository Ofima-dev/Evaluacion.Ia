using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<ApiResponse<RoleDto>>
    {
        public int Id { get; set; }
    }
}
