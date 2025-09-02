using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed record CreateCategoryCommand(
    string Name,
    string Description
) : IRequest<ApiResponse<CategoryDto>>;
