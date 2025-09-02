using MediatR;
using Evaluacion.IA.Application.Common;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class DeleteUserCommand : IRequest<ApiResponse<bool>>
    {
        public int Id { get; set; }
    }
}
