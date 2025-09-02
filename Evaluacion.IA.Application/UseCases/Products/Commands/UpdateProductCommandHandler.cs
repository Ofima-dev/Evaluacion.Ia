using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Application.UseCases.Products.Commands;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<ProductDto>.Failure("El ID del producto debe ser válido");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ApiResponse<ProductDto>.Failure("El nombre del producto es requerido");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return ApiResponse<ProductDto>.Failure("La descripción del producto es requerida");
            }

            if (request.Price <= 0)
            {
                return ApiResponse<ProductDto>.Failure("El precio debe ser mayor a 0");
            }

            if (string.IsNullOrWhiteSpace(request.Currency))
            {
                return ApiResponse<ProductDto>.Failure("La moneda es requerida");
            }

            if (request.CategoryId <= 0)
            {
                return ApiResponse<ProductDto>.Failure("Debe seleccionar una categoría válida");
            }

            // Buscar el producto existente
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
            if (product is null)
            {
                return ApiResponse<ProductDto>.Failure($"No se encontró el producto con ID {request.Id}");
            }

            // Verificar que la categoría exista y esté activa
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category is null)
            {
                return ApiResponse<ProductDto>.Failure($"No se encontró la categoría con ID {request.CategoryId}");
            }

            if (!category.IsActive)
            {
                return ApiResponse<ProductDto>.Failure("La categoría seleccionada no está activa");
            }

            // Crear Value Objects
            var name = Name.Create(request.Name);
            var description = Description.Create(request.Description);
            var price = Money.Create(request.Price, request.Currency);

            // Actualizar el producto
            product.UpdateDetails(name, description, price, request.CategoryId, request.IsActive);
            product.SetCategory(category);

            // Guardar cambios
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // Cargar imágenes del producto
            var productImages = await _unitOfWork.ProductImages
                .FindAsync(pi => pi.ProductId == product.Id);

            var imageDtos = productImages.OrderBy(pi => pi.Order).Select(pi => new ProductImageDto(
                pi.Id,
                pi.ImageUrl.Value,
                pi.Alt.Value,
                pi.Order,
                pi.IsPrimary
            )).ToList();

            // Crear DTO de respuesta
            var productDto = new ProductDto(
                product.Id,
                product.Sku.Value,
                product.Name.Value,
                product.Description.Value,
                product.Price.Amount,
                product.Price.Currency,
                product.CategoryId,
                category.Name.Value,
                product.IsActive,
                product.CreateAt,
                product.UpdateAt,
                imageDtos
            );

            return ApiResponse<ProductDto>.Success(productDto, "Producto actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.Failure($"Error al actualizar el producto: {ex.Message}");
        }
    }
}
