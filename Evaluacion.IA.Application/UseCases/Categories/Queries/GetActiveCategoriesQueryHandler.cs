using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Queries;

public sealed class GetActiveCategoriesQueryHandler : IRequestHandler<GetActiveCategoriesQuery, ApiResponse<List<CategorySummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetActiveCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<CategorySummaryDto>>> Handle(GetActiveCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener categorías activas
            var categories = await _unitOfWork.Categories.FindAsync(c => c.IsActive);

            // Ordenar por nombre y crear DTOs optimizados para dropdowns/selecciones
            var categoryDtos = new List<CategorySummaryDto>();
            foreach (var category in categories.OrderBy(c => c.Name.Value))
            {
                var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id);

                categoryDtos.Add(new CategorySummaryDto(
                    category.Id,
                    category.Name.Value,
                    category.IsActive,
                    productCount
                ));
            }

            return ApiResponse<List<CategorySummaryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CategorySummaryDto>>.Failure($"Error al obtener las categorías activas: {ex.Message}");
        }
    }
}
