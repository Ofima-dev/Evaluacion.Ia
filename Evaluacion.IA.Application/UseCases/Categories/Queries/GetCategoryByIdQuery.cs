using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Queries;

public sealed record GetCategoryByIdQuery(int Id) : IRequest<ApiResponse<CategoryDto>>;
