using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Roles.Commands
{
    public class CreateRoleCommand : IRequest<ApiResponse<RoleDto>>
    {
        public string Description { get; set; } = string.Empty;
    }
}
