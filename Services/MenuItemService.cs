using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace DigitalMenu.Services;

public class MenuItemService : IMenuItemService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserContextService _userContext;
    private readonly IFileStorageService _fileStorageService;
    
    public MenuItemService(ApplicationDbContext context, IUserContextService userContext,  IFileStorageService fileStorageService)
    {
        _dbContext = context;
        _userContext = userContext;
        _fileStorageService = fileStorageService;
    }


    public async Task<List<MenuItemDto>> GetAllForCurrentRestaurantAsync(Guid? categoryId = null)
    {
        var restaurantId = _userContext.GetRestaurantId();

        var query =  _dbContext.MenuItems
            .AsNoTracking()
            .Where(m => m.RestaurantId == restaurantId);

        if (categoryId.HasValue)
        {
            query = query.Where(m => m.CategoryId == categoryId.Value); 
        }
            
        
        return await  query
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new MenuItemDto(
                m.Id,
                m.Name,
                m.Description,
                m.BasePrice,
                m.ImageUrl,
                m.IsAvailable ?? true,
                m.CategoryId)
            )
            .ToListAsync();

    }

    public async Task<MenuItemDto?> GetByIdAsync(Guid id)
    {
        var restaurantId = _userContext.GetRestaurantId();

        var menuItem = await _dbContext.MenuItems
            .Where(m => m.RestaurantId == restaurantId && m.Id == id)
            .Select(m => new MenuItemDto(
                m.Id,
                m.Name,
                m.Description,
                m.BasePrice,
                m.ImageUrl,
                m.IsAvailable ?? true,
                m.CategoryId)
            )
            .FirstOrDefaultAsync();

        if (menuItem == null) return null;
        
        return menuItem;
    }

    public async Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto)
    {
        var restaurantId = _userContext.GetRestaurantId();
        var categoryExists = await _dbContext.Categories
            .AnyAsync(c => c.Id == dto.CategoryId && c.RestaurantId == restaurantId);

        if (!categoryExists)
        {
            throw new ArgumentException("Odabrana kategorija ne postoji ili ne pripada vašem restoranu.");
        }
        

        var menuItem = new MenuItem
        {
            Name = dto.Name,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            RestaurantId = restaurantId
            
        };
        
        _dbContext.MenuItems.Add(menuItem);
        await _dbContext.SaveChangesAsync();
        
        return new MenuItemDto(
            menuItem.Id,
            menuItem.Name,
            menuItem.Description,
            menuItem.BasePrice,
            menuItem.ImageUrl,
            menuItem.IsAvailable ?? true,
            menuItem.CategoryId);
    }

    

    public async Task<bool> UpdateAsync(Guid id, UpdateMenuItemDto dto)
    {
        var restaurantId = _userContext.GetRestaurantId();
        var categoryExists = await _dbContext.Categories
            .AnyAsync(c => c.Id == dto.CategoryId && c.RestaurantId == restaurantId);
        if (!categoryExists)
        {
            throw new ArgumentException("Odabrana kategorija ne postoji ili ne pripada vašem restoranu.");
        }
        
        var menuItem= await  _dbContext.MenuItems
            .Where(m => m.Id == id && m.RestaurantId == restaurantId)
            .FirstOrDefaultAsync();
        
        if (menuItem == null) return false;
        
        
        if (!string.IsNullOrEmpty(menuItem.ImageUrl) && menuItem.ImageUrl != dto.ImageUrl)
        {
            _fileStorageService.DeleteFile(menuItem.ImageUrl);
        }
        
        menuItem.Name = dto.Name;
        menuItem.Description = dto.Description;
        menuItem.BasePrice = dto.BasePrice;
        menuItem.ImageUrl = dto.ImageUrl;
        menuItem.IsAvailable = dto.IsAvailable;
        menuItem.CategoryId = dto.CategoryId;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var restaurantId = _userContext.GetRestaurantId();

        var menuItem = await _dbContext.MenuItems
            .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId && m.Id == id);
        
        if(menuItem == null) return false;

        if (!string.IsNullOrEmpty(menuItem.ImageUrl))
        {
            _fileStorageService.DeleteFile(menuItem.ImageUrl);
        }
        
        _dbContext.MenuItems.Remove(menuItem);
        await _dbContext.SaveChangesAsync();
        return true;

    }
}