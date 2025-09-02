using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.ProductImages.Commands;

public sealed class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductImageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<bool>.Failure("El ID de la imagen debe ser válido");
            }

            // Buscar la imagen existente
            var productImage = await _unitOfWork.ProductImages.GetByIdAsync(request.Id);
            if (productImage is null)
            {
                return ApiResponse<bool>.Failure($"No se encontró la imagen con ID {request.Id}");
            }

            // Eliminar la imagen
            _unitOfWork.ProductImages.Remove(productImage);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Imagen eliminada exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"Error al eliminar la imagen: {ex.Message}");
        }
    }
}
