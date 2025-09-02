using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Users.DTOs;

namespace Evaluacion.IA.Application.UseCases.Users.Queries
{
    public class GetUserByIdQuery : IRequest<ApiResponse<UserDto>>
    {
        public int Id { get; set; }
    }
}
