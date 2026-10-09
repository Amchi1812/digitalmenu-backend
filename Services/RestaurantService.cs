using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu.Services;

public class RestaurantService : IRestaurantService
{
    private readonly ApplicationDbContext _context;
    public RestaurantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RestaurantDto?> GetByIdAsync(Guid restaurantId)
    {

        var restaurant = await _context.Restaurants
            .Where(r => r.Id == restaurantId)
            .FirstOrDefaultAsync();
        if (restaurant == null) return null;
        
        return new RestaurantDto(restaurant.Name);


    }
}