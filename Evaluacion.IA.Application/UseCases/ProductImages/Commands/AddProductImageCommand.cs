using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed record AddProductImageCommand(
    int ProductId,
    string ImageUrl,
    string Alt,
    int Order
) : IRequest<ApiResponse<ProductImageDto>>;
