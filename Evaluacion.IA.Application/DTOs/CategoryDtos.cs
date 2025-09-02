namespace Evaluacion.IA.Application.DTOs;

public sealed record CategoryDto(
    int Id,
    string Name,
    string Description,
    bool IsActive,
    int ProductCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public sealed record CreateCategoryDto(
    string Name,
    string Description
);

public sealed record UpdateCategoryDto(
    int Id,
    string Name,
    string Description,
    bool IsActive
);

public sealed record CategorySummaryDto(
    int Id,
    string Name,
    bool IsActive,
    int ProductCount
);
