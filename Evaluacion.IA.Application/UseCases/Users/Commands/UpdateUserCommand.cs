using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Users.DTOs;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class UpdateUserCommand : IRequest<ApiResponse<UserDto>>
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}
