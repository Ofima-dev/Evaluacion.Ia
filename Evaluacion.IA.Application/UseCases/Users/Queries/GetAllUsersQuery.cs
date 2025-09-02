using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Users.DTOs;

namespace Evaluacion.IA.Application.UseCases.Users.Queries
{
    public class GetAllUsersQuery : IRequest<ApiResponse<List<UserDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
