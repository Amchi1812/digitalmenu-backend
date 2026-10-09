namespace DigitalMenu.DTOs;

    public record PublicMenuResponseDto(
        Guid Id,
        string Name,
        string Slug,
        bool IsActive,
        List<PublicCategoryDto>Categories
    );
    
    public record PublicCategoryDto(
        Guid Id,
        string Name,
        int DisplayOrder,
        List<PublicMenuItemDto> Items
        );

    public record PublicMenuItemDto(
        Guid Id,
        string Name,
        string? Description,
        decimal BasePrice,
        string? ImageUrl,
        bool IsAvailable,
        int DisplayOrder,
        List<PublicItemVariantDto> Variants,
        List<string> Allergens
    );

    public record PublicItemVariantDto(
        Guid Id,
        string Name,
        decimal Price,
        bool IsAvailable
    );
        