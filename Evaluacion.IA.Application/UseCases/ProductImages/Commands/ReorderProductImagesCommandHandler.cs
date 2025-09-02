using MediatR;
using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed class ReorderProductImagesCommandHandler : IRequestHandler<ReorderProductImagesCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReorderProductImagesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(ReorderProductImagesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.ProductId <= 0)
            {
                return ApiResponse<bool>.Failure("El ID del producto debe ser válido");
            }

            if (request.ImageOrders == null || !request.ImageOrders.Any())
            {
                return ApiResponse<bool>.Failure("Debe proporcionar al menos una imagen para reordenar");
            }

            // Verificar que el producto exista
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product is null)
            {
                return ApiResponse<bool>.Failure($"No se encontró el producto con ID {request.ProductId}");
            }

            // Obtener todas las imágenes del producto
            var productImages = await _unitOfWork.ProductImages
                .FindAsync(pi => pi.ProductId == request.ProductId);

            var imagesList = productImages.ToList();

            // Validar que todas las imágenes pertenezcan al producto
            var requestedImageIds = request.ImageOrders.Select(io => io.ImageId).ToList();
            var existingImageIds = imagesList.Select(pi => pi.Id).ToList();

            var invalidImageIds = requestedImageIds.Except(existingImageIds).ToList();
            if (invalidImageIds.Any())
            {
                return ApiResponse<bool>.Failure($"Las siguientes imágenes no pertenecen al producto: {string.Join(", ", invalidImageIds)}");
            }

            // Validar que no haya órdenes duplicados
            var duplicatedOrders = request.ImageOrders
                .GroupBy(io => io.NewOrder)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatedOrders.Any())
            {
                return ApiResponse<bool>.Failure($"Los siguientes órdenes están duplicados: {string.Join(", ", duplicatedOrders)}");
            }

            // Actualizar el orden de cada imagen
            foreach (var imageOrder in request.ImageOrders)
            {
                var image = imagesList.First(pi => pi.Id == imageOrder.ImageId);
                var url = image.ImageUrl;
                var alt = image.Alt;
                var isPrimary = image.IsPrimary;
                
                image.UpdateDetails(url, alt, imageOrder.NewOrder, isPrimary);
                _unitOfWork.ProductImages.Update(image);
            }

            // Guardar cambios
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Orden de imágenes actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"Error al reordenar las imágenes: {ex.Message}");
        }
    }
}
