using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;

namespace Evaluacion.IA.Application.UseCases.Products.Commands;

public sealed record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int? CategoryId,
    bool IsActive
) : IRequest<ApiResponse<ProductDto>>;
