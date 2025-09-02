using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Commands;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<bool>.Failure("El ID del producto debe ser válido");
            }

            // Buscar el producto existente
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
            if (product is null)
            {
                return ApiResponse<bool>.Failure($"No se encontró el producto con ID {request.Id}");
            }

            // Eliminar las imágenes asociadas al producto
            var productImages = await _unitOfWork.ProductImages
                .FindAsync(pi => pi.ProductId == request.Id);

            foreach (var image in productImages)
            {
                _unitOfWork.ProductImages.Remove(image);
            }

            // Eliminar el producto
            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Producto eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"Error al eliminar el producto: {ex.Message}");
        }
    }
}
