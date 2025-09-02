using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Domain.Entities;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed class AddProductImageCommandHandler : IRequestHandler<AddProductImageCommand, ApiResponse<ProductImageDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddProductImageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductImageDto>> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.ProductId <= 0)
            {
                return ApiResponse<ProductImageDto>.Failure("El ID del producto debe ser válido");
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                return ApiResponse<ProductImageDto>.Failure("La URL de la imagen es requerida");
            }

            if (string.IsNullOrWhiteSpace(request.Alt))
            {
                return ApiResponse<ProductImageDto>.Failure("El texto alternativo es requerido");
            }

            if (request.Order < 0)
            {
                return ApiResponse<ProductImageDto>.Failure("El orden debe ser mayor o igual a 0");
            }

            // Verificar que el producto exista
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product is null)
            {
                return ApiResponse<ProductImageDto>.Failure($"No se encontró el producto con ID {request.ProductId}");
            }

            // Si se marca como primaria, quitar la marca de las demás imágenes del producto
            if (request.IsPrimary)
            {
                var existingImages = await _unitOfWork.ProductImages
                    .FindAsync(pi => pi.ProductId == request.ProductId && pi.IsPrimary);

                foreach (var img in existingImages)
                {
                    img.RemoveAsPrimary();
                    _unitOfWork.ProductImages.Update(img);
                }
            }

            // Verificar que no exista otra imagen con el mismo orden para el mismo producto
            var imageWithSameOrder = await _unitOfWork.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == request.ProductId && pi.Order == request.Order);

            if (imageWithSameOrder is not null)
            {
                return ApiResponse<ProductImageDto>.Failure($"Ya existe una imagen con el orden {request.Order} para este producto");
            }

            // Crear Value Objects
            var imageUrl = Url.Create(request.ImageUrl);
            var alt = Description.Create(request.Alt);

            // Crear la imagen del producto
            var productImage = new ProductImage(imageUrl, alt, request.Order, request.ProductId, request.IsPrimary);
            productImage.SetProduct(product);

            // Guardar en base de datos
            await _unitOfWork.ProductImages.AddAsync(productImage);
            await _unitOfWork.SaveChangesAsync();

            // Crear DTO de respuesta
            var imageDto = new ProductImageDto(
                productImage.Id,
                productImage.ImageUrl.Value,
                productImage.Alt.Value,
                productImage.Order,
                productImage.IsPrimary
            );

            return ApiResponse<ProductImageDto>.Success(imageDto, "Imagen agregada exitosamente al producto");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductImageDto>.Failure($"Error al agregar la imagen: {ex.Message}");
        }
    }
}
