using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Queries;

public sealed record GetProductImagesQuery(
    int ProductId,
    bool OnlyPrimary = false
) : IRequest<ApiResponse<List<ProductImageDto>>>;
