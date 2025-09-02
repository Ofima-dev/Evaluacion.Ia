using Evaluacion.IA.Application.Common;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Commands;

public sealed record DeleteProductCommand(int Id) : IRequest<ApiResponse<bool>>;
