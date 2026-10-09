using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu.Services;

public class SuperAdminService : ISuperAdminService
{
    private readonly ApplicationDbContext _dbContext;
    

    public  SuperAdminService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        
    }


    public async Task<List<RestaurantResponseDto>> GetAllRestaurantsAsync()
    {
        return await _dbContext.Restaurants
            .AsNoTracking() // Dobre performanse jer samo čitamo podatke
            .Select(r => new RestaurantResponseDto(
                r.Id,
                r.Name,
                r.Slug,
                r.IsActive ?? false
            ))
            .ToListAsync();
    }

    public async Task<RestaurantResponseDto> CreateRestaurantAsync(CreateRestaurantDto dto)
    {
        var slugExist = await _dbContext.Restaurants
            .AnyAsync(r => r.Slug == dto.Slug);

        if (slugExist)
        {
            throw new ArgumentException("Restoran sa ovim slag-om već postoji.");
        }
        
        var rerstaurant = new Restaurant
        {
            Name = dto.Name,
            Slug = dto.Slug,
            IsActive = true,
        };
        
        _dbContext.Restaurants.Add(rerstaurant);
        await _dbContext.SaveChangesAsync();
        return new RestaurantResponseDto(
            rerstaurant.Id,
            rerstaurant.Name,
            rerstaurant.Slug,
            rerstaurant.IsActive ?? false
            );
    }

    public async Task<bool> CreateRestaurantAdminAsync(CreateRestaurantAdminDto dto)
    {
        var emailExist= await  _dbContext.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExist)
        {
            throw new ArgumentException("Korisnik sa ovim email-om već postoji.");
        }
        
        var restaurantExisat = await  _dbContext.Restaurants
            .AnyAsync(r => r.Id == dto.RestaurantId);

        if (!restaurantExisat)
        {
            throw new ArgumentException("Navedeni restoran ne postoji.");
        }
            
        
        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RestaurantId = dto.RestaurantId,
            Role = "RestoranAdmin"
        };
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}