using Evaluacion.IA.Application.Common;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class ChangePasswordCommand : IRequest<ApiResponse<bool>>
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
