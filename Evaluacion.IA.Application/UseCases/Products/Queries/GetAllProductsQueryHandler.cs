using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ApiResponse<PagedResult<ProductSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ProductSummaryDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Page < 1)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("La página debe ser mayor a 0");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("El tamaño de página debe estar entre 1 y 100");
            }

            // Construir query base
            var query = _unitOfWork.Products.GetQueryable();

            // Aplicar filtros
            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Name.Value.ToLower().Contains(searchTerm) ||
                                       p.Description.Value.ToLower().Contains(searchTerm) ||
                                       p.Sku.Value.ToLower().Contains(searchTerm));
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price.Amount >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price.Amount <= request.MaxPrice.Value);
            }

            // Aplicar ordenamiento
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(p => p.Name.Value) : query.OrderBy(p => p.Name),
                "price" => request.SortDescending ? query.OrderByDescending(p => p.Price.Amount) : query.OrderBy(p => p.Price.Amount),
                "sku" => request.SortDescending ? query.OrderByDescending(p => p.Sku.Value) : query.OrderBy(p => p.Sku.Value),
                "createdat" => request.SortDescending ? query.OrderByDescending(p => p.CreateAt) : query.OrderBy(p => p.CreateAt),
                _ => query.OrderBy(p => p.Name.Value)
            };

            // Obtener total de registros
            var totalCount = await _unitOfWork.Products.CountAsync(query);

            // Aplicar paginación
            var products = await _unitOfWork.Products.GetPagedAsync(query, request.Page, request.PageSize);

            // Obtener todas las categorías para el mapeo
            var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
            var categories = await _unitOfWork.Categories.FindAsync(c => categoryIds.Contains(c.Id));
            var categoryLookup = categories.ToDictionary(c => c.Id, c => c.Name.Value);

            // Crear DTOs
            var productDtos = products.Select(product => new ProductSummaryDto(
                product.Id,
                product.Sku.Value,
                product.Name.Value,
                product.Price.Amount,
                product.Price.Currency,
                categoryLookup.GetValueOrDefault(product.CategoryId ?? 0, "Sin categoría"),
                product.IsActive
            )).ToList();

            // Crear resultado paginado
            var pagedResult = new PagedResult<ProductSummaryDto>(
                productDtos,
                totalCount,
                request.Page,
                request.PageSize
            );

            return ApiResponse<PagedResult<ProductSummaryDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<ProductSummaryDto>>.Failure($"Error al obtener los productos: {ex.Message}");
        }
    }
}
