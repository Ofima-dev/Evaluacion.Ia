using MediatR;
using Evaluacion.IA.Application.Common;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed record DeleteProductImageCommand(int Id) : IRequest<ApiResponse<bool>>;
