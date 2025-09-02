using Evaluacion.IA.Application.Common;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed record DeleteProductImageCommand(int Id) : IRequest<ApiResponse<bool>>;
