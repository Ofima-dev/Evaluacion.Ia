using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.Services;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using Evaluacion.IA.Domain.Entities;
using Evaluacion.IA.Domain.ValueObjects;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Commands;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorageService;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IImageStorageService imageStorageService)
    {
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
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

            if (!request.CategoryId.HasValue || request.CategoryId.Value <= 0)
            {
                return ApiResponse<ProductDto>.Failure("Debe seleccionar una categoría válida");
            }

            // Verificar que el SKU no exista
            var existingProduct = await _unitOfWork.Products.AnyAsync(p => p.Sku == request.Sku);

            if (existingProduct)
            {
                return ApiResponse<ProductDto>.Failure($"Ya existe un producto con el SKU '{request.Sku}'");
            }

            // Verificar que la categoría exista y esté activa

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId.HasValue ? request.CategoryId.Value : 0);
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

            var urlImage = Url.Create(await _imageStorageService.SaveImageAsync(request.Image!));
            var alt = Description.Create(request.Alt);
            var productImage = new ProductImage(urlImage, alt, 1, product.Id);

            await _unitOfWork.ProductImages.AddAsync(productImage);
            await _unitOfWork.SaveChangesAsync();

            // Crear DTO de respuesta
            var productDto = new ProductDto(
                product.Id,
                product.Sku.Value,
                product.Name.Value,
                product.Description.Value,
                product.Price.Amount,
                product.Price.Currency,
                product.CategoryId ?? 0,
                category.Name.Value,
                product.IsActive,
                product.CreateAt,
                await _imageStorageService.GetImageAsync(urlImage) // Nuevo producto no tiene imágenes inicialmente
            );

            return ApiResponse<ProductDto>.Success(productDto, "Producto creado exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.Failure($"Error al crear el producto: {ex.Message}");
        }
    }
}
