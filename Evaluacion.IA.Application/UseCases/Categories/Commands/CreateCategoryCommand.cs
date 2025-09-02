using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed record CreateCategoryCommand(
    string Name,
    int? CategoryId = null
) : IRequest<ApiResponse<CategoryDto>>;
