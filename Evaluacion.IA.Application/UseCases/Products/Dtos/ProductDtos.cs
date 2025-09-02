using Microsoft.AspNetCore.Http;

namespace Evaluacion.IA.Application.UseCases.Products.Dtos;

public sealed record ProductDto(
    int Id,
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int CategoryId,
    string CategoryName,
    bool IsActive,
    DateTime CreatedAt,
    Stream? Image
);

public sealed record CreateProductDto(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int CategoryId,
    IFormFile Image
);

public sealed record UpdateProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int CategoryId,
    bool IsActive
);

public sealed record ProductSummaryDto(
    int Id,
    string Sku,
    string Name,
    decimal Price,
    string Currency,
    string CategoryName,
    bool IsActive
);

public sealed record ProductImageDto(
    int Id,
    string Url,
    string Alt,
    int Order
);
