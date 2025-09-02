using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<bool>.Failure("El ID de la categoría debe ser válido");
            }

            // Buscar la categoría existente
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
            if (category is null)
            {
                return ApiResponse<bool>.Failure($"No se encontró la categoría con ID {request.Id}");
            }

            // Verificar si la categoría tiene productos asociados
            var hasProducts = await _unitOfWork.Products.AnyAsync(p => p.CategoryId == request.Id);
            if (hasProducts)
            {
                return ApiResponse<bool>.Failure("No se puede eliminar la categoría porque tiene productos asociados");
            }

            // Verificar si la categoría tiene subcategorías
            var hasSubCategories = await _unitOfWork.Categories.AnyAsync(c => c.ParentCategoryId == request.Id);
            if (hasSubCategories)
            {
                return ApiResponse<bool>.Failure("No se puede eliminar la categoría porque tiene subcategorías asociadas");
            }

            // Eliminar la categoría
            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Categoría eliminada exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"Error al eliminar la categoría: {ex.Message}");
        }
    }
}
