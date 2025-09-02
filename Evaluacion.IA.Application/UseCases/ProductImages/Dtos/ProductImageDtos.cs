namespace Evaluacion.IA.Application.UseCases.ProductImages.Dtos;

public sealed record CreateProductImageDto(
    int ProductId,
    string ImageUrl,
    string Alt,
    int Order,
    bool IsPrimary
);

public sealed record UpdateProductImageDto(
    int Id,
    string ImageUrl,
    string Alt,
    int Order,
    bool IsPrimary
);

public sealed record ProductImageDetailDto(
    int Id,
    int ProductId,
    string ProductName,
    string ImageUrl,
    string Alt,
    int Order,
    bool IsPrimary,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
