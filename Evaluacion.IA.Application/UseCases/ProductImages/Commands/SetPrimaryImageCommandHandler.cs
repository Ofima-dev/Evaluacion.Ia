using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed class SetPrimaryImageCommandHandler : IRequestHandler<SetPrimaryImageCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPrimaryImageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(SetPrimaryImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.ImageId <= 0)
            {
                return ApiResponse<bool>.Failure("El ID de la imagen debe ser válido");
            }

            // Buscar la imagen existente
            var productImage = await _unitOfWork.ProductImages.GetByIdAsync(request.ImageId);
            if (productImage is null)
            {
                return ApiResponse<bool>.Failure($"No se encontró la imagen con ID {request.ImageId}");
            }

            // Si ya es primaria, no hacer nada
            if (productImage.IsPrimary)
            {
                return ApiResponse<bool>.Success(true, "La imagen ya es la primaria");
            }

            // Quitar la marca de primaria de las demás imágenes del producto
            var existingPrimaryImages = await _unitOfWork.ProductImages
                .FindAsync(pi => pi.ProductId == productImage.ProductId && pi.IsPrimary);

            foreach (var img in existingPrimaryImages)
            {
                img.RemoveAsPrimary();
                _unitOfWork.ProductImages.Update(img);
            }

            // Establecer esta imagen como primaria
            productImage.SetAsPrimary();
            _unitOfWork.ProductImages.Update(productImage);

            // Guardar cambios
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Imagen establecida como primaria exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"Error al establecer la imagen como primaria: {ex.Message}");
        }
    }
}
