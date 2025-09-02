using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed record UpdateProductImageCommand(
    int Id,
    string ImageUrl,
    string Alt,
    int Order,
    bool IsPrimary
) : IRequest<ApiResponse<ProductImageDto>>;
