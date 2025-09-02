using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.DTOs;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed class UpdateProductImageCommandHandler : IRequestHandler<UpdateProductImageCommand, ApiResponse<ProductImageDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductImageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductImageDto>> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<ProductImageDto>.Failure("El ID de la imagen debe ser válido");
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

            // Buscar la imagen existente
            var productImage = await _unitOfWork.ProductImages.GetByIdAsync(request.Id);
            if (productImage is null)
            {
                return ApiResponse<ProductImageDto>.Failure($"No se encontró la imagen con ID {request.Id}");
            }

            // Si se marca como primaria, quitar la marca de las demás imágenes del producto
            if (request.IsPrimary && !productImage.IsPrimary)
            {
                var existingPrimaryImages = await _unitOfWork.ProductImages
                    .FindAsync(pi => pi.ProductId == productImage.ProductId && pi.IsPrimary && pi.Id != request.Id);

                foreach (var img in existingPrimaryImages)
                {
                    img.RemoveAsPrimary();
                    _unitOfWork.ProductImages.Update(img);
                }
            }

            // Verificar que no exista otra imagen con el mismo orden para el mismo producto
            if (request.Order != productImage.Order)
            {
                var imageWithSameOrder = await _unitOfWork.ProductImages
                    .FirstOrDefaultAsync(pi => pi.ProductId == productImage.ProductId && pi.Order == request.Order && pi.Id != request.Id);

                if (imageWithSameOrder is not null)
                {
                    return ApiResponse<ProductImageDto>.Failure($"Ya existe otra imagen con el orden {request.Order} para este producto");
                }
            }

            // Crear Value Objects
            var imageUrl = Url.Create(request.ImageUrl);
            var alt = Description.Create(request.Alt);

            // Actualizar la imagen
            productImage.UpdateDetails(imageUrl, alt, request.Order, request.IsPrimary);

            // Guardar cambios
            _unitOfWork.ProductImages.Update(productImage);
            await _unitOfWork.SaveChangesAsync();

            // Crear DTO de respuesta
            var imageDto = new ProductImageDto(
                productImage.Id,
                productImage.ImageUrl.Value,
                productImage.Alt.Value,
                productImage.Order,
                productImage.IsPrimary
            );

            return ApiResponse<ProductImageDto>.Success(imageDto, "Imagen actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductImageDto>.Failure($"Error al actualizar la imagen: {ex.Message}");
        }
    }
}
