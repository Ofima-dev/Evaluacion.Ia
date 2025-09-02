using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed record GetProductByIdQuery(int Id) : IRequest<ApiResponse<ProductDto>>;
