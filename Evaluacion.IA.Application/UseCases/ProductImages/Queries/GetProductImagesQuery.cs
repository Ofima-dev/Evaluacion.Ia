using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Queries;

public sealed record GetProductImagesQuery(
    int ProductId,
    bool OnlyPrimary = false
) : IRequest<ApiResponse<List<ProductImageDto>>>;
