using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<ApiResponse<RoleDto>>
    {
        public int Id { get; set; }
    }
}
