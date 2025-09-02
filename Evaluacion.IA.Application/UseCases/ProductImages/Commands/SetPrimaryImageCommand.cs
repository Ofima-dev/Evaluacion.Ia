using Evaluacion.IA.Application.Common;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed record SetPrimaryImageCommand(
    int ImageId
) : IRequest<ApiResponse<bool>>;
