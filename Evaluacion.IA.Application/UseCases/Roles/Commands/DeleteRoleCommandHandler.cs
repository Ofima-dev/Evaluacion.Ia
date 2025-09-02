using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Roles.Commands
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(request.Id);
                if (role == null)
                {
                    return ApiResponse<bool>.Failure("Rol no encontrado");
                }

                // Verificar que no haya usuarios asociados al rol
                var usersWithRole = await _unitOfWork.Users.FindAsync(u => u.RoleId == request.Id);
                if (usersWithRole.Any())
                {
                    return ApiResponse<bool>.Failure("No se puede eliminar el rol porque tiene usuarios asociados");
                }

                _unitOfWork.Roles.Remove(role);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Rol eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Error al eliminar el rol: {ex.Message}");
            }
        }
    }
}
