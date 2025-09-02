using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed record GetAllProductsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    int? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsActive = null,
    string SortBy = "Name",
    bool SortDescending = false
) : IRequest<ApiResponse<PagedResult<ProductSummaryDto>>>;
