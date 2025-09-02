using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed record UpdateCategoryCommand(
    int Id,
    string Name,
    string Description,
    bool IsActive
) : IRequest<ApiResponse<CategoryDto>>;
