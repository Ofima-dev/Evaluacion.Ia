using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.UseCases.Categories.Dtos;
using Evaluacion.IA.Domain.ValueObjects;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<CategoryDto>.Failure("El ID de la categoría debe ser válido");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ApiResponse<CategoryDto>.Failure("El nombre de la categoría es requerido");
            }

            // Buscar la categoría existente
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
            if (category is null)
            {
                return ApiResponse<CategoryDto>.Failure($"No se encontró la categoría con ID {request.Id}");
            }

            // Verificar si el nombre ya existe en otra categoría
            var name = Name.Create(request.Name);
            var existingCategory = await _unitOfWork.Categories.FirstOrDefaultAsync(c => c.Name == name && c.Id != request.Id);

            if (existingCategory is not null)
            {
                return ApiResponse<CategoryDto>.Failure($"Ya existe otra categoría con el nombre '{request.Name}'");
            }

            // Actualizar la categoría
            category.UpdateDetails(name, request.IsActive);

            // Guardar cambios
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            // Obtener el conteo de productos
            var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id);

            // Crear el DTO de respuesta
            var categoryDto = new CategoryDto(
                category.Id,
                category.Name.Value,
                category.IsActive,
                productCount,
                category.CreateAt
            );

            return ApiResponse<CategoryDto>.Success(categoryDto, "Categoría actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.Failure($"Error al actualizar la categoría: {ex.Message}");
        }
    }
}
