using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Users.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Users.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
                if (user == null)
                {
                    return ApiResponse<UserDto>.Failure("Usuario no encontrado");
                }

                // Cargar el rol si no está cargado
                if (user.Role == null)
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);
                    user.SetRole(role);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.Description.Value ?? "",
                    CreateAt = user.CreateAt
                };

                return ApiResponse<UserDto>.Success(userDto, "Usuario encontrado");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.Failure($"Error al obtener el usuario: {ex.Message}");
            }
        }
    }
}
