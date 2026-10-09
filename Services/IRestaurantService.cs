using DigitalMenu.DTOs;

namespace DigitalMenu.Services;

public interface IRestaurantService
{
    Task<RestaurantDto> GetByIdAsync(Guid restaurantId);
}