using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Queries;

public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<CategoryDto>.Failure("El ID de la categoría debe ser válido");
            }

            // Buscar la categoría
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
            if (category is null)
            {
                return ApiResponse<CategoryDto>.Failure($"No se encontró la categoría con ID {request.Id}");
            }

            // Obtener el conteo de productos
            var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id);

            // Crear el DTO
            var categoryDto = new CategoryDto(
                category.Id,
                category.Name.Value,
                category.IsActive,
                productCount,
                category.CreateAt
            );

            return ApiResponse<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.Failure($"Error al obtener la categoría: {ex.Message}");
        }
    }
}
