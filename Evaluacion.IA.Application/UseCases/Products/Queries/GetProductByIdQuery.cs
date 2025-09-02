using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed record GetProductByIdQuery(int Id) : IRequest<ApiResponse<ProductDto>>;
