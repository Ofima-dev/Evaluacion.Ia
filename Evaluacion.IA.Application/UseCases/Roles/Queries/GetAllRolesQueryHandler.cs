using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Roles.DTOs;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Roles.Queries
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ApiResponse<List<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _unitOfWork.Roles.GetAllAsync();
                
                var roleDtos = new List<RoleDto>();

                foreach (var role in roles)
                {
                    var userCount = 0;
                    
                    if (request.IncludeUserCount)
                    {
                        var users = await _unitOfWork.Users.FindAsync(u => u.RoleId == role.Id);
                        userCount = users.Count();
                    }

                    roleDtos.Add(new RoleDto
                    {
                        Id = role.Id,
                        Description = role.Description.Value,
                        UserCount = userCount
                    });
                }

                return ApiResponse<List<RoleDto>>.Success(roleDtos, "Roles obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<RoleDto>>.Failure($"Error al obtener los roles: {ex.Message}");
            }
        }
    }
}
