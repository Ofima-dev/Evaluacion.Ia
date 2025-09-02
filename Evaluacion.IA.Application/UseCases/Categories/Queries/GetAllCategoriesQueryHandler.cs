using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Categories.Queries;

public sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, ApiResponse<PagedResult<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Page < 1)
            {
                return ApiResponse<PagedResult<CategoryDto>>.Failure("La página debe ser mayor a 0");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return ApiResponse<PagedResult<CategoryDto>>.Failure("El tamaño de página debe estar entre 1 y 100");
            }

            // Construir query base
            var query = _unitOfWork.Categories.GetQueryable();

            // Aplicar filtros
            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(c => c.Name.Value.ToLower().Contains(searchTerm) ||
                                       c.Description.Value.ToLower().Contains(searchTerm));
            }

            // Obtener total de registros
            var totalCount = await _unitOfWork.Categories.CountAsync(query);

            // Aplicar paginación y ordenamiento
            var categories = await _unitOfWork.Categories.GetPagedAsync(
                query.OrderBy(c => c.Name.Value),
                request.Page,
                request.PageSize
            );

            // Crear DTOs
            var categoryDtos = new List<CategoryDto>();
            foreach (var category in categories)
            {
                var productCount = 0;
                if (request.IncludeProductCount)
                {
                    productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id);
                }

                categoryDtos.Add(new CategoryDto(
                    category.Id,
                    category.Name.Value,
                    category.Description.Value,
                    category.IsActive,
                    productCount,
                    category.CreateAt,
                    category.UpdateAt
                ));
            }

            // Crear resultado paginado
            var pagedResult = new PagedResult<CategoryDto>(
                categoryDtos,
                totalCount,
                request.Page,
                request.PageSize
            );

            return ApiResponse<PagedResult<CategoryDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<CategoryDto>>.Failure($"Error al obtener las categorías: {ex.Message}");
        }
    }
}
