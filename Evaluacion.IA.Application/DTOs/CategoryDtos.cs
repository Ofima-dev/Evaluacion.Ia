namespace Evaluacion.IA.Application.DTOs;

public sealed record CategoryDto(
    int Id,
    string Name,
    bool IsActive,
    int ProductCount,
    DateTime CreatedAt
);

public sealed record CreateCategoryDto(
    string Name,
    int? CategoryId = null
);

public sealed record UpdateCategoryDto(
    int Id,
    string Name,
    bool IsActive
);

public sealed record CategorySummaryDto(
    int Id,
    string Name,
    bool IsActive,
    int ProductCount
);
