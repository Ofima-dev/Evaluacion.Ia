using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Categories.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Queries;

public sealed record GetAllCategoriesQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? IsActive = null,
    bool IncludeProductCount = true
) : IRequest<ApiResponse<PagedResult<CategoryDto>>>;
