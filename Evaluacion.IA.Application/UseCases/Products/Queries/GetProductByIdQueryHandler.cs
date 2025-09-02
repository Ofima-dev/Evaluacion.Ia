using Evaluacion.IA.Application.Common;
using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.Services;
using Evaluacion.IA.Application.UseCases.Products.Dtos;
using MediatR;

namespace Evaluacion.IA.Application.UseCases.Products.Queries;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorageService;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IImageStorageService imageStorageService)
    {
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
    }

    public async Task<ApiResponse<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validaciones de entrada
            if (request.Id <= 0)
            {
                return ApiResponse<ProductDto>.Failure("El ID del producto debe ser válido");
            }

            // Buscar el producto
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
            if (product is null)
            {
                return ApiResponse<ProductDto>.Failure($"No se encontró el producto con ID {request.Id}");
            }

            // Obtener la categoría
            string categoryName = "Sin categoría";
            if (product.CategoryId.HasValue && product.CategoryId.Value > 0)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId.Value);
                if (category != null)
                {
                    categoryName = category.Name.Value;
                }
            }

            // Obtener las imágenes del producto
            var productImages = await _unitOfWork.ProductImages
                .FindAsync(pi => pi.ProductId == product.Id);

            var imageDtos = productImages.OrderBy(pi => pi.Order).Select(pi => new ProductImageDto(
                pi.Id,
                pi.ImageUrl.Value,
                pi.Alt.Value,
                pi.Order
            )).ToList();

            // Crear el DTO
            var productDto = new ProductDto(
                product.Id,
                product.Sku.Value,
                product.Name.Value,
                product.Description.Value,
                product.Price.Amount,
                product.Price.Currency,
                product.CategoryId ?? 0,
                categoryName,
                product.IsActive,
                product.CreateAt,
                await _imageStorageService.GetImageAsync(imageDtos.First()?.Url)
            );

            return ApiResponse<ProductDto>.Success(productDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.Failure($"Error al obtener el producto: {ex.Message}");
        }
    }
}
