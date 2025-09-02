using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed record SearchProductsQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 10,
    bool OnlyActive = true
) : IRequest<ApiResponse<PagedResult<ProductSummaryDto>>>;
