namespace Evaluacion.IA.Application.DTOs;

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
    DateTime? UpdatedAt,
    List<ProductImageDto> Images
);

public sealed record CreateProductDto(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int CategoryId
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
