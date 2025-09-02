using Evaluacion.IA.Application.Common;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed record DeleteCategoryCommand(int Id) : IRequest<ApiResponse<bool>>;
