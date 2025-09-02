using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;

namespace Evaluacion.IA.Application.UseCases.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<ApiResponse<RoleDto>>
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
