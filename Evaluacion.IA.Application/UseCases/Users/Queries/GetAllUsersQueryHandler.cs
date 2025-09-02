using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Users.DTOs;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Users.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<List<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var roles = await _unitOfWork.Roles.GetAllAsync();
                var rolesDictionary = roles.ToDictionary(r => r.Id, r => r.Description.Value);

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    RoleId = user.RoleId,
                    RoleName = rolesDictionary.GetValueOrDefault(user.RoleId, ""),
                    CreateAt = user.CreateAt
                }).ToList();

                // Simular paginación (en una implementación real, esto debería hacerse en el repositorio)
                var paginatedUsers = userDtos
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return ApiResponse<List<UserDto>>.Success(paginatedUsers, "Usuarios obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserDto>>.Failure($"Error al obtener los usuarios: {ex.Message}");
            }
        }
    }
}
