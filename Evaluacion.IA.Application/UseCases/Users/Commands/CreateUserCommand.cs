using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Users.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class CreateUserCommand : IRequest<ApiResponse<UserDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}
