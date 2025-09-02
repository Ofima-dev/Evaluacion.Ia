using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Domain.Entities;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Application.UseCases.Products.Commands;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (string.IsNullOrWhiteSpace(request.Sku))
            {
                return ApiResponse<ProductDto>.Failure("El SKU del producto es requerido");
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

            // Verificar que el SKU no exista
            var existingProduct = await _unitOfWork.Products
                .FirstOrDefaultAsync(p => p.Sku.Value == request.Sku);

            if (existingProduct is not null)
            {
                return ApiResponse<ProductDto>.Failure($"Ya existe un producto con el SKU '{request.Sku}'");
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
            var sku = Sku.Create(request.Sku);
            var name = Name.Create(request.Name);
            var description = Description.Create(request.Description);
            var price = Money.Create(request.Price, request.Currency);

            // Crear el producto
            var product = new Product(sku, name, description, price, request.CategoryId);
            product.SetCategory(category);

            // Guardar en base de datos
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

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
                new List<ProductImageDto>() // Nuevo producto no tiene imágenes inicialmente
            );

            return ApiResponse<ProductDto>.Success(productDto, "Producto creado exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.Failure($"Error al crear el producto: {ex.Message}");
        }
    }
}
