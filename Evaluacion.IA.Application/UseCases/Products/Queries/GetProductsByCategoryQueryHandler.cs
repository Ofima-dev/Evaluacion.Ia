using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, ApiResponse<PagedResult<ProductSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsByCategoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ProductSummaryDto>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.CategoryId <= 0)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("El ID de la categoría debe ser válido");
            }

            if (request.Page < 1)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("La página debe ser mayor a 0");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure("El tamaño de página debe estar entre 1 y 100");
            }

            // Verificar que la categoría exista
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category is null)
            {
                return ApiResponse<PagedResult<ProductSummaryDto>>.Failure($"No se encontró la categoría con ID {request.CategoryId}");
            }

            // Construir query
            var query = _unitOfWork.Products.GetQueryable()
                .Where(p => p.CategoryId == request.CategoryId);

            if (request.OnlyActive)
            {
                query = query.Where(p => p.IsActive);
            }

            // Ordenar por nombre
            query = query.OrderBy(p => p.Name.Value);

            // Obtener total de registros
            var totalCount = await _unitOfWork.Products.CountAsync(query);

            // Aplicar paginación
            var products = await _unitOfWork.Products.GetPagedAsync(query, request.Page, request.PageSize);

            // Crear DTOs
            var productDtos = products.Select(product => new ProductSummaryDto(
                product.Id,
                product.Sku.Value,
                product.Name.Value,
                product.Price.Amount,
                product.Price.Currency,
                category.Name.Value,
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
            return ApiResponse<PagedResult<ProductSummaryDto>>.Failure($"Error al obtener los productos por categoría: {ex.Message}");
        }
    }
}
