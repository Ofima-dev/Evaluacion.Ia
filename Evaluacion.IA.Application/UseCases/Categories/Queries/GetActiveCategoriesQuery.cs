using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;

namespace Evaluacion.IA.Application.UseCases.Categories.Queries;

public sealed record GetActiveCategoriesQuery : IRequest<ApiResponse<List<CategorySummaryDto>>>;
