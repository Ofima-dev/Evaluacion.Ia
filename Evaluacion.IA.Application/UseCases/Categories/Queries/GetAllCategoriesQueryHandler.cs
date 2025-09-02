using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Categories.Dtos;
using Evaluacion.IA.Domain.ValueObjects;
using MediatR;

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
                var searchTerm = Name.Create(request.SearchTerm.Trim().ToLower());
                query = query.Where(c => c.Name == searchTerm);
            }

            // Obtener total de registros
            var totalCount = await _unitOfWork.Categories.CountAsync(query);

            // Aplicar paginación y ordenamiento
            var categories = await _unitOfWork.Categories.GetPagedAsync(
                query.OrderBy(c => c.Name),
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
                    var id = int.Parse(category.Id.ToString());
                    productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == id);
                }

                categoryDtos.Add(new CategoryDto(
                    category.Id,
                    category.Name.Value,
                    category.IsActive,
                    productCount,
                    category.CreateAt
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
