namespace DigitalMenu.DTOs;


public record CategoryDto(
    Guid Id,
    string Name,
    int DisplayOrder
);

public record CreateCategoryDto(
    string Name,
    int DisplayOrder
);

public record UpdateCategoryDto(
    string Name,
    int DisplayOrder
);

public record ReorderCategoryDto(
    Guid Id,
    int DisplayOrder
);

