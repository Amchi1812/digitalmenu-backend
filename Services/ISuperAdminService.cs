
using DigitalMenu.DTOs;

namespace DigitalMenu.Services;



public interface ISuperAdminService
{
    Task<List<RestaurantResponseDto>> GetAllRestaurantsAsync();
    Task<RestaurantResponseDto> CreateRestaurantAsync(CreateRestaurantDto dto);
    Task<bool> CreateRestaurantAdminAsync(CreateRestaurantAdminDto dto);
}