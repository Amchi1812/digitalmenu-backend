namespace DigitalMenu.DTOs;

public record MenuItemDto(
    Guid Id,
    string Name,
    string? Description,
    decimal BasePrice,
    string? ImageUrl,
    bool IsAvailable,
    Guid CategoryId
);


public record CreateMenuItemDto(
    string Name,
    string? Description,
    decimal BasePrice,
    string? ImageUrl,
    Guid CategoryId
);

public record UpdateMenuItemDto(
    string Name,
    string? Description,
    decimal BasePrice,
    string? ImageUrl,
    bool IsAvailable,
    Guid CategoryId
);
