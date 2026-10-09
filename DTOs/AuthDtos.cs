namespace DigitalMenu.DTOs;

public record LoginRequestDto(
    string Email, 
    string Password
    
    );
    
    public record LoginResponseDto(
        string Token,
        string Email,
        string Role,
        Guid? RestaurantId
    );