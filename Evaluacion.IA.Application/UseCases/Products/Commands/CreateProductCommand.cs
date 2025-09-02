using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Evaluacion.IA.Application.UseCases.Products.Commands;

public sealed record CreateProductCommand(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int? CategoryId,
    IFormFile? Image = null,
    string? Alt = null
) : IRequest<ApiResponse<ProductDto>>;
