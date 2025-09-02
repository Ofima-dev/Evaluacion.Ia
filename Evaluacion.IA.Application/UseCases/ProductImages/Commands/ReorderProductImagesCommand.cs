using MediatR;
using Evaluacion.IA.Application.Common;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed record ReorderProductImagesCommand(
    int ProductId,
    List<ImageOrderDto> ImageOrders
) : IRequest<ApiResponse<bool>>;

public sealed record ImageOrderDto(int ImageId, int NewOrder);
