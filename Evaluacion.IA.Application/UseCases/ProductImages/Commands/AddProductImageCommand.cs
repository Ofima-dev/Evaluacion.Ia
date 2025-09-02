using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed record AddProductImageCommand(
    int ProductId,
    string ImageUrl,
    string Alt,
    int Order
) : IRequest<ApiResponse<ProductImageDto>>;
