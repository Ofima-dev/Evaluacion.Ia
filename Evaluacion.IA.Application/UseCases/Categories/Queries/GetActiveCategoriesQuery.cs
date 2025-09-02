using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Categories.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Queries;

public sealed record GetActiveCategoriesQuery : IRequest<ApiResponse<List<CategorySummaryDto>>>;
