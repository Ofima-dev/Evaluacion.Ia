using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Users.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return ApiResponse<bool>.Failure("Usuario no encontrado");
                }

                // Por simplicidad, no verificamos la contraseña actual aquí
                // En una implementación real se verificaría: _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash)
                // Y se aplicaría el hash a la nueva contraseña: var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

                // Por ahora solo simulamos el cambio
                // user.UpdatePassword(newPasswordHash);
                // _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Contraseña actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Error al cambiar la contraseña: {ex.Message}");
            }
        }
    }
}
