using DigitalMenu.DTOs;

using DigitalMenu.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserContextService _userContext;
    
    public CategoryService(ApplicationDbContext context,  IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<List<CategoryDto>> GetAllForCurrentRestaurantAsync()
    {
        var restaurantId =  _userContext.GetRestaurantId();
        
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.RestaurantId == restaurantId)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto(c.Id, c.Name, c.DisplayOrder?? 0))
            .ToListAsync();
        
        return categories;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var restaurantId = _userContext.GetRestaurantId();

        var category = await _context.Categories
            .Where(c => c.RestaurantId == restaurantId && c.Id == id)
            .Select(c => new CategoryDto(c.Id, c.Name, c.DisplayOrder ?? 0))
            .FirstOrDefaultAsync();
        
        if (category == null) return null;
        
        return category;
        
       
        
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var restaurantId = _userContext.GetRestaurantId();

        var category = new Category
        {
            RestaurantId = restaurantId,
            Name = dto.Name,
            DisplayOrder = dto.DisplayOrder
        };
        
        
            
   
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        return new CategoryDto(category.Id, category.Name, category.DisplayOrder ?? 0);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var restaurantId = _userContext.GetRestaurantId();
        var category = await _context.Categories
            .Where(c => c.RestaurantId == restaurantId && c.Id == id)
            .FirstOrDefaultAsync();
        
        if (category == null) return false;
        
        category.Name = dto.Name;
        category.DisplayOrder = dto.DisplayOrder;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var restaurantId = _userContext.GetRestaurantId();
        var category = await _context.Categories
            .Where(c => c.RestaurantId == restaurantId && c.Id == id)
            .FirstOrDefaultAsync();
        
        if (category == null) return false;
        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderCategoryAsync(ReorderCategoryDto[] dtos)
    {
        
        if (dtos == null || dtos.Length == 0) return false;

        
        var restaurantId = _userContext.GetRestaurantId();

        var categoryIds = dtos.Select(d => d.Id).ToList();

        
        var categories = await _context.Categories
            .Where(c => categoryIds.Contains(c.Id) && c.RestaurantId == restaurantId)
            .ToListAsync();

        
        if (categories.Count != dtos.Length) 
        {
            
            return false; 
        }

        foreach (var dto in dtos)
        {
            var category = categories.FirstOrDefault(c => c.Id == dto.Id);
            if (category != null)
            {
                category.DisplayOrder = dto.DisplayOrder;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
