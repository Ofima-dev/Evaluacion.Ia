using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Categories.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed record UpdateCategoryCommand(
    int Id,
    string Name,
    bool IsActive
) : IRequest<ApiResponse<CategoryDto>>;
