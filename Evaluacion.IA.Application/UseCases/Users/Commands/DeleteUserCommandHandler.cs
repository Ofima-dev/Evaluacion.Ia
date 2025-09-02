using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
                if (user == null)
                {
                    return ApiResponse<bool>.Failure("Usuario no encontrado");
                }

                _unitOfWork.Users.Remove(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Usuario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Error al eliminar el usuario: {ex.Message}");
            }
        }
    }
}
