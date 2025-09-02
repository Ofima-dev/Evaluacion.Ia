using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed record GetProductsByCategoryQuery(
    int CategoryId,
    int Page = 1,
    int PageSize = 10,
    bool OnlyActive = true
) : IRequest<ApiResponse<PagedResult<ProductSummaryDto>>>;
