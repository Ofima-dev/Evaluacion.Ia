using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, ApiResponse<PagedResult<ProductSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ProductSummaryDto>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("El término de búsqueda es requerido");
            }

            if (request.Page < 1)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("La página debe ser mayor a 0");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("El tamaño de página debe estar entre 1 y 100");
            }

            var searchTerm = request.SearchTerm.Trim().ToLower();

            // Construir query base
            var query = _unitOfWork.Products.GetQueryable();

            // Aplicar filtros
            if (request.OnlyActive)
            {
                query = query.Where(p => p.IsActive);
            }

            // Aplicar búsqueda en múltiples campos con ponderación
            query = query.Where(p => 
                p.Name.Value.ToLower().Contains(searchTerm) ||
                p.Description.Value.ToLower().Contains(searchTerm) ||
                p.Sku.Value.ToLower().Contains(searchTerm));

            // Ordenar por relevancia (coincidencia exacta en nombre primero)
            query = query.OrderByDescending(p => p.Name.Value.ToLower() == searchTerm)
                        .ThenByDescending(p => p.Name.Value.ToLower().StartsWith(searchTerm))
                        .ThenBy(p => p.Name.Value);

            // Obtener total de registros
            var totalCount = await _unitOfWork.Products.CountAsync(query);

            // Aplicar paginación
            var products = await _unitOfWork.Products.GetPagedAsync(query, request.Page, request.PageSize);

            // Obtener categorías para el mapeo
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
                categoryLookup.GetValueOrDefault(product.CategoryId, "Sin categoría"),
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
            return ApiResponse<PagedResult<ProductSummaryDto>>.Failure($"Error al buscar productos: {ex.Message}");
        }
    }
}
