namespace  DigitalMenu.DTOs;


public record CreateRestaurantDto(
    string Name,
    string Slug
);
    

    public record RestaurantResponseDto(
        Guid Id,
        string Name,
        string Slug,
        bool IsActive
    );
    

    public record CreateRestaurantAdminDto (
        string Email, 
        string Password, 
        Guid? RestaurantId
        );