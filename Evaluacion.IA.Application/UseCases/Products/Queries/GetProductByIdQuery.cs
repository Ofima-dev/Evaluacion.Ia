using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed record GetProductByIdQuery(int Id) : IRequest<ApiResponse<ProductDto>>;
