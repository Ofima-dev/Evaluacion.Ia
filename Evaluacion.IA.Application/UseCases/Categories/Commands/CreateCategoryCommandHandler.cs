using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Domain.Entities;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Application.UseCases.Categories.Commands;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ApiResponse<CategoryDto>.Failure("El nombre de la categoría es requerido");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return ApiResponse<CategoryDto>.Failure("La descripción de la categoría es requerida");
            }

            // Verificar si ya existe una categoría con el mismo nombre
            var existingCategory = await _unitOfWork.Categories
                .FirstOrDefaultAsync(c => c.Name.Value == request.Name);

            if (existingCategory is not null)
            {
                return ApiResponse<CategoryDto>.Failure($"Ya existe una categoría con el nombre '{request.Name}'");
            }

            // Crear Value Objects
            var name = Name.Create(request.Name);
            var description = Description.Create(request.Description);

            // Crear la entidad Category
            var category = new Category(name, description);

            // Guardar en base de datos
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            // Crear el DTO de respuesta
            var categoryDto = new CategoryDto(
                category.Id,
                category.Name.Value,
                category.Description.Value,
                category.IsActive,
                0, // Nueva categoría no tiene productos
                category.CreateAt,
                category.UpdateAt
            );

            return ApiResponse<CategoryDto>.Success(categoryDto, "Categoría creada exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.Failure($"Error al crear la categoría: {ex.Message}");
        }
    }
}
