using MediatR;
using Evaluacion.IA.Application.Common;

namespace Evaluacion.IA.Application.UseCases.Roles.Commands
{
    public class DeleteRoleCommand : IRequest<ApiResponse<bool>>
    {
        public int Id { get; set; }
    }
}
